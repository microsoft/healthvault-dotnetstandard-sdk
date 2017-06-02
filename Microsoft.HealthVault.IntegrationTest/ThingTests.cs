// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Vocabulary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class ThingTests
    {
        [TestMethod]
        public async Task MultipleThingTypes()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            await DeletePreviousThings(thingClient, record);

            LocalDateTime nowLocal = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime;

            var bloodGlucose = new BloodGlucose(
                new HealthServiceDateTime(nowLocal),
                new BloodGlucoseMeasurement(
                    4.2,
                    new DisplayValue(4.2, "mmol/L", "mmol-per-l")),
                new CodableValue("Whole blood", "wb", new VocabularyKey("glucose-measurement-type", "wc", "1")));

            var weight = new Weight(
                new HealthServiceDateTime(nowLocal),
                new WeightValue(81, new DisplayValue(81, "KG", "kg")));

            var bloodPressure1 = new BloodPressure
            {
                EffectiveDate = nowLocal,
                Systolic = 110,
                Diastolic = 90,
            };

            var bloodPressure2 = new BloodPressure
            {
                EffectiveDate = nowLocal.PlusHours(-1),
                Systolic = 111,
                Diastolic = 91,
            };

            var cholesterolProfile = new CholesterolProfileV2
            {
                When = new HealthServiceDateTime(nowLocal),
                LDL = new ConcentrationMeasurement(110),
                HDL = new ConcentrationMeasurement(65),
                Triglyceride = new ConcentrationMeasurement(140)
            };

            var labTestResult = new LabTestResults(new LabTestResultGroup[] { new LabTestResultGroup(new CodableValue("test")) });

            var immunization = new Immunization(new CodableValue("diphtheria, tetanus toxoids and acellular pertussis vaccine", "DTaP", new VocabularyKey("immunizations", "wc", "1")));

            var procedure = new Procedure(new CodableValue("A surgery"));

            var allergy = new Allergy(new CodableValue("Pollen"));

            var condition = new Condition(new CodableValue("Diseased"));

            await thingClient.CreateNewThingsAsync(
                record.Id,
                new List<IThing>
                {
                    bloodGlucose,
                    weight,
                    bloodPressure1,
                    bloodPressure2,
                    cholesterolProfile,
                    labTestResult,
                    immunization,
                    procedure,
                    allergy,
                    condition
                });

            var query = CreateMultiThingQuery();
            ThingCollection thingCollection = await thingClient.GetThingsAsync(record.Id, query);

            Assert.AreEqual(10, thingCollection.Count);

            var returnedBloodGlucose = (BloodGlucose)thingCollection.First(t => t.TypeId == BloodGlucose.TypeId);
            Assert.AreEqual(bloodGlucose.Value.Value, returnedBloodGlucose.Value.Value);

            var returnedWeight = (Weight)thingCollection.First(t => t.TypeId == Weight.TypeId);
            Assert.AreEqual(weight.Value.Kilograms, returnedWeight.Value.Kilograms);

            var returnedBloodPressures = thingCollection.Where(t => t.TypeId == BloodPressure.TypeId).Cast<BloodPressure>().ToList();
            Assert.AreEqual(2, returnedBloodPressures.Count);

            Assert.AreEqual(bloodPressure1.Systolic, returnedBloodPressures[0].Systolic);
        }

        [TestMethod]
        public async Task MultipleQueries()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            await DeletePreviousThings(thingClient, record);

            LocalDateTime nowLocal = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime;

            var bloodGlucose = new BloodGlucose(
                new HealthServiceDateTime(nowLocal),
                new BloodGlucoseMeasurement(
                    4.2,
                    new DisplayValue(4.2, "mmol/L", "mmol-per-l")),
                new CodableValue("Whole blood", "wb", new VocabularyKey("glucose-measurement-type", "wc", "1")));

            var weight = new Weight(
                new HealthServiceDateTime(nowLocal),
                new WeightValue(81, new DisplayValue(81, "KG", "kg")));

            await thingClient.CreateNewThingsAsync(
                record.Id,
                new List<IThing>
                {
                    bloodGlucose,
                    weight,
                });

            var resultSet = await thingClient.GetThingsAsync(record.Id, new[] { new ThingQuery(BloodGlucose.TypeId), new ThingQuery(Weight.TypeId) });
            Assert.AreEqual(2, resultSet.Count);
            var resultList = resultSet.ToList();

            ThingCollection glucoseCollection = resultList[0];
            var returnedBloodGlucose = (BloodGlucose)glucoseCollection.First();
            Assert.AreEqual(bloodGlucose.Value.Value, returnedBloodGlucose.Value.Value);

            ThingCollection weightCollection = resultList[1];
            var returnedWeight = (Weight)weightCollection.First();
            Assert.AreEqual(weight.Value.Kilograms, returnedWeight.Value.Kilograms);
        }

        private static async Task DeletePreviousThings(IThingClient thingClient, HealthRecordInfo record)
        {
            var query = CreateMultiThingQuery();

            var thingCollection = await thingClient.GetThingsAsync(record.Id, query);

            var thingsToDelete = new List<IThing>();

            foreach (IThing thing in thingCollection)
            {
                thingsToDelete.Add(thing);
            }

            if (thingsToDelete.Count > 0)
            {
                await thingClient.RemoveThingsAsync(record.Id, thingsToDelete);
            }
        }

        private static ThingQuery CreateMultiThingQuery()
        {
            return new ThingQuery(
                BloodGlucose.TypeId,
                Weight.TypeId,
                BloodPressure.TypeId,
                CholesterolProfile.TypeId,
                LabTestResults.TypeId,
                Immunization.TypeId,
                Procedure.TypeId,
                Allergy.TypeId,
                Condition.TypeId);
        }
    }
}
