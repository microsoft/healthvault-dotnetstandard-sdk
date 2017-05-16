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
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Provides low-level access to the HealthVault service.
    /// </summary>
    /// <remarks>
    /// HealthServicePlatform provides access to the HealthVault service at a low level.
    ///
    /// For an easier-to-use interface, please see the following abstractions:
    /// <b>Data item operations</b>
    /// <see cref="HealthRecordAccessor"/> and <see cref="HealthRecordInfo"/>
    /// </remarks>
    internal static class HealthVaultPlatform
    {
        #region ApplicationSettings

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// The application settings XML.
        /// </returns>
        ///
        /// <remarks>
        /// This might be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        public static async Task<IXPathNavigable> GetApplicationSettingsAsXmlAsync(IHealthVaultConnection connection)
        {
            ApplicationSettings applicationSettings =
                await HealthVaultPlatformPerson
                    .Current
                    .GetApplicationSettingsAsync(connection)
                    .ConfigureAwait(false);

            return applicationSettings.XmlSettings;
        }

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// The complete set application settings including the XML settings, selected record ID, etc.
        /// </returns>
        ///
        public static async Task<ApplicationSettings> GetApplicationSettingsAsync(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformPerson.Current.GetApplicationSettingsAsync(connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="applicationSettings">
        /// The application settings XML.
        /// </param>
        ///
        /// <remarks>
        /// This may be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        public static async Task SetApplicationSettingsAsync(
            IHealthVaultConnection connection,
            IXPathNavigable applicationSettings)
        {
            string requestParameters =
                HealthVaultPlatformPerson.GetSetApplicationSettingsParameters(applicationSettings);

            await HealthVaultPlatformPerson
                .Current
                .SetApplicationSettingsAsync(connection, requestParameters)
                .ConfigureAwait(false);
        }

        #endregion

        #region Vocabulary

        /// <summary>
        /// Retrieves a list of vocabulary items for the specified vocabulary.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <param name="name">
        /// The name of the vocabulary requested.
        /// </param>
        /// <returns>
        /// The requested vocabulary and its items.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="name" /> parameter <b>null</b> or an empty
        /// string.
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
        /// -Or-
        /// <br></br>
        /// There is an error loading the vocabulary.
        /// </exception>
        ///
        public static async Task<Vocabulary.Vocabulary> GetVocabularyAsync(
            IHealthVaultConnection connection,
            string name)
        {
            Validator.ThrowIfStringNullOrEmpty(name, "name");

            VocabularyKey key = new VocabularyKey(name);
            return await GetVocabularyAsync(connection, key, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of vocabulary items for the specified vocabulary
        /// and culture.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <param name="vocabularyKey">
        /// A key identifying the vocabulary requested.
        /// </param>
        ///
        /// <param name="cultureIsFixed">
        /// HealthVault looks for the vocabulary items for the device's culture info.
        /// If <paramref name="cultureIsFixed"/> is set to <b>false</b> and if
        /// items are not found for the specified culture, items for the
        /// default fallback culture are returned. If
        /// <paramref name="cultureIsFixed"/> is set to <b>true</b>,
        /// fallback will not occur, and if items are not found for the
        /// specified culture, empty strings are returned.
        /// </param>
        ///
        /// <returns>
        /// The specified vocabulary and its items, or empty strings.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="vocabularyKey"/> is <b>null</b>.
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
        /// <br></br>
        /// The requested vocabulary does not contain representations
        /// for its items for the specified culture when
        /// <paramref name="cultureIsFixed"/> is <b>true</b>.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// There is an error loading the vocabulary.
        /// </exception>
        ///
        public static async Task<Vocabulary.Vocabulary> GetVocabularyAsync(
            IHealthVaultConnection connection,
            VocabularyKey vocabularyKey,
            bool cultureIsFixed)
        {
            ReadOnlyCollection<Vocabulary.Vocabulary> vocabularies =
                await GetVocabularyAsync(
                    connection,
                    new[] { vocabularyKey },
                    cultureIsFixed)
                    .ConfigureAwait(false);

            return vocabularies[0];
        }

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
        public static async Task<ReadOnlyCollection<Vocabulary.Vocabulary>> GetVocabularyAsync(
            IHealthVaultConnection connection,
            IList<VocabularyKey> vocabularyKeys,
            bool cultureIsFixed = false)
        {
            return await HealthVaultPlatformVocabulary.Current.GetVocabularyAsync(
                connection,
                vocabularyKeys,
                cultureIsFixed).ConfigureAwait(false);
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
        public static async Task<ReadOnlyCollection<VocabularyKey>> GetVocabularyKeysAsync(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformVocabulary.Current.GetVocabularyKeysAsync(connection);
        }

        /// <summary>
        /// Searches the keys of vocabularies defined by the HealthVault service.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability.
        /// </param>
        ///
        /// <remarks>
        /// This method does a text search of vocabulary names and descriptions.
        /// </remarks>
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
        /// <returns>
        /// A <b>ReadOnlyCollection</b> of <see cref="VocabularyKey"/> with entries
        /// matching the search criteria.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
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
        /// </exception>
        ///
        public static async Task<ReadOnlyCollection<VocabularyKey>> SearchVocabularyKeysAsync(
            IHealthVaultConnection connection,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults)
        {
            return (await HealthVaultPlatformVocabulary.Current.SearchVocabularyAsync(
                connection,
                null,
                searchValue,
                searchType,
                maxResults).ConfigureAwait(false)).MatchingKeys;
        }

        /// <summary>
        /// Searches a specific vocabulary and retrieves the matching vocabulary items.
        /// </summary>
        ///
        /// <remarks>
        /// This method does text search matching of display text and abbreviation text
        /// for the device's culture.
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
        /// <returns>
        /// A <see cref="VocabularyItemCollection"/> populated with entries matching
        /// the search criteria.
        /// </returns>
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
        public static async Task<VocabularyItemCollection> SearchVocabulary(
            IHealthVaultConnection connection,
            VocabularyKey vocabularyKey,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults)
        {
            Validator.ThrowIfArgumentNull(vocabularyKey, nameof(vocabularyKey), Resources.VocabularyKeyNullOrEmpty);

            return (await HealthVaultPlatformVocabulary.Current.SearchVocabularyAsync(
                connection,
                vocabularyKey,
                searchValue,
                searchType,
                maxResults).ConfigureAwait(false)).MatchingVocabulary;
        }

        #endregion Vocabulary

        #region GetPersonInfo

        /// <summary>
        /// Gets the information about the person specified.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// Information about the person's HealthVault account.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It is recommended that the calling application cache
        /// the return value and only call this method again if it needs to
        /// refresh the cache.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public static async Task<PersonInfo> GetPersonInfoAsync(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformPerson.Current.GetPersonInfoAsync(connection).ConfigureAwait(false);
        }

        #endregion GetPersonInfo

        #region GetAuthorizedPeople

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application-level. </param>
        ///
        /// <remarks>
        /// The returned IEnumerable iterator will access the HealthVault service
        /// across the network. The default <see cref="GetAuthorizedPeopleSettings"/>
        /// values are used.
        /// </remarks>
        ///
        /// <returns>
        /// An IEnumerable iterator of <see cref="PersonInfo"/> objects representing
        /// people authorized for the application.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        public static IEnumerable<Task<PersonInfo>> GetAuthorizedPeopleAsync(IHealthVaultConnection connection)
        {
            return HealthVaultPlatformApplication.Current.GetAuthorizedPeopleAsync(connection, new GetAuthorizedPeopleSettings());
        }

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <remarks>
        /// The returned IEnumerable iterator will access the HealthVault service
        /// across the network. See <see cref="GetAuthorizedPeopleSettings"/> for applicable
        /// settings.
        /// </remarks>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application-level. </param>
        ///
        /// <param name="settings">
        /// The <see cref="GetAuthorizedPeopleSettings" /> object used to configure the
        /// IEnumerable iterator returned by this method.
        /// </param>
        ///
        /// <returns>
        /// An IEnumerable iterator of <see cref="PersonInfo"/> objects representing
        /// people authorized for the application.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. The retrieval can be retried from the
        /// current position by calling this method again and using the last successfully
        /// retrieved person Id for <see cref="GetAuthorizedPeopleSettings.StartingPersonId"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null.
        /// </exception>
        ///
        public static IEnumerable<Task<PersonInfo>> GetAuthorizedPeopleAsync(
            IHealthVaultConnection connection,
            GetAuthorizedPeopleSettings settings)
        {
            return HealthVaultPlatformApplication.Current.GetAuthorizedPeopleAsync(connection, settings);
        }

        #endregion GetAuthorizedPeople

        #region GetAuthorizedRecords

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the records identified
        /// by the specified <paramref name="recordIds"/>.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="recordIds">
        /// The unique identifiers for the records to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A collection of the records matching the specified record
        /// identifiers and authorized for the authenticated person.
        /// </returns>
        ///
        /// <remarks>
        /// This method is useful in cases where the application is storing
        /// record identifiers and needs access to the functionality provided
        /// by the object model.
        /// </remarks>
        ///
        public static async Task<Collection<HealthRecordInfo>> GetAuthorizedRecordsAsync(
            IHealthVaultConnection connection,
            IList<Guid> recordIds)
        {
            return await HealthVaultPlatformPerson.Current.GetAuthorizedRecordsAsync(connection, recordIds).ConfigureAwait(false);
        }

        #endregion GetAuthorizedRecords

        #region GetApplicationInfo

        /// <summary>
        /// Gets the application configuration information for the calling application.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application-level. </param>
        ///
        /// <returns>
        /// An ApplicationInfo object for the calling application.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It returns installation configuration about the calling
        /// application.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public static async Task<ApplicationInfo> GetApplicationInfo(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformApplication.Current.GetApplicationInfoAsync(connection, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the application configuration information for the calling application.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="allLanguages">
        /// A boolean value indicating whether the localized values all languages should be
        /// returned, just one language. This affects all properties which can have multiple
        /// localized values, including <see cref="ApplicationInfo.CultureSpecificNames"/>,
        /// <see cref="ApplicationInfo.CultureSpecificDescriptions"/>,
        /// <see cref="ApplicationInfo.CultureSpecificAuthorizationReasons"/>,
        /// <see cref="ApplicationInfo.LargeLogo"/>,
        /// <see cref="ApplicationInfo.SmallLogo"/>,
        /// <see cref="ApplicationInfo.PrivacyStatement"/>,
        /// <see cref="ApplicationInfo.TermsOfUse"/>,
        /// and <see cref="ApplicationInfo.DtcSuccessMessage"/>
        /// </param>
        ///
        /// <returns>
        /// An ApplicationInfo object for the calling application.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It returns installation configuration about the calling
        /// application.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public static async Task<ApplicationInfo> GetApplicationInfoAsync(
            IHealthVaultConnection connection,
            bool allLanguages)
        {
            return await HealthVaultPlatformApplication.Current.GetApplicationInfoAsync(connection, allLanguages).ConfigureAwait(false);
        }

        #endregion

        #region GetUpdatedRecordsForApplication

        /// <summary>
        /// Gets a list of health record IDs for the current application,
        /// that optionally have been updated since a specified date.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="updatedDate">
        /// Date that is used to filter health record IDs according to whether or not they have
        /// been updated since the specified date.
        /// </param>
        ///
        /// <returns>
        /// List of health record IDs filtered by any specified input parameters.
        /// </returns>
        ///
        public static async Task<IList<Guid>> GetUpdatedRecordsForApplicationAsync(
            IHealthVaultConnection connection,
            DateTime? updatedDate)
        {
            return await HealthVaultPlatformApplication.Current.GetUpdatedRecordsForApplicationAsync(connection, updatedDate).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of <see cref="HealthRecordUpdateInfo"/> objects for the current application,
        /// that optionally have been updated since a specified date.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <param name="updatedDate">
        /// Date that is used to filter health record IDs according to whether or not they have
        /// been updated since the specified date.
        /// </param>
        ///
        /// <returns>
        /// List of <see cref="HealthRecordUpdateInfo"/> objects filtered by any specified input parameters.
        /// </returns>
        ///
        public static async Task<IList<HealthRecordUpdateInfo>> GetUpdatedRecordInfoForApplicationAsync(
            IHealthVaultConnection connection,
            DateTime? updatedDate)
        {
            return await HealthVaultPlatformApplication.Current.GetUpdatedRecordInfoForApplicationAsync(connection, updatedDate).ConfigureAwait(false);
        }

        #endregion

        #region NewSignupCode

        /// <summary>
        /// Generates a new signup code that should be passed to HealthVault Shell in order
        /// to create a new user account.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be application level. </param>
        ///
        /// <returns>
        /// A signup code that can be used to create an account.
        /// </returns>
        ///
        public static async Task<string> NewSignupCodeAsync(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformApplication.Current.NewSignupCodeAsync(connection).ConfigureAwait(false);
        }

        #endregion

        #region GetServiceDefinitionAsync

        /// <summary>
        /// Gets information about the HealthVault service.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. This
        /// includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public static async Task<ServiceInfo> GetServiceDefinitionAsync(IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service only if it has been updated since
        /// the specified update time.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// This includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.  Otherwise, if there were no updates,
        /// returns <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public static async Task<ServiceInfo> GetServiceDefinitionAsync(IHealthVaultConnection connection, DateTime lastUpdatedTime)
        {
            return await HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(connection, lastUpdatedTime).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="responseSections">
        /// The categories of information to be populated in the <see cref="ServiceInfo"/>
        /// instance, represented as the result of XOR'ing the desired categories.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public static async Task<ServiceInfo> GetServiceDefinitionAsync(
            IHealthVaultConnection connection,
            ServiceInfoSections responseSections)
        {
            return await HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(connection, responseSections).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories if the requested information has been updated since the specified
        /// update time.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation.</param>
        ///
        /// <param name="responseSections">
        /// The categories of information to be populated in the <see cref="ServiceInfo"/>
        /// instance, represented as the result of XOR'ing the desired categories.
        /// </param>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK  assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.  Otherwise, if there were no updates, returns
        /// <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public static async Task<ServiceInfo> GetServiceDefinitionAsync(
            IHealthVaultConnection connection,
            ServiceInfoSections responseSections,
            DateTime lastUpdatedTime)
        {
            return await HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(connection, responseSections, lastUpdatedTime).ConfigureAwait(false);
        }

        #endregion GetServiceDefinitionAsync

        #region RecordOperations

        /// <summary>
        /// Releases the authorization of the application on the health record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors during the authorization release.
        /// </exception>
        ///
        /// <remarks>
        /// Once the application releases the authorization to the health record,
        /// calling any methods of this <see cref="HealthRecordAccessor"/> will result
        /// in a <see cref="HealthServiceAccessDeniedException"/>."
        /// </remarks>
        public static async Task RemoveApplicationAuthorizationAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor)
        {
            await HealthVaultPlatformRecord.Current.RemoveApplicationAuthorizationAsync(connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this  record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of unique identifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// Returns a dictionary of <see cref="ThingTypePermission"/>
        /// with thing types as the keys.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty dictionary is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// <b> null </b> will be returned for that type in the dictionary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public static async Task<IDictionary<Guid, ThingTypePermission>> QueryPermissionsByTypesAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatformRecord.Current.QueryPermissionsByTypesAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A list of <see cref="ThingTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public static async Task<Collection<ThingTypePermission>> QueryPermissionsAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatformRecord.Current.QueryPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record as well as the other permission settings such as IsMeaningfulUseTrackingEnabled.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthRecordPermissions"/> object
        /// which contains a collection of <see cref="ThingTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// </exception>
        ///
        public static async Task<HealthRecordPermissions> QueryRecordPermissionsAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatformRecord.Current.QueryRecordPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets valid group memberships for a record.
        /// </summary>
        ///
        /// <remarks>
        /// Group membership thing types allow an application to signify that the
        /// record belongs to an application defined group.  A record in the group may be
        /// eligible for special programs offered by other applications, for example.
        /// Applications then need a away to query for valid group memberships.
        /// <br/>
        /// Valid group memberships are those memberships which are not expired, and whose
        /// last updating application is authorized by the last updating person to
        /// read and delete the membership.
        /// </remarks>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="applicationIds">
        /// A collection of unique application identifiers for which to
        /// search for group memberships.  For a null or empty application identifier
        /// list, return all valid group memberships for the record.  Otherwise,
        /// return only those group memberships last updated by one of the
        /// supplied application identifiers.
        /// </param>
        ///
        /// <returns>
        /// A List of things representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public static async Task<Collection<ThingBase>> GetValidGroupMembershipAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> applicationIds)
        {
            return await HealthVaultPlatformRecord.Current.GetValidGroupMembershipAsync(connection, accessor, applicationIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the things specified by the
        /// <see cref="HealthRecordSearcher"/> and runs them through the specified
        /// transform.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="searcher">
        /// The searcher that defines what items to return.
        /// </param>
        ///
        /// <param name="transform">
        /// A URL to a transform to run on the resulting XML. This can be
        /// a fully-qualified URL or the name of one of the standard XSLs
        /// provided by the HealthVault system.
        /// </param>
        ///
        /// <returns>
        /// The string resulting from performing the specified transform on
        /// the XML representation of the items.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Any call to HealthVault may specify a transform to be run on the
        /// response XML. The transform can be specified as a XSL fragment or
        /// a well-known transform tag provided by the HealthVault service. If a
        /// XSL fragment is specified, it gets compiled and cached on the server.
        /// <br/>
        /// <br/>
        /// A final-xsl is useful when you want to convert the result from XML to
        /// HTML so that you can display the result directly in a web page.
        /// You may also use it to generate other data formats like CCR, CCD, CSV,
        /// RSS, etc.
        /// <br/>
        /// <br/>
        /// Transform fragments cannot contain embedded script. The following set
        /// of parameters are passed to all final-xsl transforms:<br/>
        /// <ul>
        ///     <li>currentDateTimeUtc - the date and time just before the transform
        ///     started executing</li>
        ///     <li>requestingApplicationName - the name of the application that
        ///     made the request to HealthVault.</li>
        ///     <li>countryCode - the ISO 3166 country code from the request.</li>
        ///     <li>languageCode - the ISO 639-1 language code from the request.</li>
        ///     <li>personName - the name of the person making the request.</li>
        ///     <li>recordName - if the request identified a HealthVault record to
        ///     be used, this parameter contains the name of that record.</li>
        /// </ul>
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="transform"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// <see cref="HealthRecordView.Sections"/> does not
        /// contain the XML section in the view.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is a failure retrieving the items.
        /// -or-
        /// No filters have been specified.
        /// </exception>
        ///
        public static Task<string> GetTransformedItemsAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher,
            string transform)
        {
            throw new NotImplementedException();

            // return await HealthVaultPlatformItem.Current.GetTransformedItemsAsync(connection, accessor, searcher, transform).ConfigureAwait(false);
        }

        #endregion

        #region ItemTypeDefinitions

        /// <summary>
        /// Removes all item type definitions from the client-side cache.
        /// </summary>
        ///
        public static void ClearItemTypeCache()
        {
            HealthVaultPlatformInformation.Current.ClearItemTypeCache();
        }

        /// <summary>
        /// Gets the definitions for one or more thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null
        /// indicates that all health item types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type
        /// of details to be returned for the specified health item records(s).
        /// </param>
        ///
        /// <param name="imageTypes">
        /// A collection of strings that identify which health item record images should be
        /// retrieved.
        ///
        /// This requests an image of the specified mime type should be returned. For example,
        /// to request a GIF image, "image/gif" should be specified. For icons, "image/vnd.microsoft.icon"
        /// should be specified. Note, not all health item records will have all image types and
        /// some may not have any images at all.
        ///
        /// If '*' is specified, all image types will be returned.
        /// </param>
        ///
        /// <param name="lastClientRefreshDate">
        /// A <see cref="DateTime"/> instance that specifies the time of the last refresh
        /// made by the client.
        /// </param>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <returns>
        /// The type definitions for the specified types, or empty if the
        /// <paramref name="typeIds"/> parameter does not represent a known unique
        /// type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method calls the HealthVault service if the types are not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="typeIds"/> is <b>null</b> and empty, or
        /// <paramref name="typeIds"/> is <b>null</b> and member in <paramref name="typeIds"/> is
        /// <see cref="System.Guid.Empty"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            DateTime? lastClientRefreshDate,
            IHealthVaultConnection connection)
        {
            return await HealthVaultPlatformInformation.Current.GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                sections,
                imageTypes,
                lastClientRefreshDate,
                connection).ConfigureAwait(false);
        }

        #endregion ItemTypeDefinitions

        #region SelectInstance

        /// <summary>
        /// Gets the instance where a HealthVault account should be created
        /// for the specified account location.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to perform the operation.
        /// </param>
        ///
        /// <param name="preferredLocation">
        /// A user's preferred geographical location, used to select the best instance
        /// in which to create a new HealthVault account. If there is a location associated
        /// with the credential that will be used to log into the account, that location
        /// should be used.
        /// </param>
        ///
        /// <remarks>
        /// If no suitable instance can be found, a null value is returned. This can happen,
        /// for example, if the account location is not supported by HealthVault.
        ///
        /// Currently the returned instance IDs all parse to integers, but that is not
        /// guaranteed and should not be relied upon.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="HealthServiceInstance"/> object represents the selected instance,
        /// or null if no suitable instance exists.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="preferredLocation"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        public static async Task<HealthServiceInstance> SelectInstanceAsync(
            IHealthVaultConnection connection,
            Location preferredLocation)
        {
            return await HealthVaultPlatformInformation.Current.SelectInstanceAsync(
                connection,
                preferredLocation).ConfigureAwait(false);
        }

        #endregion
    }
}
