using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
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
        private readonly AsyncLock asyncLock = new AsyncLock();

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

        public PersonInfo PersonInfo { get; internal set; }

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
        /// <param name="record">The record to associate the thing client with</param>
        /// <returns>
        /// An instance implementing IThingClient
        /// </returns>
        public IThingClient GetThingClient(HealthRecordInfo record)
        {
            IThingClient thingClient = this.GetClient<IThingClient>();
            thingClient.Record = record;
            return thingClient;
        }

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <param name="record">The record to associate the action plan client with</param>
        /// <returns>
        /// An instance implementing IActionPlanClient
        /// </returns>
        public IActionPlanClient GetActionPlanClient(HealthRecordInfo record)
        {
            IActionPlanClient actionPlanClient = this.GetClient<IActionPlanClient>();
            actionPlanClient.Record = record;
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
                if (this.SessionCredential != null)
                {
                    await this.RefreshCredentialsAsync(CancellationToken.None).ConfigureAwait(false);
                    return await request.ExecuteAsync().ConfigureAwait(false);
                }

                throw;
            }
        }

        protected virtual async Task RefreshCredentialsAsync(CancellationToken token)
        {
            ISessionCredentialClient sessionCredentialClient = this.CreateSessionCredentialClient();

            using (await this.asyncLock.LockAsync())
            {
                this.SessionCredential = await sessionCredentialClient.GetSessionCredentialAsync(token).ConfigureAwait(false);
            }
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

        // TODO: temp fix
        public virtual void StoreSessionCredentialInCookieXml(XmlWriter writer)
        {
        }

        public virtual void SetSessionCredentialFromCookieXml(XPathNavigator navigator)
        {
        }
    }
}