using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class RecordTests
    {
        [TestMethod]
        public async Task BasicRecordFields()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            Assert.AreEqual("HealthVault SDK Integration Test", record.Name);
            Assert.AreEqual(new Guid("2d4e32e6-9511-42b3-8ac2-5f6524b305a2"), record.Id);
            Assert.AreEqual(RelationshipType.Self, record.RelationshipType);
        }
    }
}
