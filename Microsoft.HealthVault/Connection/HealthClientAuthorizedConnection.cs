// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Web;
using Microsoft.HealthVault.Web.Authentication;
using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents an authorized connection between a HealthVault client application
    /// and the HealthVault service.
    /// </summary>
    ///
    /// <remarks>
    /// HealthClientAuthorizedConnection is a connection used by a HealthVault client
    /// application. The connection is authenticated using an
    /// application certificate in the user store, and does not require an authenticated user.
    /// The connection is only valid for <see cref="HealthClientApplication"/> instances, and
    /// each instance must be authorized using the HealthVault Shell service.
    /// </remarks>
    ///
    public class HealthClientAuthorizedConnection : OfflineWebApplicationConnection
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of HealthClientAuthorizedConnection using
        /// connection information stored in a configuration file.
        /// </summary>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// The web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        ///
        public HealthClientAuthorizedConnection()
        {
        }

        /// <summary>
        /// Creates an instance of HealthClientAuthorizedConnection
        /// using a specified <see cref="WebApplicationCredential"/> and
        /// connection information stored in a configuration file..
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application.
        /// </param>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// The web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        ///
        public HealthClientAuthorizedConnection(
                WebApplicationCredential webApplicationCredential)
            : base(webApplicationCredential, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of HealthClientAuthorizedConnection
        /// using a specified <see cref="WebApplicationCredential"/> and HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application.
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
        public HealthClientAuthorizedConnection(
            WebApplicationCredential webApplicationCredential,
            HealthServiceInstance serviceInstance)
            : base(webApplicationCredential, serviceInstance, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of HealthClientAuthorizedConnection
        /// using a specified <see cref="WebApplicationCredential"/>, application ID, and HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The ID of the client application.
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
        public HealthClientAuthorizedConnection(
            WebApplicationCredential webApplicationCredential,
            Guid applicationId,
            HealthServiceInstance serviceInstance)
            : base(webApplicationCredential, applicationId, serviceInstance, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of HealthClientAuthorizedConnection
        /// using a specified <see cref="WebApplicationCredential"/>, application ID, and health service URL.
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The ID of the client application.
        /// </param>
        ///
        /// <param name="healthServiceUri">
        /// The URL of the HealthVault platform service.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUri"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUri"/> parameter is not a properly
        /// formatted URL.
        /// </exception>
        ///
        public HealthClientAuthorizedConnection(
            WebApplicationCredential webApplicationCredential,
            Guid applicationId,
            Uri healthServiceUri)
            : base(webApplicationCredential, applicationId, healthServiceUri, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of HealthClientAuthorizedConnection
        /// with a WebApplicationCredential, an application ID,
        /// HealthVault web-service instance, and person ID.
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The ID of the client application.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        ///
        /// <param name="personId">
        /// The ID of the person for whom the authorized connection is created.
        /// </param>
        ///
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        ///
        public HealthClientAuthorizedConnection(
            WebApplicationCredential webApplicationCredential,
            Guid applicationId,
            HealthServiceInstance serviceInstance,
            Guid personId)
            : base(webApplicationCredential, applicationId, serviceInstance, personId)
        {
        }

        /// <summary>
        /// Creates a new instance of HealthClientAuthorizedConnection
        /// with a WebApplicationCredential, an applicationID,
        /// healthServiceUri and personId
        /// </summary>
        ///
        /// <param name="webApplicationCredential">
        /// Credential for authenticating the application
        /// </param>
        ///
        /// <param name="applicationId">
        /// The ID of the client application.
        /// </param>
        ///
        /// <param name="healthServiceUri">
        /// The URL of the HealthVault platform service.
        /// </param>
        ///
        /// <param name="personId">
        /// The ID of the person for whom the authorized connection is created.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUri"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The <paramref name="healthServiceUri"/> parameter is not a properly
        /// formatted URL.
        /// </exception>
        ///
        public HealthClientAuthorizedConnection(
            WebApplicationCredential webApplicationCredential,
            Guid applicationId,
            Uri healthServiceUri,
            Guid personId)
            : base(webApplicationCredential, applicationId, healthServiceUri, personId)
        {
        }

        #endregion
    }
}
