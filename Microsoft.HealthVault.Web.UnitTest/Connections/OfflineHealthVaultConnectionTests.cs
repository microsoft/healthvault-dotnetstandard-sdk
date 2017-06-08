// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.Web.UnitTest.Connections
{
    /// <summary>
    /// Verifies functionality for offline healthvault web connection apps
    /// </summary>
    [TestClass]
    public class OfflineHealthVaultConnectionTests
    {
        private const string SessionSharedSecret = "abc";
        private const string SessionToken = "def";
        private static readonly Guid RecordId = Guid.Parse("51c6cdcc-a5b3-438c-95b9-9602ab92e1e4");
        private static readonly Guid OfflinePersonId = Guid.Parse("ef17ac35-adc6-4a5a-afc0-84dc8937caac");

        private OfflineHealthVaultConnection _offlineHealthVaultConnection;

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

            HealthServiceInstance healthServiceInstance = new HealthServiceInstance();
            SessionCredential sessionCredential = new SessionCredential
            {
                ExpirationUtc = DateTimeOffset.UtcNow.AddHours(4),
                SharedSecret = SessionSharedSecret,
                Token = SessionToken
            };

            _offlineHealthVaultConnection =
                new OfflineHealthVaultConnection(serviceLocator)
                {
                    OfflinePersonId = OfflinePersonId,
                    ServiceInstance = healthServiceInstance,
                    SessionCredential = sessionCredential
                };
        }

        /// <summary>
        /// In case of offline connection, there is no concept of PersonInfo.
        /// Offline Connection is kind of anonymous connection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task WhenPersonInfoIsCalled()
        {
            // Act
            await _offlineHealthVaultConnection.GetPersonInfoAsync();
        }

        [TestMethod]
        public async Task WhenAuthorizeRestRequestInvoked_ThenHeadersArePopulated()
        {
            HttpRequestMessage message = new HttpRequestMessage();

            await _offlineHealthVaultConnection.AuthorizeRestRequestAsync(message, RecordId);

            Assert.AreEqual("MSH-V1", message.Headers.Authorization.Scheme);

            string authParameters = message.Headers.Authorization.Parameter;
            List<string> authParametersList = authParameters.Split(',').ToList();

            Assert.IsTrue(authParametersList.Contains("app-token=" + SessionToken));
            Assert.IsTrue(authParametersList.Contains("offline-person-id=" + OfflinePersonId));
            Assert.IsTrue(authParametersList.Contains("record-id=" + RecordId));
        }

        /// <summary>
        /// Verifies the format of auth session header in case of Xml over Http protocol
        /// </summary>
        [TestMethod]
        public void WhenAuthSessionHeaderIsRequested()
        {
            // Act
            AuthSession authSession = _offlineHealthVaultConnection.GetAuthSessionHeader();

            // Assert
            Assert.AreEqual(authSession.Person.OfflinePersonId, OfflinePersonId);
        }
    }
}
