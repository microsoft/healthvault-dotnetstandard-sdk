// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformPrimitives
{
    /// <summary>
    /// Provides low-level access to the HealthVault vocabulary operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformVocabulary.Current to a derived class to intercept all message calls.
    /// </remarks>
    public class HealthVaultPlatformVocabulary
    {
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        ///
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked.
        /// </remarks>
        ///
        /// <param name="mock">The mocking class.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        ///
        public static void EnableMock(HealthVaultPlatformVocabulary mock)
        {
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        ///
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }

        internal static HealthVaultPlatformVocabulary Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformVocabulary _current = new HealthVaultPlatformVocabulary();
        private static HealthVaultPlatformVocabulary _saved;

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
        public virtual ReadOnlyCollection<Vocabulary> GetVocabulary(
            HealthServiceConnection connection,
            IList<VocabularyKey> vocabularyKeys,
            bool cultureIsFixed)
        {
            Validator.ThrowIfArgumentNull(vocabularyKeys, "vocabularyKeys", "VocabularyKeysNullOrEmpty");

            Validator.ThrowArgumentExceptionIf(
                vocabularyKeys.Count == 0,
                "vocabularyKeys",
                "VocabularyKeysNullOrEmpty");

            string methodName = "GetVocabulary";
            HealthServiceRequest request = new HealthServiceRequest(connection, methodName, 2);

            StringBuilder requestParameters = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                writer.WriteStartElement("vocabulary-parameters");

                for (int i = 0; i < vocabularyKeys.Count; i++)
                {
                    Validator.ThrowIfArgumentNull(vocabularyKeys[i], "vocabularyKeys[i]", "VocabularyKeysNullOrEmpty");

                    vocabularyKeys[i].WriteXml(writer);
                }

                writer.WriteElementString(
                    "fixed-culture",
                    SDKHelper.XmlFromBool(cultureIsFixed));

                writer.WriteEndElement(); //<vocabulary-parameters>
                writer.Flush();
            }
            request.Parameters = requestParameters.ToString();

            request.Execute();

            ReadOnlyCollection<Vocabulary> vocabularies
                = CreateVocabulariesFromResponse(methodName, request.Response);

            return vocabularies;
        }

        private static ReadOnlyCollection<Vocabulary> CreateVocabulariesFromResponse(
                string methodNSSuffix,
                HealthServiceResponseData response)
        {
            XPathNavigator infoNav =
                response.InfoNavigator.SelectSingleNode(
                    Vocabulary.GetInfoXPathExpression(
                        methodNSSuffix,
                        response.InfoNavigator));

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
        public virtual ReadOnlyCollection<VocabularyKey> GetVocabularyKeys(HealthServiceConnection connection)
        {
            string methodName = "GetVocabulary";
            HealthServiceRequest request = new HealthServiceRequest(connection, methodName, 1);

            request.Execute();

            ReadOnlyCollection<VocabularyKey> keys
                = CreateVocabularyKeysFromResponse(methodName, request.Response);
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
        /// <param name="matchingVocabulary">
        /// A <see cref="VocabularyItemCollection"/> populated with entries matching
        /// the search criteria.
        /// </param>
        ///
        /// <param name="matchingKeys">
        /// A <b>ReadOnlyCollection</b> of <see cref="VocabularyKey"/> with entries
        /// matching the search criteria.
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
        public virtual void SearchVocabulary(
            HealthServiceConnection connection,
            VocabularyKey vocabularyKey,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults,
            out VocabularyItemCollection matchingVocabulary,
            out ReadOnlyCollection<VocabularyKey> matchingKeys)
        {
            Validator.ThrowArgumentExceptionIf(
                String.IsNullOrEmpty(searchValue) || searchValue.Length > 255,
                "searchString",
                "VocabularySearchStringInvalid");

            Validator.ThrowArgumentExceptionIf(
                !Enum.IsDefined(typeof(VocabularySearchType), searchType),
                "searchType",
                "VocabularySearchTypeUnknown");

            Validator.ThrowArgumentExceptionIf(
                maxResults.HasValue && maxResults.Value < 1,
                "maxResults",
                "SearchMaxResultsInvalid");

            matchingVocabulary = null;
            matchingKeys = null;

            string methodName = "SearchVocabulary";
            HealthServiceRequest request = new HealthServiceRequest(connection, methodName, 1);
            StringBuilder requestParameters = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                if (vocabularyKey != null)
                {
                    vocabularyKey.WriteXml(writer);
                }

                writer.WriteStartElement("text-search-parameters");

                writer.WriteStartElement("search-string");
                writer.WriteAttributeString("search-mode", searchType.ToString());
                writer.WriteString(searchValue);
                writer.WriteEndElement(); // <search-string>

                if (maxResults.HasValue)
                {
                    writer.WriteElementString("max-results", maxResults.Value.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteEndElement(); //<text-search-parameters>
                writer.Flush();
            }
            request.Parameters = requestParameters.ToString();

            request.Execute();

            if (vocabularyKey != null)
            {
                matchingVocabulary = CreateVocabularyItemCollectionFromResponse(
                                        methodName, request.Response);
            }
            else
            {
                matchingKeys = CreateVocabularyKeysFromResponse(methodName, request.Response);
            }
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
    }
}
