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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

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

            LocalDateTime nowLocal = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime;

            List<Weight> weightList = new List<Weight>();
            weightList.Add(new Weight(
                new HealthServiceDateTime(nowLocal.PlusHours(-1)),
                new WeightValue(81, new DisplayValue(81, "KG", "kg"))));

            weightList.Add(new Weight(
                new HealthServiceDateTime(nowLocal),
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
