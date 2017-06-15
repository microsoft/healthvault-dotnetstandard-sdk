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
    public class HealthVaultConfiguration
    {
        private Uri _defaultHealthVaultShellUrl;
        private Uri _defaultHealthVaultUrl;
        private Uri _restHealthVaultRootUrl;

        /// <summary>
        /// Create instance of HealthVaultConfiguration with default values as defined at <see cref="HealthVaultConfigurationDefaults"></see>
        /// </summary>
        public HealthVaultConfiguration()
        {
            DefaultHealthVaultUrl = HealthVaultConfigurationDefaults.HealthVaultRootUrl;
            DefaultHealthVaultShellUrl = HealthVaultConfigurationDefaults.ShellUrl;
            RequestTimeoutDuration = HealthVaultConfigurationDefaults.RequestTimeoutDuration;
            RequestTimeToLiveDuration = HealthVaultConfigurationDefaults.RequestTimeToLiveDuration;
            RetryOnInternal500Count = HealthVaultConfigurationDefaults.RetryOnInternal500Count;
            RetryOnInternal500SleepDuration = HealthVaultConfigurationDefaults.RetryOnInternal500SleepDuration;
            InlineBlobHashBlockSize = BlobHasher.DefaultInlineBlobHashBlockSizeBytes;
        }

        /// <summary>
        /// Gets or sets the root URL for the default instance of the
        /// HealthVault web-service. ( ex: https://platform.healthvault.com/platform/ )
        /// </summary>
        /// <remarks>
        /// This may be overwritten if an environment instance bounce happens.
        /// This property corresponds to the "HV_HealthServiceUrl" configuration value when reading from web.config.
        /// </remarks>
        public Uri DefaultHealthVaultUrl
        {
            get { return _defaultHealthVaultUrl; }
            set { _defaultHealthVaultUrl = EnsureTrailingSlash(value); }
        }

        /// <summary>
        /// Gets the HealthVault Shell URL for
        /// the configured default instance of the HealthVault web-service.
        /// ( ex: https://account.healthvault.com )
        /// </summary>
        /// <remarks>
        /// This may be overwritten if an environment instance bounce happens.
        /// This property corresponds to the "HV_ShellUrl" configuration value when reading from web.config.
        /// </remarks>
        public Uri DefaultHealthVaultShellUrl
        {
            get { return _defaultHealthVaultShellUrl; }
            set { _defaultHealthVaultShellUrl = EnsureTrailingSlash(value); }
        }

        /// <summary>
        /// Gets the application's unique identifier.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_ApplicationId" configuration value when reading from web.config.
        /// </remarks>
        public Guid MasterApplicationId { get; set; }

        /// <summary>
        /// Gets the request timeout
        /// </summary>
        /// <remarks>
        /// This value is used to set the <see cref="Timeout"/> property
        /// when making the request to HealthVault. The timeout is the number of seconds that a
        /// request will wait for a response from HealtVault. If the method response is not
        /// returned within the time-out period the request will throw a <see cref="HealthHttpException"/>
        /// with the <see cref="HealthHttpException.StatusCode">Status</see> property set to
        /// <see cref="Timeout"/>.
        /// This property corresponds to the "HV_DefaultRequestTimeoutSeconds" configuration value when reading from web.config.
        /// The value defaults to 30 seconds.
        /// </remarks>
        public TimeSpan RequestTimeoutDuration { get; set; }

        /// <summary>
        /// Gets the request time to live.
        /// </summary>
        /// <remarks>
        /// This property defines the "msg-ttl" in the HealthVault request header XML. It determines
        /// how long the same XML can be used before HealthVault determines the request invalid.
        /// This property corresponds to the "HV_DefaultRequestTimeToLiveSeconds" configuration value when reading from web.config.
        /// The value defaults to 1800 seconds.
        /// </remarks>
        public TimeSpan RequestTimeToLiveDuration { get; set; }

        /// <summary>
        /// Gets the number of retries the .NET APIs will make when getting an internal
        /// error response (error 500) from HealthVault.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_RequestRetryOnInternal500Count" configuration value when reading from web.config.
        /// The value defaults to 2.
        /// </remarks>
        public int RetryOnInternal500Count { get; set; }

        /// <summary>
        /// Gets the sleep duration between retries due to HealthVault returning
        /// an internal error (error 500).
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_RequestRetryOnInternal500SleepSeconds" configuration value when reading from web.config.
        /// The value defaults to 1 second.
        /// </remarks>
        public TimeSpan RetryOnInternal500SleepDuration { get; set; }

        /// <summary>
        /// Gets the size in bytes of the block used to hash inlined BLOB data.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_DefaultInlineBlobHashBlockSize" configuration value when reading from web.config.
        /// The value defaults to 2MB.
        /// </remarks>
        public int InlineBlobHashBlockSize { get; set; }

        /// <summary>
        /// Gets the type version identifiers of types supported by this application.
        /// </summary>
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
        /// This property corresponds to the "HV_SupportedTypeVersions" configuration value when reading from web.config.
        /// </remarks>
        /// <exception cref="InvalidConfigurationException">
        /// If the configuration contains the name of a type that is not registered as a type handler
        /// in <see cref="ItemTypeManager"/>.
        /// </exception>
        public IList<Guid> SupportedTypeVersions { get; set; } = new List<Guid>();

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
        /// be observed.
        /// This property corresponds to the "HV_SupportedTypeVersions" configuration value when reading from web.config.
        /// This property defaults to false
        /// </remarks>
        public bool UseLegacyTypeVersionSupport { get; set; }

        /// <summary>
        /// Gets the value which indicates whether the application is able to handle connecting to multiple
        /// instances of the HealthVault web-service.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_MultiInstanceAware" configuration value when reading from web.config.
        /// This property defaults to true
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
        public bool MultiInstanceAware { get; set; } = true;

        /// <summary>
        /// Gets the root URL for a default instance of the
        /// Rest HealthVault service
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_RestHealthServiceUrl" configuration value when reading from web.config.
        /// </remarks>
        public Uri RestHealthVaultUrl
        {
            get { return _restHealthVaultRootUrl; }
            set { _restHealthVaultRootUrl = EnsureTrailingSlash(value); }
        }

        /// <summary>
        /// Gets the value which indicates whether the application marked as multi record
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_IsMRA" configuration value when reading from web.config.
        /// </remarks>
        public bool IsMultiRecordApp { get; set; }

        private static Uri EnsureTrailingSlash(Uri uri)
        {
            string uriString = uri.AbsoluteUri;
            return new Uri(uriString.EndsWith("/", StringComparison.Ordinal) ? uriString : uriString + "/");
        }
    }
}
