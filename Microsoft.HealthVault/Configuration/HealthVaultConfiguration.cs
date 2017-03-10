using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Configuration
{
    /// <summary>
    /// Gives access to the configuration file for the application and
    /// exposes some of the settings directly.
    /// </summary>
    public abstract class HealthVaultConfiguration
    {
        /// <summary>
        /// The default number of internal retries.
        /// </summary>
        protected const int DefaultRetryOnInternal500Count = 2;

        /// <summary>
        /// Default sleep duration in seconds.
        /// </summary>
        protected const int DefaultRetryOnInternal500SleepSeconds = 1;

        /// <summary>
        /// Base class for the app, web and soda configurations
        /// </summary>
        /// <summary>
        /// The default request time to live value.
        /// </summary>
        protected const int DefaultDefaultRequestTimeToLive = 30 * 60;

        /// <summary>
        /// The default request time out value.
        /// </summary>
        protected const int DefaultDefaultRequestTimeout = 30;

        /// <summary>
        /// Gets or sets a value indicating whether the configuration is locked.
        /// </summary>
        protected bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the root URL for the default instance of the
        /// HealthVault web-service. This may be overwritten if an environment
        /// instance bounce happens.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HealthServiceUrl" configuration
        /// value with "wildcat.ashx" removed.
        /// </remarks>
        ///
        public virtual Uri DefaultHealthVaultUrl
        {
            get
            {
                return this.defaultHealthVaultRootUrl;
            }

            set
            {
                this.EnsureNotLocked();
                this.defaultHealthVaultRootUrl = EnsureTrailingSlash(value);
            }
        }

        private volatile Uri defaultHealthVaultRootUrl;

        /// <summary>
        /// Gets the HealthVault Shell URL for
        /// the configured default instance of the HealthVault web-service. This
        /// may be overwritten if an environment instance bounce happens.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "ShellUrl" configuration
        /// value.
        /// </remarks>
        ///
        public virtual Uri DefaultHealthVaultShellUrl
        {
            get
            {
                return this.shellUrl;
            }

            set
            {
                this.EnsureNotLocked();
                this.shellUrl = EnsureTrailingSlash(value);
            }
        }

        private volatile Uri shellUrl;

        /// <summary>
        /// Gets the application's unique identifier.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "ApplicationId" configuration
        /// value.
        /// </remarks>
        ///
        public virtual Guid MasterApplicationId
        {
            get
            {
                return this.masterAppId;
            }

            set
            {
                this.EnsureNotLocked();
                this.masterAppId = value;
            }
        }

        private Guid masterAppId;

        /// <summary>
        /// Gets or sets the crypto configuration.
        /// </summary>
        /// <value>
        /// The crypto configuration.
        /// </value>
        /// <remarks>
        /// This property needs to be set as part of app initialization
        /// </remarks>
        public virtual ICryptoConfiguration CryptoConfiguration
        {
            get
            {
                return this.cryptoConfiguration ?? (this.cryptoConfiguration = new BaseCryptoConfiguration());
            }

            set
            {
                this.EnsureNotLocked();
                this.cryptoConfiguration = value;
            }
        }

        public string RequestCompressionMethod { get; set; }

        private ICryptoConfiguration cryptoConfiguration;

        /// <summary>
        /// Gets or sets the application certificate password.
        /// </summary>
        public virtual string ApplicationCertificatePassword
        {
            get { return this.applicationCertificatePassword; }

            set
            {
                this.EnsureNotLocked();
                this.applicationCertificatePassword = value;
            }
        }

        private string applicationCertificatePassword;

        /// <summary>
        /// Gets or sets the application certificate file name.
        /// </summary>
        public virtual string ApplicationCertificateFileName
        {
            get { return this.applicationCertificateFileName; }

            set
            {
                this.EnsureNotLocked();
                this.applicationCertificateFileName = value;
            }
        }

        private string applicationCertificateFileName;

        /// <summary>
        /// Gets or sets the certificate subject.
        /// </summary>
        public virtual string CertSubject
        {
            get { return this.certSubject; }

            set
            {
                this.EnsureNotLocked();
                this.certSubject = value;
            }
        }

        private string certSubject;

        #region web request/response configuration

        /// <summary>
        /// Gets the request timeout in seconds.
        /// </summary>
        /// 
        /// <remarks>
        /// This value is used to set the <see cref="Timeout"/> property 
        /// when making the request to HealthVault. The timeout is the number of seconds that a 
        /// request will wait for a response from HealtVault. If the method response is not
        /// returned within the time-out period the request will throw a <see cref="HealthHttpException"/>
        /// with the <see cref="HealthHttpException.StatusCode">Status</see> property set to
        /// <see cref="Timeout"/>.
        /// This property corresponds to the "defaultRequestTimeout" configuration
        /// value. The value defaults to 30 seconds.
        /// </remarks>
        ///
        public virtual int DefaultRequestTimeout
        {
            get
            {
                if (!this.configurationRequestTimeoutInitialized)
                {
                    this.configuredRequestTimeout = DefaultDefaultRequestTimeout;
                    this.configurationRequestTimeoutInitialized = true;
                }

                return this.configuredRequestTimeout;
            }

            set
            {
                this.EnsureNotLocked();

                int tempRequestTimeout = value;

                // Note, -1 signifies an infinite timeout so that is OK.
                if (tempRequestTimeout < -1)
                {
                    tempRequestTimeout = DefaultDefaultRequestTimeout;
                }

                this.configuredRequestTimeout = tempRequestTimeout;
                this.configurationRequestTimeoutInitialized = true;
            }
        }

        private volatile int configuredRequestTimeout;
        private volatile bool configurationRequestTimeoutInitialized;

        /// <summary>
        /// Gets the request time to live in seconds.
        /// </summary>
        ///
        /// <remarks>
        /// This property defines the "msg-ttl" in the HealthVault request header XML. It determines
        /// how long the same XML can be used before HealthVault determines the request invalid.
        /// This property corresponds to the "defaultRequestTimeToLive" configuration
        /// value. The value defaults to 1800 seconds.
        /// </remarks>
        ///
        public virtual int DefaultRequestTimeToLive
        {
            get
            {
                if (!this.configuredRequestTimeToLiveInitialized)
                {
                    this.configuredRequestTimeToLive = DefaultDefaultRequestTimeToLive;
                    this.configuredRequestTimeToLiveInitialized = true;
                }

                return this.configuredRequestTimeToLive;
            }

            set
            {
                this.EnsureNotLocked();

                int tempRequestTimeToLive = value;

                if (tempRequestTimeToLive < -1)
                {
                    tempRequestTimeToLive = DefaultDefaultRequestTimeToLive;
                }

                this.configuredRequestTimeToLive = tempRequestTimeToLive;
                this.configuredRequestTimeToLiveInitialized = true;
            }
        }

        private volatile int configuredRequestTimeToLive;
        private volatile bool configuredRequestTimeToLiveInitialized;

        /// <summary>
        /// Gets the number of retries the .NET APIs will make when getting an internal
        /// error response (error 500) from HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "RequestRetryOnInternal500" configuration
        /// value. The value defaults to 2.
        /// </remarks>
        ///
        public virtual int RetryOnInternal500Count
        {
            get
            {
                if (!this.retryOnInternal500CountInitialized)
                {
                    this.retryOnInternal500Count = DefaultRetryOnInternal500Count;
                    this.retryOnInternal500CountInitialized = true;
                }

                return this.retryOnInternal500Count;
            }

            set
            {
                this.EnsureNotLocked();

                this.retryOnInternal500Count = value;
                this.retryOnInternal500CountInitialized = true;
            }
        }

        private volatile int retryOnInternal500Count;
        private volatile bool retryOnInternal500CountInitialized;

        /// <summary>
        /// Gets the sleep duration in seconds between retries due to HealthVault returning
        /// an internal error (error 500).
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "RequestRetryOnInternal500SleepSeconds" configuration
        /// value. The value defaults to 1 second.
        /// </remarks>
        ///
        public virtual int RetryOnInternal500SleepSeconds
        {
            get
            {
                if (!this.retryOnInternal500SleepSecondsInitialized)
                {
                    this.retryOnInternal500SleepSeconds = DefaultRetryOnInternal500SleepSeconds;
                    this.retryOnInternal500SleepSecondsInitialized = true;
                }

                return this.retryOnInternal500SleepSeconds;
            }

            set
            {
                this.EnsureNotLocked();

                this.retryOnInternal500SleepSeconds = value;
                this.retryOnInternal500SleepSecondsInitialized = true;
            }
        }

        private volatile int retryOnInternal500SleepSeconds;
        private volatile bool retryOnInternal500SleepSecondsInitialized;

        #endregion web request/response configuration

        /// <summary>
        /// Gets the size in bytes of the block used to hash inlined BLOB data.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "defaultInlineBlobHashBlockSize" configuration
        /// value. The value defaults to 2MB.
        /// </remarks>
        ///
        public virtual int InlineBlobHashBlockSize
        {
            get
            {
                if (!this.configuredInlineBlobHashBlockSizeInitilialized)
                {
                    this.configuredInlineBlobHashBlockSize = BlobHasher.DefaultInlineBlobHashBlockSizeBytes;
                    this.configuredInlineBlobHashBlockSizeInitilialized = true;
                }

                return this.configuredInlineBlobHashBlockSize;
            }

            set
            {
                this.EnsureNotLocked();

                int tempBlobHashSize = value;

                if (tempBlobHashSize < 1)
                {
                    tempBlobHashSize = BlobHasher.DefaultInlineBlobHashBlockSizeBytes;
                }

                this.configuredInlineBlobHashBlockSize = tempBlobHashSize;
                this.configuredInlineBlobHashBlockSizeInitilialized = true;
            }
        }

        private volatile int configuredInlineBlobHashBlockSize;
        private volatile bool configuredInlineBlobHashBlockSizeInitilialized;

        /// <summary>
        /// Gets the type version identifiers of types supported by this application.
        /// </summary>
        ///
        /// <remarks>
        /// Although most applications don't need this configuration setting, if an application
        /// calls <see cref="HealthRecordAccessor.GetItemAsync(System.Guid,Microsoft.HealthVault.Thing.HealthRecordItemSections)"/> or makes any query to HealthVault
        /// that doesn't specify the type identifier in the filter, this configuration setting
        /// will tell HealthVault the format of the type to reply with. For example, if a web
        /// application has two servers and makes a call to GetItemAsync for EncounterV1 and the
        /// application authorization is set to the EncounterV1 format then the application will
        /// get EncounterV1 instances back even if the record contains Encounter v2 instances. Now
        /// the application wants to upgrade to Encounter v2 without having application down-time.
        /// In order to do this, one of the application servers must be updated to Encounter v2 while
        /// the other still works with EncounterV1. If we were to rely solely on application
        /// authorization one of the servers would be broken during the upgrade. However, by using
        /// this configuration value to specify what type version the server supports (rather than
        /// the application), then both servers can continue to work while the application is
        /// upgraded.
        /// </remarks>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// If the configuration contains the name of a type that is not registered as a type handler
        /// in <see cref="ItemTypeManager"/>.
        /// </exception>
        ///
        public virtual IList<Guid> SupportedTypeVersions
        {
            get { return this.supportedTypeVersions ?? (this.supportedTypeVersions = new List<Guid>()); }

            set
            {
                this.EnsureNotLocked();
                this.supportedTypeVersions = value;
            }
        }

        private volatile IList<Guid> supportedTypeVersions;

        /// <summary>
        /// Gets a value indicating whether or not legacy type versioning support should be used.
        /// </summary>
        ///
        /// <remarks>
        /// Type versions support was initially determined by an applications base authorizations
        /// and/or the <see cref="HealthRecordView.TypeVersionFormat"/>. Some of these behaviors
        /// were unexpected which led to changes to automatically put the <see cref="HealthRecordFilter.TypeIds"/>
        /// and <see cref="HealthVaultConfiguration.SupportedTypeVersions"/> into the
        /// <see cref="HealthRecordView.TypeVersionFormat"/> automatically for developers. This
        /// exhibits the expected behavior for most applications. However, in some rare cases
        /// applications may need to revert back to the original behavior. When this property
        /// returns true the original behavior will be observed. If false, the new behavior will
        /// be observed. This property defaults to false and can be changed in the web.config file
        /// "UseLegacyTypeVersionSupport" setting.
        /// </remarks>
        ///
        public virtual bool UseLegacyTypeVersionSupport
        {
            get
            {
                return this.useLegacyTypeVersionSupport;
            }

            set
            {
                this.EnsureNotLocked();
                this.useLegacyTypeVersionSupport = value;
            }
        }

        private volatile bool useLegacyTypeVersionSupport;

        /// <summary>
        /// Gets the value which indicates whether the application is able to handle connecting to multiple
        /// instances of the HealthVault web-service.
        /// </summary>
        ///
        /// <remarks>
        /// This setting defaults to <b>true</b> and can be set in an application
        /// configuration file, using the "MultiInstanceAware" setting key.
        /// <p>
        /// Applications in HealthVault can be configured to support more than one instance of the HealthVault web-service.
        /// In such a case, and when the MultiInstanceAware configuration is set to <b>true</b>, all redirects generated
        /// through the HealthVault .NET API will have a flag set indicating that the application is able to deal with
        /// HealthVault accounts that may reside in other HealthVault instances.  In such a case, HealthVault Shell can
        /// redirect back with an account associated with any one of the instances of the HealthVault web-service which
        /// the application has chosen to support.  The application may then need to be able to handle connecting to the
        /// appropriate instance of the HealthVault web-service for each account.
        /// </p>
        /// <p>
        /// For more information see the <a href="http://go.microsoft.com/?linkid=9830913">Global HealthVault Architecture</a> article.
        /// </p>
        /// </remarks>
        public virtual bool MultiInstanceAware
        {
            get
            {
                return this.multiInstanceAware;
            }

            set
            {
                this.EnsureNotLocked();
                this.multiInstanceAware = value;
            }
        }

        private volatile bool multiInstanceAware = true;

        /// <summary>
        /// Gets the amount of time, in milliseconds, that the application's connection can
        /// remain idle before the HealthVault framework closes the connection.
        /// </summary>
        ///
        /// <remarks>
        /// This default value is 110 seconds of inactivity.
        /// <p>
        /// This setting only applies when using HTTP Persistent Connections
        /// <see cref="HealthVaultConfiguration.ConnectionUseHttpKeepAlive"/>.
        /// </p>
        /// <p>
        /// Setting this property to -1 indicates the connection should never
        /// time out.
        /// </p>
        /// <p>
        /// This property corresponds to the "ConnectionMaxIdleTime" configuration value.
        /// </p>
        /// </remarks>
        public virtual int ConnectionMaxIdleTime
        {
            get
            {
                if (!this.connectionMaxIdleTimeInitialized)
                {
                    this.connectionMaxIdleTime = 110 * 1000;
                    this.connectionMaxIdleTimeInitialized = true;
                }

                return this.connectionMaxIdleTime;
            }

            set
            {
                this.EnsureNotLocked();
                this.connectionMaxIdleTime = value;

                if (this.connectionMaxIdleTime < -1)
                {
                    this.connectionMaxIdleTime = -1;
                }

                this.connectionMaxIdleTimeInitialized = true;
            }
        }

        private volatile int connectionMaxIdleTime;
        private volatile bool connectionMaxIdleTimeInitialized;

        /// <summary>
        /// Gets the amount of time, in milliseconds, that the application's connection can
        /// remain open before the HealthVault framework closes the connection.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is 5 minutes.
        /// <p>
        /// This setting only applies when using HTTP Persistent Connections
        /// <see cref="HealthVaultConfiguration.ConnectionUseHttpKeepAlive"/>.
        /// </p>
        /// <p>
        /// Using this property ensures that active connections do not remain open
        /// indefinitely, even if actively used. This property is intended
        /// when connections should be dropped and reestablished periodically, such
        /// as load balancing scenarios.
        /// </p>
        /// <p>
        /// Setting the property to -1 indicates connections should stay open idefinitely.
        /// </p>
        /// <p>
        /// This property corresponds to the "ConnectionLeaseTimeout" configuration value.
        /// </p>
        /// </remarks>
        public virtual int ConnectionLeaseTimeout
        {
            get
            {
                if (!this.connectionLeaseTimeoutInitialized)
                {
                    this.connectionLeaseTimeout = 5 * 60 * 1000;
                    this.connectionLeaseTimeoutInitialized = true;
                }

                return this.connectionLeaseTimeout;
            }

            set
            {
                this.EnsureNotLocked();
                this.connectionLeaseTimeout = value;

                if (this.connectionLeaseTimeout < -1)
                {
                    this.connectionLeaseTimeout = -1;
                }

                this.connectionLeaseTimeoutInitialized = true;
            }
        }

        private volatile int connectionLeaseTimeout;
        private volatile bool connectionLeaseTimeoutInitialized;

        /// <summary>
        /// Gets a value that indicates whether the application uses Http 1.1 persistent
        /// connections to HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// True to use persistent connections; otherwise false. The default is true.
        /// <p>
        /// This property corresponds to the "ConnectionUseHttpKeepAlive" configuration value.
        /// </p>
        /// </remarks>
        public virtual bool ConnectionUseHttpKeepAlive
        {
            get
            {
                return this.connectionUseHttpKeepAlive;
            }

            set
            {
                this.EnsureNotLocked();
                this.connectionUseHttpKeepAlive = value;
            }
        }

        private volatile bool connectionUseHttpKeepAlive = true;

        /// <summary>
        /// Gets the value which specifies the period of time before the <see cref="P:ServiceInfo.Current"/> built-in cache is considered expired.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// Default value is <b>24 hours</b>.  This property corresponds to the "ServiceInfoDefaultCacheTtlMilliseconds" configuration value.
        /// </p>
        /// <p>
        /// The next request for the object after the cache is expired will result in a call to the HealthVault web-service
        /// to obtain an up-to-date copy of the service information.
        /// </p>
        /// <p>
        /// An application can override the entire caching and service info retrieval behavior
        /// by passing its own implementation of <see cref="IServiceInfoProvider"/> to
        /// <see cref="ServiceInfo.SetSingletonProvider(IServiceInfoProvider)"/>.  In such
        /// a case this configuration is no longer applicable.
        /// </p>
        /// </remarks>
        public TimeSpan ServiceInfoDefaultCacheTtl
        {
            get
            {
                // _serviceInfoDefaultCacheTtl cannot have volatile semantics because
                // it's a struct type (it can't be guaranteed to be assigned atomically).
                // So we will use a full lock here instead.
                // (an alternative is to use Thread.MemoryBarrier after the value write
                // but before the init flag write, and another barrier
                // for the read.

                lock (this.serviceInfoDefaultCacheTtlInitLock)
                {
                    if (!this.serviceInfoDefaultCacheTtlInitialized)
                    {
                        this.serviceInfoDefaultCacheTtl = TimeSpan.FromDays(1);
                        this.serviceInfoDefaultCacheTtlInitialized = true;
                    }

                    return this.serviceInfoDefaultCacheTtl;
                }
            }

            set
            {
                this.EnsureNotLocked();

                lock (this.serviceInfoDefaultCacheTtlInitLock)
                {
                    this.serviceInfoDefaultCacheTtl = value;
                    this.serviceInfoDefaultCacheTtlInitialized = true;
                }
            }
        }

        private TimeSpan serviceInfoDefaultCacheTtl;
        private bool serviceInfoDefaultCacheTtlInitialized;
        private readonly object serviceInfoDefaultCacheTtlInitLock = new object();

        /// <summary>
        /// Gets the root URL for a default instance of the
        /// Rest HealthVault service
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "RestHealthServiceUrl" configuration.
        /// </remarks>
        ///
        public virtual Uri RestHealthVaultUrl
        {
            get
            {
                return this.restHealthVaultRootUrl;
            }

            set
            {
                this.EnsureNotLocked();
                this.restHealthVaultRootUrl = EnsureTrailingSlash(value);
            }
        }

        private volatile Uri restHealthVaultRootUrl;

        public virtual int? RequestTimeoutSeconds
        {
            get
            {
                return this.requestTimeoutSeconds;
            }

            set
            {
                this.EnsureNotLocked();
                this.requestTimeoutSeconds = value;
            }
        }

        private int? requestTimeoutSeconds;

        public virtual bool IsMultiRecordApp
        {
            get
            {
                return this.isMultiRecordApp;
            }

            set
            {
                this.EnsureNotLocked();
                this.isMultiRecordApp = value;
            }
        }

        private bool isMultiRecordApp;

        private static Uri EnsureTrailingSlash(Uri uri)
        {
            string uriString = uri.AbsoluteUri;
            return new Uri(uriString.EndsWith("/", StringComparison.Ordinal) ? uriString : uriString + "/");
        }

        internal void Lock()
        {
            this.IsLocked = true;
        }

        /// <summary>
        /// Users are only allowed to change these values before app initialization.
        /// </summary>
        protected void EnsureNotLocked()
        {
            if (this.IsLocked)
            {
                throw new InvalidOperationException("Changing app configuration values after initialization is not permitted.");
            }
        }
    }
}
