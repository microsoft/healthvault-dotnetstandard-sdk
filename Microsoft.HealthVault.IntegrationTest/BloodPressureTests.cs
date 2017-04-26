using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class BloodPressureTests
    {
        [TestMethod]
        public async Task SimpleBloodPressure()
        {
            var config = new HealthVaultConfiguration
            {
                MasterApplicationId = new Guid("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
            };

            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(config);
            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();

            await this.RemoveAllBloodPressuresAsync(thingClient, personInfo.SelectedRecord.Id);

            // Create a new blood pressure object with random values
            Random rand = new Random();
            BloodPressure newBloodPressure = new BloodPressure
            {
                Diastolic = rand.Next(20, 100),
                Systolic = rand.Next(80, 120)
            };

            // use our thing client to create the new blood pressure
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure> { newBloodPressure });

            // use our thing client to get all things of type blood pressure
            IReadOnlyCollection<BloodPressure> bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);

            Assert.AreEqual(1, bloodPressures.Count);

            BloodPressure bp = bloodPressures.First();
            Assert.AreEqual(newBloodPressure.Systolic, bp.Systolic);
            Assert.AreEqual(newBloodPressure.Diastolic, bp.Diastolic);
        }

        private async Task RemoveAllBloodPressuresAsync(IThingClient thingClient, Guid recordId)
        {
            IReadOnlyCollection<BloodPressure> bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(recordId);
            await thingClient.RemoveThingsAsync(recordId, bloodPressures.ToList());
        }
    }
}
