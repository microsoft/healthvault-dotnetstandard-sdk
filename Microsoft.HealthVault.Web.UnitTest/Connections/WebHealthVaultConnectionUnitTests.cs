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
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Arg = NSubstitute.Arg;

namespace Microsoft.HealthVault.Web.UnitTest.Connections
{
    /// <summary>
    /// Verifies functionality for online healthvault connection apps
    /// </summary>
    [TestClass]
    public class WebHealthVaultConnectionUnitTests
    {
        private const string SessionSharedSecret = "abc";
        private const string SessionToken = "def";
        private static readonly Guid RecordId = Guid.Parse("51c6cdcc-a5b3-438c-95b9-9602ab92e1e4");

        private WebHealthVaultConnection _webHealthVaultConnection;
        private string _userAuthToken;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            Ioc.Container = new DependencyInjectionContainer();

            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(new WebHealthVaultConfiguration
            {
                DefaultHealthVaultUrl = new Uri("http://www.bing.com"),
                DefaultHealthVaultShellUrl = new Uri("http://www.bing.com")
            });

            HealthServiceInstance healthServiceInstance = Substitute.For<HealthServiceInstance>();
            SessionCredential sessionCredential = new SessionCredential
            {
                ExpirationUtc = DateTimeOffset.UtcNow.AddHours(4),
                SharedSecret = SessionSharedSecret,
                Token = SessionToken
            };
            _userAuthToken = "someToken";

            _webHealthVaultConnection = new WebHealthVaultConnection(serviceLocator)
            {
                UserAuthToken = _userAuthToken,
                ServiceInstance = healthServiceInstance,
                SessionCredential = sessionCredential
            };
        }

        [TestMethod]
        public async Task WhenAuthorizeRestRequestInvoked_ThenHeadersArePopulated()
        {
            HttpRequestMessage message = new HttpRequestMessage();

            await _webHealthVaultConnection.AuthorizeRestRequestAsync(message, RecordId);

            Assert.AreEqual("MSH-V1", message.Headers.Authorization.Scheme);

            string authParameters = message.Headers.Authorization.Parameter;
            List<string> authParametersList = authParameters.Split(',').ToList();

            Assert.IsTrue(authParametersList.Contains("app-token=" + SessionToken));
            Assert.IsTrue(authParametersList.Contains("user-token=someToken"));
            Assert.IsTrue(authParametersList.Contains("record-id=" + RecordId));
        }

        /// <summary>
        /// Verifies the format of auth session header in case of Xml over Http protocol
        /// </summary>
        [TestMethod]
        public void WhenAuthSessionHeaderIsInvoked()
        {
            // Act
            AuthSession authSession = _webHealthVaultConnection.GetAuthSessionHeader();

            // Assert
            Assert.AreEqual(_webHealthVaultConnection.UserAuthToken, authSession.UserAuthToken);
        }

        /// <summary>
        /// Verifies that WebHealthVaultConnection restores PersonInfo
        /// </summary>
        [TestMethod]
        public async Task WhenPersonInfoRestoredFromIdentity()
        {
            var webConnectionInfo = CreateWebConnectionInfo();

            PersonInfo personInfo = await _webHealthVaultConnection.GetPersonInfoAsync();

            Assert.AreEqual(webConnectionInfo.PersonInfo.Name, personInfo.Name);
        }

        /// <summary>
        /// Verify that when a cookie is restored with minimized application setting, then
        /// a call to platform is made to populate the application settings from server
        /// </summary>
        [TestMethod]
        public async Task WhenPersonInfoRestoredFromIdentityWithMinimizedApplicationSettings()
        {
            CreateWebConnectionInfo(minimizedApplicationSettings: true);

            PersonInfo mockedPersonInfoWithApplicationSettings = new PersonInfo()
            {
                Name = "Test",
                ApplicationSettingsDocument = Substitute.For<XDocument>()
            };

            IPersonClient personClient = Substitute.For<IPersonClient>();
            personClient.GetPersonInfoAsync().Returns(Task.FromResult(mockedPersonInfoWithApplicationSettings));

            Ioc.Container.Configure(c => c.ExportInstance(f => personClient).As<IPersonClient>());

            PersonInfo personInfo = await _webHealthVaultConnection.GetPersonInfoAsync();

            Assert.AreEqual(mockedPersonInfoWithApplicationSettings.Name, personInfo.Name);
            Assert.IsNotNull(personInfo.ApplicationSettingsDocument);
        }

        /// <summary>
        /// Verify that when a cookie is restored with minimized authorized records, then
        /// a call to platform is made to populate the AuthorizedRecords from server
        /// </summary>
        [TestMethod]
        public async Task WhenPersonInfoRestoredFromIdentityWithMinimizedAuthorizedRecords()
        {
            CreateWebConnectionInfo(minimizedPersonInfoRecords: true);

            PersonInfo mockedPersonInfoWithExpandedAuthorizedRecords = new PersonInfo()
            {
                Name = "Test",
                AuthorizedRecords = new Dictionary<Guid, HealthRecordInfo>
                {
                    { Guid.NewGuid(), new HealthRecordInfo() },
                    { Guid.NewGuid(), new HealthRecordInfo() }
                }
            };

            IPersonClient personClient = Substitute.For<IPersonClient>();
            personClient.GetPersonInfoAsync().Returns(Task.FromResult(mockedPersonInfoWithExpandedAuthorizedRecords));

            Ioc.Container.Configure(c => c.ExportInstance(f => personClient).As<IPersonClient>());

            PersonInfo personInfo = await _webHealthVaultConnection.GetPersonInfoAsync();

            Assert.AreEqual(mockedPersonInfoWithExpandedAuthorizedRecords.Name, personInfo.Name);
            Assert.AreEqual(
                mockedPersonInfoWithExpandedAuthorizedRecords.AuthorizedRecords.Count,
                personInfo.AuthorizedRecords.Count);
        }

        private WebConnectionInfo CreateWebConnectionInfo(
            bool minimizedPersonInfoRecords = false,
            bool minimizedApplicationSettings = false)
        {
            var webConnectionInfo = new WebConnectionInfo
            {
                MinimizedPersonInfoRecords = minimizedPersonInfoRecords,
                MinimizedPersonInfoApplicationSettings = minimizedApplicationSettings,
                PersonInfo = new PersonInfo
                {
                    Name = "Test",
                    AuthorizedRecords = new Dictionary<Guid, HealthRecordInfo> { { Guid.NewGuid(), new HealthRecordInfo() } }
                },
                ServiceInstanceId = "1",
                SessionCredential = new SessionCredential(),
                UserAuthToken = "someToken"
            };

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://www.bing.com", ""),
                new HttpResponse(new StringWriter()));

            HttpContext.Current.User = new GenericPrincipal(
                new HealthVaultIdentity { WebConnectionInfo = webConnectionInfo },
                null);
            return webConnectionInfo;
        }
    }
}
