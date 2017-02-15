// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Web.Authentication;
using System;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Represents a connection for an application to HealthVault for
    /// operations that are performed when a user is offline using the
    /// permissions granted by the user to the application.
    /// </summary>
    ///
    /// <remarks>
    /// A connection must be made to HealthVault to access the
    /// Web methods that the service exposes. This class does not maintain
    /// an open connection to the service.  It uses XML over HTTP to
    /// to make requests and receive responses from the service. The connection
    /// just maintains the data necessary to make the request.
    /// <br/><br/>
    /// For operations that require the user to be online, use the
    /// <see cref="AuthenticatedConnection"/> class.
    /// </remarks>
    ///
    public class OfflineWebApplicationConnection : ApplicationConnection
    {
        #region ctors

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with default values.
        /// </summary>
        ///
        public OfflineWebApplicationConnection()
            : this(
                null,
                null,
                Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified person identification.
        /// </summary>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        public OfflineWebApplicationConnection(
            Guid offlinePersonId)
            : this(
                null,
                null,
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class using the specified HealthVault web-service instance and person identification.
        /// </summary>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        public OfflineWebApplicationConnection(
            HealthServiceInstance serviceInstance,
            Guid offlinePersonId)
            : this(
                null,
                serviceInstance,
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified credential, HealthVault web-service instance, and person identification.
        /// </summary>
        ///
        /// <param name="credential">
        /// The HealthVault application credential used to authenticate
        /// the connection.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// If the web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        public OfflineWebApplicationConnection(
            WebApplicationCredential credential,
            HealthServiceInstance serviceInstance,
            Guid offlinePersonId)
            : base(serviceInstance)
        {
            if (credential == null)
            {
                credential =
                    new WebApplicationCredential(
                        ApplicationId,
                        ApplicationCertificateStore.Current.ApplicationCertificate);
            }
            Credential = credential;
            if (offlinePersonId != Guid.Empty)
            {
                _offlinePersonId = offlinePersonId;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified credential and person identification.
        /// </summary>
        ///
        /// <param name="credential">
        /// The HealthVault application credential used to authenticate
        /// the connection.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// If the web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        ///
        public OfflineWebApplicationConnection(
            WebApplicationCredential credential,
            Guid offlinePersonId)
            : this(credential, null, offlinePersonId)
        {
        }

        /// <summary>
        /// Creates an instance of the class for the specified application,
        /// and HealthVault URL.
        /// </summary>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service. If an application does not add "/wildcat.ashx" at the end of
        /// the URL, the constructor will add it automatically.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUrl"/> property is not properly
        /// formatted.
        /// </exception>
        ///
        public OfflineWebApplicationConnection(
            Guid callingApplicationId,
            string healthServiceUrl,
            Guid offlinePersonId)
            : this(
                new WebApplicationCredential(callingApplicationId),
                callingApplicationId,
                new Uri(healthServiceUrl),
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified application, HealthVault web-service instance, and
        /// person identification.
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
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        ///
        public OfflineWebApplicationConnection(
            Guid callingApplicationId,
            HealthServiceInstance serviceInstance,
            Guid offlinePersonId)
            : this(
                new WebApplicationCredential(callingApplicationId),
                callingApplicationId,
                serviceInstance,
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified application, HealthVault service URL, and
        /// person identification.
        /// </summary>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service. If an application does not add "/wildcat.ashx" at the end of
        /// the URL, the constructor will add it automatically.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUrl"/> property is not properly
        /// formatted.
        /// </exception>
        ///
        public OfflineWebApplicationConnection(
            Guid callingApplicationId,
            Uri healthServiceUrl,
            Guid offlinePersonId)
            : this(
                new WebApplicationCredential(callingApplicationId),
                callingApplicationId,
                healthServiceUrl,
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified credential, application, HealthVault
        /// web-service instance, and person identification.
        /// </summary>
        ///
        /// <param name="credential">
        /// The HealthVault application credential used to authenticate
        /// the connection.
        /// </param>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        ///
        public OfflineWebApplicationConnection(
            WebApplicationCredential credential,
            Guid callingApplicationId,
            HealthServiceInstance serviceInstance,
            Guid offlinePersonId)
            : base(
                callingApplicationId,
                serviceInstance)
        {
            Credential = credential;
            if (offlinePersonId != Guid.Empty)
            {
                _offlinePersonId = offlinePersonId;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified credential, application, string-formatted
        /// HealthVault service URL, and person identification.
        /// </summary>
        ///
        /// <param name="credential">
        /// The HealthVault application credential used to authenticate
        /// the connection.
        /// </param>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service. If an application does not add "/wildcat.ashx" at the end of
        /// the URL, the constructor will add it automatically.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUrl"/> parameter is not a properly
        /// formatted URL.
        /// </exception>
        ///
        public OfflineWebApplicationConnection(
            WebApplicationCredential credential,
            Guid callingApplicationId,
            string healthServiceUrl,
            Guid offlinePersonId)
            : this(
                credential,
                callingApplicationId,
                new Uri(healthServiceUrl),
                offlinePersonId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OfflineWebApplicationConnection"/>
        /// class with the specified credential, application, HealthVault
        /// service URL, and person identification.
        /// </summary>
        ///
        /// <param name="credential">
        /// The HealthVault application credential used to authenticate the connection.
        /// </param>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault service. If an application does not add "/wildcat.ashx" at the end of
        /// the URL, the constructor will add it automatically.
        /// </param>
        ///
        /// <param name="offlinePersonId">
        /// The unique identifier of the offline person who granted permissions
        /// to the application to perform operations.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthServiceUrl"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUrl"/> property is not properly
        /// formatted.
        /// </exception>
        ///
        public OfflineWebApplicationConnection(
            WebApplicationCredential credential,
            Guid callingApplicationId,
            Uri healthServiceUrl,
            Guid offlinePersonId)
            : base(
                callingApplicationId,
                healthServiceUrl)
        {
            Credential = credential;
            if (offlinePersonId != Guid.Empty)
            {
                _offlinePersonId = offlinePersonId;
            }
        }

        #endregion ctors

        #region App Authentication

        /// <summary>
        /// Authenticates the application with HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// It is not necessary to explicitly call this method before calling
        /// any of the methods that access the service. Those methods will
        /// call this method if the user has not already been authenticated.
        /// This method is provided as a convenience to allow for separate
        /// error handling for authorization errors.
        /// </remarks>
        ///
        /// <exception cref="SecurityException">
        /// The caller does not have permission to connect to the requested
        /// URI or a URI that the request is redirected to.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The authorization URL specified to the constructor is not a
        /// valid URI.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The authorization was not returned in the response from the server.
        /// </exception>
        ///
        public void Authenticate()
        {
            Credential.AuthenticateIfRequired(
                this,
                this.ApplicationId);
        }

        #endregion App Authentication

        #region ApplicationSettings

        /// <summary>
        /// Gets the application settings for the current application and person.
        /// </summary>
        ///
        /// <returns>
        /// A complete set of application settings including the XML, selected record ID, etc.
        /// </returns>
        ///
        public ApplicationSettings GetAllApplicationSettings()
        {
            return HealthVaultPlatform.GetApplicationSettings(this);
        }

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
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
        public IXPathNavigable GetApplicationSettings()
        {
            return HealthVaultPlatform.GetApplicationSettingsAsXml(this);
        }

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="applicationSettings">
        /// The application settings XML.
        /// </param>
        ///
        /// <remarks>
        /// This might be <b>null</b> if no application settings have been stored
        /// for the application or user.
        /// </remarks>
        ///
        public void SetApplicationSettings(
                IXPathNavigable applicationSettings)
        {
            HealthVaultPlatform.SetApplicationSettings(this, applicationSettings);
        }

        #endregion ApplicationSettings

        /// <summary>
        /// Gets or sets the unique identifier of the offline person who granted
        /// permissions to the calling application to perform certain
        /// operations.
        /// </summary>
        ///
        /// <value>
        /// A GUID representing the offline person.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <see cref="System.Guid.Empty"/>.
        /// </exception>
        ///
        public Guid OfflinePersonId
        {
            get { return _offlinePersonId; }
            set
            {
                Validator.ThrowArgumentExceptionIf(
                    value == Guid.Empty,
                    "offlinePersonId",
                    "CtorOfflinePersonIdEmpty");

                _offlinePersonId = value;
            }
        }
        private Guid _offlinePersonId;
    }
}
