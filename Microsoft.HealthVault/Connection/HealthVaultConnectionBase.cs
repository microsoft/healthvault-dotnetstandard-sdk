// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Base implementations of IConnection
    /// </summary>
    /// <seealso cref="IHealthVaultConnection" />
    internal abstract class HealthVaultConnectionBase : IConnectionInternal
    {
        private const int SessionCredentialCallThresholdMinutes = 5;
        private readonly AsyncLock sessionCredentialLock = new AsyncLock();
        private DateTimeOffset lastRefreshedSessionCredential;

        protected HealthVaultConnectionBase(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        public static HashSet<HealthVaultMethods> AnonymousMethods => new HashSet<HealthVaultMethods>()
        {
            HealthVaultMethods.NewApplicationCreationInfo,
            HealthVaultMethods.GetServiceDefinition,
            HealthVaultMethods.CreateAuthenticatedSessionToken
        };

        protected IServiceLocator ServiceLocator { get; }

        public HealthServiceInstance ServiceInstance { get; internal set; }

        public SessionCredential SessionCredential { get; internal set; }

        public abstract Task<PersonInfo> GetPersonInfoAsync();

        public abstract Guid ApplicationId { get; }

        public TClient GetClient<TClient>()
            where TClient : IClient
        {
            TClient client = this.ServiceLocator.GetInstance<TClient>();
            client.Connection = this;

            return client;
        }

        /// <summary>
        /// A client that can be used to access information about the platform.
        /// </summary>
        public IPlatformClient PlatformClient => this.GetClient<IPlatformClient>();

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        public IPersonClient PersonClient => this.GetClient<IPersonClient>();

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        public IVocabularyClient VocabularyClient => this.GetClient<IVocabularyClient>();

        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        /// <returns>
        /// An instance implementing IThingClient
        /// </returns>
        public IThingClient GetThingClient()
        {
            IThingClient thingClient = this.GetClient<IThingClient>();
            return thingClient;
        }

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <returns>
        /// An instance implementing IActionPlanClient
        /// </returns>
        public IActionPlanClient GetActionPlanClient()
        {
            IActionPlanClient actionPlanClient = this.GetClient<IActionPlanClient>();
            return actionPlanClient;
        }

        public abstract Task AuthenticateAsync();

        public async Task<HealthServiceResponseData> ExecuteAsync(
            HealthVaultMethods method,
            int methodVersion,
            string parameters = null,
            Guid? recordId = null)
        {
            // Make sure that session credential is set for method calls requiring
            // authentication
            if (!AnonymousMethods.Contains(method)
                && this.SessionCredential == null)
            {
                await this.AuthenticateAsync().ConfigureAwait(false);
            }

            HealthServiceRequest request = new HealthServiceRequest(this, method, methodVersion, recordId)
            {
                Parameters = parameters
            };

            try
            {
                return await request.ExecuteAsync().ConfigureAwait(false);
            }
            catch (HealthServiceAuthenticatedSessionTokenExpiredException)
            {
                using (await this.sessionCredentialLock.LockAsync().ConfigureAwait(false))
                {
                    if (this.SessionCredential != null)
                    {
                        // SessionCredential should last for a day.
                        // So, check if we the refresh happened within 5 mins.
                        // If we just refreshed then there is no reason to go to fetch the session credential from
                        // server again.
                        if (DateTimeOffset.Now.Subtract(this.lastRefreshedSessionCredential) > TimeSpan.FromMinutes(SessionCredentialCallThresholdMinutes))
                        {
                            await this.RefreshSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);

                            this.lastRefreshedSessionCredential = DateTimeOffset.Now;
                        }

                        return await request.ExecuteAsync().ConfigureAwait(false);
                    }

                    throw;
                }
            }
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

        public virtual CryptoData GetAuthData(HealthVaultMethods method, byte[] data)
        {
            // No need to create auth headers for anonymous methods
            if (AnonymousMethods.Contains(method))
            {
                return null;
            }

            if (this.SessionCredential == null)
            {
                throw new NotSupportedException($"{nameof(this.SessionCredential)} is required to prepare auth header");
            }

            var cryptographer = Ioc.Get<ICryptographer>();
            return cryptographer.Hmac(this.SessionCredential.SharedSecret, data);
        }

        public virtual CryptoData GetInfoHash(byte[] data)
        {
            var cryptographer = Ioc.Get<ICryptographer>();
            return cryptographer.Hash(data);
        }

        public abstract void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId);
    }
}