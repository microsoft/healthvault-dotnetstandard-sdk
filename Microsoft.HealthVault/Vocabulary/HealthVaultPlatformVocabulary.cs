// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// Provides low-level access to the HealthVault vocabulary operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformVocabulary.Current to a derived class to intercept all message calls.
    /// </remarks>
    internal class HealthVaultPlatformVocabulary
    {
        internal static HealthVaultPlatformVocabulary Current { get; private set; } = new HealthVaultPlatformVocabulary();

        /// <summary>
        /// Retrieves lists of vocabulary items for the specified
        /// vocabularies and culture.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <param name="vocabularyKeys">
        /// A list of keys identifying the requested vocabularies.
        /// </param>
        ///
        /// <param name="cultureIsFixed">
        /// HealthVault looks for the vocabulary items for the culture info
        /// specified using <see cref="HealthServiceConnection.Culture"/>.
        /// If <paramref name="cultureIsFixed"/> is set to <b>false</b> and if
        /// items are not found for the specified culture, items for the
        /// default fallback culture are returned. If
        /// <paramref name="cultureIsFixed"/> is set to <b>true</b>,
        /// fallback will not occur, and if items are not found for the
        /// specified culture, empty strings are returned.
        /// </param>
        ///
        /// <returns>
        /// The specified vocabularies and their items, or empty strings.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="vocabularyKeys"/> list is empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="vocabularyKeys"/> list is <b>null</b>
        /// or contains a <b>null</b> entry.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// One of the requested vocabularies is not found on the server.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// One of the requested vocabularies does not contain representations
        /// for its items for the specified culture when
        /// <paramref name="cultureIsFixed"/> is <b>true</b>.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// There is an error loading the vocabulary.
        /// </exception>
        ///
        public virtual async Task<ReadOnlyCollection<Vocabulary>> GetVocabularyAsync(
            IHealthVaultConnection connection,
            IList<VocabularyKey> vocabularyKeys,
            bool cultureIsFixed)
        {
            Validator.ThrowIfArgumentNull(vocabularyKeys, nameof(vocabularyKeys), Resources.VocabularyKeysNullOrEmpty);

            if (vocabularyKeys.Count == 0)
            {
                throw new ArgumentException(Resources.VocabularyKeysNullOrEmpty, nameof(vocabularyKeys));
            }

            var method = HealthVaultMethods.GetVocabulary;
            int methodVersion = 2;

            StringBuilder requestParameters = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                writer.WriteStartElement("vocabulary-parameters");

                for (int i = 0; i < vocabularyKeys.Count; i++)
                {
                    Validator.ThrowIfArgumentNull(vocabularyKeys[i], "vocabularyKeys[i]", Resources.VocabularyKeysNullOrEmpty);

                    vocabularyKeys[i].WriteXml(writer);
                }

                writer.WriteElementString(
                    "fixed-culture",
                    SDKHelper.XmlFromBool(cultureIsFixed));

                writer.WriteEndElement();
                writer.Flush();
            }

            string parameters = requestParameters.ToString();

            HealthServiceResponseData responseData = await connection.ExecuteAsync(method, methodVersion, parameters).ConfigureAwait(false);

            ReadOnlyCollection<Vocabulary> vocabularies
                = CreateVocabulariesFromResponse(method.ToString(), responseData);

            return vocabularies;
        }

        private static ReadOnlyCollection<Vocabulary> CreateVocabulariesFromResponse(
                string methodNSSuffix,
                HealthServiceResponseData response)
        {
            XPathExpression node = Vocabulary.GetInfoXPathExpression(
                methodNSSuffix,
                response.InfoNavigator);
            XPathNavigator infoNav = response.InfoNavigator.SelectSingleNode(node);

            List<Vocabulary> vocabularies = new List<Vocabulary>();

            XPathNodeIterator vocabIter = infoNav.Select("vocabulary");

            foreach (XPathNavigator vocabNav in vocabIter)
            {
                Vocabulary vocabulary = new Vocabulary();
                vocabulary.PopulateFromXml(vocabNav);
                vocabularies.Add(vocabulary);
            }

            return new ReadOnlyCollection<Vocabulary>(vocabularies);
        }

        /// <summary>
        /// Retrieves a collection of key information for identifying and
        /// describing the vocabularies in the system.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <returns>
        /// A collection of keys identifying the vocabularies in the system.
        /// </returns>
        ///
        public virtual async Task<ReadOnlyCollection<VocabularyKey>> GetVocabularyKeysAsync(IHealthVaultConnection connection)
        {
            var method = HealthVaultMethods.GetVocabulary;
            int methodVersion = 1;

            HealthServiceResponseData responseData = await connection.ExecuteAsync(method, methodVersion).ConfigureAwait(false);

            ReadOnlyCollection<VocabularyKey> keys = CreateVocabularyKeysFromResponse(method.ToString(), responseData);
            return keys;
        }

        private static ReadOnlyCollection<VocabularyKey> CreateVocabularyKeysFromResponse(
            string methodNameNSSuffix,
            HealthServiceResponseData response)
        {
            XPathNavigator infoNav =
                response.InfoNavigator.SelectSingleNode(
                    Vocabulary.GetInfoXPathExpression(
                        methodNameNSSuffix,
                        response.InfoNavigator));

            List<VocabularyKey> vocabularyKeys = new List<VocabularyKey>();

            XPathNodeIterator vocabKeyIter = infoNav.Select("vocabulary-key");
            foreach (XPathNavigator vocabKeyNav in vocabKeyIter)
            {
                VocabularyKey vocabularyKey = new VocabularyKey();
                vocabularyKey.ParseXml(vocabKeyNav);
                vocabularyKeys.Add(vocabularyKey);
            }

            return new ReadOnlyCollection<VocabularyKey>(vocabularyKeys);
        }

        /// <summary>
        /// Searches a specific vocabulary and retrieves the matching vocabulary items.
        /// </summary>
        ///
        /// <remarks>
        /// This method does text search matching of display text and abbreviation text
        /// for the culture defined by the <see cref="HealthServiceConnection.Culture"/>.
        /// The <paramref name="searchValue"/> is a string of characters in the specified
        /// culture.
        /// </remarks>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <param name="vocabularyKey">
        /// The <see cref="VocabularyKey"/> defining the vocabulary to search. If the
        /// family is not specified, the default HealthVault vocabulary family is used.
        /// If the version is not specified, the most current version of the vocabulary
        /// is used.
        /// </param>
        ///
        /// <param name="searchValue">
        /// The search string to use.
        /// </param>
        ///
        /// <param name="searchType">
        /// The type of search to perform.
        /// </param>
        ///
        /// <param name="maxResults">
        /// The maximum number of results to return. If null, all matching results
        /// are returned, up to a maximum number defined by the service config
        /// value with key maxResultsPerVocabularyRetrieval.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="vocabularyKey"/> is <b>null</b>.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// If <paramref name="searchValue"/> is <b>null</b> or empty or greater
        /// than <b>255</b> characters.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// if <paramref name="searchType"/> is not a known
        /// <see cref="VocabularySearchType"/> value.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// when <paramref name="maxResults"/> is defined but has a value less than 1.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// The requested vocabulary is not found on the server.
        /// <br></br>
        /// -Or-
        /// The requested search culture is not supported.
        /// </exception>
        ///
        public virtual async Task<VocabularySearchResult> SearchVocabularyAsync(
            IHealthVaultConnection connection,
            VocabularyKey vocabularyKey,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults)
        {
            if (string.IsNullOrEmpty(searchValue) || searchValue.Length > 255)
            {
                throw new ArgumentException(Resources.VocabularySearchStringInvalid, nameof(searchValue));
            }

            if (!Enum.IsDefined(typeof(VocabularySearchType), searchType))
            {
                throw new ArgumentException(Resources.VocabularySearchTypeUnknown, nameof(searchType));
            }

            if (maxResults.HasValue && maxResults.Value < 1)
            {
                throw new ArgumentException(Resources.SearchMaxResultsInvalid, nameof(maxResults));
            }

            var method = HealthVaultMethods.SearchVocabulary;
            int methodVersion = 1;

            StringBuilder requestParameters = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                vocabularyKey?.WriteXml(writer);

                writer.WriteStartElement("text-search-parameters");

                writer.WriteStartElement("search-string");
                writer.WriteAttributeString("search-mode", searchType.ToString());
                writer.WriteString(searchValue);
                writer.WriteEndElement(); // <search-string>

                if (maxResults.HasValue)
                {
                    writer.WriteElementString("max-results", maxResults.Value.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteEndElement();
                writer.Flush();
            }

            string parameters = requestParameters.ToString();

            HealthServiceResponseData responseData = await connection.ExecuteAsync(method, methodVersion, parameters).ConfigureAwait(false);

            if (vocabularyKey != null)
            {
                return new VocabularySearchResult(CreateVocabularyItemCollectionFromResponse(method.ToString(), responseData));
            }

            return new VocabularySearchResult(CreateVocabularyKeysFromResponse(method.ToString(), responseData));
        }

        private static VocabularyItemCollection CreateVocabularyItemCollectionFromResponse(
            string methodNSSuffix,
            HealthServiceResponseData response)
        {
            XPathNavigator infoNav =
                response.InfoNavigator.SelectSingleNode(
                    Vocabulary.GetInfoXPathExpression(
                        methodNSSuffix,
                        response.InfoNavigator));

            XPathNavigator vocabNav = infoNav.SelectSingleNode("code-set-result");
            VocabularyItemCollection itemCollection = new VocabularyItemCollection();
            itemCollection.PopulateFromXml(vocabNav);
            return itemCollection;
        }

        internal class VocabularySearchResult
        {
            public VocabularySearchResult(VocabularyItemCollection matchingVocabulary)
            {
                MatchingVocabulary = matchingVocabulary;
            }

            public VocabularySearchResult(ReadOnlyCollection<VocabularyKey> matchingKeys)
            {
                MatchingKeys = matchingKeys;
            }

            public VocabularyItemCollection MatchingVocabulary { get; }

            public ReadOnlyCollection<VocabularyKey> MatchingKeys { get; }
        }
    }
}
