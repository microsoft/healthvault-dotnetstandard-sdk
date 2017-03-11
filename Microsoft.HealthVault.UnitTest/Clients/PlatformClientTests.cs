using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class PlatformClientTests
    {
        [TestMethod]
        public async void SelectInstanceAsyncTest()
        {
            var location = new Location("USA", "Washington");
            var platformClient = Substitute.For<IPlatformClient>();
            var result = await platformClient.SelectInstanceAsync(location).ConfigureAwait(false);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public async void GetServiceDefinitionAsyncTest()
        {
            var platformClient = Substitute.For<IPlatformClient>();
            var result = await platformClient.GetServiceDefinitionAsync().ConfigureAwait(false);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public async void GetServiceDefinitionAsyncWithSectionsTest()
        {
            var platformClient = Substitute.For<IPlatformClient>();
            var result = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.All).ConfigureAwait(false);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public async void GetServiceDefinitionAsyncWithDateTimeTest()
        {
            DateTime lastUpdatedTime = DateTime.Now;
            var platformClient = Substitute.For<IPlatformClient>();
            var result = await platformClient.GetServiceDefinitionAsync(lastUpdatedTime).ConfigureAwait(false);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public async void GetServiceDefinitionAsyncWithSectionsDateTimeTest()
        {
            DateTime lastUpdatedTime = DateTime.Now;
            var platformClient = Substitute.For<IPlatformClient>();
            var result = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.All, lastUpdatedTime).ConfigureAwait(false);

            Assert.IsTrue(result != null);
        }
    }
}
