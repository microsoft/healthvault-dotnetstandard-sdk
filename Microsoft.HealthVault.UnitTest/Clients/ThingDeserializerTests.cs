// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class ThingDeserializerTests
    {
        private ThingDeserializer _thingDeserializer;
        private HealthRecordAccessor _healthRecordAccessor;

        [TestInitialize]
        public void InitializeTest()
        {
            IConnectionInternal connection = Substitute.For<IConnectionInternal>();

            _healthRecordAccessor = Substitute.For<HealthRecordAccessor>();
            _thingDeserializer = new ThingDeserializer(connection);
        }

        [TestMethod]
        public void WhenOneGroup()
        {
            string xml = @"<group><thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing></group>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            var thingCollections = _thingDeserializer.Deserialize(responseData, query);

            Assert.AreEqual(1, thingCollections.Count);
        }

        [TestMethod]
        public void WhenMutipleGroups()
        {
            string firstGroup = @"<group><thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing></group>";
            string secondGroup = @"<group><thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing></group>";

            string xml = $"<test>{firstGroup}{secondGroup}</test>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            var thingCollections = _thingDeserializer.Deserialize(responseData, query);

            Assert.AreEqual(2, thingCollections.Count);
        }

        [TestMethod]
        public void WhenMutipleThingsInAGroup()
        {
            string firstThing = @"<thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing>";
            string secondThing = @"<thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing>";

            string xml = $"<group>{firstThing}{secondThing}</group>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            IReadOnlyCollection<ThingCollection> thingCollections = _thingDeserializer.Deserialize(responseData, query);

            ThingCollection thingCollection = thingCollections.FirstOrDefault();

            Assert.AreEqual(1, thingCollections.Count);
            Assert.AreEqual(2, thingCollection.Count);
        }

        [TestMethod]
        public void WhenUnProcessedKeyIsPresent()
        {
            string thing = @"<thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing>";
            string unprocessedThingKeyInfo = @"<unprocessed-thing-key-info><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id></unprocessed-thing-key-info>";

            string xml = $"<group>{thing}{unprocessedThingKeyInfo}</group>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            IReadOnlyCollection<ThingCollection> thingCollections = _thingDeserializer.Deserialize(responseData, query);

            ThingCollection thingCollection = thingCollections.FirstOrDefault();

            // validate that we have both thing and unprocessed thing is parsed
            Assert.AreEqual(2, thingCollection.Count);

            // validate that the max full thing is 1 and doesn't include partial item
            Assert.AreEqual(1, thingCollection.MaxResultsPerRequest);
        }

        [TestMethod]
        public void WhenFilteredIsPresent()
        {
            string thing = @"<thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing>";
            string filtered = @"<filtered>true</filtered>";

            string xml = $"<group>{thing}{filtered}</group>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            IReadOnlyCollection<ThingCollection> thingCollections = _thingDeserializer.Deserialize(responseData, query);

            ThingCollection thingCollection = thingCollections.FirstOrDefault();

            Assert.AreEqual(true, thingCollection.WasFiltered);
        }

        [TestMethod]
        public void WhenOrderedByCultureIsPresent()
        {
            string thing = @"<thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing>";
            string filtered = @"<order-by-culture>en-us</order-by-culture>";

            string xml = $"<group>{thing}{filtered}</group>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            IReadOnlyCollection<ThingCollection> thingCollections = _thingDeserializer.Deserialize(responseData, query);

            ThingCollection thingCollection = thingCollections.FirstOrDefault();

            Assert.AreEqual("en-us", thingCollection.OrderByCulture);
        }

        [TestMethod]
        public void WhenGroupNameIsPresent()
        {
            string firstGroup = @"<group name=""Test""><thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing></group>";
            string secondGroup = @"<group><thing><thing-id version-stamp=""583a502e-69a1-4546-be74-dc2ba8db7d98"">e4bfb506-e94e-4d69-ba04-0c12197c0bb8</thing-id><type-id name=""Exercise"">85a21ddb-db20-4c65-8d30-33c899ccf612</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-01-31T20:00:00</eff-date><created><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-02-01T00:55:06.92Z</timestamp><app-id name=""HealthVault Insights"">52e62dc1-0834-4e25-9612-36f6692a9bcf</app-id><person-id name=""Test"">b54d4b92-9e05-4f60-a428-8b6262d18ec7</person-id><access-avenue>Offline</access-avenue><audit-action>Created</audit-action></updated><data-xml><exercise><when><structured><date><y>2017</y><m>1</m><d>31</d></date><time><h>20</h><m>0</m><s>0</s><f>0</f></time></structured></when><activity><text>Bicycling</text><code><value>Bicycling</value><family>wc</family><type>exercise-activities</type><version>1</version></code></activity><duration>120</duration><detail><name><value>CaloriesBurned_calories</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>544</value><units><text>Calories</text><code><value>Calories</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationGain_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail><detail><name><value>ElevationLoss_meters</value><family>wc</family><type>exercise-detail-names</type><version>1</version></name><value><value>0</value><units><text>meters</text><code><value>Meters</value><family>wc</family><type>exercise-units</type><version>1</version></code></units></value></detail></exercise><common><source>Microsoft HealthVault Insights</source><client-thing-id>Biking_a01442b7-213e-4ccf-b057-bf00fb809b46_636214896000000000-Duration</client-thing-id></common></data-xml><eff-permissions immutable=""false""><permission>Read</permission><permission>Update</permission><permission>Create</permission><permission>Delete</permission></eff-permissions></thing></group>";

            string xml = $"<test>{firstGroup}{secondGroup}</test>";

            HealthRecordSearcher query = new HealthRecordSearcher(_healthRecordAccessor);

            HealthServiceResponseData responseData = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(xml)).CreateNavigator()
            };

            IReadOnlyCollection<ThingCollection> thingCollections = _thingDeserializer.Deserialize(responseData, query);
            ThingCollection firstCollection = thingCollections.FirstOrDefault();

            Assert.AreEqual(2, thingCollections.Count);
            Assert.AreEqual("Test", firstCollection.Name);
        }
    }
}
