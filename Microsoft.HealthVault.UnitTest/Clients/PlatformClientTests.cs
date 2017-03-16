using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class PlatformClientTests
    {
        private IConnectionInternal connection;
        private PlatformClient platformClient;
        private HealthServiceResponseData responseDate;

        [TestInitialize]
        public void InitializeTest()
        {
            connection = Substitute.For<IConnectionInternal>();
            platformClient = new PlatformClient { Connection = this.connection };
            responseDate = GetDefinition();
        }

        [TestMethod]
        public async Task SelectInstanceTest()
        {
            var location = new Location("US", "Washington");

            var response = new HealthServiceResponseData
            {
                // TODO: Add samples to William's samples folder to be able to use this code below
                // InfoNavigator = new XPathDocument(new StringReader(instanceSample)).CreateNavigator(),
                // ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(instanceSample))
            };

            connection.ExecuteAsync(HealthVaultMethods.SelectInstance, Arg.Any<int>(), Arg.Any<string>()).Returns(response);

            var result = await platformClient.SelectInstanceAsync(location).ConfigureAwait(false);

            await this.connection.Received().ExecuteAsync(HealthVaultMethods.SelectInstance, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetServiceDefinitionTest()
        {
            var response = GetDefinition();

            connection.ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>()).Returns(response);

            var result = await platformClient.GetServiceDefinitionAsync().ConfigureAwait(false);
            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetServiceDefinitionWithSectionsTest()
        {
            var response = GetDefinition();

            connection.ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>()).Returns(response);
            var result = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.All).ConfigureAwait(false);
            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetServiceDefinitionWithDateTimeTest()
        {
            DateTime lastUpdatedTime = DateTime.Now;
            var response = GetDefinition();

            connection.ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>()).Returns(response);

            var result = await platformClient.GetServiceDefinitionAsync(lastUpdatedTime).ConfigureAwait(false);
            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetServiceDefinitionWithSectionsDateTimeTest()
        {
            DateTime lastUpdatedTime = DateTime.Now;
            var response = GetDefinition();

            connection.ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>()).Returns(response);

            var result = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.All, lastUpdatedTime).ConfigureAwait(false);
            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetServiceDefinition, Arg.Any<int>(), Arg.Any<string>());
        }
        private HealthServiceResponseData GetDefinition()
        {
            if (responseDate == null)
            {
                responseDate = new HealthServiceResponseData
                {
                    // TODO: Add samples to William's samples folder to be able to use this code below
                    // InfoNavigator = new XPathDocument(new StringReader(serviceDefinitionSample)).CreateNavigator(),
                    // ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(serviceDefinitionSample))
                };
            }
            return responseDate;
        }
    }
}
