using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Microsoft.HealthVault.UnitTest.Clients
{
	[TestClass]
	public class PersonClientTests
	{
		[TestMethod]
		public async void GetPersonInfoTest()
		{
			var personClient = Substitute.For<IPersonClient>();
			var result = await personClient.GetPersonInfoAsync();

			Assert.IsTrue(result != null);

		}

		[TestMethod]
		public async void GetApplicationSettingsTest()
		{
			var personClient = Substitute.For<IPersonClient>();
			var result = await personClient.GetApplicationSettingsAsync();

			Assert.IsTrue(result != null);
		}

		[TestMethod]
		public async void SetApplicationSettingsWitXPathNavTest()
		{
		    var nav = Substitute.For<IXPathNavigable>();
            var personClient = Substitute.For<IPersonClient>();
			await personClient.SetApplicationSettingsAsync(nav).ConfigureAwait(false);
		}

		[TestMethod]
		public async void SetApplicationSettingsWithRequestParametersTest()
		{
            string requestParameters = "<app-settings />";
            var personClient = Substitute.For<IPersonClient>();
            await personClient.SetApplicationSettingsAsync(requestParameters).ConfigureAwait(false);
        }

        [TestMethod]
        public async void GetAuthorizedRecordsAsyncTest()
        {
            IList<Guid> recordIds = new List<Guid> { new Guid(), new Guid(), new Guid() };
            var personClient = Substitute.For<IPersonClient>();
            var result = await personClient.GetAuthorizedRecordsAsync(recordIds).ConfigureAwait(false);
            Assert.IsTrue(result != null);
        }
    }
}
