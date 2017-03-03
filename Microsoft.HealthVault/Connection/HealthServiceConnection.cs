// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Simplifies access to the HealthVault service. This class is
    /// abstract.
    /// </summary>
    ///
    /// <remarks>
    /// A connection must be made to the HealthVault service to access the
    /// web methods that the service exposes. The class does not maintain
    /// an open connection to the service. It uses XML over HTTP to
    /// make requests and receive responses from the service. The connection
    /// just maintains the data necessary to make the request.
    /// <br/><br/>
    /// You cannot directly instantiate this abstract class. Instead, instantiate
    /// an instance of <see cref="ApplicationConnection"/> or
    /// <see cref="AuthenticatedConnection"/> to communicate with the Microsoft
    /// HealthVault service.
    /// </remarks>
    ///
    /// <seealso cref="AuthenticatedConnection" />
    /// <seealso cref="ApplicationConnection" />
    ///
    public abstract class HealthServiceConnection : IConnection
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceConnection"/>
        /// class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The application ID and HealthVault URL are taken from the application
        /// or web configuration file.
        /// </remarks>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// The web or application configuration file does not contain
        /// configuration entries for "ApplicationID" or "HealthServiceUrl".
        /// </exception>
        ///
        internal HealthServiceConnection()
        {
            if (this.ApplicationId == Guid.Empty)
            {
                throw Validator.InvalidConfigurationException("InvalidApplicationIdConfiguration");
            }

            if (this.RequestUrl == null)
            {
                throw Validator.InvalidConfigurationException("InvalidRequestUrlConfiguration");
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceConnection"/>
        /// class with the specified application identifier and HealthVault
        /// web-service URL.
        /// </summary>
        ///
        /// <param name="callingApplicationId">
        /// The HealthVault application identifier.
        /// </param>
        ///
        /// <param name="healthServiceUrl">
        /// The URL of the HealthVault web-service.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> is <b>null</b>.
        /// </exception>
        ///
        internal HealthServiceConnection(Guid callingApplicationId, Uri healthServiceUrl)
        {
            Validator.ThrowIfArgumentNull(healthServiceUrl, "healthServiceUrl", "CtorServiceUrlNull");

            // If the HealthServiceUrl is set in a .config file,
            // HealthApplicationConfiguration.Current.HealthVaultMethodUrl
            // will automatically append "wildcat.ashx" to it.
            // Users of OfflineWebApplicationConnection need the same help, so we do it here if necessary...
            if (!healthServiceUrl.AbsoluteUri.ToUpperInvariant().EndsWith("WILDCAT.ASHX", StringComparison.Ordinal))
            {
                string newUri = healthServiceUrl.AbsoluteUri;
                if (!newUri.EndsWith("/", StringComparison.Ordinal))
                {
                    newUri = newUri + "/wildcat.ashx";
                }
                else
                {
                    newUri = newUri + "wildcat.ashx";
                }

                healthServiceUrl = new Uri(newUri);
            }

            this.ApplicationId = callingApplicationId;
            this.requestUrl = healthServiceUrl;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceConnection"/>
        /// class for the specified instance of HealthVault web-service.
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
        internal HealthServiceConnection(HealthServiceInstance serviceInstance)
        {
            if (serviceInstance != null)
            {
                if (serviceInstance.HealthServiceUrl == null)
                {
                    throw Validator.ArgumentException("serviceInstance", "ServiceInstanceMustHaveServiceUrl");
                }

                this.ServiceInstance = serviceInstance;
                this.requestUrl = serviceInstance.HealthServiceUrl;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceConnection"/>
        /// class for the specified instance of HealthVault web-service with the specified
        /// application identifier.
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
        /// <remarks>
        /// If <paramref name="serviceInstance"/> is <b>null</b>, the URL for the configured
        /// default HealthVault web-service instance is used.
        /// </remarks>
        ///
        internal HealthServiceConnection(Guid callingApplicationId, HealthServiceInstance serviceInstance = null)
            : this(serviceInstance)
        {
            this.ApplicationId = callingApplicationId;
        }

        #endregion constructors

        #region public methods

        /// <summary>
        /// Provides a wrapper around the XML request for the web service.
        /// </summary>
        ///
        /// <param name="methodName">
        /// The name of the method of the web-service to call.
        /// </param>
        ///
        /// <param name="methodVersion">
        /// The version of the method to call.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthServiceRequest"/> that acts as a
        /// wrapper to XML request for the HealthVault web-service.
        /// </returns>
        ///
        /// <remarks>
        /// This method skips the object model provided by the other
        /// methods of this class and acts as a simple wrapper around the XML
        /// request for the web service. The caller must provide the parameters
        /// in the correct format for the called method and in order to parse
        /// the response data.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="methodName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public virtual HealthServiceRequest CreateRequest(string methodName, int methodVersion)
        {
            return this.CreateRequest(methodName, methodVersion, false);
        }

        /// <summary>
        /// Cancels any pending request to HealthVault that was initiated with this connection
        /// instance and prevents any new requests from being made.
        /// </summary>
        ///
        /// <remarks>
        /// Setting this property to true will cancel any requests that was started using this
        /// connection and will prevent new requests from being made.
        /// It is up to the caller to start the request on another thread. Cancelling will cause
        /// an OperationCancelledException to be thrown on the thread the request was
        /// executed on.
        /// <br/><br/>
        /// If you want to start reusing the connection set the property to false.
        /// </remarks>
        ///
        public bool CancelAllRequests
        {
            get { return this.cancelAllRequests; }

            set
            {
                if (value)
                {
                    lock (this.PendingRequests)
                    {
                        for (int index = this.PendingRequests.Count - 1; index >= 0; --index)
                        {
                            this.PendingRequests[index].CancelRequest();
                            this.PendingRequests.RemoveAt(index);
                        }
                    }
                }

                this.cancelAllRequests = value;
            }
        }

        private bool cancelAllRequests;

        internal List<HealthServiceRequest> PendingRequests { get; } = new List<HealthServiceRequest>();

        #region GetServiceDefinitionAsync

        /// <summary>
        /// Gets information about the HealthVault service.
        /// </summary>
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
        public async Task<ServiceInfo> GetServiceDefinitionAsync()
        {
            return await HealthVaultPlatform.GetServiceDefinitionAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service only if it has been updated since
        /// the specified update time.
        /// </summary>
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
        public async Task<ServiceInfo> GetServiceDefinitionAsync(DateTime lastUpdatedTime)
        {
            return await HealthVaultPlatform.GetServiceDefinitionAsync(this, lastUpdatedTime).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories.
        /// </summary>
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
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public async Task<ServiceInfo> GetServiceDefinitionAsync(ServiceInfoSections responseSections)
        {
            return await HealthVaultPlatform.GetServiceDefinitionAsync(this, responseSections).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories if the requested information has been updated since the specified
        /// update time.
        /// </summary>
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
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        public async Task<ServiceInfo> GetServiceDefinitionAsync(
            ServiceInfoSections responseSections,
            DateTime lastUpdatedTime)
        {
            return await HealthVaultPlatform.GetServiceDefinitionAsync(this, responseSections, lastUpdatedTime).ConfigureAwait(false);
        }

        #endregion GetServiceDefinitionAsync

        #endregion public methods

        #region public properties

        /// <summary>
        /// Gets or sets the default proxy to use for all instances of
        /// <see cref="HealthServiceConnection"/>.
        /// To disable proxy usage, set this property to <b>null</b>.
        /// </summary>
        ///
        /// <value>
        /// An instance of IWebProxy.
        /// </value>
        ///
        /// <remarks>
        /// The initial value is system default,
        /// which is the value returned by System.Net.WebRequest.DefaultWebProxy.
        /// </remarks>
        ///
        internal static IWebProxy DefaultWebProxy { get; set; }

        /// <summary>
        /// Gets or sets the proxy to use with this instance of
        /// <see cref="HealthServiceConnection"/>.
        /// </summary>
        ///
        /// <remarks>
        /// The default setting is to use.
        /// To disable proxy usage, set this property to <b>null</b>.
        /// </remarks>
        ///
        /// <value>
        /// An instance of <see cref="IWebProxy"/>.
        /// </value>
        ///
        public IWebProxy WebProxy { get; set; } = DefaultWebProxy;

        /// <summary>
        /// Gets the calling application's ID.
        /// </summary>
        ///
        public Guid ApplicationId { get; } = HealthApplicationConfiguration.Current.ApplicationId;

        /// <summary>
        /// Gets the HealthVault web-service URL.
        /// </summary>
        ///
        /// <value>
        /// An instance of Uri representing the HealthVault web-service URL.
        /// </value>
        ///
        public Uri RequestUrl
        {
            get
            {
                if (this.requestUrl == null)
                {
                    return HealthApplicationConfiguration.Current.GetHealthVaultMethodUrl();
                }

                return this.requestUrl;
            }
        }

        private readonly Uri requestUrl;

        /// <summary>
        /// Gets the HealthVault web-service instance associated with this connection.
        /// </summary>
        public HealthServiceInstance ServiceInstance
        {
            get;
            private set;
        }

        public IHealthApplicationConfiguration ApplicationConfiguration { get; set; }

        public ISessionCredential SessionCredential { get; set; }

        public T GetClient<T>() 
            where T : IClient
        {
            throw new NotImplementedException();
        }

        public IPlatformClient PlatformClient { get; }

        public IPersonClient PersonClient { get; }

        public IVocabularyClient VocabularyClient { get; }

        public IThingClient GetThingClient(HealthRecordInfo record)
        {
            throw new NotImplementedException();
        }

        public IActionPlanClient GetActionPlanClient(HealthRecordInfo record)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the HealthVault web-service URL to use when retrieving and setting the other data
        /// in the HealthRecordItem using the http binary channel API.
        /// </summary>
        ///
        /// <value>
        /// An instance of Uri representing the HealthVault web-service URL.
        /// </value>
        ///
        internal Uri OtherDataStreamUrl { get; } = HealthApplicationConfiguration.Current.GetBlobStreamUrl();

        /// <summary>
        /// Gets or sets the request timeout in seconds.
        /// </summary>
        ///
        public int RequestTimeoutSeconds { get; set; } = HealthApplicationConfiguration.Current.DefaultRequestTimeout;

        /// <summary>
        /// Gets or sets the request time-to-live in seconds.
        /// </summary>
        ///
        public int RequestTimeToLive { get; set; } = HealthApplicationConfiguration.Current.DefaultRequestTimeToLive;

        /// <summary>
        /// Gets or sets the language to be sent to the server when making
        /// requests.
        /// </summary>
        ///
        /// <value>
        /// A CultureInfo representing the language.
        /// </value>
        ///
        public CultureInfo Culture
        {
            get
            {
                return this.culture ?? CultureInfo.CurrentUICulture;
            }

            set
            {
                this.culture = value;
            }
        }

        private CultureInfo culture;

        /// <summary>
        /// Gets or sets the request compression method for this connection.
        /// </summary>
        ///
        /// <value>
        /// A string representing the request compression method.
        /// </value>
        ///
        public string RequestCompressionMethod
        {
            get
            {
                return this.requestCompressionMethod;
            }

            set
            {
                this.requestCompressionMethod = value;

                if (string.IsNullOrEmpty(this.requestCompressionMethod))
                {
                    this.requestCompressionMethod = string.Empty;
                }
                else
                {
                    if (!string.Equals(
                            this.requestCompressionMethod, "gzip", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(
                            this.requestCompressionMethod, "deflate", StringComparison.OrdinalIgnoreCase))
                    {
                        throw Validator.HealthServiceException("InvalidRequestCompressionMethod");
                    }
                }
            }
        }

        private string requestCompressionMethod = "gzip";

        #endregion public properties

        #region internal helpers

        /// <summary>
        /// Represents a simple wrapper around the XML request for the web
        /// service. This method is abstract.
        /// </summary>
        ///
        /// <param name="methodName">
        /// The name of the method to call.
        /// </param>
        ///
        /// <param name="methodVersion">
        /// The version of the method to call.
        /// </param>
        ///
        /// <param name="forAuthentication">
        /// <b>true</b> if the request generates an authentication token;
        /// <b>false</b> if the request calls one of the other web methods.
        /// </param>
        ///
        /// <remarks>
        /// Override this method in a derived class to provide specific behavior
        /// such as authentication.
        /// </remarks>
        ///
        internal abstract HealthServiceRequest CreateRequest(
            string methodName,
            int methodVersion,
            bool forAuthentication);

        /// <summary>
        /// Gets header content specific to this configuration.
        /// </summary>
        /// <param name="writer">The XML writer to use in the configuration check</param>
        internal void GetSessionAuthorizationHeader(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion internal helpers

        /// <summary>
        /// Gets the authorization token to be used in all requests for this user.
        /// </summary>
        ///
        /// <value>
        /// A base64 encoded string that represents the person ID, application
        /// ID, expiration, and group membership of the person and application
        /// requesting access to HealthVault.
        /// </value>
        ///
        /// <remarks>
        /// The authorization token can be retrieved using the
        /// <see cref="Authentication.Credential.CreateAuthenticatedSessionTokenAsync"/> web method from
        /// HealthVault or by calling the HealthVault Shell authentication web page.
        /// <br/><br/>
        /// </remarks>
        ///
        public string AuthenticationToken
        {
            get
            {
                if (this.credentialValue == null)
                {
                    return null;
                }

                CreateAuthenticationTokenResult result =
                    this.Credential.GetAuthenticationResult(this.ApplicationId);

                if (result == null)
                {
                    throw Validator.HealthServiceException("ConnectionNotAuthenticated");
                }

                return result.AuthenticationToken;
            }
        }

        /// <summary>
        /// Gets or sets the application credential that is used to access
        /// HealthVault.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Credential Credential
        {
            get { return this.credentialValue; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Credential", "CredentialMustBeSpecified");
                this.credentialValue = value;
            }
        }

        private Credential credentialValue;

        HealthServiceInstance IConnection.ServiceInstance { get; }
    }
}
