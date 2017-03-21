using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            personClient = new PersonClient { Connection = this.connection };
        }

        [TestMethod]
        public async Task GetPersonInfoTest()
        {
            var personId = new Guid("f2455640-2294-4e8f-99b0-a386fd478699");

            var response = SampleUtils.GetResponseData("PersonInfoSample.xml");

            connection.ExecuteAsync(HealthVaultMethods.GetPersonInfo, Arg.Any<int>()).Returns(response);

            var result = await personClient.GetPersonInfoAsync();

            await this.connection.Received().ExecuteAsync(HealthVaultMethods.GetPersonInfo, Arg.Any<int>());
            Assert.AreEqual(result.PersonId, personId);
        }

        [TestMethod]
        public async Task GetApplicationSettingsTest()
        {
            var response = SampleUtils.GetResponseData("AppSettingsSample.xml");

            connection.ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>()).Returns(response);

            var result = await personClient.GetApplicationSettingsAsync();

            await connection.Received().ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>(), null);
            Assert.IsNotNull(result.SelectedRecordId);
        }

        [TestMethod]
        public async Task SetApplicationSettingsWitXPathNavTest()
        {
            var response = SampleUtils.GetResponseData("AppSettingsSample.xml");

            var nav = response.InfoNavigator;

            await personClient.SetApplicationSettingsAsync(nav).ConfigureAwait(false);

            await connection.Received()
                .ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains("7a231675-4e78-451f-b94d-1e05b2a24586")));
        }

        [TestMethod]
        public async Task SetApplicationSettingsWithRequestParametersTest()
        {
            string requestParameters = "<app-settings />";

            await personClient.SetApplicationSettingsAsync(requestParameters).ConfigureAwait(false);

            await connection.Received()
                .ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Is<string>(x=> x.Contains(requestParameters)));
        }

        [TestMethod]
        public async Task GetAuthorizedRecordsAsyncTest()
        {
            var appSpecificId = "741934";

            IList<Guid> recordIds = new List<Guid> { new Guid("7a231675-4e78-451f-b94d-1e05b2a24586") };

            var response = SampleUtils.GetResponseData("AuthorizedRecordsSample.xml");

            connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>())
                .Returns(response);
            var result = await personClient.GetAuthorizedRecordsAsync(recordIds).ConfigureAwait(false);

            await connection.Received()
                .ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>());

            Assert.AreEqual(result.FirstOrDefault().ApplicationSpecificRecordId, appSpecificId);
        }
    }
}
