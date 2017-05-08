using System;
using System.Collections.Generic;
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
    public class WeightTests
    {
        [TestMethod]
        public async Task SimpleWeights()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            await TestUtilities.RemoveAllThingsAsync<Weight>(thingClient, record.Id);

            List<Weight> weightList = new List<Weight>();
            weightList.Add(new Weight(
                new HealthServiceDateTime(DateTime.Now.AddHours(-1)),
                new WeightValue(81, new DisplayValue(81, "KG", "kg"))));

            weightList.Add(new Weight(
                new HealthServiceDateTime(DateTime.Now),
                new WeightValue(85, new DisplayValue(187, "LBS", "lb"))));

            await thingClient.CreateNewThingsAsync<Weight>(record.Id, weightList);

            IReadOnlyCollection<Weight> retrievedWeights = await thingClient.GetThingsAsync<Weight>(record.Id);
            Assert.AreEqual(2, retrievedWeights.Count);

            var retrievedWeightsList = retrievedWeights.ToList();
            Weight firstWeight = retrievedWeightsList[1];
            Weight secondWeight = retrievedWeightsList[0];

            Assert.AreEqual(81, firstWeight.Value.Kilograms);
            Assert.AreEqual(81, firstWeight.Value.DisplayValue.Value);
            Assert.AreEqual("KG", firstWeight.Value.DisplayValue.Units);
            Assert.AreEqual("kg", firstWeight.Value.DisplayValue.UnitsCode);

            Assert.AreEqual(85, secondWeight.Value.Kilograms);
            Assert.AreEqual(187, secondWeight.Value.DisplayValue.Value);
            Assert.AreEqual("LBS", secondWeight.Value.DisplayValue.Units);
            Assert.AreEqual("lb", secondWeight.Value.DisplayValue.UnitsCode);
        }
    }
}
