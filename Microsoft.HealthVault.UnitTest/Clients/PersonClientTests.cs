using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.UnitTest.Samples;

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
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("PersonInfoSample.xml"))).CreateNavigator(),
                ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(SampleUtils.GetSampleContent("PersonInfoSample.xml")))
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
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("AppSettingsSample.xml"))).CreateNavigator(),
                ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(SampleUtils.GetSampleContent("AppSettingsSample.xml")))
            };

            connection.ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>()).Returns(response);

            var result = await personClient.GetApplicationSettingsAsync();

            await connection.Received().ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>(), null);
            Assert.IsNotNull(result.SelectedRecordId);
        }

		[TestMethod]
		public async Task SetApplicationSettingsWitXPathNavTest()
		{
            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("AppSettingsSample.xml"))).CreateNavigator(),
                ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(SampleUtils.GetSampleContent("AppSettingsSample.xml")))
            };

		    var nav = response.InfoNavigator;

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
           
            IList<Guid> recordIds = new List<Guid> { new Guid("7a231675-4e78-451f-b94d-1e05b2a24586") };
            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("AuthorizedRecordsSample.xml"))).CreateNavigator(),
                ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(SampleUtils.GetSampleContent("AuthorizedRecordsSample.xml")))
            };

            connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>()).Returns(response);
            var result = await personClient.GetAuthorizedRecordsAsync(recordIds).ConfigureAwait(false);

            await connection.Received().ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>());
        }
    }
}
