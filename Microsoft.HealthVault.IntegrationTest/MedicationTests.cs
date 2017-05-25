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

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class MedicationTests
    {
        [TestMethod]
        public async Task SimpleMedications()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            await TestUtilities.RemoveAllThingsAsync<Medication>(thingClient, record.Id);

            var medication = new Medication
            {
                Name = new CodableValue("My med"),
                Dose = new GeneralMeasurement("2 tablets"),
                Strength = new GeneralMeasurement("200mg"),
                Frequency = new GeneralMeasurement("Twice per day"),
                Route = new CodableValue("By mouth", "po", new VocabularyKey("medication-routes", "wc", "2"))
            };

            await thingClient.CreateNewThingsAsync(record.Id, new IThing[] { medication });

            IReadOnlyCollection<Medication> medications = await thingClient.GetThingsAsync<Medication>(record.Id);

            Assert.AreEqual(1, medications.Count);

            Medication returnedMedication = medications.First();
            Assert.AreEqual(medication.Dose.Display, returnedMedication.Dose.Display);
            Assert.AreEqual(medication.Strength.Display, returnedMedication.Strength.Display);
            Assert.AreEqual(medication.Frequency.Display, returnedMedication.Frequency.Display);
            Assert.AreEqual(medication.Route.Text, returnedMedication.Route.Text);
        }
    }
}
