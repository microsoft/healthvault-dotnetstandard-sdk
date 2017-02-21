// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
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
    }
}

