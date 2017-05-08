// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Arg = NSubstitute.Arg;

namespace Microsoft.HealthVault.Web.UnitTest
{
    /// <summary>
    /// Contains unit tests for <see cref="WebHealthVaultFactory"/> for online scenarios
    /// </summary>
    [TestClass]
    public class WebHealthVaultFactoryOnlineUnitTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();
        }

        [TestMethod]
        public async Task WhenUserPrincipalDoesntExist_ThenAnonymousConnectionCreated()
        {
            // Arrange
            // Mocked identity provider would return a null value for "TryGetIdentity", in which case
            // an anonymous web connection is created
            IHealthVaultIdentityProvider healthVaultIdentityProvider = Substitute.For<IHealthVaultIdentityProvider>();
            Ioc.Container.Configure(c => c.ExportInstance(healthVaultIdentityProvider).As<IHealthVaultIdentityProvider>());

            IWebHealthVaultConnection webHealthVaultConnection = Substitute.For<IWebHealthVaultConnection>();
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConnection).As<IWebHealthVaultConnection>());

            WebHealthVaultFactory factory = new WebHealthVaultFactory();

            // Act
            IWebHealthVaultConnection resultWebHealthVaultConnection = await factory.CreateWebConnectionInternalAsync();

            // Assert
            Assert.IsNotNull(resultWebHealthVaultConnection);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task WhenUserIdentityHasNoWebConnectionInfo_ThenExceptionRaised()
        {
            // Arrange
            IHealthVaultIdentityProvider healthVaultIdentityProvider = Substitute.For<IHealthVaultIdentityProvider>();
            healthVaultIdentityProvider.TryGetIdentity().Returns(new HealthVaultIdentity());
            Ioc.Container.Configure(c => c.ExportInstance(healthVaultIdentityProvider).As<IHealthVaultIdentityProvider>());

            IWebHealthVaultConnection webHealthVaultConnection = Substitute.For<IWebHealthVaultConnection>();
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConnection).As<IWebHealthVaultConnection>());

            WebHealthVaultFactory factory = new WebHealthVaultFactory();

            // Act
            await factory.CreateWebConnectionInternalAsync();
        }

        /// <summary>
        /// Verify that <see cref="WebHealthVaultFactory"/> properties are hydrated
        /// from web connection info provided by <see cref="IHealthVaultIdentityProvider"/>
        /// </summary>
        [TestMethod]
        public async Task WhenUserIdentityHasWebConnectionInfo()
        {
            // Arrange
            var webConnectionInfo = new WebConnectionInfo
            {
                ServiceInstanceId = "1",
                PersonInfo = new PersonInfo(),
                SessionCredential = new SessionCredential(),
                UserAuthToken = "some"
            };

            // Mock HealthVaultIdentityProvider
            IHealthVaultIdentityProvider healthVaultIdentityProvider = Substitute.For<IHealthVaultIdentityProvider>();
            healthVaultIdentityProvider.TryGetIdentity().Returns(new HealthVaultIdentity
            {
                WebConnectionInfo = webConnectionInfo
            });
            Ioc.Container.Configure(c => c.ExportInstance(healthVaultIdentityProvider).As<IHealthVaultIdentityProvider>());

            // Mock HealthVaultConnection
            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration();
            webHealthVaultConfiguration.DefaultHealthVaultUrl = new Uri("http://www.bing.com");
            webHealthVaultConfiguration.DefaultHealthVaultShellUrl = new Uri("http://www.bing.com");

            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);
            serviceLocator.GetInstance<IHealthWebRequestClient>().Returns(Substitute.For<IHealthWebRequestClient>());
            serviceLocator
                .GetInstance<IHealthServiceResponseParser>()
                .Returns(Substitute.For<IHealthServiceResponseParser>());

            WebHealthVaultConnection webHealthVaultConnection = Substitute.For<WebHealthVaultConnection>(serviceLocator);
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConnection).As<IWebHealthVaultConnection>());

            // Mock ServiceInstanceProvider
            IServiceInstanceProvider serviceInstanceProvider = Substitute.For<IServiceInstanceProvider>();
            serviceInstanceProvider
                .GetHealthServiceInstanceAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new HealthServiceInstance()));
            Ioc.Container.Configure(c => c.ExportInstance(serviceInstanceProvider).As<IServiceInstanceProvider>());

            WebHealthVaultFactory factory = new WebHealthVaultFactory();

            // Act
            IWebHealthVaultConnection resultWebHealthVaultConnection = await factory.CreateWebConnectionInternalAsync();

            // Assert
            Assert.AreEqual(webConnectionInfo.UserAuthToken, resultWebHealthVaultConnection.UserAuthToken);
        }
    }
}
