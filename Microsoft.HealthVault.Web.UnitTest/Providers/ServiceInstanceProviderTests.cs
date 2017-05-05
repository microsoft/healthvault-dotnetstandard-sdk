// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace Microsoft.HealthVault.Web.UnitTest.Providers
{
    /// <summary>
    /// Verifies that <see cref="ServiceInstanceProvider"/> provides the
    /// ServiceInstance
    /// </summary>
    [TestClass]
    public class ServiceInstanceProviderTests
    {
        private const string UsInstanceId = "1";
        private ServiceInstanceProvider serviceInstanceProvider;
        private IWebHealthVaultConnection webHealthVaultConnection;

        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();

            // Arrange
            Ioc.Container = new DependencyInjectionContainer();
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();
            serviceInstanceProvider = new ServiceInstanceProvider(serviceLocator);
        }

        /// <summary>
        /// Verify that when US Instance ServiceInfo is requested from ServiceInstanceProvider
        /// then ServiceInstance for US is returned from ServiceInstanceProvider
        /// </summary>
        [TestMethod]
        public async Task WhenInstanceIsUs()
        {
            // Act
            var serviceInstance = await GetHealthServiceInstanceAsync();

            // Assert
            Assert.AreEqual(UsInstanceId, serviceInstance.Id);
        }

        /// <summary>
        /// Verify that when a service instance has been cached, then the cached instance is returned 
        /// without a need to call platform
        /// </summary>
        [TestMethod]
        public async Task WhenInstanceIsUsAndHasBeenCached()
        {
            // First call the platfom so that the service instance is cached
            var serviceInstance = await GetHealthServiceInstanceAsync();

            // Let's clear all the substitutes, there is no need to call
            // the platform when the instance has been cached earlier.
            webHealthVaultConnection.ClearSubstitute();

            var cachedServiceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync("1");

            Assert.AreEqual(serviceInstance.Id, cachedServiceInstance.Id);
        }

        /// <summary>
        /// We will mock webhealthvaultconnection and substitute CreatePlatformClient call
        /// on the mock to return a mock platform client. Platform client will substitute 
        /// call to platform "getservicedefintion" by returning a dummy serviceinstance with 
        /// id set to "US instance".
        /// </summary>
        private async Task<HealthServiceInstance> GetHealthServiceInstanceAsync()
        {
            webHealthVaultConnection = Substitute.For<IWebHealthVaultConnection>();
            Ioc.Container.Configure(c => { c.ExportInstance(webHealthVaultConnection).As<IWebHealthVaultConnection>(); });

            IPlatformClient platformClient = Substitute.For<IPlatformClient>();
            webHealthVaultConnection.CreatePlatformClient().Returns(platformClient);

            ServiceInfo serviceInfo = Substitute.For<ServiceInfo>();
            serviceInfo.ServiceInstances.Add(UsInstanceId, new HealthServiceInstance { Id = UsInstanceId });

            platformClient
                .GetServiceDefinitionAsync(ServiceInfoSections.Topology)
                .Returns(serviceInfo);

            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync("1");
            return serviceInstance;
        }
    }
}
