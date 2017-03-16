using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.UnitTest.Clients
{
	[TestClass]
	public class PersonClientTests
	{
        private IConnectionInternal connection;
        private PersonClient personClient;

        [TestInitialize]
        public void InitializeTest()
        {
            connection = Substitute.For<IConnectionInternal>();
            personClient = new PersonClient { Connection = this.connection};
        }

        [TestMethod]
		public async Task GetPersonInfoTest()
		{
            var response = new HealthServiceResponseData
            {
                // TODO: Add samples to William's samples folder to be able to use this code below
                // InfoNavigator = new XPathDocument(new StringReader(personInfoSample)).CreateNavigator(),
                // ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(personInfoSample))
            };

            connection.ExecuteAsync(HealthVaultMethods.GetPersonInfo, Arg.Any<int>()).Returns(response);

            var result = await personClient.GetPersonInfoAsync();

            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetPersonInfo, Arg.Any<int>());
            Assert.IsNotNull(result.PersonId);
        }

        [TestMethod]
		public async Task GetApplicationSettingsTest()
		{
            var response = new HealthServiceResponseData
            {
                // TODO: Add samples to William's samples folder to be able to use this code below
                // InfoNavigator = new XPathDocument(new StringReader(appSettingSample)).CreateNavigator(),
                // ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(appSettingSample))
            };

            connection.ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>()).Returns(response);

            var result = await personClient.GetApplicationSettingsAsync();

            await connection.Received().ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>(), null);
            Assert.IsNotNull(result.SelectedRecordId);
        }

		[TestMethod]
		public async Task SetApplicationSettingsWitXPathNavTest()
		{
		    var nav = Substitute.For<IXPathNavigable>();

            await personClient.SetApplicationSettingsAsync(nav).ConfigureAwait(false);

            await connection.Received().ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
		public async Task SetApplicationSettingsWithRequestParametersTest()
		{
            string requestParameters = "<app-settings />";
            await personClient.SetApplicationSettingsAsync(requestParameters).ConfigureAwait(false);

            await connection.Received().ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetAuthorizedRecordsAsyncTest()
        {
            IList<Guid> recordIds = new List<Guid> { new Guid()};
            var response = new HealthServiceResponseData
            {
                // TODO: Add samples to William's samples folder to be able to use this code below
                // InfoNavigator = new XPathDocument(new StringReader(authhorizedRecordsSample)).CreateNavigator(),
                // ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(authhorizedRecordsSample))
            };

            connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>()).Returns(response);
            var result = await personClient.GetAuthorizedRecordsAsync(recordIds).ConfigureAwait(false);

            await connection.Received().ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>());
        }
    }
}
