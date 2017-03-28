using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Client
{
    internal class HealthVaultSodaConnection : HealthVaultConnectionBase, IHealthVaultSodaConnection
    {
        private const string ServiceInstanceKey = "ServiceInstance";
        private const string ApplicationCreationInfoKey = "ApplicationCreationInfo";
        private const string SessionCredentialKey = "SessionCredential";
        private const string PersonInfoKey = "PersonInfo";

        private readonly ILocalObjectStore localObjectStore;
        private readonly IShellAuthService shellAuthService;
        private readonly ClientHealthVaultConfiguration clientHealthVaultConfiguration;

        private readonly AsyncLock authenticateLock = new AsyncLock();

        private PersonInfo personInfo;

        public HealthVaultSodaConnection(IServiceLocator serviceLocator, ILocalObjectStore localObjectStore, IShellAuthService shellAuthService, ClientHealthVaultConfiguration clientHealthVaultConfiguration)
            : base(serviceLocator)
        {
            this.localObjectStore = localObjectStore;
            this.shellAuthService = shellAuthService;
            this.clientHealthVaultConfiguration = clientHealthVaultConfiguration;
        }

        public ApplicationCreationInfo ApplicationCreationInfo { get; internal set; }

        public override Guid ApplicationId => this.ApplicationCreationInfo.AppInstanceId;

        public override async Task AuthenticateAsync()
        {
            using (await this.authenticateLock.LockAsync().ConfigureAwait(false))
            {
                await this.ReadPropertiesFromLocalStorageAsync().ConfigureAwait(false);

                if (this.ApplicationCreationInfo == null)
                {
                    await this.ProvisionForSodaAuthAsync().ConfigureAwait(false);
                }

                if (this.SessionCredential == null)
                {
                    await this.RefreshSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);
                }

                if (this.personInfo == null)
                {
                    await this.GetAndSavePersonInfoAsync().ConfigureAwait(false);
                }
            }
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            writer.WriteStartElement("auth-session");
            writer.WriteElementString("auth-token", this.SessionCredential.Token);
            if (recordId != null && recordId != Guid.Empty)
            {
                writer.WriteStartElement("offline-person-info");
                writer.WriteElementString("offline-person-id", this.personInfo.PersonId.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public async Task AuthorizeAdditionalRecordsAsync()
        {
            using (await this.authenticateLock.LockAsync().ConfigureAwait(false))
            {
                // First run through shell with web browser to get additional records authorized.
                await this.shellAuthService.AuthorizeAdditionalRecordsAsync(this.ServiceInstance.ShellUrl, this.clientHealthVaultConfiguration.MasterApplicationId).ConfigureAwait(false);

                // Update the person info to add the newly authorized records.
                await this.GetAndSavePersonInfoAsync().ConfigureAwait(false); 
            }
        }

        public async Task DeauthorizeApplicationAsync()
        {
            using (await this.authenticateLock.LockAsync().ConfigureAwait(false))
            {
                // Delete session data from store to ensure we will always be in a clean state
                await this.localObjectStore.DeleteAsync(ServiceInstanceKey).ConfigureAwait(false);
                await this.localObjectStore.DeleteAsync(ApplicationCreationInfoKey).ConfigureAwait(false);
                await this.localObjectStore.DeleteAsync(SessionCredentialKey).ConfigureAwait(false);
                await this.localObjectStore.DeleteAsync(PersonInfoKey).ConfigureAwait(false);

                if (this.ServiceInstance != null &&
                    this.ApplicationCreationInfo != null &&
                    this.SessionCredential != null &&
                    this.personInfo != null)
                {
					var platformClient = ClientHealthVaultFactory.GetPlatformClient(this);
				
                    foreach (HealthRecordInfo record in this.personInfo.AuthorizedRecords.Values)
                    {
                        try
                        {
                        	await platformClient.RemoveApplicationRecordAuthorizationAsync(record.Id).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            // Ignore, this is a non-essential cleanup step
                        }
                    }
                }

                this.ServiceInstance = null;
                this.ApplicationCreationInfo = null;
                this.SessionCredential = null;
                this.personInfo = null;
            }
        }

        protected override ISessionCredentialClient CreateSessionCredentialClient()
        {
            var sessionCredentialClient = this.ServiceLocator.GetInstance<IClientSessionCredentialClient>();
            sessionCredentialClient.AppSharedSecret = this.ApplicationCreationInfo.SharedSecret;
            sessionCredentialClient.Connection = this;

            return sessionCredentialClient;
        }

        private async Task ReadPropertiesFromLocalStorageAsync()
        {
            if (this.ServiceInstance == null)
            {
                this.ServiceInstance = await this.localObjectStore.ReadAsync<HealthServiceInstance>(ServiceInstanceKey).ConfigureAwait(false);
            }

            if (this.ApplicationCreationInfo == null)
            {
                this.ApplicationCreationInfo = await this.localObjectStore.ReadAsync<ApplicationCreationInfo>(ApplicationCreationInfoKey).ConfigureAwait(false);
            }

            if (this.SessionCredential == null)
            {
                this.SessionCredential = await this.localObjectStore.ReadAsync<SessionCredential>(SessionCredentialKey).ConfigureAwait(false);
            }

            if (this.personInfo == null)
            {
                this.personInfo = await this.localObjectStore.ReadAsync<PersonInfo>(PersonInfoKey).ConfigureAwait(false);
            }
        }

        private async Task ProvisionForSodaAuthAsync()
        {
            // Set a temporary service instance for the NewApplicationCreationInfo and GetServiceDefinition calls.
            this.ServiceInstance = new HealthServiceInstance(
                "1",
                "Default",
                "Default HealthVault instance",
                UrlUtilities.GetFullPlatformUrl(this.clientHealthVaultConfiguration.DefaultHealthVaultUrl),
                this.clientHealthVaultConfiguration.DefaultHealthVaultShellUrl);

            // TODO: Eliminate circular call. This method is called from AuthenticateAsync. PlatformClient is calling HealthVaultConnectionBase.ExecuteAsync, which is calling AuthenticateAsync
            IPlatformClient platformClient = ClientHealthVaultFactory.GetPlatformClient(this);
            ApplicationCreationInfo newApplicationCreationInfo = await platformClient.NewApplicationCreationInfoAsync().ConfigureAwait(false);

            string environmentInstanceId = await this.shellAuthService.ProvisionApplicationAsync(
                this.clientHealthVaultConfiguration.DefaultHealthVaultShellUrl,
                this.clientHealthVaultConfiguration.MasterApplicationId,
                newApplicationCreationInfo.AppCreationToken,
                newApplicationCreationInfo.AppInstanceId.ToString()).ConfigureAwait(false);

            ServiceInfo serviceInfo = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.Topology).ConfigureAwait(false);

            HealthServiceInstance bouncedHealthServiceInstance;
            if (!serviceInfo.ServiceInstances.TryGetValue(environmentInstanceId, out bouncedHealthServiceInstance))
            {
                // TODO: Come up with better error for this. Current HealthServiceException is restrictive.
                throw new HealthServiceException(HealthServiceStatusCode.Failed);
            }

            // We've successfully made it through the flow. Save all the information.
            await this.localObjectStore.WriteAsync(ServiceInstanceKey, bouncedHealthServiceInstance).ConfigureAwait(false);
            this.ServiceInstance = bouncedHealthServiceInstance;

            await this.localObjectStore.WriteAsync(ApplicationCreationInfoKey, newApplicationCreationInfo).ConfigureAwait(false);
            this.ApplicationCreationInfo = newApplicationCreationInfo;
        }

        protected override async Task RefreshSessionCredentialAsync(CancellationToken token)
        {
            IClientSessionCredentialClient sessionCredentialClient = this.ServiceLocator.GetInstance<IClientSessionCredentialClient>();
            sessionCredentialClient.Connection = this;
            sessionCredentialClient.AppSharedSecret = this.ApplicationCreationInfo.SharedSecret;
            SessionCredential newCredential = await sessionCredentialClient.GetSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);
            await this.localObjectStore.WriteAsync(SessionCredentialKey, newCredential).ConfigureAwait(false);
            this.SessionCredential = newCredential;
        }

        private async Task GetAndSavePersonInfoAsync()
        {
            var personClient = ClientHealthVaultFactory.GetPersonClient(this);
            // TODO: Eliminate circular call. This method is called from AuthenticateAsync. PersonClient is calling HealthVaultConnectionBase.ExecuteAsync, which is calling AuthenticateAsync
            PersonInfo newPersonInfo = await personClient.GetPersonInfoAsync().ConfigureAwait(false);
            await this.localObjectStore.WriteAsync(PersonInfoKey, newPersonInfo).ConfigureAwait(false);
            this.personInfo = newPersonInfo;
        }

        public override async Task<PersonInfo> GetPersonInfoAsync()
        {
            if (this.personInfo == null)
            {
                await this.AuthenticateAsync().ConfigureAwait(false);
            }

            return this.personInfo;
        }
    }
}
