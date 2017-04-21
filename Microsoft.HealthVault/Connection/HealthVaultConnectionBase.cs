// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Text;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Rest;
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
        private const string CorrelationIdContextKey = "WC_CorrelationId";
        private const string ResponseIdContextKey = "WC_ResponseId";
        private const int SessionCredentialCallThresholdMinutes = 5;

        private DateTimeOffset lastRefreshedSessionCredential;
        private readonly AsyncLock sessionCredentialLock = new AsyncLock();
        private HashSet<HealthVaultMethods> anonymousMethodSet = new HashSet<HealthVaultMethods>();

        private readonly HealthVaultConfiguration config;
        private readonly HealthWebRequestClient webRequestClient;
        private readonly IHealthServiceResponseParser healthServiceResponseParser;

        protected HealthVaultConnectionBase(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;

            this.config = this.ServiceLocator.GetInstance<HealthVaultConfiguration>();
            this.webRequestClient = new HealthWebRequestClient(this.config, this.ServiceLocator.GetInstance<IHttpClientFactory>());
            this.healthServiceResponseParser = serviceLocator.GetInstance<IHealthServiceResponseParser>();
        }

        public HealthServiceInstance ServiceInstance { get; internal set; }

        public SessionCredential SessionCredential { get; internal set; }

        public abstract Guid? ApplicationId { get; }

        protected IServiceLocator ServiceLocator { get; }

        protected abstract SessionFormatter SessionFormatter { get; }

        public abstract Task<PersonInfo> GetPersonInfoAsync();

        public abstract Task AuthenticateAsync();

        public abstract string GetRestAuthSessionHeader();

        #region Clients

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
        public IThingClient CreateThingClient() => new ThingClient(this, new ThingDeserializer(this));

        /// <summary>
        /// Creates a rest client that commmunicates with HealthVault
        /// </summary>
        public IHealthVaultRestClient CreateRestClient() => new HealthVaultRestClient(this.config, this, this.webRequestClient);

        #endregion

        public async Task<HealthServiceResponseData> ExecuteAsync(
            HealthVaultMethods method, 
            int methodVersion,
            string parameters = null, 
            Guid? recordId = null)
        {
            bool allowAnonymous = this.IsMethodAnonymous(method);

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

        protected virtual async Task RefreshSessionCredentialAsync(CancellationToken token)
        {
            ISessionCredentialClient sessionCredentialClient = this.CreateSessionCredentialClient();
            this.SessionCredential = await sessionCredentialClient.GetSessionCredentialAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a session credential client with values populated from this connection.
        /// </summary>
        /// <returns>The session credential client.</returns>
        /// <remarks>The values required to populate this client vary based on the authentication method.</remarks>
        protected abstract ISessionCredentialClient CreateSessionCredentialClient();

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

                HealthServiceResponseData responseData = await this.healthServiceResponseParser.ParseResponseAsync(response).ConfigureAwait(false);

                return responseData;
            }
            catch (XmlException xmlException)
            {
                throw new HealthServiceException(
                    Resources.InvalidResponseFromXMLRequest,
                    xmlException);
            }
        }

        private IAuthSessionOrAppId GetAuthSessionOrAppId(bool allowAnonymous, HealthVaultMethods method)
        {
            return allowAnonymous ? (IAuthSessionOrAppId)new AppIdFormatter(method, this.config.MasterApplicationId, this.ApplicationId) : this.SessionFormatter;
        }

        private AuthenticationFormatter GetAuthenticationFormatter(bool allowAnonymous)
        {
            return allowAnonymous ? new NoAuthenticationFormatter() : new AuthenticationFormatter(this.SessionCredential.SharedSecret);
        }

        private bool IsMethodAnonymous(HealthVaultMethods method)
        {
            if (this.anonymousMethodSet.Contains(method))
            {
                return true;
            }

            var type = method.GetType().GetTypeInfo();
            var member = type.GetDeclaredField(method.ToString());

            if (member.GetCustomAttribute<AnonymousMethodAttribute>() != null)
            {
                this.anonymousMethodSet.Add(method);
                return true;
            }

            return false;
        }
    }
}