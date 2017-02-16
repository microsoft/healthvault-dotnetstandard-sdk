// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents a connection for an application to the HealthVault service
    /// for operations that require neither user authentication nor
    /// application identifier verification.
    /// </summary>
    ///
    /// <remarks>
    /// You must connect to the HealthVault service to access its
    /// web methods. This class does not maintain
    /// an open connection to the service, but uses XML over HTTP to
    /// make requests and receive responses from the service. The connection
    /// only maintains the data necessary for the request.
    /// <br/><br/>
    /// Use an anonymous connection to access HealthVault methods that
    /// require only a valid application identifier, such as
    /// <see cref="HealthServiceConnection.GetServiceDefinition()"/>.
    /// <br/><br/>
    /// For operations that require authentication, use the
    /// <see cref="AuthenticatedConnection"/> class and its derived classes.
    /// For operations that require more specific functionality, such as
    /// querying a vocabulary list, use the <see cref="ApplicationConnection"/>
    /// class and its derived classes.
    /// </remarks>
    ///
    public class AnonymousConnection : HealthServiceConnection
    {
        #region ctors

        /// <summary>
        /// Creates an instance of the <see cref="AnonymousConnection"/> class
        /// for the configured application ID and the configured default HealthVault
        /// web-service instance.
        /// </summary>
        ///
        /// <remarks>
        /// The default constructor takes values from the application or web
        /// configuration file.
        /// </remarks>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// If the web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        ///
        public AnonymousConnection()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="AnonymousConnection"/> class
        /// for the application for the specified HealthVault web-service instance.
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
        public AnonymousConnection(
            HealthServiceInstance serviceInstance)
            : base(serviceInstance)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="AnonymousConnection"/> class
        /// for the application having the specified globally unique
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
        public AnonymousConnection(
            Guid callingApplicationId,
            HealthServiceInstance serviceInstance)
            : base(
                callingApplicationId,
                serviceInstance)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="AnonymousConnection"/> class
        /// for the application having the specified globally unique
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
        public AnonymousConnection(
            Guid callingApplicationId,
            Uri healthServiceUrl)
            : base(
                callingApplicationId,
                healthServiceUrl)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="AnonymousConnection"/> class
        /// for the application having the specified globally unique
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
        public AnonymousConnection(
            Guid callingApplicationId,
            string healthServiceUrl)
            : this(
                callingApplicationId,
                new Uri(healthServiceUrl))
        {
        }

        #endregion ctors

        #region CreateRequest

        internal override HealthServiceRequest CreateRequest(
            string methodName,
            int methodVersion,
            bool forAuthentication)
        {
            Validator.ThrowIfStringNullOrEmpty(methodName, nameof(methodName));

            HealthServiceRequest request =
                new HealthServiceRequest(
                    this,
                    methodName,
                    methodVersion);

            return request;
        }

        #endregion CreateRequest
    }
}
