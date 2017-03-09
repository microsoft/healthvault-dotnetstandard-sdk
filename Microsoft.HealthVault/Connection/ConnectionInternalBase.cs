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
    internal abstract class ConnectionInternalBase : IConnectionInternal
    {
        private readonly AsyncLock asyncLock = new AsyncLock();

        private IServiceLocator serviceLocator;

        public ConnectionInternalBase(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public static string SessionAuthenticationMethodName => "CreateAuthenticatedSessionToken";

        public static HashSet<string> AnonymousMethods => new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NewApplicationCreationInfo",
            "GetServiceDefinition",
            SessionAuthenticationMethodName
        };

        protected HealthServiceInstance HealthServiceInstanceInternal { get; set; }

        protected SessionCredential SessionCredentialInternal { get; set; }

        // protected PersonInfo PersonInfoInternal { get; set; }

        protected Guid ApplicationIdInternal { get; set; }

        public HealthServiceInstance ServiceInstance => this.HealthServiceInstanceInternal;

        public SessionCredential SessionCredential => this.SessionCredentialInternal;

        // comment for now
        // public PersonInfo PersonInfo => this.PersonInfoInternal;

        public Guid ApplicationId => this.ApplicationIdInternal;

        public IConfiguration ApplicationConfiguration { get; set; }

        public TClient GetClient<TClient>()
            where TClient : IClient
        {
            TClient client = this.serviceLocator.GetInstance<TClient>();
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
            string methodName,
            int methodVersion,
            string parameters = null,
            Guid? recordId = null)
        {
            Validator.ThrowIfStringNullOrEmpty(methodName, "methodName");

            // Make sure that session credential is set for method calls requiring
            // authentication
            if (!AnonymousMethods.Contains(methodName)
                && this.SessionCredential == null)
            {
                await this.AuthenticateAsync().ConfigureAwait(false);
            }

            HealthServiceRequest request = new HealthServiceRequest(this, methodName, methodVersion, recordId)
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
            ISessionCredentialClient sessionCredentialClient = this.serviceLocator.GetInstance<ISessionCredentialClient>();
            sessionCredentialClient.Connection = this;

            using (await this.asyncLock.LockAsync())
            {
                this.SessionCredentialInternal = await sessionCredentialClient.GetSessionCredentialAsync(token).ConfigureAwait(false);
            }
        }

        public virtual CryptoData GetAuthData(string methodName, byte[] data)
        {
            // No need to create auth headers for anonymous methods
            if (AnonymousMethods.Contains(methodName))
            {
                return null;
            }

            Validator.ThrowIfStringIsEmptyOrWhitespace(methodName, nameof(methodName));

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