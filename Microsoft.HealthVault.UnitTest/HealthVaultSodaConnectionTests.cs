// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Arg = NSubstitute.Arg;

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

        private static readonly Guid RecordId = Guid.Parse("51c6cdcc-a5b3-438c-95b9-9602ab92e1e4");
        private static readonly Guid PersonId = Guid.Parse("ef17ac35-adc6-4a5a-afc0-84dc8937caac");

        private IServiceLocator _subServiceLocator;
        private IHealthWebRequestClient _subHealthWebRequestClient;
        private ILocalObjectStore _subLocalObjectStore;
        private IShellAuthService _subShellAuthService;
        private IMessageHandlerFactory _subMessageHandlerFactory;
        private IClientSessionCredentialClient _subClientSessionCredentialClient;
        private HealthVaultConfiguration _healthVaultConfiguration;

        private static readonly Guid s_masterApplicationId = new Guid("30945bac-d221-4f89-8197-6983a390066d");

        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();

            _subServiceLocator = Substitute.For<IServiceLocator>();
            _subHealthWebRequestClient = Substitute.For<IHealthWebRequestClient>();
            _subLocalObjectStore = Substitute.For<ILocalObjectStore>();
            _subShellAuthService = Substitute.For<IShellAuthService>();
            _subMessageHandlerFactory = Substitute.For<IMessageHandlerFactory>();
            _subClientSessionCredentialClient = Substitute.For<IClientSessionCredentialClient>();
            _healthVaultConfiguration = new HealthVaultConfiguration
            {
                MasterApplicationId = s_masterApplicationId,
                DefaultHealthVaultUrl = new Uri("https://platform2.healthvault.com/platform/"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault.com")
            };

            _subServiceLocator.GetInstance<HealthVaultConfiguration>().Returns(_healthVaultConfiguration);
            _subServiceLocator.GetInstance<IHealthWebRequestClient>().Returns(_subHealthWebRequestClient);
            _subServiceLocator.GetInstance<SdkTelemetryInformation>().Returns(new SdkTelemetryInformation { FileVersion = "1.0.0.0" });
            _subServiceLocator.GetInstance<ICryptographer>().Returns(new Cryptographer());
            _subServiceLocator.GetInstance<IHealthServiceResponseParser>().Returns(new HealthServiceResponseParser());

            Ioc.Container.RegisterTransient<IPersonClient, PersonClient>();
        }

        [TestMethod]
        public async Task WhenAuthenticateCalledWithNoStoredInfo_ThenInfoIsFetchedAndStored()
        {
            SetupEmptyLocalStore();

            _healthVaultConfiguration.MasterApplicationId = s_masterApplicationId;

            var responseMessage1 = GenerateResponseMessage("NewApplicationCreationInfoResult.xml");
            var responseMessage2 = GenerateResponseMessage("GetServiceDefinitionResult.xml");

            // #3 is CAST call - but goes through IClientSessionCredentialClient and not HealthWebRequestClient
            var responseMessage4 = GenerateResponseMessage("GetAuthorizedPeopleResult.xml");

            // The first few calls use the default endpoint
            _subHealthWebRequestClient
                .SendAsync(
                    new Uri("https://platform2.healthvault.com/platform/wildcat.ashx"),
                    Arg.Any<byte[]>(),
                    Arg.Any<int>(),
                    Arg.Any<IDictionary<string, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(responseMessage1, responseMessage2);

            // After GetServiceDefinition called, we are calling new endpoint
            _subHealthWebRequestClient
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

            _subClientSessionCredentialClient
                .GetSessionCredentialAsync(Arg.Any<CancellationToken>())
                .Returns(sessionCredential);

            _subServiceLocator
                .GetInstance<IClientSessionCredentialClient>()
                .Returns(_subClientSessionCredentialClient);

            // These values match the values in NewApplicationCreationInfoResult.xml
            _subShellAuthService
                .ProvisionApplicationAsync(
                    new Uri("https://account.healthvault.com"),
                    s_masterApplicationId,
                    ApplicationCreationToken,
                    ApplicationInstanceId)
                .Returns("1");

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthenticateAsync();

            _subClientSessionCredentialClient.Received().AppSharedSecret = ApplicationSharedSecret;
            _subClientSessionCredentialClient.Received().Connection = healthVaultSodaConnection;

            await _subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.ServiceInstanceKey,
                    Arg.Is<object>(o => ((HealthServiceInstance)o).HealthServiceUrl == new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx")));
            await _subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.ApplicationCreationInfoKey,
                    Arg.Is<object>(o => ((ApplicationCreationInfo)o).AppInstanceId == new Guid("b5c5593b-afb4-466d-88f2-31707fb8634b")));
            await _subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.SessionCredentialKey,
                    Arg.Is<object>(o => ((SessionCredential)o).SharedSecret == SessionSharedSecret));
            await _subLocalObjectStore.Received()
                .WriteAsync(
                    HealthVaultSodaConnection.PersonInfoKey,
                    Arg.Is<object>(o => ((PersonInfo)o).Name == "David Rickard"));
        }

        [TestMethod]
        public async Task WhenAuthenticateCalledWithStoredInfo_ThenSessionCredentialPopulated()
        {
            SetupLocalStore();

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthenticateAsync();

            Assert.IsNotNull(healthVaultSodaConnection.SessionCredential);
        }

        [TestMethod]
        public async Task WhenAuthorizingAdditionalRecords_ThenShellAuthServiceInvoked()
        {
            SetupLocalStore();

            var responseMessage = GenerateResponseMessage("GetAuthorizedPeopleResult.xml");
            _subHealthWebRequestClient
                .SendAsync(
                    new Uri("https://platform.healthvault-ppe.com/platform/wildcat.ashx"),
                    Arg.Any<byte[]>(),
                    Arg.Any<int>(),
                    Arg.Any<IDictionary<string, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(responseMessage);

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthorizeAdditionalRecordsAsync();

            await _subShellAuthService.Received()
                .AuthorizeAdditionalRecordsAsync(new Uri("https://account.healthvault-ppe.com/"), s_masterApplicationId);

            await _subLocalObjectStore.Received()
                .WriteAsync(HealthVaultSodaConnection.PersonInfoKey, Arg.Any<object>());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public async Task WhenAuthorizingAdditionalRecordsBeforeFirstAuth_ThenInvalidOperationExceptionThrown()
        {
            SetupEmptyLocalStore();

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();
            await healthVaultSodaConnection.AuthorizeAdditionalRecordsAsync();
        }

        [TestMethod]
        public void WhenMessageHandlerCreateCalled_ThenFactoryIsInvoked()
        {
            var handler = new HttpClientHandler();
            _subMessageHandlerFactory.Create().Returns(handler);

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();
            IMessageHandlerFactory factory = healthVaultSodaConnection;
            var handlerResult = factory.Create();

            Assert.AreEqual(handler, handlerResult);
        }

        [TestMethod]
        public async Task WhenAuthorizeRestRequestInvoked_ThenHeadersArePopulated()
        {
            SetupLocalStore();

            HealthVaultSodaConnection healthVaultSodaConnection = CreateHealthVaultSodaConnection();

            HttpRequestMessage message = new HttpRequestMessage();

            await healthVaultSodaConnection.AuthorizeRestRequestAsync(message, RecordId);

            Assert.AreEqual("MSH-V1", message.Headers.Authorization.Scheme);

            string authParameters = message.Headers.Authorization.Parameter;
            List<string> authParametersList = authParameters.Split(',').ToList();

            Assert.IsTrue(authParametersList.Contains("app-token=" + SessionToken));
            Assert.IsTrue(authParametersList.Contains("offline-person-id=" + PersonId));
            Assert.IsTrue(authParametersList.Contains("record-id=" + RecordId));
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
                SharedSecret = SessionSharedSecret,
                ExpirationUtc = DateTimeOffset.UtcNow.AddHours(4)
            };

            var personInfo = new PersonInfo
            {
                PersonId = PersonId
            };

            _subLocalObjectStore
                .ReadAsync<HealthServiceInstance>(HealthVaultSodaConnection.ServiceInstanceKey)
                .Returns(serviceInstance);

            _subLocalObjectStore
                .ReadAsync<ApplicationCreationInfo>(HealthVaultSodaConnection.ApplicationCreationInfoKey)
                .Returns(applicationCreationInfo);

            _subLocalObjectStore
                .ReadAsync<SessionCredential>(HealthVaultSodaConnection.SessionCredentialKey)
                .Returns(sessionCredential);

            _subLocalObjectStore
                .ReadAsync<PersonInfo>(HealthVaultSodaConnection.PersonInfoKey)
                .Returns(personInfo);
        }

        private void SetupEmptyLocalStore()
        {
            _subLocalObjectStore
                .ReadAsync<HealthServiceInstance>(HealthVaultSodaConnection.ServiceInstanceKey)
                .Returns((HealthServiceInstance)null);

            _subLocalObjectStore
                .ReadAsync<ApplicationCreationInfo>(HealthVaultSodaConnection.ApplicationCreationInfoKey)
                .Returns((ApplicationCreationInfo)null);

            _subLocalObjectStore
                .ReadAsync<SessionCredential>(HealthVaultSodaConnection.SessionCredentialKey)
                .Returns((SessionCredential)null);

            _subLocalObjectStore
                .ReadAsync<PersonInfo>(HealthVaultSodaConnection.PersonInfoKey)
                .Returns((PersonInfo)null);
        }

        private HealthVaultSodaConnection CreateHealthVaultSodaConnection()
        {
            return new HealthVaultSodaConnection(
                _subServiceLocator,
                _subLocalObjectStore,
                _subShellAuthService,
                _subMessageHandlerFactory);
        }
    }
}
