using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest
{
    [TestClass]
    public class HealthVaultSodaConnectionTests
    {
        private const string ApplicationInstanceId = "b5c5593b-afb4-466d-88f2-31707fb8634b";

        private const string ApplicationSharedSecret = "bYmBR7gCnkc48amiuhiod6g5qQU0gYioqA3KA+3qCjo=";

        private const string ApplicationCreationToken =
            "AiAAACH/OO7nZ9ZOtOr7JE7RzhC7rPSay87cX4LyKDyVka/dDC62cR9c2Jzk3HpJjkRAZuAAAADGGYShB8PnpleM+P7OWr9I0WLuqLfcRPBUzqtJrjiMlpzsnyIwOJ2iqp+99Lj6SrlcAf3s5Ea8eMqi5xQDOljUcqbkhTVTavsWUSwT1GdfGvu2VwCRW6gnrGWriGV4dn+SMQru2xDH+u1ZubLIFkLy4Omw7n0LXWaVFjNmtH7AQjheGornC5QidP3p5WCSuT+V4ye+sH51ie9rfx5ZZRSclFdIWTVxGhquQciSnBaX31X9PHY+VikmNFJwY5CAA2IRxrNWzETquZZzxLoIbpiTTcC4azwSp1I44uH0bsqHmSAAAAB4hhVJRM5ftA7iYjnKPtw0hK2lSHtZ88p9H/DpB9CDZCAAAAB4hhVJRM5ftA7iYjnKPtw0hK2lSHtZ88p9H/DpB9CDZA==";

        private const string SessionToken =
            "AiAAAML47WGYEfRCjJD4xSaFJiAsp6b2KZH75mHIc3KBv7aq38UKMTpSBJLqcocAH6hQasAAAAB43aAYDIttNM4dwX5z5atIDyTWwwji1ZUt6Ug58+YzHEz06mlSfb9zi0nfg8TTwT+wXLpe96Qh/H7r11xlLqFkpq8pus0dPRaad1GAtRUE4HbbPCOxaiKiVyW1zYjUB3z9+33m3ek0W2n+AIX3sBX44JCAHZf9+rkATGAXQpSL6z95VcJUqaiV68wducJexnX7fOLh3Cs5YP1vHsATYGqOJhGnD2ZZwArkDwkKWih5MghZpwFHSfTRT/jrHSu3J+4gAAAA5Ch3uw48sJd5R0We3XB65ZEqfgSHBAvYedK3uCsc0PAgAAAA5Ch3uw48sJd5R0We3XB65ZEqfgSHBAvYedK3uCsc0PA=";

        private const string SessionSharedSecret = "Jop6pGrETq2wvczma4LkEGjknPPF76MN+XE7t9xyiyC3ZzlWVk++6i4o4Ia+D8V3YHu/elyppKRJJaOR5MWUuA==";

        private IServiceLocator subServiceLocator;
        private IHealthWebRequestClient subHealthWebRequestClient;
        private ILocalObjectStore subLocalObjectStore;
        private IShellAuthService subShellAuthService;
        private IClientSessionCredentialClient subClientSessionCredentialClient;
        private HealthVaultConfiguration healthVaultConfiguration;

        private static readonly Guid MasterApplicationId = new Guid("30945bac-d221-4f89-8197-6983a390066d");

        [TestInitialize]
        public void TestInitialize()
        {
            this.subServiceLocator = Substitute.For<IServiceLocator>();
            this.subHealthWebRequestClient = Substitute.For<IHealthWebRequestClient>();
            this.subLocalObjectStore = Substitute.For<ILocalObjectStore>();
            this.subShellAuthService = Substitute.For<IShellAuthService>();
            this.subClientSessionCredentialClient = Substitute.For<IClientSessionCredentialClient>();
            this.healthVaultConfiguration = new HealthVaultConfiguration
            {
                MasterApplicationId = MasterApplicationId,
                DefaultHealthVaultUrl = new Uri("https://platform2.healthvault.com/platform/"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault.com")
            };

            this.subServiceLocator.GetInstance<HealthVaultConfiguration>().Returns(this.healthVaultConfiguration);
            this.subServiceLocator.GetInstance<IHealthWebRequestClient>().Returns(this.subHealthWebRequestClient);
            this.subServiceLocator.GetInstance<SdkTelemetryInformation>().Returns(new SdkTelemetryInformation { FileVersion = "1.0.0.0" });
            this.subServiceLocator.GetInstance<ICryptographer>().Returns(new Cryptographer());
            this.subServiceLocator.GetInstance<IHealthServiceResponseParser>().Returns(new HealthServiceResponseParser());

            Ioc.Container.RegisterTransient<IPersonClient, PersonClient>();
        }

        [TestMethod]
        public async Task WhenAuthenticateCalledWithNoStoredInfo_ThenInfoIsFetchedAndStored()
        {
            this.SetupEmptyLocalStore();

            this.healthVaultConfiguration.MasterApplicationId = MasterApplicationId;

            var responseMessage1 = GenerateResponseMessage("NewApplicationCreationInfoResult.xml");
            var responseMessage2 = GenerateResponseMessage("GetServiceDefinitionResult.xml");
            // #3 is CAST call - but goes through IClientSessionCredentialClient and not HealthWebRequestClient
            var responseMessage4 = GenerateResponseMessage("GetAuthorizedPeopleResult.xml");

            // The first few calls use the default endpoint
            this.subHealthWebRequestClient
                .SendAsync(
                    new Uri("https://platform2.healthvault.com/platform/wildcat.ashx"),
                    Arg.Any<byte[]>(),
                    Arg.Any<int>(),
                    Arg.Any<IDictionary<string, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(responseMessage1, responseMessage2);

            // After GetServiceDefinition called, we are calling new endpoint
            this.subHealthWebRequestClient
                .SendAsync(
                    new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx"),
                    Arg.Any<byte[]>(),
                    Arg.Any<int>(),
                    Arg.Any<IDictionary<string, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(responseMessage4);

            var sessionCredential = new SessionCredential
            {
                Token = SessionToken,
                SharedSecret = SessionSharedSecret
            };

            this.subClientSessionCredentialClient
                .GetSessionCredentialAsync(Arg.Any<CancellationToken>())
                .Returns(sessionCredential);

            this.subServiceLocator
                .GetInstance<IClientSessionCredentialClient>()
                .Returns(this.subClientSessionCredentialClient);

            // These values match the values in NewApplicationCreationInfoResult.xml
            this.subShellAuthService
                .ProvisionApplicationAsync(
                    new Uri("https://account.healthvault.com"),
                    MasterApplicationId,
                    ApplicationCreationToken,
                    ApplicationInstanceId)
                .Returns("1");

            HealthVaultSodaConnection healthVaultSodaConnection = this.CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthenticateAsync();

            this.subClientSessionCredentialClient.Received().AppSharedSecret = ApplicationSharedSecret;
            this.subClientSessionCredentialClient.Received().Connection = healthVaultSodaConnection;

            await this.subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.ServiceInstanceKey,
                    Arg.Is<object>(o => ((HealthServiceInstance)o).HealthServiceUrl == new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx")));
            await this.subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.ApplicationCreationInfoKey, 
                    Arg.Is<object>(o => ((ApplicationCreationInfo)o).AppInstanceId == new Guid("b5c5593b-afb4-466d-88f2-31707fb8634b")));
            await this.subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.SessionCredentialKey, 
                    Arg.Is<object>(o => ((SessionCredential)o).SharedSecret == SessionSharedSecret));
            await this.subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.PersonInfoKey,
                    Arg.Is<object>(o => ((PersonInfo)o).Name == "David Rickard"));
        }

        [TestMethod]
        public async Task WhenAuthenticateCalledWithStoredInfo_ThenSessionCredentialPopulated()
        {
            this.SetupLocalStore();

            HealthVaultSodaConnection healthVaultSodaConnection = this.CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthenticateAsync();

            Assert.IsNotNull(healthVaultSodaConnection.SessionCredential);
        }

        [TestMethod]
        public async Task WhenAuthorizingAdditionalRecords_ThenShellAuthServiceInvoked()
        {
            this.SetupLocalStore();

            var responseMessage = GenerateResponseMessage("GetAuthorizedPeopleResult.xml");
            this.subHealthWebRequestClient
                .SendAsync(
                    new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx"),
                    Arg.Any<byte[]>(),
                    Arg.Any<int>(),
                    Arg.Any<IDictionary<string, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(responseMessage);

            HealthVaultSodaConnection healthVaultSodaConnection = this.CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthorizeAdditionalRecordsAsync();

            await this.subShellAuthService.Received()
                .AuthorizeAdditionalRecordsAsync(new Uri("https://account.healthvault-ppe.com/"), MasterApplicationId);

            await this.subLocalObjectStore.Received()
                .WriteAsync(HealthVaultSodaConnection.PersonInfoKey, Arg.Any<object>());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public async Task WhenAuthorizingAdditionalRecordsBeforeFirstAuth_ThenInvalidOperationExceptionThrown()
        {
            this.SetupEmptyLocalStore();

            HealthVaultSodaConnection healthVaultSodaConnection = this.CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthorizeAdditionalRecordsAsync();
        }

        private static HttpResponseMessage GenerateResponseMessage(string samplePath)
        {
            var responseMessage = Substitute.For<HttpResponseMessage>();
            var content = Substitute.For<HttpContent>();
            content.ReadAsStreamAsync().Returns(new MemoryStream(Encoding.UTF8.GetBytes(SampleUtils.GetSampleContent(samplePath))));
            responseMessage.Content = content;

            return responseMessage;
        }

        private void SetupLocalStore()
        {
            var serviceInstance = new HealthServiceInstance
            {
                Id = "1",
                Name = "US",
                Description = "US instance",
                HealthServiceUrl = new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx"),
                ShellUrl = new Uri("https://account.healthvault-ppe.com/")
            };

            var applicationCreationInfo = new ApplicationCreationInfo
            {
                AppInstanceId = new Guid(ApplicationInstanceId),
                SharedSecret = ApplicationSharedSecret,
                AppCreationToken = ApplicationCreationToken
            };

            var sessionCredential = new SessionCredential
            {
                Token = SessionToken,
                SharedSecret = SessionSharedSecret
            };

            var personInfo = new PersonInfo();

            this.subLocalObjectStore
                .ReadAsync<HealthServiceInstance>(HealthVaultSodaConnection.ServiceInstanceKey)
                .Returns(serviceInstance);

            this.subLocalObjectStore
                .ReadAsync<ApplicationCreationInfo>(HealthVaultSodaConnection.ApplicationCreationInfoKey)
                .Returns(applicationCreationInfo);

            this.subLocalObjectStore
                .ReadAsync<SessionCredential>(HealthVaultSodaConnection.SessionCredentialKey)
                .Returns(sessionCredential);

            this.subLocalObjectStore
                .ReadAsync<PersonInfo>(HealthVaultSodaConnection.PersonInfoKey)
                .Returns(personInfo);
        }

        private void SetupEmptyLocalStore()
        {
            this.subLocalObjectStore
                .ReadAsync<HealthServiceInstance>(HealthVaultSodaConnection.ServiceInstanceKey)
                .Returns((HealthServiceInstance)null);

            this.subLocalObjectStore
                .ReadAsync<ApplicationCreationInfo>(HealthVaultSodaConnection.ApplicationCreationInfoKey)
                .Returns((ApplicationCreationInfo)null);

            this.subLocalObjectStore
                .ReadAsync<SessionCredential>(HealthVaultSodaConnection.SessionCredentialKey)
                .Returns((SessionCredential)null);

            this.subLocalObjectStore
                .ReadAsync<PersonInfo>(HealthVaultSodaConnection.PersonInfoKey)
                .Returns((PersonInfo)null);
        }

        private HealthVaultSodaConnection CreateHealthVaultSodaConnection()
        {
            return new HealthVaultSodaConnection(
                this.subServiceLocator,
                this.subLocalObjectStore,
                this.subShellAuthService);
        }
    }
}
