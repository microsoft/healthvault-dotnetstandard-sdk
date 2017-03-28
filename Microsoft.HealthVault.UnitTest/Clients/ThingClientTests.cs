using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.UnitTest.Samples;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class ThingClientTests
    {
        private IConnectionInternal connection;
        private ThingClient client;
        private Guid recordId;

        [TestInitialize]
        public void InitializeTest()
        {
            this.connection = Substitute.For<IConnectionInternal>();
            this.recordId = Guid.NewGuid();
        }

        /// <summary>
        /// Tests that test clients are created properly and expose the correct connection
        /// </summary>
        [TestMethod]
        public void CreateClientTest()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            var guid = Guid.NewGuid();
            this.client.CorrelationId = guid;
            Assert.IsTrue(this.client.CorrelationId == guid);
        }

        /// <summary>
        /// Tests that the request to create things sends the correct method to the connection
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateNewThingsTest()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            ICollection<IThing> things = new Collection<IThing> { await this.CreateSampleBloodGlucose() };
            await client.CreateNewThingsAsync(this.recordId, things);
            await connection.Received().ExecuteAsync(HealthVaultMethods.PutThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains("blood-glucose")), Arg.Is<Guid>((x) => x == recordId));
        }

        /// <summary>
        /// Tests that the request to get things is called correctly and returns the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetThingsTest()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            ThingQuery query = this.GetThingQuery();
            var result = await client.GetThingsAsync(this.recordId, query);

            // ensure that the connection was called with the proper values
            await connection.Received().ExecuteAsync(HealthVaultMethods.GetThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())), Arg.Is<Guid>((x) => x == recordId));

            // Assert that all results are parsed, grouped, and returned correctly.  
            // Note that the sample data was not from this exact call, so it includes some other types of things in the results
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(33, result.FirstOrDefault()?.Count);
        }

        /// <summary>
        /// Tests that the request to remove things is called correctly
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RemoveThingsTest()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            var thing = await this.CreateSampleBloodGlucose();
            ICollection<IThing> things = new Collection<IThing> { thing };
            await client.RemoveThingsAsync(recordId, things);
            await connection.Received().ExecuteAsync(HealthVaultMethods.RemoveThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(thing.Key.Id.ToString())), Arg.Is<Guid>((x) => x == recordId));
        }

        /// <summary>
        /// Tests that the request to update things calls the correct values
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateThings()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            ICollection<IThing> things = new Collection<IThing> { await this.CreateSampleBloodGlucose() };
            await client.UpdateThingsAsync(recordId, things);
            await connection.Received().ExecuteAsync(HealthVaultMethods.PutThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(BloodGlucose.TypeId.ToString())), Arg.Is<Guid>((x) => x == recordId));
        }

        /// <summary>
        /// Tests that the request to get things with a specific type only gets things of that type
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTypedThings()
        {
            this.initializeResponse(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"));
            var query = this.GetThingQuery();
            var result = await client.GetThingsAsync<BloodPressure>(recordId, query);
            await connection.Received().ExecuteAsync(HealthVaultMethods.GetThings, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(BloodPressure.TypeId.ToString())), Arg.Is<Guid>((x) => x == recordId));

            // Assert that non-Blood Pressure results were filtered
            Assert.AreEqual(30, result.Count);
        }

        private async Task<BloodGlucose> CreateSampleBloodGlucose()
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

        private void initializeResponse(string sample)
        {
            this.client = new ThingClient(connection);

            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(sample)).CreateNavigator(),
                ResponseText = new ArraySegment<byte>(Encoding.ASCII.GetBytes(sample))
            };

            connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>()).Returns(response);
            connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>()).Returns(response);
            connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Guid>()).Returns(response);
        }
    }
}
