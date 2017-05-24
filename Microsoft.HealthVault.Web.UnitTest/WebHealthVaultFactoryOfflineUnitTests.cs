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
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Arg = NSubstitute.Arg;

namespace Microsoft.HealthVault.Web.UnitTest
{
    /// <summary>
    /// Contains unit tests for <see cref="WebHealthVaultFactory"/> for offline scenarios
    /// </summary>
    [TestClass]
    public class WebHealthVaultFactoryOfflineUnitTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task WhenOfflinePersonIdIsNull()
        {
            // Arrange
            WebHealthVaultFactory factory = new WebHealthVaultFactory();

            // Act
            await factory.CreateOfflineConnectionInternalAsync(null);
        }

        [TestMethod]
        public async Task WhenCreateOfflineConnection_WithOfflinePersonId()
        {
            // Arrange
            WebHealthVaultConfiguration configuration = new WebHealthVaultConfiguration
            {
                DefaultHealthVaultShellUrl = new Uri("http://www.bing.com"),
                DefaultHealthVaultUrl = new Uri("http://www.bing.com")
            };

            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(configuration);

            IOfflineHealthVaultConnection mokcedOfflineHealthVaultConnection = Substitute.For<OfflineHealthVaultConnection>(serviceLocator);
            Ioc.Container.Configure(c => c.ExportInstance(mokcedOfflineHealthVaultConnection).As<IOfflineHealthVaultConnection>());

            WebHealthVaultFactory factory = new WebHealthVaultFactory();
            string offlinePersonId = Guid.NewGuid().ToString();

            // Act
            IOfflineHealthVaultConnection offlineHealthVaultConnection = await factory.CreateOfflineConnectionInternalAsync(
                offlinePersonId: offlinePersonId);

            // Assert
            Assert.AreEqual(offlinePersonId, offlineHealthVaultConnection.OfflinePersonId.ToString());

            // Assert that default service instance is created and is set to default shell url from web configuration
            Assert.AreEqual(configuration.DefaultHealthVaultShellUrl, ((OfflineHealthVaultConnection)offlineHealthVaultConnection).ServiceInstance.ShellUrl);
        }

        [TestMethod]
        public async Task WhenCreateOfflineConnection_WithOfflinePersonIdAndInstanceId()
        {
            // Arrange
            WebHealthVaultConfiguration configuration = new WebHealthVaultConfiguration
            {
                DefaultHealthVaultShellUrl = new Uri("http://www.bing.com"),
                DefaultHealthVaultUrl = new Uri("http://www.bing.com")
            };

            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(configuration);

            IOfflineHealthVaultConnection mokcedOfflineHealthVaultConnection = Substitute.For<OfflineHealthVaultConnection>(serviceLocator);
            Ioc.Container.Configure(c => c.ExportInstance(mokcedOfflineHealthVaultConnection).As<IOfflineHealthVaultConnection>());

            WebHealthVaultFactory factory = new WebHealthVaultFactory();
            string offlinePersonId = Guid.NewGuid().ToString();

            IServiceInstanceProvider serviceInstanceProvider = Substitute.For<IServiceInstanceProvider>();
            serviceInstanceProvider.GetHealthServiceInstanceAsync(Arg.Any<string>())
                .Returns(new HealthServiceInstance { Name = "Test" });
            Ioc.Container.Configure(c => c.ExportInstance(serviceInstanceProvider).As<IServiceInstanceProvider>());

            string instanceId = Guid.NewGuid().ToString();

            // Act
            IOfflineHealthVaultConnection offlineHealthVaultConnection = await factory.CreateOfflineConnectionInternalAsync(
                offlinePersonId: offlinePersonId,
                instanceId: instanceId);

            // Assert
            Assert.AreEqual(offlinePersonId, offlineHealthVaultConnection.OfflinePersonId.ToString());
            Assert.AreEqual("Test", ((OfflineHealthVaultConnection)offlineHealthVaultConnection).ServiceInstance.Name);
        }
    }
}
