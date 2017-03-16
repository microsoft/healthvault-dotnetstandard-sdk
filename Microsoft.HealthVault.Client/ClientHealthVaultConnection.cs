using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Utilities;

namespace Microsoft.HealthVault.Client
{
    internal class ClientHealthVaultConnection : HealthVaultConnectionBase, IClientHealthVaultConnection
    {
        private const string ServiceInstanceKey = "ServiceInstance";
        private const string ApplicationCreationInfoKey = "ApplicationCreationInfo";
        private const string SessionCredentialKey = "SessionCredential";
        private const string PersonInfoKey = "PersonInfo";

        private readonly ILocalObjectStore localObjectStore;
        private readonly IShellAuthService shellAuthService;
        private readonly ClientConfiguration clientConfiguration;

        private readonly AsyncLock authenticateLock = new AsyncLock();

        public ClientHealthVaultConnection(IServiceLocator serviceLocator, ILocalObjectStore localObjectStore, IShellAuthService shellAuthService, ClientConfiguration clientConfiguration)
            : base(serviceLocator)
        {
            this.localObjectStore = localObjectStore;
            this.shellAuthService = shellAuthService;
            this.clientConfiguration = clientConfiguration;
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
                    await this.GetAndSaveSessionCredential().ConfigureAwait(false);
                }

                if (this.PersonInfo == null)
                {
                    await this.GetAndSavePersonInfoAsync().ConfigureAwait(false);
                }
            }
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            writer.WriteStartElement("auth-session");
            writer.WriteElementString("auth-token", this.SessionCredential.Token);
            if (recordId != null)
            {
                writer.WriteStartElement("offline-person-info");
                writer.WriteElementString("offline-person-id", this.PersonInfo.PersonId.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public async Task AuthorizeAdditionalRecordsAsync()
        {
            using (await this.authenticateLock.LockAsync().ConfigureAwait(false))
            {
                // First run through shell with web browser to get additional records authorized.
                await this.shellAuthService.AuthorizeAdditionalRecordsAsync(this.ServiceInstance.ShellUrl, this.clientConfiguration.MasterApplicationId).ConfigureAwait(false);

                // Update the person info to add the newly authorized records.
                await this.GetAndSavePersonInfoAsync().ConfigureAwait(false); 
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

            if (this.PersonInfo == null)
            {
                this.PersonInfo = await this.localObjectStore.ReadAsync<PersonInfo>(PersonInfoKey).ConfigureAwait(false);
            }
        }

        private async Task ProvisionForSodaAuthAsync()
        {
            // Set a temporary service instance for the NewApplicationCreationInfo and GetServiceDefinition calls.
            this.ServiceInstance = new HealthServiceInstance(
                "1",
                "Default",
                "Default HealthVault instance",
                UrlUtilities.GetFullPlatformUrl(this.clientConfiguration.DefaultHealthVaultUrl),
                this.clientConfiguration.DefaultHealthVaultShellUrl);

            ApplicationCreationInfo newApplicationCreationInfo = await this.PlatformClient.NewApplicationCreationInfoAsync().ConfigureAwait(false);

            string environmentInstanceId = await this.shellAuthService.ProvisionApplicationAsync(
                this.clientConfiguration.DefaultHealthVaultShellUrl,
                this.clientConfiguration.MasterApplicationId,
                this.ApplicationCreationInfo.AppCreationToken,
                this.ApplicationCreationInfo.AppInstanceId.ToString()).ConfigureAwait(false);

            ServiceInfo serviceInfo = await this.PlatformClient.GetServiceDefinitionAsync(ServiceInfoSections.Topology).ConfigureAwait(false);

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

        private async Task GetAndSaveSessionCredential()
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
            PersonInfo newPersonInfo = await this.PersonClient.GetPersonInfoAsync().ConfigureAwait(false);
            await this.localObjectStore.WriteAsync(PersonInfoKey, newPersonInfo).ConfigureAwait(false);
            this.PersonInfo = newPersonInfo;
        }
    }
}
