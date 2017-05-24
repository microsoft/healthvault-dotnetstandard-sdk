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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;

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

        private DateTimeOffset _lastRefreshedSessionCredential;
        private readonly AsyncLock _sessionCredentialLock = new AsyncLock();
        private HashSet<HealthVaultMethods> _anonymousMethodSet = new HashSet<HealthVaultMethods>();

        private readonly IHealthWebRequestClient _webRequestClient;
        private readonly IHealthServiceResponseParser _healthServiceResponseParser;
        private readonly IRequestMessageCreator _requestMessageCreator;

        protected HealthVaultConnectionBase(IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;

            Configuration = serviceLocator.GetInstance<HealthVaultConfiguration>();
            _webRequestClient = serviceLocator.GetInstance<IHealthWebRequestClient>();
            _healthServiceResponseParser = serviceLocator.GetInstance<IHealthServiceResponseParser>();

            _requestMessageCreator = new RequestMessageCreator(this, serviceLocator);
        }

        public HealthServiceInstance ServiceInstance { get; internal set; }

        public SessionCredential SessionCredential { get; internal set; }

        public abstract Guid? ApplicationId { get; }

        internal HealthVaultConfiguration Configuration { get; }

        protected IServiceLocator ServiceLocator { get; }

        public abstract Task<PersonInfo> GetPersonInfoAsync();

        public abstract Task AuthenticateAsync();

        public abstract string GetRestAuthSessionHeader();

        public abstract AuthSession GetAuthSessionHeader();

        #region Clients

        /// <summary>
        /// A client that can be used to access information about the platform.
        /// </summary>
        public IPlatformClient CreatePlatformClient() => new PlatformClient(this);

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        public IPersonClient CreatePersonClient() => Ioc.Container.Locate<IPersonClient>(extraData: new { connection = this });

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
        public IHealthVaultRestClient CreateRestClient() => new HealthVaultRestClient(Configuration, this, _webRequestClient);

        #endregion

        public async Task<HealthServiceResponseData> ExecuteAsync(
            HealthVaultMethods method,
            int methodVersion,
            string parameters = null,
            Guid? recordId = null,
            Guid? correlationId = null)
        {
            bool isMethodAnonymous = IsMethodAnonymous(method);

            // Make sure that session credential is set for method calls requiring
            // authentication
            if (!isMethodAnonymous && string.IsNullOrEmpty(SessionCredential?.Token))
            {
                await AuthenticateAsync().ConfigureAwait(false);
            }

            // Create the message using a Func in case we need to re-generate it for a retry later
            Func<string> requestXmlCreator = () => _requestMessageCreator.Create(
               method,
               methodVersion,
               isMethodAnonymous,
               parameters,
               recordId,
               isMethodAnonymous && method == HealthVaultMethods.CreateAuthenticatedSessionToken
                   ? ApplicationId
                   : Configuration.MasterApplicationId);

            var requestXml = requestXmlCreator();

            HealthServiceResponseData responseData = null;
            try
            {
                responseData = await SendRequestAsync(requestXml, correlationId).ConfigureAwait(false);
            }
            catch (HealthServiceAuthenticatedSessionTokenExpiredException)
            {
                if (!isMethodAnonymous)
                {
                    await RefreshSessionAsync(CancellationToken.None);

                    // Re-generate the message so it pulls in the new SessionCredential
                    requestXml = requestXmlCreator();
                    return await SendRequestAsync(requestXml, correlationId).ConfigureAwait(false);
                }
            }

            return responseData;
        }

        public async Task RefreshSessionAsync(CancellationToken token)
        {
            using (await _sessionCredentialLock.LockAsync().ConfigureAwait(false))
            {
                // To prevent multiple token refresh calls being made from simultaneous requests, we check if the token has been refreshed in the last
                // {SessionCredentialCallThresholdMinutes} minutes and if so we do not make the call again.
                if (DateTimeOffset.Now.Subtract(_lastRefreshedSessionCredential) > TimeSpan.FromMinutes(SessionCredentialCallThresholdMinutes))
                {
                    await RefreshSessionCredentialAsync(token).ConfigureAwait(false);
                    _lastRefreshedSessionCredential = DateTimeOffset.Now;
                }
            }
        }

        protected virtual async Task RefreshSessionCredentialAsync(CancellationToken token)
        {
            ISessionCredentialClient sessionCredentialClient = CreateSessionCredentialClient();
            SessionCredential = await sessionCredentialClient.GetSessionCredentialAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a session credential client with values populated from this connection.
        /// </summary>
        /// <returns>The session credential client.</returns>
        /// <remarks>The values required to populate this client vary based on the authentication method.</remarks>
        protected abstract ISessionCredentialClient CreateSessionCredentialClient();

        private async Task<HealthServiceResponseData> SendRequestAsync(string requestXml, Guid? correlationId = null)
        {
            try
            {
                Debug.WriteLine($"Sent message: {requestXml}");

                byte[] requestXmlBytes = Encoding.UTF8.GetBytes(requestXml);

                CancellationTokenSource cancellationTokenSource = null;
                HttpResponseMessage response;
                try
                {
                    cancellationTokenSource = new CancellationTokenSource(Configuration.RequestTimeoutDuration);

                    response = await _webRequestClient.SendAsync(
                        ServiceInstance.HealthServiceUrl,
                        requestXmlBytes,
                        requestXml.Length,
                        new Dictionary<string, string> { { CorrelationIdContextKey, correlationId.GetValueOrDefault(Guid.NewGuid()).ToString() } },
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
                    // TODO: Provide a plug in for applications to plug in their telemetry
                    if (HealthVaultPlatformTrace.LoggingEnabled)
                    {
                        HealthVaultPlatformTrace.Log(TraceEventType.Information, "Response Id: {0}", responseId);
                    }
                }

                HealthServiceResponseData responseData = await _healthServiceResponseParser.ParseResponseAsync(response).ConfigureAwait(false);

                return responseData;
            }
            catch (XmlException xmlException)
            {
                throw new HealthServiceException(
                    Resources.InvalidResponseFromXMLRequest,
                    xmlException);
            }
        }

        private bool IsMethodAnonymous(HealthVaultMethods method)
        {
            if (_anonymousMethodSet.Contains(method))
            {
                return true;
            }

            var type = method.GetType().GetTypeInfo();
            var member = type.GetDeclaredField(method.ToString());

            if (member.GetCustomAttribute<AnonymousMethodAttribute>() != null)
            {
                _anonymousMethodSet.Add(method);
                return true;
            }

            return false;
        }
    }
}