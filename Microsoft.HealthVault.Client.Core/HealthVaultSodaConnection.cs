using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Transport.MessageFormatters.SessionFormatters;

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

        private readonly AsyncLock authenticateLock = new AsyncLock();

        private PersonInfo personInfo;

        public HealthVaultSodaConnection(
            IServiceLocator serviceLocator,
            ILocalObjectStore localObjectStore,
            IShellAuthService shellAuthService,
            HealthVaultConfiguration configuration)
            : base(serviceLocator)
        {
            this.localObjectStore = localObjectStore;
            this.shellAuthService = shellAuthService;
            this.Configuration = configuration;
        }

        public ApplicationCreationInfo ApplicationCreationInfo { get; internal set; }

        public HealthVaultConfiguration Configuration { get; }

        public override Guid? ApplicationId => this.ApplicationCreationInfo?.AppInstanceId;

        protected override SessionFormatter SessionFormatter => new OfflineSessionFormatter(this.SessionCredential?.Token, () => this.personInfo?.PersonId);

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

        public override string GetRestAuthSessionHeader(Guid? recordId)
        {
            string authToken = this.SessionCredential.Token;
            if (string.IsNullOrEmpty(authToken))
            {
                return string.Empty;
            }

            List<string> tokens = new List<string>();
            tokens.Add(this.FormatRestHeaderToken(RestConstants.AppToken, authToken));
            if (recordId.HasValue && recordId != Guid.Empty)
            {
                tokens.Add(this.FormatRestHeaderToken(RestConstants.OfflinePersonId, this.personInfo.PersonId.ToString()));
                tokens.Add(this.FormatRestHeaderToken(RestConstants.RecordId, recordId.Value.ToString()));
            }

            return string.Format(CultureInfo.InvariantCulture, RestConstants.MSHV1HeaderFormat, string.Join(",", tokens));
        }

        private string FormatRestHeaderToken(string name, string value)
        {
            return string.Format(CultureInfo.InvariantCulture, RestConstants.AuthorizationHeaderElement, name, value);
        }

        public async Task AuthorizeAdditionalRecordsAsync()
        {
            using (await this.authenticateLock.LockAsync().ConfigureAwait(false))
            {
                // First run through shell with web browser to get additional records authorized.
                var masterApplicationId = this.Configuration.MasterApplicationId;
                await this.shellAuthService.AuthorizeAdditionalRecordsAsync(this.ServiceInstance.ShellUrl, masterApplicationId).ConfigureAwait(false);

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
                    var platformClient = this.CreatePlatformClient();

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
            var defaultHealthVaultUrl = this.Configuration.HealthVaultUrl;
            var defaultHealthVaultShellUrl = this.Configuration.HealthVaultShellUrl;
            var masterApplicationId = this.Configuration.MasterApplicationId;

            this.ServiceInstance = new HealthServiceInstance(
                "1",
                "Default",
                "Default HealthVault instance",
                UrlUtilities.GetFullPlatformUrl(defaultHealthVaultUrl),
                defaultHealthVaultShellUrl);

            // Note: This apparent circular call is intentional. This method is called from AuthenticateAsync.
            // PlatformClient is calling HealthVaultConnectionBase.ExecuteAsync("NewApplicationCreationInfo"),
            // which avoids calling AuthenticateAsync because "NewApplicationCreationInfo" is an anonymous method.
            IPlatformClient platformClient = this.CreatePlatformClient();
            ApplicationCreationInfo newApplicationCreationInfo = await platformClient.NewApplicationCreationInfoAsync().ConfigureAwait(false);

            string environmentInstanceId = await this.shellAuthService.ProvisionApplicationAsync(
                defaultHealthVaultShellUrl,
                masterApplicationId,
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
            var personClient = this.CreatePersonClient();

            // Note: This apparent circular call is intentional. This method is called from AuthenticateAsync. PersonClient is calling HealthVaultConnectionBase.ExecuteAsync,
            // which would call AuthenticateAsync again if not for the fact that Authenticate filled in its SessionCredential.Token before calling this method the first time through.
            PersonInfo newPersonInfo = (await personClient.GetAuthorizedPeopleAsync().ConfigureAwait(false)).FirstOrDefault();

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
