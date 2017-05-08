using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class BasicV2Tests
    {
        [TestMethod]
        public async Task BasicInformationFields()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            IThingClient thingClient = connection.CreateThingClient();

            BasicV2 basicInfo = (await thingClient.GetThingsAsync<BasicV2>(record.Id)).First();
            Assert.AreEqual("Redmond", basicInfo.City);
            Assert.AreEqual(1985, basicInfo.BirthYear);
            Assert.AreEqual(Gender.Male, basicInfo.Gender);
            Assert.AreEqual("98052", basicInfo.PostalCode);
            Assert.AreEqual("United States", basicInfo.Country.Text);
            Assert.AreEqual("Washington", basicInfo.StateOrProvince.Text);
        }
    }
}
