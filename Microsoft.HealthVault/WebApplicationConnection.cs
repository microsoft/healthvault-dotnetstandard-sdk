// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault.Web
{

    /// <summary>
    /// Represents an authenticated interface to HealthVault. 
    /// </summary>
    /// 
    /// <remarks>
    /// Most operations performed against the service require authentication. 
    /// A connection must be made to HealthVault to access the
    /// web methods that the service exposes. The class does not maintain
    /// an open connection to the service. It uses XML over HTTP to 
    /// make requests and receive responses from the service. The connection
    /// just maintains the data necessary to make the request.
    /// <br/><br/>
    /// An authenticated connection takes the user name and password, and
    /// authenticates against HealthVault and then stores an 
    /// authentication token which is then passed to the service on each 
    /// subsequent request. An authenticated connection is required for 
    /// accessing a person's health record. 
    /// <br/><br/>
    /// For operations that do not require authentication, the 
    /// <see cref="AnonymousConnection"/> or <see cref="ApplicationConnection"/>
    /// class can be used.
    /// </remarks>
    /// 
    public class WebApplicationConnection : AuthenticatedConnection
    {
        #region ctors

        internal WebApplicationConnection()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class with 
        /// default values from the application or web configuration file.
        /// </summary>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to
        /// HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="credential"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public WebApplicationConnection(Credential credential)
            : base(credential)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class 
        /// with the HealthVault web-service instance and credential.
        /// </summary>
        /// 
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to 
        /// HealthVault.
        /// </param>
        /// 
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        /// 
        public WebApplicationConnection(
            HealthServiceInstance serviceInstance,
            Credential credential)
            : base(
                serviceInstance,
                credential)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class 
        /// with the specified app-ID, and credential.
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to 
        /// HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="credential"/> is <b>null</b>.
        /// </exception>
        /// 
        public WebApplicationConnection(
            Guid callingApplicationId,
            Credential credential)
            : base(
                callingApplicationId,
                credential)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class 
        /// with the specified app-ID, HealthVault web-service instance, and credential.
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        /// 
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to 
        /// HealthVault.
        /// </param>
        /// 
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        /// 
        public WebApplicationConnection(
            Guid callingApplicationId,
            HealthServiceInstance serviceInstance,
            Credential credential)
            : base(
                callingApplicationId,
                serviceInstance,
                credential)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class 
        /// with the specified app-ID, URL, and credential.
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        /// 
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service.
        /// </param>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to 
        /// HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter or
        /// <paramref name="credential"/> is <b>null</b>.
        /// </exception>
        /// 
        public WebApplicationConnection(
            Guid callingApplicationId,
            Uri healthServiceUrl,
            Credential credential)
            : base(
                callingApplicationId,
                healthServiceUrl,
                credential)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationConnection"/> class 
        /// with the specified app-ID, string-formatted URL, and credential.
        /// </summary>
        /// 
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        /// 
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service.
        /// </param>
        /// 
        /// <param name="credential">
        /// The credential of the user to authenticate for access to 
        /// HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter or
        /// <paramref name="credential"/> is <b>null</b>.
        /// </exception>
        /// 
        public WebApplicationConnection(
            Guid callingApplicationId,
            string healthServiceUrl,
            Credential credential)
            : this(
                callingApplicationId,
                new Uri(healthServiceUrl),
                credential)
        {
        }

        #endregion ctors

        #region Message

        /// <summary>
        /// Sends an insecure message to the specified message recipients.
        /// </summary>
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
        //[Obsolete("Use HealthServicePlatform.SendInsecureMessage instead")]
        public void SendInsecureMessage(
            IList<MailRecipient> mailRecipient,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessage(this,
                                                    mailRecipient,
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
        //[Obsolete("Use HealthServicePlatform.SendInsecureMessage instead")]
        public void SendInsecureMessage(
            IList<Guid> personIds,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessage(this,
                                                    personIds,
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
        //[Obsolete("Use HealthServicePlatform.SendInsecureMessageToCustodians instead")]
        public void SendInsecureMessageToCustodians(
            Guid recordId,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            HealthVaultPlatform.SendInsecureMessageToCustodians(this,
                                                                recordId,
                                                                addressMustBeValidated,
                                                                subject,
                                                                textBody,
                                                                htmlBody);
        }

        #endregion Message
    }
}

