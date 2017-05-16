using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class PersonClientTests
    {
        private IConnectionInternal _connection;
        private PersonClient _personClient;

        [TestInitialize]
        public void InitializeTest()
        {
            _connection = Substitute.For<IConnectionInternal>();
            _personClient = new PersonClient(_connection);
        }

        [TestMethod]
        public async Task GetAuthorizedPeopleTest()
        {
            string requestParameters = "<parameters />";
            var personId = new Guid("2d44d876-3bde-482b-a2af-ba133bc41fa9");

            var response = SampleUtils.GetResponseData("AuthorizedPeopleSample.xml");

            _connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedPeople, Arg.Any<int>(), Arg.Any<string>()).Returns(response);
            var result = (await _personClient.GetAuthorizedPeopleAsync()).FirstOrDefault();

            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetAuthorizedPeople, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(requestParameters)));

            Assert.IsNotNull(result);
            Assert.AreEqual(result.PersonId, personId);
        }

        [TestMethod]
        public async Task GetApplicationSettingsTest()
        {
            var response = SampleUtils.GetResponseData("AppSettingsSample.xml");

            _connection.ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>()).Returns(response);

            var result = await _personClient.GetApplicationSettingsAsync();

            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetApplicationSettings, Arg.Any<int>(), null);
            Assert.IsNotNull(result.SelectedRecordId);
        }

        [TestMethod]
        public async Task SetApplicationSettingsWitXPathNavTest()
        {
            var response = SampleUtils.GetResponseData("AppSettingsSample.xml");

            var nav = response.InfoNavigator;

            await _personClient.SetApplicationSettingsAsync(nav).ConfigureAwait(false);

            await _connection.Received()
                .ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains("7a231675-4e78-451f-b94d-1e05b2a24586")));
        }

        [TestMethod]
        public async Task SetApplicationSettingsWithRequestParametersTest()
        {
            string requestParameters = "<app-settings />";

            await _personClient.SetApplicationSettingsAsync(requestParameters).ConfigureAwait(false);

            await _connection.Received()
                .ExecuteAsync(HealthVaultMethods.SetApplicationSettings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(requestParameters)));
        }

        [TestMethod]
        public async Task GetAuthorizedRecordsAsyncTest()
        {
            var appSpecificId = "741934";

            IList<Guid> recordIds = new List<Guid> { new Guid("7a231675-4e78-451f-b94d-1e05b2a24586") };

            var response = SampleUtils.GetResponseData("AuthorizedRecordsSample.xml");

            _connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>())
                .Returns(response);
            var result = await _personClient.GetAuthorizedRecordsAsync(recordIds).ConfigureAwait(false);

            await _connection.Received()
                .ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, Arg.Any<int>(), Arg.Any<string>());

            Assert.AreEqual(result.FirstOrDefault().ApplicationSpecificRecordId, appSpecificId);
        }
    }
}
