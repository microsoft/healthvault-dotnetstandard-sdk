// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Text;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Transport.MessageFormatters.AuthenticationFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.HeaderFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.SessionFormatters;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Base implementations of IConnection
    /// </summary>
    /// <seealso cref="IHealthVaultConnection" />
    internal abstract class HealthVaultConnectionBase : IConnectionInternal
    {
        private const int SessionCredentialCallThresholdMinutes = 5;
        private const string CorrelationIdContextKey = "WC_CorrelationId";
        private const string ResponseIdContextKey = "WC_ResponseId";
        private readonly AsyncLock sessionCredentialLock = new AsyncLock();
        private readonly HealthVaultConfiguration config;
        private readonly HealthWebRequestClient webRequestClient;
        private DateTimeOffset lastRefreshedSessionCredential;

        protected HealthVaultConnectionBase(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.config = serviceLocator.GetInstance<HealthVaultConfiguration>();
            this.webRequestClient = new HealthWebRequestClient(this.config, serviceLocator.GetInstance<IHttpClientFactory>());
        }

        protected IServiceLocator ServiceLocator { get; }

        public HealthServiceInstance ServiceInstance { get; internal set; }

        public SessionCredential SessionCredential { get; internal set; }

        public abstract Task<PersonInfo> GetPersonInfoAsync();

        public abstract Guid? ApplicationId { get; }

        protected abstract SessionFormatter SessionFormatter { get; }

        public abstract Task AuthenticateAsync();

        public async Task<HealthServiceResponseData> ExecuteAsync(HealthVaultMethods method, int methodVersion, string parameters = null, Guid? recordId = null)
        {
            bool allowAnonymous = IsMethodAnonymous(method);

            // Make sure that session credential is set for method calls requiring
            // authentication
            if (!allowAnonymous && string.IsNullOrEmpty(this.SessionCredential?.Token))
            {
                await this.AuthenticateAsync().ConfigureAwait(false);
            }

            var request = new HealthServiceMessage(
                method,
                methodVersion,
                parameters,
                recordId,
                authenticationFormatter: this.GetAuthenticationFormatter(allowAnonymous),
                authSessionOrAppId: this.GetAuthSessionOrAppId(allowAnonymous, method));

            HealthServiceResponseData responseData = null;
            try
            {
                responseData = await this.SendRequestAsync(request);
            }
            catch (HealthServiceAuthenticatedSessionTokenExpiredException)
            {
                if (!allowAnonymous)
                {
                    using (await this.sessionCredentialLock.LockAsync().ConfigureAwait(false))
                    {
                        if (this.SessionCredential != null)
                        {
                            // To prevent multiple token refresh calls being made from simultaneous requests, we check if the token has been refreshed in the last 
                            // {SessionCredentialCallThresholdMinutes} minutes and if so we do not make the call again.
                            if (DateTimeOffset.Now.Subtract(this.lastRefreshedSessionCredential) > TimeSpan.FromMinutes(SessionCredentialCallThresholdMinutes))
                            {
                                await this.RefreshSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);

                                this.lastRefreshedSessionCredential = DateTimeOffset.Now;
                            }

                            return await this.SendRequestAsync(request).ConfigureAwait(false);
                        }

                        throw;
                    }
                }
            }

            return responseData;
        }

        private async Task<HealthServiceResponseData> SendRequestAsync(HealthServiceMessage message)
        {
            try
            {
                message.BuildRequestXml();

                Debug.WriteLine($"Sent message: {Encoding.UTF8.GetString(message.XmlRequest)}");

                // Do we need this log
                HealthVaultPlatformTrace.LogRequest(message.XmlRequest, message.CorrelationId);

                CancellationTokenSource cancellationTokenSource = null;
                HttpResponseMessage response;
                try
                {
                    cancellationTokenSource = new CancellationTokenSource(this.config.RequestTimeoutDuration);
                    response = await this.webRequestClient.SendAsync(
                        this.ServiceInstance.GetHealthVaultMethodUrl(),
                        message.XmlRequest,
                        message.XmlRequestLength,
                        new Dictionary<string, string> { { CorrelationIdContextKey, message.CorrelationId.ToString() } },
                        (CancellationToken)cancellationTokenSource?.Token).ConfigureAwait(false);
                }
                finally
                {
                    cancellationTokenSource?.Dispose();
                }

                // Platform returns a platform request id with the responses. This allows
                // developers to have additional information if necessary for debugging/logging purposes.
                Guid responseId;
                if (response.Headers != null
                    && response.Headers.Contains(ResponseIdContextKey)
                    && Guid.TryParse(response.Headers.GetValues(ResponseIdContextKey)?.FirstOrDefault(), out responseId))
                {
                    message.ResponseId = responseId;

                    if (HealthVaultPlatformTrace.LoggingEnabled)
                    {
                        HealthVaultPlatformTrace.Log(TraceEventType.Information, "Response Id: {0}", responseId);
                    }
                }

                Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                return CreateHealthServiceResponseData(responseStream, response.Headers);
            }
            catch (XmlException xmlException)
            {
                throw new HealthServiceException(
                    Resources.InvalidResponseFromXMLRequest,
                    xmlException);
            }
        }

        protected virtual async Task RefreshSessionCredentialAsync(CancellationToken token)
        {
            ISessionCredentialClient sessionCredentialClient = this.CreateSessionCredentialClient();
            this.SessionCredential = await sessionCredentialClient.GetSessionCredentialAsync(token).ConfigureAwait(false);
        }

        private static bool IsMethodAnonymous(HealthVaultMethods method)
        {
            var type = method.GetType().GetTypeInfo();
            var member = type.GetDeclaredField(method.ToString());
            return member.GetCustomAttribute<AnonymousMethodAttribute>() != null;
        }

        private IAuthSessionOrAppId GetAuthSessionOrAppId(bool allowAnonymous, HealthVaultMethods method)
        {
            return allowAnonymous ? (IAuthSessionOrAppId)new AppIdFormatter(method, this.config.MasterApplicationId, this.ApplicationId) : this.SessionFormatter;
        }

        private AuthenticationFormatter GetAuthenticationFormatter(bool allowAnonymous)
        {
            return allowAnonymous ? new NoAuthenticationFormatter() : new AuthenticationFormatter(this.SessionCredential.SharedSecret);
        }

        /// <summary>
        /// Creates a session credential client with values populated from this connection.
        /// </summary>
        /// <returns>The session credential client.</returns>
        /// <remarks>The values required to populate this client vary based on the authentication method.</remarks>
        protected abstract ISessionCredentialClient CreateSessionCredentialClient();

        public abstract string GetRestAuthSessionHeader(Guid? recordId);

        /// <summary>
        /// Handles the data retrieved by making the web request.
        /// </summary>
        ///
        /// <param name="stream">
        /// The response stream from the web request.
        /// </param>
        /// <param name="responseHeaders">The web response headers.</param>
        ///
        /// <exception cref ="HealthServiceException">
        /// HealthVault returns an exception in the form of an
        /// exception section in the response XML.
        /// </exception>
        ///
        public static HealthServiceResponseData CreateHealthServiceResponseData(Stream stream, HttpResponseHeaders responseHeaders)
        {
            HealthServiceResponseData result = new HealthServiceResponseData();
            result.ResponseHeaders = responseHeaders;

            bool newStreamCreated = false;
            MemoryStream responseStream = stream as MemoryStream;

            try
            {
                if (responseStream == null)
                {
                    newStreamCreated = true;
                    responseStream = new MemoryStream();
                    stream.CopyTo(responseStream);
                }

                result = ParseResponse(responseStream);
            }
            finally
            {
                if (newStreamCreated)
                {
                    responseStream?.Dispose();
                }
            }

            return result;
        }

        private static HealthServiceResponseData ParseResponse(MemoryStream responseStream)
        {
            HealthServiceResponseData result = new HealthServiceResponseData();
            XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
            settings.CloseInput = false;
            settings.IgnoreWhitespace = false;
            responseStream.Position = 0;
            XmlReader reader = XmlReader.Create(responseStream, settings);
            reader.NameTable.Add("wc");

            if (!SDKHelper.ReadUntil(reader, "code"))
            {
                throw new MissingFieldException("code");
            }

            result.CodeId = reader.ReadElementContentAsInt();

            if (result.Code == HealthServiceStatusCode.Ok)
            {
                if (reader.ReadToFollowing("wc:info"))
                {
                    result.InfoReader = reader;

                    byte[] buff = responseStream.ToArray();
                    int offset = 0;
                    int count = (int)responseStream.Length;

                    while (offset < count && buff[offset] != '<')
                    {
                        offset++;
                    }

                    result.ResponseText = new ArraySegment<byte>(buff, offset, count - offset);
                }

                return result;
            }

            result.Error = HandleErrorResponse(reader);

            HealthServiceException e =
                HealthServiceExceptionHelper.GetHealthServiceException(result);

            throw e;
        }

        internal static HealthServiceResponseError HandleErrorResponse(XmlReader reader)
        {
            HealthServiceResponseError error = new HealthServiceResponseError();

            // <error>
            if (string.Equals(reader.Name, "error", StringComparison.Ordinal))
            {
                // <message>
                if (!SDKHelper.ReadUntil(reader, "message"))
                {
                    throw new MissingFieldException("message");
                }

                error.Message = reader.ReadElementContentAsString();

                // <context>
                SDKHelper.SkipToElement(reader);
                if (string.Equals(reader.Name, "context", StringComparison.Ordinal))
                {
                    HealthServiceErrorContext errorContext = new HealthServiceErrorContext();

                    // <server-name>
                    if (SDKHelper.ReadUntil(reader, "server-name"))
                    {
                        errorContext.ServerName = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        throw new MissingFieldException("server-name");
                    }

                    // <server-ip>
                    Collection<IPAddress> ipAddresses = new Collection<IPAddress>();

                    SDKHelper.SkipToElement(reader);
                    while (reader.Name.Equals("server-ip", StringComparison.Ordinal))
                    {
                        string ipAddressString = reader.ReadElementContentAsString();
                        IPAddress ipAddress = null;
                        if (IPAddress.TryParse(ipAddressString, out ipAddress))
                        {
                            ipAddresses.Add(ipAddress);
                        }

                        SDKHelper.SkipToElement(reader);
                    }

                    errorContext.SetServerIpAddresses(ipAddresses);

                    // <exception>
                    if (reader.Name.Equals("exception", StringComparison.Ordinal))
                    {
                        errorContext.InnerException = reader.ReadElementContentAsString();
                        SDKHelper.SkipToElement(reader);
                    }
                    else
                    {
                        throw new MissingFieldException("exception");
                    }

                    error.Context = errorContext;
                }

                // <error-info>
                if (SDKHelper.ReadUntil(reader, "error-info"))
                {
                    error.ErrorInfo = reader.ReadElementContentAsString();
                    SDKHelper.SkipToElement(reader);
                }
            }

            return error;
        }

        /// <summary>
        /// A client that can be used to access information about the platform.
        /// </summary>
        public IPlatformClient CreatePlatformClient() => new PlatformClient(this);

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        public IPersonClient CreatePersonClient() => new PersonClient(this);

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        public IVocabularyClient CreateVocabularyClient() => new VocabularyClient(this);

        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        /// <returns>
        /// An instance implementing IThingClient
        /// </returns>
        public IThingClient CreateThingClient() => new ThingClient(this);

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <returns>
        /// An instance implementing IActionPlanClient
        /// </returns>
        public IActionPlanClient CreateActionPlanClient() => new ActionPlanClient(this);
    }
}