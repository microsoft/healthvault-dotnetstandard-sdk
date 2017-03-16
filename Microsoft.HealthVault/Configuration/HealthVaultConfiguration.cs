// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.ItemTypes;
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
        /// HealthVault web-service. ( ex: https://platform.healthvault.com/platform/ )
        /// </summary>
        /// <remarks>
        /// This may be overwritten if an environment instance bounce happens.
        /// </remarks>
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

        private volatile Uri defaultHealthVaultRootUrl = new Uri("https://platform.healthvault.com/platform/");

        /// <summary>
        /// Gets the HealthVault Shell URL for
        /// the configured default instance of the HealthVault web-service.
        /// ( ex: https://account.healthvault.com )
        /// </summary>
        /// <remarks> This may be overwritten if an environment instance bounce happens.</remarks>
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

        private volatile Uri shellUrl = new Uri("https://account.healthvault.com");

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
        /// calls <see cref="HealthRecordAccessor.GetItemAsync(System.Guid,ThingSections)"/> or makes any query to HealthVault
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
        /// were unexpected which led to changes to automatically put the <see cref="ThingQuery.TypeIds"/>
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
                throw new InvalidOperationException(Resources.CannotChangeConfigurationAfterInit);
            }
        }
    }
}
