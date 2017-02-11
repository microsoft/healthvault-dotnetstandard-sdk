// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.ItemTypes;
using Microsoft.Health.MeaningfulUse;
using Microsoft.Health.Package;
using Microsoft.Health.PatientConnect;
using Microsoft.Health.PlatformPrimitives;
using Microsoft.Health.Web;

namespace Microsoft.Health
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
    public static class HealthVaultPlatform
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
        public static IXPathNavigable GetApplicationSettingsAsXml(HealthServiceConnection connection)
        {
            return HealthVaultPlatformPerson.Current.GetApplicationSettings(connection).XmlSettings;
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
        public static ApplicationSettings GetApplicationSettings(HealthServiceConnection connection)
        {
            return HealthVaultPlatformPerson.Current.GetApplicationSettings(connection);
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
        public static void SetApplicationSettings(
            HealthServiceConnection connection,
            IXPathNavigable applicationSettings)
        {
            string requestParameters =
                HealthVaultPlatformPerson.GetSetApplicationSettingsParameters(applicationSettings);

            HealthVaultPlatformPerson.Current.SetApplicationSettings(connection, requestParameters);
        }

        #endregion

        #region Message

        /// <summary>
        /// Sends an insecure message to the specified message recipients.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="mailRecipient">
        /// The addresses and display names of the people to send the 
        /// message to.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="mailRecipient"/> property is <b>null</b> or empty, 
        /// or the <paramref name="subject"/> parameter is <b>null</b> or empty, or 
        /// the <paramref name="textBody"/> and <paramref name="htmlBody"/> 
        /// parameters are both <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The server returned a failure when making the request.
        /// </exception>
        /// 
        public static void SendInsecureMessage(
            HealthServiceConnection connection,
            IList<MailRecipient> mailRecipient,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessage(
                connection,
                mailRecipient,
                null,
                null,
                true,
                subject,
                textBody,
                htmlBody);
        }

        /// <summary>
        /// Sends an insecure message to the specified message recipients.
        /// </summary>
        /// 
        /// <param name="personIds">
        /// The unique identifiers of the people to which the message should be
        /// sent.
        /// </param>
        /// 
        /// <param name="addressMustBeValidated">
        /// <b>true</b> if HealthVault ensures that the person has validated 
        /// their message address before sending the mail; <b>false</b> if the 
        /// message will be sent even if the person's address has not been 
        /// validated.
        /// </param>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="personIds"/> or <paramref name="subject"/>, 
        /// <paramref name="textBody"/>  or <paramref name="htmlBody"/> parameters 
        /// are <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The server returned a failure when making the request.        
        /// </exception>
        /// 
        public static void SendInsecureMessage(
            HealthServiceConnection connection,
            IList<Guid> personIds,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessage(
                connection,
                null,
                personIds,
                null,
                addressMustBeValidated,
                subject,
                textBody,
                htmlBody);
        }

        /// <summary>
        /// Sends an insecure message to custodians of the specified health 
        /// record.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="recordId">
        /// The unique identifier of the health record for which the 
        /// custodians should be sent the message.
        /// </param>
        /// 
        /// <param name="addressMustBeValidated">
        /// <b>true</b> if HealthVault ensures that the person has validated 
        /// their message address before sending the mail; <b>false</b> if the 
        /// message will be sent even if the person's address has not been 
        /// validated.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified, then a
        /// multi-part message is sent so that the HTML body will be used
        /// and falls back to text if not supported by the client.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException"> 
        /// The <paramref name="recordId"/> parameter is <see cref="System.Guid.Empty"/>
        /// -or-
        /// The <paramref name="subject"/> parameter is <b>null</b> or empty,
        /// -or-
        /// The <paramref name="textBody"/> and <paramref name="htmlBody"/> parameters 
        /// are both <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The server returned a failure when making the request.
        /// </exception>
        /// 
        public static void SendInsecureMessageToCustodians(
            HealthServiceConnection connection,
            Guid recordId,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessage(
                connection,
                null,
                null,
                recordId,
                addressMustBeValidated,
                subject,
                textBody,
                htmlBody);
        }

        /// <summary>
        /// Sends an insecure message originating from the application to 
        /// the specified message recipients. 
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="mailRecipient">
        /// The addresses and display names of the people to send the 
        /// message to.
        /// </param>
        /// 
        /// <param name="senderMailboxName">
        /// An application specified mailbox name that's sending the message.
        /// The mailbox name is appended to the application's domain name to 
        /// form the From email address of the message. This parameter should
        /// only contain the characters before the @ symbol of the email 
        /// address.
        /// </param>
        /// 
        /// <param name="senderDisplayName">
        /// The message sender's display name.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// 
        /// If the domain name of the application has not been previously 
        /// set (usually through app registration), this method will throw 
        /// a <see cref="HealthServiceException"/>.        
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="mailRecipient"/> is null or empty,
        /// -or-
        /// if <paramref name="senderMailboxName"/> is null or empty,
        /// -or-
        /// if <paramref name="senderDisplayName"/> is null or empty,
        /// -or-
        /// if <paramref name="subject"/> is null or empty,
        /// -or-
        /// if <paramref name="textBody"/> and <paramref name="htmlBody"/>
        /// are both null or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If the server returned a failure when making the request.
        /// </exception>
        /// 
        public static void SendInsecureMessageFromApplication(
            HealthServiceConnection connection,
            IList<MailRecipient> mailRecipient,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessageFromApplication(
                connection,
                mailRecipient,
                null,
                null,
                true,
                senderMailboxName,
                senderDisplayName,
                subject,
                textBody,
                htmlBody);

        }

        /// <summary>
        /// Sends an insecure message originating from the application
        /// to the specified message recipients.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="recipientPersonIds">
        /// The unique identifiers of the people to which the message should be
        /// sent.
        /// </param>
        /// 
        /// <param name="addressMustBeValidated">
        /// If true, HealthVault will ensure that the person has validated 
        /// their message address before sending the mail. If false, the 
        /// message will be sent even if the person's address has not been 
        /// validated.
        /// </param>
        /// 
        /// <param name="senderMailboxName">
        /// An application specified mailbox name that's sending the message.
        /// The mailbox name is appended to the application's domain name to 
        /// form the From email address of the message. This parameter should
        /// only contain the characters before the @ symbol of the email 
        /// address.
        /// </param>
        /// 
        /// <param name="senderDisplayName">
        /// The message sender's display name.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// 
        /// If the domain name of the application has not been previously 
        /// set (usually through app registration), this method will throw
        /// a <see cref="HealthServiceException"/>.        
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="recipientPersonIds"/> is null or empty,
        /// -or-
        /// if <paramref name="senderMailboxName"/> is null or empty,
        /// -or-
        /// if <paramref name="senderDisplayName"/> is null or empty,
        /// -or-
        /// if <paramref name="subject"/> is null or empty,
        /// -or-
        /// if <paramref name="textBody"/> and <paramref name="htmlBody"/>
        /// are both null or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If the server returned a failure when making the request.        
        /// </exception>
        /// 
        public static void SendInsecureMessageFromApplication(
            HealthServiceConnection connection,
            IList<Guid> recipientPersonIds,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessageFromApplication(
                connection,
                null,
                recipientPersonIds,
                null,
                addressMustBeValidated,
                senderMailboxName,
                senderDisplayName,
                subject,
                textBody,
                htmlBody);
        }

        /// <summary>
        /// Sends an insecure message originating from the application 
        /// to custodians of the specified health record.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        /// 
        /// <param name="recordId">
        /// The unique identifier of the health record for which the 
        /// custodians should be sent the message.
        /// </param>
        /// 
        /// <param name="addressMustBeValidated">
        /// If true, HealthVault will only send the message to custodians with 
        /// validated e-mail addresses. If false, the message will
        /// be sent even if the custodians' addresses have not been validated.
        /// </param>
        /// 
        /// <param name="senderMailboxName">
        /// An application specified mailbox name that's sending the message.
        /// The mailbox name is appended to the application's domain name to 
        /// form the From email address of the message. This parameter should
        /// only contain the characters before the @ symbol of the email 
        /// address.
        /// </param>
        /// 
        /// <param name="senderDisplayName">
        /// The message sender's display name.
        /// </param>
        /// 
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// 
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        /// 
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        /// 
        /// <remarks>
        /// If both the <paramref name="textBody"/> and 
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// 
        /// If the domain name of the application has not been previously 
        /// set (usually through app registration), this method will throw 
        /// a <see cref="HealthServiceException"/>.
        ///         
        /// The calling application and the person through which authorization to the 
        /// specified record was obtained must be authorized for the record. 
        /// The person must be either authenticated, or if the person is offline,
        /// their person Id specified as the offline person Id.
        /// See <see cref="Microsoft.Health.Web.OfflineWebApplicationConnection" /> 
        /// for more information.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException"> 
        /// If <paramref name="recordId"/> is <see cref="System.Guid.Empty"/>
        /// -or-
        /// if <paramref name="senderMailboxName"/> is null or empty,
        /// -or-
        /// if <paramref name="senderDisplayName"/> is null or empty,
        /// -or-
        /// if <paramref name="subject"/> is null or empty,
        /// -or-
        /// if <paramref name="textBody"/> and <paramref name="htmlBody"/>
        /// are both null or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If the server returned a failure when making the request.
        /// </exception>
        /// 
        public static void SendInsecureMessageToCustodiansFromApplication(
            HealthServiceConnection connection,
            Guid recordId,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatformMessage.Current.SendInsecureMessageFromApplication(
                connection,
                null,
                null,
                recordId,
                addressMustBeValidated,
                senderMailboxName,
                senderDisplayName,
                subject,
                textBody,
                htmlBody);
        }

        #endregion Message

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
        public static Vocabulary GetVocabulary(
            HealthServiceConnection connection,
            string name)
        {
            Validator.ThrowIfStringNullOrEmpty(name, "name");

            VocabularyKey key = new VocabularyKey(name);
            return GetVocabulary(connection, key, false);
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
        public static Vocabulary GetVocabulary(
            HealthServiceConnection connection,
            VocabularyKey vocabularyKey,
            bool cultureIsFixed)
        {
            ReadOnlyCollection<Vocabulary> vocabularies =
                GetVocabulary(
                    connection,
                    new VocabularyKey[] { vocabularyKey },
                    cultureIsFixed);
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
        public static ReadOnlyCollection<Vocabulary> GetVocabulary(
            HealthServiceConnection connection,
            IList<VocabularyKey> vocabularyKeys,
            bool cultureIsFixed)
        {
            return HealthVaultPlatformVocabulary.Current.GetVocabulary(
                connection,
                vocabularyKeys,
                cultureIsFixed);
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
        public static ReadOnlyCollection<VocabularyKey> GetVocabularyKeys(HealthServiceConnection connection)
        {
            return HealthVaultPlatformVocabulary.Current.GetVocabularyKeys(connection);
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
        public static ReadOnlyCollection<VocabularyKey> SearchVocabularyKeys(
            HealthServiceConnection connection,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults)
        {
            ReadOnlyCollection<VocabularyKey> matchingKeys;
            VocabularyItemCollection matchingVocabulary;

            HealthVaultPlatformVocabulary.Current.SearchVocabulary(
                connection,
                null,
                searchValue,
                searchType,
                maxResults,
                out matchingVocabulary,
                out matchingKeys);

            return matchingKeys;
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
        public static VocabularyItemCollection SearchVocabulary(
            HealthServiceConnection connection,
            VocabularyKey vocabularyKey,
            string searchValue,
            VocabularySearchType searchType,
            int? maxResults)
        {
            Validator.ThrowIfArgumentNull(vocabularyKey, "vocabularyKey", "VocabularyKeyNullOrEmpty");

            VocabularyItemCollection matchingVocabulary;
            ReadOnlyCollection<VocabularyKey> matchingKeys;

            HealthVaultPlatformVocabulary.Current.SearchVocabulary(
                connection,
                vocabularyKey,
                searchValue,
                searchType,
                maxResults,
                out matchingVocabulary,
                out matchingKeys);

            return matchingVocabulary;
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
        public static PersonInfo GetPersonInfo(ApplicationConnection connection)
        {
            return HealthVaultPlatformPerson.Current.GetPersonInfo(connection);
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
        public static IEnumerable<PersonInfo> GetAuthorizedPeople(ApplicationConnection connection)
        {
            return HealthVaultPlatformApplication.Current.GetAuthorizedPeople(connection, new GetAuthorizedPeopleSettings());
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
        public static IEnumerable<PersonInfo> GetAuthorizedPeople(
            ApplicationConnection connection,
            GetAuthorizedPeopleSettings settings)
        {
            return HealthVaultPlatformApplication.Current.GetAuthorizedPeople(connection, settings);
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
        public static Collection<HealthRecordInfo> GetAuthorizedRecords(
            ApplicationConnection connection,
            IList<Guid> recordIds)
        {
            return HealthVaultPlatformPerson.Current.GetAuthorizedRecords(connection, recordIds);
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
        public static ApplicationInfo GetApplicationInfo(HealthServiceConnection connection)
        {
            return HealthVaultPlatformApplication.Current.GetApplicationInfo(connection, false);
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
        public static ApplicationInfo GetApplicationInfo(
            HealthServiceConnection connection,
            Boolean allLanguages)
        {
            return HealthVaultPlatformApplication.Current.GetApplicationInfo(connection, allLanguages);
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
        public static IList<Guid> GetUpdatedRecordsForApplication(
            HealthServiceConnection connection,
            DateTime? updatedDate)
        {
            return HealthVaultPlatformApplication.Current.GetUpdatedRecordsForApplication(connection, updatedDate);
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
        public static IList<HealthRecordUpdateInfo> GetUpdatedRecordInfoForApplication(
            HealthServiceConnection connection,
            DateTime? updatedDate)
        {
            return HealthVaultPlatformApplication.Current.GetUpdatedRecordInfoForApplication(connection, updatedDate);
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
        public static string NewSignupCode(HealthServiceConnection connection)
        {
            return HealthVaultPlatformApplication.Current.NewSignupCode(connection);
        }

        #endregion

        #region GetServiceDefinition

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
        public static ServiceInfo GetServiceDefinition(HealthServiceConnection connection)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinition(connection);
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
        public static ServiceInfo GetServiceDefinition(HealthServiceConnection connection, DateTime lastUpdatedTime)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinition(connection, lastUpdatedTime);
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
        public static ServiceInfo GetServiceDefinition(
            HealthServiceConnection connection,
            ServiceInfoSections responseSections)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinition(connection, responseSections);
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
        public static ServiceInfo GetServiceDefinition(
            HealthServiceConnection connection,
            ServiceInfoSections responseSections,
            DateTime lastUpdatedTime)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinition(connection, responseSections, lastUpdatedTime);
        }
        #endregion GetServiceDefinition

        #region ItemOperations
        /// <summary>
        /// Creates new health record items associated with the record.
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
        /// <param name="items">
        /// The health record items from which to create new instances.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur, 
        /// no items will have been created.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// At least one HealthRecordItem in the supplied list was null.
        /// </exception>
        /// 
        public static void NewItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItem> items)
        {
            HealthVaultPlatformItem.Current.NewItems(connection, accessor, items);
        }

        /// <summary>
        /// Updates the specified health record items in one batch call to 
        /// the service.
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
        /// <param name="itemsToUpdate">
        /// The health record items to be updated.
        /// </param>
        /// 
        /// <remarks>
        /// Only new items are updated with the appropriate unique identifier. 
        /// All other sections must be updated manually.
        /// <br/><br/>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="itemsToUpdate"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemsToUpdate"/> contains a <b>null</b> member or
        /// a <see cref="HealthRecordItem"/> instance that does not have an ID.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur, 
        /// no items will have been updated.
        /// </exception>
        /// 
        public static void UpdateItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItem> itemsToUpdate)
        {
            HealthVaultPlatformItem.Current.UpdateItems(connection, accessor, itemsToUpdate);
        }

        /// <summary>
        /// Marks the specified health record item as deleted.
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
        /// <param name="itemsToRemove">
        /// The unique item identifiers of the items to remove.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Health record items are never completely deleted. They are marked 
        /// as deleted and are ignored for most normal operations. Items can 
        /// be undeleted by contacting customer service.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemsToRemove"/> parameter is empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// Errors removed the health record items from the server.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur, 
        /// no items will have been removed.
        /// </exception>
        /// 
        public static void RemoveItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItemKey> itemsToRemove)
        {
            HealthVaultPlatformItem.Current.RemoveItems(connection, accessor, itemsToRemove);
        }

        #endregion

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
        public static void RemoveApplicationAuthorization(
            ApplicationConnection connection,
            HealthRecordAccessor accessor)
        {
            HealthVaultPlatformRecord.Current.RemoveApplicationAuthorization(connection, accessor);
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
        /// Returns a dictionary of <see cref="HealthRecordItemTypePermission"/> 
        /// with health record item types as the keys. 
        /// </returns>
        /// 
        /// <remarks> 
        /// If the list of health record item types is empty, an empty dictionary is 
        /// returned. If for a health record item type, the person has 
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
        public static IDictionary<Guid, HealthRecordItemTypePermission> QueryPermissionsByTypes(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return HealthVaultPlatformRecord.Current.QueryPermissionsByTypes(connection, accessor, healthRecordItemTypeIds);
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
        /// A list of <see cref="HealthRecordItemTypePermission"/> 
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        /// 
        /// <remarks> 
        /// If the list of health record item types is empty, an empty list is 
        /// returned. If for a health record item type, the person has 
        /// neither online access nor offline access permissions, 
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type. 
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
        public static Collection<HealthRecordItemTypePermission> QueryPermissions(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return HealthVaultPlatformRecord.Current.QueryPermissions(connection, accessor, healthRecordItemTypeIds);
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
        /// which contains a collection of <see cref="HealthRecordItemTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        /// 
        /// <remarks> 
        /// If the list of health record item types is empty, an empty list is 
        /// returned. If for a health record item type, the person has 
        /// neither online access nor offline access permissions, 
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type. 
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
        public static HealthRecordPermissions QueryRecordPermissions(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return HealthVaultPlatformRecord.Current.QueryRecordPermissions(connection, accessor, healthRecordItemTypeIds);
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
        /// last updating application is authorized by the the last updating person to 
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
        /// A List of HealthRecordItems representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public static Collection<HealthRecordItem> GetValidGroupMembership(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> applicationIds)
        {
            return HealthVaultPlatformRecord.Current.GetValidGroupMembership(connection, accessor, applicationIds);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by 
        /// the properties of this class.
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
        /// The searcher that defines what items to return. .
        /// </param>
        /// 
        /// <returns>
        /// A collection of health record items that match the applied filters.
        /// </returns>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The response from the server was anything but 
        /// <see cref="HealthServiceStatusCode.Ok"/>.
        /// -or-
        /// <see cref="Microsoft.Health.HealthRecordSearcher.Filters"/> is empty
        /// or contains invalid filters.
        /// </exception>
        /// 
        public static ReadOnlyCollection<HealthRecordItemCollection> GetMatchingItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            return HealthVaultPlatformItem.Current.GetMatchingItems(connection, accessor, searcher);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by 
        /// the properties of this class.
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
        /// <returns>
        /// An XmlReader representing the raw results of the search.
        /// </returns>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// This method is typically used when the calling application wants to
        /// handle the raw health record item XML directly instead of using the 
        /// object model.
        /// </remarks>
        /// 
        public static XmlReader GetMatchingItemsReader(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            return HealthVaultPlatformItem.Current.GetMatchingItemsReader(connection, accessor, searcher);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by 
        /// the properties of this class.
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
        /// <returns>
        /// An XPathNavigator representing the raw results of the search.
        /// </returns>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// This method is typically used when the calling application wants to
        /// handle the raw health record item XML directly instead of using the 
        /// object model.
        /// </remarks>
        /// 
        public static XPathNavigator GetMatchingItemsRaw(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            return HealthVaultPlatformItem.Current.GetMatchingItemsRaw(connection, accessor, searcher);
        }

        /// <summary>
        /// Gets the health record items specified by the 
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
        /// <see cref="Microsoft.Health.HealthRecordView.Sections"/> does not
        /// contain the XML section in the view.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// There is a failure retrieving the items.
        /// -or-
        /// No filters have been specified.
        /// </exception>
        /// 
        public static string GetTransformedItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher,
            string transform)
        {
            return HealthVaultPlatformItem.Current.GetTransformedItems(connection, accessor, searcher, transform);
        }

        #endregion

        #region Provisioning
        /// <summary>
        /// Updates the application's configuration in HealthVault.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to make the update.
        /// </param>
        /// 
        /// <param name="applicationInfo">
        /// The updated <see cref="ApplicationInfo"/> instance.
        /// </param>
        /// 
        /// <remarks>
        /// This method makes a remote call to the HealthVault service.
        /// The calling application in the <paramref name="connection"/> must be the same as
        /// the application specified by this ApplicationInfo instance or its master application.
        /// Note, this update will replace all configuration elements for the application. It is 
        /// advised that <see cref="ApplicationProvisioning.Provisioner.GetApplication"/> is 
        /// called to retrieve the existing application configuration before changing values and 
        /// calling Update.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        public static void UpdateChildApplication(
            ApplicationConnection connection,
            ApplicationInfo applicationInfo)
        {
            HealthVaultPlatformProvisioning.Current.UpdateChildApplication(connection, applicationInfo);
        }

        /// <summary>
        /// Gets the configuration information for the specified child application ID.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to get the application information.
        /// </param>
        /// 
        /// <param name="childApplicationId">
        /// The unique application identifier for the child application to get the configuration
        /// information for.
        /// </param>
        /// 
        /// <returns>
        /// Configuration information for the specified child application.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="childApplicationId"/> is <see cref="Guid.Empty"/>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceAccessDeniedException">
        /// If the application specified in the <paramref name="connection"/> is not a master
        /// application, or if <paramref name="childApplicationId"/> does not identify a child
        /// application of the calling application.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If there is an error when the HealthVault service is called.
        /// </exception>
        /// 
        public static ApplicationInfo GetChildApplication(
            OfflineWebApplicationConnection connection,
            Guid childApplicationId)
        {
            return HealthVaultPlatformProvisioning.Current.GetChildApplication(connection, childApplicationId);
        }

        /// <summary>
        /// Adds a HealthVault application instance for a "child" application of the calling
        /// application.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to add the application.
        /// </param>
        /// 
        /// <param name="applicationConfigurationInformation">
        /// Configuration information about the application being provisioned.
        /// </param>
        /// 
        /// <returns>
        /// The new application identifier for the new application provided by HealthVault.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> or <paramref name="applicationConfigurationInformation"/>
        /// is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <see cref="ApplicationInfo.Name"/>, <see cref="ApplicationInfo.PublicKeys"/>,
        /// <see cref="ApplicationInfo.OfflineBaseAuthorizations"/>, <see cref="ApplicationInfo.Description"/>,
        /// <see cref="ApplicationInfo.AuthorizationReason"/>, or <see cref="ApplicationInfo.LargeLogo"/> 
        /// is not specified.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If there is an error when the HealthVault service is called.
        /// </exception>
        /// 
        public static Guid AddChildApplication(
            OfflineWebApplicationConnection connection,
            ApplicationInfo applicationConfigurationInformation)
        {
            return HealthVaultPlatformProvisioning.Current.AddChildApplication(connection, applicationConfigurationInformation);
        }

        #endregion Provisioning

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
        /// Gets the definitions for one or more health record item type definitions
        /// supported by HealthVault.
        /// </summary>
        /// 
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null 
        /// indicates that all health item types should be returned.
        /// </param>
        /// 
        /// <param name="sections">
        /// A collection of HealthRecordItemTypeSections enumeration values that indicate the type 
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
        public static IDictionary<Guid, HealthRecordItemTypeDefinition> GetHealthRecordItemTypeDefinition(
            IList<Guid> typeIds,
            HealthRecordItemTypeSections sections,
            IList<String> imageTypes,
            DateTime? lastClientRefreshDate,
            HealthServiceConnection connection)
        {
            return HealthVaultPlatformInformation.Current.GetHealthRecordItemTypeDefinition(
                typeIds,
                sections,
                imageTypes,
                lastClientRefreshDate,
                connection);
        }

        #endregion ItemTypeDefinitions

        #region PatientConnect
        /// <summary>
        /// Asks HealthVault to create a pending patient connection for the application specified
        /// by the connection with the specified user specific parameters.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// answer when they go to validate the connection in the HealthVault Shell.
        /// </param>
        /// 
        /// <param name="securityAnswer">
        /// The answer to the <paramref name="securityQuestion"/> which the patient must use
        /// when validating the connection in HealthVault Shell. The answer is case-insensitive but
        /// otherwise must match exactly. In most cases it is recommended that this is a single 
        /// word to prevent entry problems when validating the connection.
        /// </param>
        /// 
        /// <param name="callbackUrl">
        /// Not yet implemented. May be null.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="friendlyName"/>, <paramref name="securityQuestion"/>,
        /// <paramref name="securityAnswer"/>, or <paramref name="applicationPatientId"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static string CreatePatientConnection(
            OfflineWebApplicationConnection connection,
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            Uri callbackUrl,
            string applicationPatientId)
        {
            return HealthVaultPlatformPatientConnect.Current.CreatePatientConnection(
                connection,
                friendlyName,
                securityQuestion,
                securityAnswer,
                callbackUrl,
                applicationPatientId);
        }

        /// <summary>
        /// Deletes a request for a connection that has been made by the calling application but
        /// has not been validated by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application's identifier for the user which was used to create the connection 
        /// request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationPatientId"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void DeletePendingPatientConnection(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            HealthVaultPlatformPatientConnect.Current.DeletePendingPatientConnection(connection, applicationPatientId);
        }

        /// <summary>
        /// Updates an existing pending patient connection with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="oldApplicationPatientId">
        /// The application patient identifier that was used to make the initial connection request.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="oldApplicationPatientId"/> or <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void UpdatePatientConnectionApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            HealthVaultPlatformPatientConnect.Current.UpdatePatientConnectionApplicationPatientId(
                connection,
                oldApplicationPatientId,
                newApplicationPatientId);
        }

        /// <summary>
        /// Gets the connections for the application that people have accepted since the specified
        /// date.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application's connection to HealthVault.
        /// </param>
        /// 
        /// <param name="validatedSince">
        /// Connections that have been validated since this date will be returned. The date passed
        /// should be in UTC time.
        /// </param>
        /// 
        /// <returns>
        /// A collection of the connections that people have accepted.
        /// </returns>
        /// 
        /// <remarks>
        /// Validated connect requests are removed by HealthVault after 90 days. It is advised 
        /// that applications call <see cref="GetValidatedPatientConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
        /// daily or weekly to ensure that all validated connect requests are retrieved.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        public static Collection<ValidatedPatientConnection> GetValidatedPatientConnections(
            OfflineWebApplicationConnection connection,
            DateTime validatedSince)
        {
            return HealthVaultPlatformPatientConnect.Current.GetValidatedPatientConnections(
                connection,
                validatedSince);
        }

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters and the pre-allocated
        /// identity code.
        /// </summary>
        /// 
        /// <remarks>
        /// The password protected package supports 2 encryption algorithms, AES256 (recommended)
        /// and TripleDES. 
        /// <br/><br/>
        /// For AES256, the supported key size is 256 bits, the blocksize is 256 bits, the IV 
        /// length is 32 bytes.
        /// <br/><br/>
        /// For TripleDES, the supported key size is 192 bits, the blocksize is 64 bits, the IV 
        /// length is 8 bytes.
        /// <br/><br/>
        /// The encryption key should be derived using the answer, the salt, and the number of hash 
        /// iterations. The decryption will generate this key via the 
        /// <see cref="Rfc2898DeriveBytes"/> class, hence, encryption should use a similar or 
        /// identical process. To ensure case-insensitivity, the answer should be converted to its
        /// lower cased form using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The salt should be at least 8 bytes long.
        /// <br/><br/>
        /// It is recommended that the number of hash iterations be at least 10000.
        /// </remarks>
        /// 
        /// <param name="creationParameters">
        /// The parameters to use when creating the package. 
        /// </param>
        /// 
        /// <param name="connectPackage">
        /// The pending connect package that the user will add to his/her record. 
        /// This package's
        /// <see cref="HealthRecordItem"/>'s <see cref="BlobStore"/> must be an encrypted 
        /// blob of xml that represents a list of HealthRecordItems. This xml blob
        /// must be a sequence of <thing/> elements, each wrapping the XML representation of a 
        /// single HealthRecordItem. Each <thing/> element may be generated by calling 
        /// <see cref="HealthRecordItem.GetItemXml()"/>.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public static string CreateConnectPackage(
            ConnectPackageCreationParameters creationParameters,
            PasswordProtectedPackage connectPackage)
        {
            return CreateConnectPackage(creationParameters, connectPackage, null);
        }

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters and the pre-allocated
        /// identity code.
        /// </summary>
        /// 
        /// <remarks>
        /// The password protected package supports 2 encryption algorithms, AES256 (recommended)
        /// and TripleDES. 
        /// <br/><br/>
        /// For AES256, the supported key size is 256 bits, the blocksize is 256 bits, the IV 
        /// length is 32 bytes.
        /// <br/><br/>
        /// For TripleDES, the supported key size is 192 bits, the blocksize is 64 bits, the IV 
        /// length is 8 bytes.
        /// <br/><br/>
        /// The encryption key should be derived using the answer, the salt, and the number of hash 
        /// iterations. The decryption will generate this key via the 
        /// <see cref="Rfc2898DeriveBytes"/> class, hence, encryption should use a similar or 
        /// identical process. To ensure case-insensitivity, the answer should be converted to its
        /// lower cased form using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The salt should be at least 8 bytes long.
        /// <br/><br/>
        /// It is recommended that the number of hash iterations be at least 10000.
        /// </remarks>
        /// 
        /// <param name="creationParameters">
        /// The parameters to use when creating the package. 
        /// </param>
        /// 
        /// <param name="connectPackage">
        /// The pending connect package that the user will add to his/her record. 
        /// This package's
        /// <see cref="HealthRecordItem"/>'s <see cref="BlobStore"/> must be an encrypted 
        /// blob of xml that represents a list of HealthRecordItems. This xml blob
        /// must be a sequence of <thing/> elements, each wrapping the XML representation of a 
        /// single HealthRecordItem. Each <thing/> element may be generated by calling 
        /// <see cref="HealthRecordItem.GetItemXml()"/>.
        /// </param>
        /// 
        /// <param name="packageContentsBlobUrls">
        /// URLs of the streamed blobs of the package contents.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public static string CreateConnectPackage(
            ConnectPackageCreationParameters creationParameters,
            PasswordProtectedPackage connectPackage,
            IEnumerable<Uri> packageContentsBlobUrls)
        {
            return HealthVaultPlatformPatientConnect.Current.CreateConnectPackage(
                creationParameters,
                connectPackage,
                packageContentsBlobUrls);
        }

        /// <summary>
        /// Deletes all packages that have been created by the calling application 
        /// for the applicationPatientId and have not been accepted by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific user ID that was used to create the connection 
        /// request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationPatientId"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void DeletePendingConnectPackages(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            HealthVaultPlatformPatientConnect.Current.DeletePendingConnectPackages(
                connection,
                applicationPatientId);
        }

        /// <summary>
        /// Deletes a single package that has been created by the calling application but has not been 
        /// accepted by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="identityCode">
        /// The unique token that identifies the package.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="identityCode"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void DeletePendingConnectionPackageForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode)
        {
            HealthVaultPlatformPatientConnect.Current.DeletePendingConnectionPackageForIdentityCode(
                connection,
                identityCode);
        }

        /// <summary>
        /// Updates existing pending packages with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="oldApplicationPatientId">
        /// The application patient identifier that was used to create the initial package.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="oldApplicationPatientId"/> or <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void UpdateConnectPackageApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            HealthVaultPlatformPatientConnect.Current.UpdateConnectPackageApplicationPatientId(
                connection,
                oldApplicationPatientId,
                newApplicationPatientId);
        }

        /// <summary>
        /// Updates an existing pending package with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="identityCode">
        /// The unique token that identifies the package.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void UpdateConnectPackageApplicationPatientIdForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode,
            string newApplicationPatientId)
        {
            HealthVaultPlatformPatientConnect.Current.UpdateConnectPackageApplicationPatientIdForIdentityCode(
                connection,
                identityCode,
                newApplicationPatientId);
        }

        /// <summary>
        /// Allocates a package ID within HealthVault and returns it.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The package ID is allocated as a place holder for information that 
        /// is identifiable but not yet available through the HealthVault service.
        /// The returned package ID token should be stored or given to a patient, 
        /// then used in a call to CreateConnectPackage()
        /// to send the package data to the HealthVault service.</para>
        /// <para>The package ID is not a GUID.  It uses a shorter format that is more
        /// convenient for offline delivery and manual data entry.  The HealthVault 
        /// service guarantees that each package ID is unique for the lifetime of the 
        /// package.  Once the package has been accepted by the patient using the 
        /// HealthVault Shell, or explicitly deleted using the API, the package ID is 
        /// deallocated and may be reused.</para>
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public static String AllocateConnectPackageId(
            OfflineWebApplicationConnection connection)
        {
            return HealthVaultPlatformPatientConnect.Current.AllocateConnectPackageId(connection);
        }

        #endregion

        #region AlternateId

        /// <summary>
        /// Associates an alternate ID with a record.
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
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate ID is already associated by this application, the ErrorCode property
        /// will be set to DuplicateAlternateId.
        /// If the number of alternate IDs associated with a record exceeds the limit, the ErrorCode
        /// property will be set to AlternateIdsLimitExceeded.
        /// </exception>
        /// 
        public static void AssociateAlternateId(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            string alternateId)
        {
            HealthVaultPlatformAlternateId.Current.AssociateAlternateId(connection, accessor, alternateId);
        }

        /// <summary>
        /// Disassociates an alternate id with a record.
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
        /// <param name="alternateId">
        /// The alternate id.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public static void DisassociateAlternateId(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            string alternateId)
        {
            HealthVaultPlatformAlternateId.Current.DisassociateAlternateId(connection, accessor, alternateId);
        }

        /// <summary>
        /// Disassociates an alternate id with a record.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// 
        /// <param name="alternateId">
        /// The alternate id.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The connection, or alternateId parameters are null
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public static void DisassociateAlternateId(
            ApplicationConnection connection,
            string alternateId)
        {
            HealthVaultPlatformAlternateId.Current.DisassociateAlternateId(connection, alternateId);
        }

        /// <summary>
        /// Gets the list of alternate IDs that are associated with a record.
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
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public static Collection<string> GetAlternateIds(
            ApplicationConnection connection,
            HealthRecordAccessor accessor)
        {
            return HealthVaultPlatformAlternateId.Current.GetAlternateIds(connection, accessor);
        }

        /// <summary>
        /// Gets the person and record IDs that were previosly associated
        /// with an alternate ID.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// 
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public static PersonInfo GetPersonAndRecordForAlternateId(
            ApplicationConnection connection,
            string alternateId)
        {
            return HealthVaultPlatformAlternateId.Current.GetPersonAndRecordForAlternateId(connection, alternateId);
        }

        #endregion

        #region SelectInstance

        /// <summary>
        /// Gets the instance where a HealthVault account should be created
        /// for the specified account location.
        /// </summary>
        /// 
        /// <param name="preferredLocation">
        /// A user's preferred geographical location, used to select the best instance
        /// in which to create a new HealthVault account. If there is a location associated
        /// with the credential that will be used to log into the account, that location
        /// should be used.
        /// </param>
        /// 
        /// <param name="connection">
        /// The connection to use to perform the operation.
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
        public static HealthServiceInstance SelectInstance(
            HealthServiceConnection connection,
            Location preferredLocation)
        {
            return HealthVaultPlatformInformation.Current.SelectInstance(
                connection,
                preferredLocation);
        }
        #endregion

        #region MeaningfulUse
        /// <summary>
        /// Retrieves the Meaningful Use View, Download, and Transmit (VDT) Report for the application.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability. 
        /// </param>
        /// 
        /// <param name="reportingPeriodFilter">
        /// The UTC reporting period to be used to retrieve the VDT Report.
        /// </param>
        /// 
        /// <returns>
        /// An IEnumerable iterator of <see cref="PatientActivity"/> objects which contains the list of patients to which the
        /// data source sent a CCDA document where the event date (visit or discharge date) is within the specified reporting period,
        /// and additionally, the patient viewed, downloaded or transmitted to a 3rd party, health information in the record.
        /// </returns>
        /// 
        /// <remarks>
        /// <p>
        /// The View, Download, and Transmit (VDT) Report gives applications information to determine if they meet the Meaningful Use Stage 2 measure
        /// for patients taking action on their health information. For ambulatory settings, the measure states
        /// "More than 5 percent of all unique patients seen by the EP during the EHR reporting period (or their authorized representatives)
        /// view, download or transmit to a third party their health information". For inpatient settings, the measure states
        /// "More than 5 percent of all patients who are discharged from the Inpatient or emergency department (POS 21 or 23) of an eligible hospital
        /// or CAH (or their authorized representative) view, download or transmit to a third party their information during the EHR reporting period."
        /// (For more regarding this measure see "View, download, and transmit to a 3rd party"
        /// in the <a href="http://www.healthit.gov/policy-researchers-implementers/meaningful-use-stage-2">Meaningful Use Stage 2 Reference Grid</a>.)
        /// </p>
        /// <p>
        /// HealthVault returns an entry in the report for each patient to which the data source has sent a CCDA document and
        /// where the patient has viewed, downloaded, or transmitted to a 3rd party health information from the health record.
        /// The entries in the report are filtered such that only those CCDAs having an event date (which is the visit date for ambulatory settings,
        /// and the discharge date for inpatient settings) within the specified reporting period filter, will lead to the receiving patient being included in the report.
        /// The application can use the patient identifiers in the report to help calculate the percentage value of the measure.
        /// </p>
        /// <p>
        /// Note that HealthVault does not know the full list of patients seen by the provider. To calculate the percentage for this measure,
        /// the application must take into account the total number of unique patients seen or discharged within the desired reporting period.
        /// </p>
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception> 
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> or <paramref name="reportingPeriodFilter"/>
        /// is <b>null</b>.
        /// </exception>
        public static IEnumerable<PatientActivity> GetMeaningfulUseVDTReport(
            HealthServiceConnection connection,
            DateRange reportingPeriodFilter)
        {
            return new PatientActivityCollection(connection, reportingPeriodFilter);
        }

        /// <summary>
        /// Retrieves the Meaningful Use Timely Access Report for the application.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability. 
        /// </param>
        /// 
        /// <param name="reportingPeriodFilter">
        /// The UTC reporting period to be used to retrieve the Timely Access Report.
        /// </param>
        /// 
        /// <returns>
        /// An IEnumerable iterator of <see cref="DocumentReceipt"/> objects which contains the
        /// list of document receipts having an event date (visit or discharge date) within the specified reporting period.
        /// </returns>
        /// 
        /// <remarks>
        /// <p>
        /// The Timely Access Report gives applications information to determine if they meet the Meaningful Use Stage 2 measure for timely patient access to information.
        /// For ambulatory settings, the measure states "More than 50 percent of all unique patients seen by the EP during the EHR reporting period are provided timely
        /// (available to the patient within 4 business days after the information is available to the EP)
        /// online access to their health information subject to the EP's discretion to withhold certain information".
        /// For the inpatient settings, the measure states "More than 50 percent of all patients who are discharged from the
        /// Inpatient or emergency department (POS 21 or 23) of an eligible hospital or CAH have their information available online
        /// within 36 hours of discharge". (For more regarding this measure see "View, download, and transmit to a 3rd party"
        /// in the <a href="http://www.healthit.gov/policy-researchers-implementers/meaningful-use-stage-2">Meaningful Use Stage 2 Reference Grid</a>.)
        /// </p>
        /// <p>
        /// HealthVault returns an entry in the report for each CCDA document received by the data source having an event date
        /// (which is the visit date for ambulatory settings, and the discharge date for inpatient settings) within the specified
        /// reporting period filter. Each entry consists of the date the document was made available to the patient in HealthVault, the event date,
        /// and the patient ID as specified in the CCDA document. The application can use this information to help calculate the percentage value of the measure.
        /// </p>
        /// <p>
        /// Note that HealthVault does not know the full list of patients seen by the provider and does not apply any logic to determine whether patients
        /// had access to the data within the timeframe specified by the measure. To calculate the percentage for this measure, the application must take into
        /// account the total number of unique patients seen or discharged within the desired reporting period, and for each patient determine if the data was made
        /// available to them within the measure's specified timeframe.
        /// </p>
        /// 
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception> 
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> or <paramref name="reportingPeriodFilter"/> is <b>null</b>.
        /// </exception>
        public static IEnumerable<DocumentReceipt> GetMeaningfulUseTimelyAccessDocumentReport(
            HealthServiceConnection connection,
            DateRange reportingPeriodFilter)
        {
            return new DocumentReceiptCollection(connection, reportingPeriodFilter);
        }

        /// <summary>
        /// Retrieves the Meaningful Use Timely Access Report for applications that use DOPU to transfer CCDA documents to HealthVault.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for this operation. The connection
        /// must have application capability. 
        /// </param>
        /// 
        /// <param name="availableDateFilter">
        /// The UTC date range used to filter the entries in the Timely Access Report by the date the CCDA document was made available to HealthVault.
        /// </param>
        /// 
        /// <returns>
        /// An IEnumerable iterator of <see cref="DOPUDocumentReceipt"/> objects which contains the list of CCDA document receipts sent to HealthVault using DOPU, filtered by the available date range. 
        /// </returns>
        /// 
        /// <remarks>
        /// <p>
        /// The Timely Access Report gives applications information to determine if they meet the Meaningful Use Stage 2 measure for timely patient access to information.
        /// For ambulatory settings, the measure states "More than 50 percent of all unique patients seen by the EP during the EHR reporting period are provided timely
        /// (available to the patient within 4 business days after the information is available to the EP) online access to their health information subject to the EP's discretion to withhold certain information".
        /// For the inpatient settings, the measure states "More than 50 percent of all patients who are discharged from the Inpatient or emergency department (POS 21 or 23) of an eligible hospital
        /// or CAH have their information available online within 36 hours of discharge". (For more regarding this measure see "View, download, and transmit to a 3rd party"
        /// in the <a href="http://www.healthit.gov/policy-researchers-implementers/meaningful-use-stage-2">Meaningful Use Stage 2 Reference Grid</a>.)
        /// </p>
        /// <p>
        /// HealthVault returns an entry in the report for each CCDA document received by the data source using DOPU.
        /// The entries are filtered to those that were made available to HealthVault within the specified available date range filter.
        /// Each entry consists of the date the document was made available to the patient in HealthVault, and the identifier of the DOPU package that was made available to the patient.
        /// The application can use this information to help calculate the percentage value of the measure.
        /// </p>
        /// <p>
        /// Note that HealthVault does not know the full list of patients seen by the provider and does not apply any logic to determine whether patients had access to the data within the
        /// timeframe specified by the measure. To calculate the percentage for this measure, the application must take into account the total number of unique patients seen or
        /// discharged within the desired reporting period, and for each patient determine if the data was made available to them within the measure's specified timeframe.
        /// </p>
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception> 
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> or <paramref name="availableDateFilter"/>
        /// is <b>null</b>.
        /// </exception>
        public static IEnumerable<DOPUDocumentReceipt> GetMeaningfulUseTimelyAccessDOPUDocumentReport(
            HealthServiceConnection connection,
            DateRange availableDateFilter)
        {
            return new DOPUDocumentReceiptCollection(connection, availableDateFilter);
        }
        #endregion
    }
}

