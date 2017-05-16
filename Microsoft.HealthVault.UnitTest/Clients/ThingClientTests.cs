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
        private ThingClient _client;
        private Guid _recordId;

        [TestInitialize]
        public void InitializeTest()
        {
            _connection = Substitute.For<IConnectionInternal>();
            _recordId = Guid.NewGuid();
        }

        /// <summary>
        /// Tests that test clients are created properly and expose the correct connection
        /// </summary>
        [TestMethod]
        public void CreateClientTest()
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
        public async Task CreateNewThingsTest()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            ICollection<IThing> things = new Collection<IThing> { CreateSampleBloodGlucose() };
            await _client.CreateNewThingsAsync(_recordId, things);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.PutThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains("blood-glucose")), Arg.Is<Guid>((x) => x == _recordId));
        }

        /// <summary>
        /// Tests that the request to get things is called correctly and returns the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetThingsTest()
        {
            InitializeResponse(SampleUtils.GetSampleContent("ThingsSampleBloodPressure.xml"));
            ThingQuery query = GetThingQuery();
            var result = await _client.GetThingsAsync(_recordId, query);

            // ensure that the connection was called with the proper values
            await _connection.Received().ExecuteAsync(
                HealthVaultMethods.GetThings,
                Arg.Any<int>(),
                Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())),
                Arg.Is<Guid>((x) => x == _recordId));

            // Assert that all results are parsed, grouped, and returned correctly.
            // Note that the sample data was not from this exact call, so it includes some other types of things in the results
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(33, result.FirstOrDefault()?.Count);
        }

        /// <summary>
        /// Tests that the request to get thing is called correctly and returns the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetThingTest()
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
        public async Task RemoveThingsTest()
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
            var query = GetThingQuery();
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

        private ThingQuery GetThingQuery()
        {
            var config = Substitute.For<HealthVaultConfiguration>();
            var query = new ThingQuery(config);
            query.ItemIds.Add(BloodPressure.TypeId);
            return query;
        }

        private void InitializeResponse(string sample)
        {
            _client = new ThingClient(_connection, new ThingDeserializer(_connection));

            var infoReader = XmlReader.Create(new StringReader(sample), SDKHelper.XmlReaderSettings);

            infoReader.NameTable.Add("wc");
            infoReader.ReadToFollowing("wc:info");

            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(sample)).CreateNavigator(),
            };

            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>()).Returns(response);
            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>()).Returns(response);
            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Guid>()).Returns(response);
            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(response);
        }
    }
}
