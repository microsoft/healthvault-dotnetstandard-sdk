// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Client
{
    internal class HealthVaultSodaConnection : HealthVaultConnectionBase, IHealthVaultSodaConnection, IMessageHandlerFactory
    {
        internal const string ServiceInstanceKey = "ServiceInstance";
        internal const string ApplicationCreationInfoKey = "ApplicationCreationInfo";
        internal const string SessionCredentialKey = "SessionCredential";
        internal const string PersonInfoKey = "PersonInfo";

        private readonly ILocalObjectStore _localObjectStore;
        private readonly IShellAuthService _shellAuthService;
        private readonly IMessageHandlerFactory _messageHandlerFactory;

        private readonly AsyncLock _authenticateLock = new AsyncLock();

        private PersonInfo _personInfo;

        public HealthVaultSodaConnection(
            IServiceLocator serviceLocator,
            ILocalObjectStore localObjectStore,
            IShellAuthService shellAuthService,
            IMessageHandlerFactory messageHandlerFactory)
            : base(serviceLocator)
        {
            _localObjectStore = localObjectStore;
            _shellAuthService = shellAuthService;
            _messageHandlerFactory = messageHandlerFactory;
        }

        public ApplicationCreationInfo ApplicationCreationInfo { get; internal set; }

        public override Guid? ApplicationId => ApplicationCreationInfo?.AppInstanceId;

        public override async Task AuthenticateAsync()
        {
            using (await _authenticateLock.LockAsync().ConfigureAwait(false))
            {
                await ReadPropertiesFromLocalStorageAsync().ConfigureAwait(false);

                if (ApplicationCreationInfo == null)
                {
                    await ProvisionForSodaAuthAsync().ConfigureAwait(false);
                }

                if (SessionCredential == null)
                {
                    await RefreshSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);
                }

                if (_personInfo == null)
                {
                    await GetAndSavePersonInfoAsync().ConfigureAwait(false);
                }
            }
        }

        protected override string GetPlatformSpecificRestAuthHeaderPortion()
        {
            return $"{RestConstants.OfflinePersonId}={_personInfo.PersonId}";
        }

        public override AuthSession GetAuthSessionHeader()
        {
            AuthSession authSession = new AuthSession
            {
                AuthToken = SessionCredential.Token
            };

            // Person info will be null for "GetAuthorizedPeople" method.
            if (_personInfo != null)
            {
                authSession.Person = new OfflinePersonInfo { OfflinePersonId = _personInfo.PersonId };
            }

            return authSession;
        }

        public async Task AuthorizeAdditionalRecordsAsync()
        {
            using (await _authenticateLock.LockAsync().ConfigureAwait(false))
            {
                await ReadPropertiesFromLocalStorageAsync().ConfigureAwait(false);

                if (SessionCredential == null || ApplicationCreationInfo == null)
                {
                    throw new InvalidOperationException(Resources.CannotCallAuthorizeAdditionalRecords);
                }

                // First run through shell with web browser to get additional records authorized.
                var masterApplicationId = Configuration.MasterApplicationId;
                await _shellAuthService.AuthorizeAdditionalRecordsAsync(ServiceInstance.ShellUrl, masterApplicationId).ConfigureAwait(false);

                // Update the person info to add the newly authorized records.
                await GetAndSavePersonInfoAsync().ConfigureAwait(false);
            }
        }

        public async Task DeauthorizeApplicationAsync()
        {
            using (await _authenticateLock.LockAsync().ConfigureAwait(false))
            {
                // Delete session data from store to ensure we will always be in a clean state
                await _localObjectStore.DeleteAsync(ServiceInstanceKey).ConfigureAwait(false);
                await _localObjectStore.DeleteAsync(ApplicationCreationInfoKey).ConfigureAwait(false);
                await _localObjectStore.DeleteAsync(SessionCredentialKey).ConfigureAwait(false);
                await _localObjectStore.DeleteAsync(PersonInfoKey).ConfigureAwait(false);

                if (ServiceInstance != null &&
                    ApplicationCreationInfo != null &&
                    SessionCredential != null &&
                    _personInfo != null)
                {
                    var platformClient = CreatePlatformClient();

                    foreach (HealthRecordInfo record in _personInfo.AuthorizedRecords.Values)
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

                ServiceInstance = null;
                ApplicationCreationInfo = null;
                SessionCredential = null;
                _personInfo = null;
            }
        }

        protected override ISessionCredentialClient CreateSessionCredentialClient()
        {
            var sessionCredentialClient = ServiceLocator.GetInstance<IClientSessionCredentialClient>();
            sessionCredentialClient.AppSharedSecret = ApplicationCreationInfo.SharedSecret;
            sessionCredentialClient.Connection = this;

            return sessionCredentialClient;
        }

        private async Task ReadPropertiesFromLocalStorageAsync()
        {
            if (ServiceInstance == null)
            {
                ServiceInstance = await _localObjectStore.ReadAsync<HealthServiceInstance>(ServiceInstanceKey).ConfigureAwait(false);
            }

            if (ApplicationCreationInfo == null)
            {
                ApplicationCreationInfo = await _localObjectStore.ReadAsync<ApplicationCreationInfo>(ApplicationCreationInfoKey).ConfigureAwait(false);
            }

            if (SessionCredential == null)
            {
                SessionCredential = await _localObjectStore.ReadAsync<SessionCredential>(SessionCredentialKey).ConfigureAwait(false);
            }

            if (_personInfo == null)
            {
                _personInfo = await _localObjectStore.ReadAsync<PersonInfo>(PersonInfoKey).ConfigureAwait(false);
            }
        }

        private async Task ProvisionForSodaAuthAsync()
        {
            // Set a temporary service instance for the NewApplicationCreationInfo and GetServiceDefinition calls.
            var defaultHealthVaultUrl = Configuration.DefaultHealthVaultUrl;
            var defaultHealthVaultShellUrl = Configuration.DefaultHealthVaultShellUrl;
            var masterApplicationId = Configuration.MasterApplicationId;

            ServiceInstance = new HealthServiceInstance(
                "1",
                "Default",
                "Default HealthVault instance",
                UrlUtilities.GetFullPlatformUrl(defaultHealthVaultUrl),
                defaultHealthVaultShellUrl);

            // Note: This apparent circular call is intentional. This method is called from AuthenticateAsync.
            // PlatformClient is calling HealthVaultConnectionBase.ExecuteAsync("NewApplicationCreationInfo"),
            // which avoids calling AuthenticateAsync because "NewApplicationCreationInfo" is an anonymous method.
            IPlatformClient platformClient = CreatePlatformClient();
            ApplicationCreationInfo newApplicationCreationInfo = await platformClient.NewApplicationCreationInfoAsync().ConfigureAwait(false);

            string environmentInstanceId = await _shellAuthService.ProvisionApplicationAsync(
                defaultHealthVaultShellUrl,
                masterApplicationId,
                newApplicationCreationInfo.AppCreationToken,
                newApplicationCreationInfo.AppInstanceId.ToString()).ConfigureAwait(false);

            ServiceInfo serviceInfo = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.Topology).ConfigureAwait(false);

            HealthServiceInstance bouncedHealthServiceInstance;
            if (!serviceInfo.ServiceInstances.TryGetValue(environmentInstanceId, out bouncedHealthServiceInstance))
            {
                // TODO: Come up with better error for  Current HealthServiceException is restrictive.
                throw new HealthServiceException(HealthServiceStatusCode.Failed);
            }

            // We've successfully made it through the flow. Save all the information.
            await _localObjectStore.WriteAsync(ServiceInstanceKey, bouncedHealthServiceInstance).ConfigureAwait(false);
            ServiceInstance = bouncedHealthServiceInstance;

            await _localObjectStore.WriteAsync(ApplicationCreationInfoKey, newApplicationCreationInfo).ConfigureAwait(false);
            ApplicationCreationInfo = newApplicationCreationInfo;
        }

        protected override async Task RefreshSessionCredentialAsync(CancellationToken token)
        {
            IClientSessionCredentialClient sessionCredentialClient = ServiceLocator.GetInstance<IClientSessionCredentialClient>();
            sessionCredentialClient.Connection = this;
            sessionCredentialClient.AppSharedSecret = ApplicationCreationInfo.SharedSecret;
            SessionCredential newCredential = await sessionCredentialClient.GetSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);
            await _localObjectStore.WriteAsync(SessionCredentialKey, newCredential).ConfigureAwait(false);
            SessionCredential = newCredential;
        }

        private async Task GetAndSavePersonInfoAsync()
        {
            var personClient = CreatePersonClient();

            // Note: This apparent circular call is intentional. This method is called from AuthenticateAsync. PersonClient is calling HealthVaultConnectionBase.ExecuteAsync,
            // which would call AuthenticateAsync again if not for the fact that Authenticate filled in its SessionCredential.Token before calling this method the first time through.
            PersonInfo newPersonInfo = (await personClient.GetAuthorizedPeopleAsync().ConfigureAwait(false)).FirstOrDefault();

            await _localObjectStore.WriteAsync(PersonInfoKey, newPersonInfo).ConfigureAwait(false);
            _personInfo = newPersonInfo;
        }

        public override async Task<PersonInfo> GetPersonInfoAsync()
        {
            if (_personInfo == null)
            {
                await AuthenticateAsync().ConfigureAwait(false);
            }

            return _personInfo;
        }

        HttpClientHandler IMessageHandlerFactory.Create()
        {
            return _messageHandlerFactory.Create();
        }
    }
}
