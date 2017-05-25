// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.HealthVault.Vocabulary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class VocabularyClientTests
    {
        private IConnectionInternal _connection;
        private VocabularyClient _client;

        [TestInitialize]
        public void InitializeTest()
        {
            _connection = Substitute.For<IConnectionInternal>();
            _client = new VocabularyClient(_connection);

            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("VocabularySample.xml"))).CreateNavigator(),
            };
            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns(response);
        }

        /// <summary>
        /// Tests that test clients are created properly and expose the correct connection
        /// </summary>
        [TestMethod]
        public void CreateClientTest()
        {
            var guid = Guid.NewGuid();
            _client.CorrelationId = guid;
            Assert.IsTrue(_client.CorrelationId == guid);
        }

        /// <summary>
        /// Tests that the request to get vocabulary keys sends the correct method to the connection
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetVocabularyKeysTest()
        {
            await _client.GetVocabularyKeysAsync();
            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetVocabulary, Arg.Any<int>());
        }

        /// <summary>
        /// Tests that the request to get vocabularies contains the correct terms
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetVocabularyTest()
        {
            var vocabName = "vocabName";
            var vocabFamily = "vocabFamily";
            var vocabVersion = "vocabVersion";
            var vocabularies = await _client.GetVocabularyAsync(new VocabularyKey(vocabName, vocabFamily, vocabVersion));

            // ensure that the connection was called with the proper values
            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetVocabulary, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(vocabName) && x.Contains(vocabFamily) && x.Contains(vocabVersion)));

            // Ensure that the vocabularies returned were parsed correctly
            Assert.AreEqual(vocabularies.Family, "wc");
            Assert.AreEqual(vocabularies.Count, 3);
        }

        /// <summary>
        /// Tests that the request to get multiple vocabularies contains the correct terms
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetVocabulariesTest()
        {
            var vocabName1 = "vocabName1";
            var vocabFamily1 = "vocabFamily1";
            var vocabVersion1 = "vocabVersion1";
            var vocabName2 = "vocabName2";
            var vocabFamily2 = "vocabFamily2";
            var vocabVersion2 = "vocabVersion2";

            var key1 = new VocabularyKey(vocabName1, vocabFamily1, vocabVersion1);
            var key2 = new VocabularyKey(vocabName2, vocabFamily2, vocabVersion2);
            var vocabularies = await _client.GetVocabulariesAsync(new[] { key1, key2 });
            await _connection.Received().ExecuteAsync(HealthVaultMethods.GetVocabulary, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(vocabName1) && x.Contains(vocabName2) && x.Contains(vocabFamily1) && x.Contains(vocabVersion2)));

            // Ensure that the vocabularies returned were parsed correctly
            Assert.AreEqual(vocabularies.Count, 1);
            Assert.AreEqual(vocabularies.FirstOrDefault()?.Family, "wc");
        }

        /// <summary>
        /// Tests that the request to search vocabularies contains the correct search terms
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SearchVocabulariesTest()
        {
            var response = new HealthServiceResponseData
            {
                InfoNavigator = new XPathDocument(new StringReader(SampleUtils.GetSampleContent("VocabularySearchSample.xml"))).CreateNavigator()
            };

            _connection.ExecuteAsync(Arg.Any<HealthVaultMethods>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns(response);
            var searchTerm = "hypertension";
            await _client.SearchVocabularyAsync(searchTerm, VocabularySearchType.Contains, null);
            await _connection.Received().ExecuteAsync(HealthVaultMethods.SearchVocabulary, Arg.Any<int>(), Arg.Is<string>(x => x.Contains(searchTerm)));
        }

        /// <summary>
        /// Tests that vocabulary searches with incompatible values correctly throw an exception
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SearchVocabulariesIncompatibleValuesTest()
        {
            try
            {
                // Testing a search for a value over 256 characters
                await _client.SearchVocabularyAsync("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII",
                                                   VocabularySearchType.Contains, null);
                Assert.Fail("Expecting an exception when searching for incompatible values.");
            }
            catch (ArgumentException)
            {
                // pass
            }

            try
            {
                //Testing for a search with invalid results count
                await _client.SearchVocabularyAsync("hypertension", VocabularySearchType.Contains, 0);
                Assert.Fail("Expecting an exception when searching for too many vocabulary.");
            }
            catch (ArgumentException)
            {
                // pass
            }
        }
    }
}
