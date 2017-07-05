// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class ThingClientTests
    {
        private IConnectionInternal _connection;
        private IThingTypeRegistrar _itemTypeManager;

        private ThingClient _client;
        private Guid _recordId;

        [TestInitialize]
        public void InitializeTest()
        {
            _connection = Substitute.For<IConnectionInternal>();
            _itemTypeManager = new ThingTypeRegistrar();

            _recordId = Guid.NewGuid();
        }

        /// <summary>
        /// Tests that test clients are created properly and expose the correct connection
        /// </summary>
        [TestMethod]
        public void CreateClient()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            var guid = Guid.NewGuid();
            _client.CorrelationId = guid;
            Assert.IsTrue(_client.CorrelationId == guid);
        }

        /// <summary>
        /// Tests that the request to create things sends the correct method to the connection
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateNewThings()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            ICollection<IThing> things = new Collection<IThing> { CreateSampleBloodGlucose() };
            await _client.CreateNewThingsAsync(_recordId, things);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.PutThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains("blood-glucose")), Arg.Is<Guid>((x) => x == _recordId));
        }

        /// <summary>
        /// Tests that the request to get things is called correctly and returns the correct values
        /// </summary>
        [TestMethod]
        public async Task GetThings()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            ThingQuery query = this.GetBloodPressureThingQuery();
            var result = await _client.GetThingsAsync(_recordId, query);

            // ensure that the connection was called with the proper values
            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())),
                Arg.Is<Guid>(x => x == _recordId));

            // Assert that all results are parsed, grouped, and returned correctly.
            // Note that the sample data was not from this exact call, so it includes some other types of things in the results
            Assert.AreEqual(33, result.Count);
        }

        /// <summary>
        /// Tests that the request to get things with multiple queries is called correctly and returns the correct values
        /// </summary>
        [TestMethod]
        public async Task GetThingsMultiQuery()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsMultiQueryResult.xml"));
            var result = await _client.GetThingsAsync(_recordId, new [] { this.GetBloodPressureThingQuery(), this.GetWeightThingQuery() });

            // ensure that the connection was called with the proper values
            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString()) && x.Contains(Weight.TypeId.ToString())),
                Arg.Is<Guid>((x) => x == _recordId));

            // Assert that all results are parsed, grouped, and returned correctly.
            Assert.AreEqual(2, result.Count);

            var resultList = result.ToList();
            Assert.AreEqual(3, resultList[0].Count);
            Assert.AreEqual(1, resultList[1].Count);
        }

        [TestMethod]
        public async Task GetThingsPaged()
        {
            InitializeResponse(
                SampleUtils.GetSampleContent("ThingsPagedResult1.xml"),
                SampleUtils.GetSampleContent("ThingsPagedResult2.xml"),
                SampleUtils.GetSampleContent("ThingsPagedResult3.xml"));

            ThingQuery query = this.GetBloodPressureThingQuery();
            var result = await _client.GetThingsAsync<BloodPressure>(_recordId, query);
            List<BloodPressure> resultList = result.ToList();

            // The first call should be a normal one to get blood pressures.
            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())),
                Arg.Is<Guid>(x => x == _recordId));

            // The first response contains some unresolved items, which need another couple of calls to fetch.
            // We make sure we see these calls and the thing IDs they are requesting.
            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains("c0464a97-2832-4f50-a683-6d98c396da08")),
                Arg.Is<Guid>(x => x == _recordId));

            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains("f5f2c6f0-6924-4744-9338-8e3d81c31259")),
                Arg.Is<Guid>(x => x == _recordId));

            Assert.AreEqual(503, result.Count);

            BloodPressure lastBloodPressure = resultList[502];
            Assert.AreEqual(117, lastBloodPressure.Systolic);
            Assert.AreEqual(70, lastBloodPressure.Diastolic);
        }

        /// <summary>
        /// Tests that the request to get thing is called correctly and returns the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetThing()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));

            Guid correlationid = Guid.NewGuid();
            _client.CorrelationId = correlationid;
            BloodPressure bloodPressure = await _client.GetThingAsync<BloodPressure>(_recordId, Guid.NewGuid());

            // ensure that the connection was called with the proper values
            await _connection.Received().ExecuteAsync(
                method: HealthVaultMethods.GetThings,
                methodVersion: 3,
                parameters: Arg.Any<string>(),
                recordId: Arg.Is<Guid>(x => x == _recordId),
                correlationId: correlationid);

            Assert.IsNotNull(bloodPressure);
        }

        /// <summary>
        /// Tests that the request to remove things is called correctly
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RemoveThings()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            var thing = CreateSampleBloodGlucose();
            ICollection<IThing> things = new Collection<IThing> { thing };
            await _client.RemoveThingsAsync(_recordId, things);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.RemoveThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(thing.Key.Id.ToString())), Arg.Is<Guid>((x) => x == _recordId));
        }

        /// <summary>
        /// Tests that the request to update things calls the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateThings()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            ICollection<IThing> things = new Collection<IThing> { CreateSampleBloodGlucose() };
            await _client.UpdateThingsAsync(_recordId, things);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.PutThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(BloodGlucose.TypeId.ToString())), Arg.Is<Guid>((x) => x == _recordId));
        }

        /// <summary>
        /// Tests that the request to get things with a specific type only gets things of that type
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTypedThings()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            var query = this.GetBloodPressureThingQuery();
            var result = await _client.GetThingsAsync<BloodPressure>(_recordId, query);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())), Arg.Is<Guid>((x) => x == _recordId));

            // Assert that non-Blood Pressure results were filtered
            Assert.AreEqual(30, result.Count);
        }

        private BloodGlucose CreateSampleBloodGlucose()
        {
            BloodGlucose thing = new BloodGlucose();
            var document =
                new XPathDocument(new StringReader(SampleUtils.GetSampleContent("ThingSample.xml")));
            thing.ParseXml(document.CreateNavigator().SelectSingleNode("thing"), SampleUtils.GetSampleContent("ThingSample.xml"));
            return thing;
        }

        private ThingQuery GetBloodPressureThingQuery()
        {
            var config = Substitute.For<HealthVaultConfiguration>();
            var query = new ThingQuery(config);
            query.ItemIds.Add(BloodPressure.TypeId);
            return query;
        }

        private ThingQuery GetWeightThingQuery()
        {
            var config = Substitute.For<HealthVaultConfiguration>();
            var query = new ThingQuery(config);
            query.ItemIds.Add(Weight.TypeId);
            return query;
        }

        private void InitializeResponse(params string[] samples)
        {
            _client = new ThingClient(_connection, new ThingDeserializer(_connection, _itemTypeManager));
            _connection.CreateThingClient().Returns(_client);

            var responseData = new List<HealthServiceResponseData>();

            foreach (string sample in samples)
            {
            var infoReader = XmlReader.Create(new StringReader(sample), SDKHelper.XmlReaderSettings);

            infoReader.NameTable.Add("wc");
            infoReader.ReadToFollowing("wc:info");

            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(sample)).CreateNavigator(),
            };

                responseData.Add(response);
            }

            if (responseData.Count == 1)
            {
                _connection.ExecuteAsync(
                    Arg.Any<HealthVaultMethods>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<Guid?>(),
                    Arg.Any<Guid?>()).Returns(responseData[0]);
        }
            else
            {
                _connection.ExecuteAsync(
                    Arg.Any<HealthVaultMethods>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<Guid?>(),
                    Arg.Any<Guid?>()).Returns(responseData[0], responseData.Skip(1).ToArray());
            }
        }
    }
}
