// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.HealthVault.MeaningfulUse;
using Microsoft.HealthVault.Web;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Base class that represents a connection of an application 
    /// to the HealthVault service for either online or offline operations.
    /// </summary>
    /// 
    /// <remarks>
    /// You must connect to the HealthVault service to access its
    /// web methods. This class does not maintain
    /// an open connection to the service, but uses XML over HTTP to 
    /// make requests and receive responses from the service. The connection
    /// only maintains the data necessary for the request.
    /// <br/><br/>
    /// For operations that require authentication, use the 
    /// <see cref="AuthenticatedConnection"/> class.
    /// </remarks>
    /// 
    public class ApplicationConnection : HealthServiceConnection
    {
        #region ctors

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class with default values taken from the application or web 
        /// configuration file.
        /// </summary>
        /// 
        /// <exception cref="InvalidConfigurationException">
        /// If the web or application configuration file does not contain 
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        /// 
        public ApplicationConnection()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class for the application having the specified
        /// HealthVault web-service instance.
        /// </summary>
        /// 
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        /// 
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        /// 
        public ApplicationConnection(
            HealthServiceInstance serviceInstance)
            : base(serviceInstance)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class for the application having the specified globally unique 
        /// identifier (GUID) and HealthVault web-service instance.
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The GUID of the HealthVault application.
        /// </param>
        /// 
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        /// 
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        /// 
        public ApplicationConnection(
            Guid callingApplicationId,
            HealthServiceInstance serviceInstance)
            : base(
                callingApplicationId,
                serviceInstance)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class for the application having the specified globally unique 
        /// identifier (GUID) and HealthVault service uniform resource
        /// locator (URL).
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The GUID of the HealthVault application.
        /// </param>
        /// 
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault web service.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public ApplicationConnection(
            Guid callingApplicationId,
            Uri healthServiceUrl)
            : base(
                callingApplicationId,
                healthServiceUrl)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class for the application having the specified globally unique 
        /// identifier (GUID) and string representing the HealthVault service 
        /// uniform resource locator (URL).
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The GUID of the HealthVault application.
        /// </param>
        /// 
        /// <param name="healthServiceUrl">
        /// A string representing the URL of the HealthVault application.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUrl"/> string is not formatted 
        /// properly.
        /// </exception>
        /// 
        public ApplicationConnection(
            Guid callingApplicationId,
            string healthServiceUrl)
            : this(
                callingApplicationId,
                new Uri(healthServiceUrl))
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ApplicationConnection"/> 
        /// class for the application having the specified globally unique 
        /// identifier (GUID). 
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The GUID of the HealthVault application.
        /// </param>
        /// 
        public ApplicationConnection(
            Guid callingApplicationId)
            : base(callingApplicationId)
        {
        }

        #endregion ctors

        #region CreateRequest

        /// <summary>
        /// Represents a simple wrapper around the XML request for the web 
        /// service.
        /// </summary>
        /// 
        /// <param name="record">
        /// The record that prepopulates the request.
        /// </param>
        /// 
        /// <param name="methodName">
        /// The name of the method to call.
        /// </param>
        /// 
        /// <param name="methodVersion">
        /// The version of the method to call.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="HealthServiceRequest"/> that wraps 
        /// the XML request for the web service.
        /// </returns>
        /// 
        /// <remarks>
        /// This method skips the object model provided by the other
        /// methods of this class and acts as a simple wrapper around
        /// the XML request for the web service. The caller must provide the
        /// parameters in the correct format for the called method and parse 
        /// the response data.
        /// The information in the <paramref name="record"/> parameter
        /// prepopulates the request.
        /// <br/><br/>
        /// By creating the request object directly rather than using the 
        /// object model, you can pass parameters that are not directly 
        /// exposed by the object model. Please provide feedback
        /// to us if this is the case. This also allows for request-specific
        /// parameters that are set by default when using the object model. 
        /// For example, you can change the language for a specific request 
        /// without affecting other requests to the HealthVault service through
        /// the same connection.
        /// <br/><br/>
        /// <br/><br/>
        /// You can find a list of the HealthVault methods (including their
        /// request and response schema) at 
        /// <a href="http://labs.microsoftlivehealth.com/Lab">the Microsoft
        /// Live Health Lab</a> site.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="methodName"/> parameter is <b>null</b> or 
        /// empty.
        /// </exception>
        /// 
        public virtual HealthServiceRequest CreateRequest(
            HealthRecordAccessor record,
            string methodName,
            int methodVersion)
        {
            Validator.ThrowIfArgumentNull(record, "record", "CreateRequestNullRecord");
            Validator.ThrowIfStringNullOrEmpty(methodName, "methodName");

            HealthServiceRequest request =
                CreateRequest(methodName, methodVersion, false);

            request.RecordId = record.Id;

            return request;
        }

        internal override HealthServiceRequest CreateRequest(
            string methodName,
            int methodVersion,
            bool forAuthentication)
        {
            Validator.ThrowIfStringNullOrEmpty(methodName, "methodName");

            HealthServiceRequest request =
                new HealthServiceRequest(
                    this,
                    methodName,
                    methodVersion);

            return request;
        }

        #endregion CreateRequest

        #region Health Lexicon

        /// <summary>
        /// Retrieves a list of vocabulary items for the specified vocabulary.  
        /// </summary>
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
        public Vocabulary GetVocabulary(string name)
        {
            return HealthVaultPlatform.GetVocabulary(this, name);
        }

        /// <summary>
        /// Retrieves a list of vocabulary items for the specified vocabulary
        /// and culture.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetVocabulary() instead.")]
        public Vocabulary GetVocabulary(VocabularyKey vocabularyKey, bool cultureIsFixed)
        {
            return HealthVaultPlatform.GetVocabulary(this, vocabularyKey, cultureIsFixed);
        }

        /// <summary>
        /// Retrieves lists of vocabulary items for the specified 
        /// vocabularies and culture.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetVocabulary() instead.")]
        public ReadOnlyCollection<Vocabulary> GetVocabulary(
            IList<VocabularyKey> vocabularyKeys, bool cultureIsFixed)
        {
            return HealthVaultPlatform.GetVocabulary(this, vocabularyKeys, cultureIsFixed);
        }

        /// <summary>
        /// Retrieves a collection of key information for identifying and 
        /// describing the vocabularies in the system.
        /// </summary>
        /// 
        /// <returns>
        /// A collection of keys identifying the vocabularies in the system.
        /// </returns>
        /// 
        //[Obsolete("Use HealthServicePlatform.GetVocabularyKeys() instead.")]
        public ReadOnlyCollection<VocabularyKey> GetVocabularyKeys()
        {
            return HealthVaultPlatform.GetVocabularyKeys(this);
        }

        /// <summary>
        /// Searches the keys of vocabularies defined by the HealthVault service.
        /// </summary>
        /// 
        /// <remarks>
        /// This method does a text search of vocabulary names and descriptions.
        /// </remarks>
        /// 
        /// <param name="searchString">
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
        /// If <paramref name="searchString"/> is <b>null</b> or empty or greater 
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
        //[Obsolete("Use HealthServicePlatform.SearchVocabularyKeys() instead.")]
        public ReadOnlyCollection<VocabularyKey> SearchVocabularyKeys(
            string searchString,
            VocabularySearchType searchType,
            int? maxResults)
        {
            return HealthVaultPlatform.SearchVocabularyKeys(this, searchString, searchType, maxResults);
        }

        /// <summary>
        /// Searches a specific vocabulary and retrieves the matching vocabulary items.
        /// </summary>
        /// 
        /// <remarks>
        /// This method does text search matching of display text and abbreviation text
        /// for the culture defined by the <see cref="HealthServiceConnection.Culture"/>. 
        /// The <paramref name="searchString"/> is a string of characters in the specified 
        /// culture. 
        /// </remarks>
        /// 
        /// <param name="vocabularyKey">
        /// The <see cref="VocabularyKey"/> defining the vocabulary to search. If the 
        /// family is not specified, the default HealthVault vocabulary family is used. 
        /// If the version is not specified, the most current version of the vocabulary 
        /// is used.
        /// </param>
        /// 
        /// <param name="searchString">
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
        /// If <paramref name="searchString"/> is <b>null</b> or empty or greater 
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
        //[Obsolete("Use HealthServicePlatform.SearchVocabulary() instead.")]
        public VocabularyItemCollection SearchVocabulary(
            VocabularyKey vocabularyKey,
            string searchString,
            VocabularySearchType searchType,
            int? maxResults)
        {
            return HealthVaultPlatform.SearchVocabulary(this, vocabularyKey, searchString, searchType, maxResults);
        }

        #endregion Health Lexicon

        #region Send Message

        /// <summary>
        /// Sends an insecure message originating from the application to 
        /// the specified message recipients. 
        /// </summary>
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
        public void SendInsecureMessageFromApplication(
            IList<MailRecipient> mailRecipient,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessageFromApplication(
                this,
                mailRecipient,
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
        public void SendInsecureMessageFromApplication(
            IList<Guid> recipientPersonIds,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessageFromApplication(
                this,
                recipientPersonIds,
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
        /// See <see cref="OfflineWebApplicationConnection" /> 
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
        public void SendInsecureMessageToCustodiansFromApplication(
            Guid recordId,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessageToCustodiansFromApplication(
                this,
                recordId,
                addressMustBeValidated,
                senderMailboxName,
                senderDisplayName,
                subject,
                textBody,
                htmlBody);
        }


        #endregion Send Message

        #region GetPersonInfo

        /// <summary>
        /// Gets the information about the person specified.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetPersonInfo() instead.")]
        public PersonInfo GetPersonInfo()
        {
            return HealthVaultPlatform.GetPersonInfo(this);
        }

        #endregion GetPersonInfo

        #region GetAuthorizedPeople

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetAuthorizedPeople() instead.")]
        public IEnumerable<PersonInfo> GetAuthorizedPeople()
        {
            return GetAuthorizedPeople(new GetAuthorizedPeopleSettings());
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
        //[Obsolete("Use HealthServicePlatform.GetAuthorizedPeople() instead.")]
        public IEnumerable<PersonInfo> GetAuthorizedPeople(GetAuthorizedPeopleSettings settings)
        {
            return HealthVaultPlatform.GetAuthorizedPeople(this, settings);
        }

        #endregion GetAuthorizedPeople

        #region GetAuthorizedRecords

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the records identified
        /// by the specified <paramref name="recordIds"/>.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetAuthorizedRecords() instead.")]
        public Collection<HealthRecordInfo> GetAuthorizedRecords(
            IList<Guid> recordIds)
        {
            return HealthVaultPlatform.GetAuthorizedRecords(this, recordIds);
        }

        #endregion GetAuthorizedRecords

        #region GetApplicationInfo

        /// <summary>
        /// Gets the application configuration information for the calling application.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetApplicationInfo() instead.")]
        public ApplicationInfo GetApplicationInfo()
        {
            return HealthVaultPlatform.GetApplicationInfo(this);
        }

        /// <summary>
        /// Gets the application configuration information for the calling application.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetApplicationInfo() instead.")]
        public ApplicationInfo GetApplicationInfo(
            Boolean allLanguages)
        {
            return HealthVaultPlatform.GetApplicationInfo(this, allLanguages);
        }

        #endregion

        #region GetUpdatedRecordsForApplication

        /// <summary>
        /// Gets a list of health record IDs for the current application, 
        /// that optionally have been updated since a specified date.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetUpdatedRecordsForApplication() instead.")]
        public IList<Guid> GetUpdatedRecordsForApplication(DateTime? updatedDate)
        {
            return HealthVaultPlatform.GetUpdatedRecordsForApplication(this, updatedDate);
        }

        /// <summary>
        /// Gets a list of <see cref="HealthRecordUpdateInfo"/> objects for the current application, 
        /// that optionally have been updated since a specified date.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.GetUpdatedRecordInfoForApplication() instead.")]
        public IList<HealthRecordUpdateInfo> GetUpdatedRecordInfoForApplication(
            DateTime? updatedDate)
        {
            return HealthVaultPlatform.GetUpdatedRecordInfoForApplication(this, updatedDate);
        }

        #endregion

        #region NewSignupCode

        /// <summary>
        /// Generates a new signup code that should be passed to HealthVault Shell in order
        /// to create a new user account.
        /// </summary>
        /// 
        /// <returns>
        /// A signup code that can be used to create an account.
        /// </returns>
        /// 
        //[Obsolete("Use HealthServicePlatform.NewSignupCode() instead.")]
        public string NewSignupCode()
        {
            return HealthVaultPlatform.NewSignupCode(this);
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
        public HealthServiceInstance SelectInstance(Location preferredLocation)
        {
            return HealthVaultPlatform.SelectInstance(this, preferredLocation);
        }

        #endregion

        #region MeaningfulUse
        /// <summary>
        /// Retrieves the Meaningful Use View, Download, and Transmit (VDT) Report for the application.
        /// </summary>
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
        /// If <paramref name="reportingPeriodFilter"/> is <b>null</b>.
        /// </exception>
        public virtual IEnumerable<PatientActivity> GetMeaningfulUseVDTReport(
            DateRange reportingPeriodFilter)
        {
            return HealthVaultPlatform.GetMeaningfulUseVDTReport(this, reportingPeriodFilter);
        }

        /// <summary>
        /// Retrieves the Meaningful Use Timely Access Report for the application.
        /// </summary>
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
        /// If <paramref name="reportingPeriodFilter"/> is <b>null</b>.
        /// </exception>
        public virtual IEnumerable<DocumentReceipt> GetMeaningfulUseTimelyAccessDocumentReport(
            DateRange reportingPeriodFilter)
        {
            return HealthVaultPlatform.GetMeaningfulUseTimelyAccessDocumentReport(this, reportingPeriodFilter);
        }

        /// <summary>
        /// Retrieves the Meaningful Use Timely Access Report for applications that use DOPU to transfer CCDA documents to HealthVault.
        /// </summary>
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
        /// If <paramref name="availableDateFilter"/> is <b>null</b>.
        /// </exception>
        public virtual IEnumerable<DOPUDocumentReceipt> GetMeaningfulUseTimelyAccessDOPUDocumentReport(
            DateRange availableDateFilter)
        {
            return HealthVaultPlatform.GetMeaningfulUseTimelyAccessDOPUDocumentReport(this, availableDateFilter);
        }
        #endregion
    }
}