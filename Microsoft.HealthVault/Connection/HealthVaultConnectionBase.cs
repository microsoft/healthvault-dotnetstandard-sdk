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
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
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
                        // To prevent multiple token refresh calls being made from simultaneous requests, we check if the token has been refreshed in the last 
                        // {SessionCredentialCallThresholdMinutes} minutes and if so we do not make the call again.
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

        public abstract string GetRestAuthSessionHeader(Guid? recordId);
    }
}