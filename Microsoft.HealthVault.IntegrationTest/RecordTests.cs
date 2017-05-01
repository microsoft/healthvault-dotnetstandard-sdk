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
            Assert.AreEqual(new Guid("ea5b4b21-b2de-4da6-bf5a-3e551cd0c54e"), record.Id);
            Assert.AreEqual(RelationshipType.Self, record.RelationshipType);
        }
    }
}
