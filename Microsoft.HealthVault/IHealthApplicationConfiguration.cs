using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Application configuration containing properties
    /// which used by the service
    /// </summary>
    public interface IHealthApplicationConfiguration
    {
        /// <summary>
        /// Gets the root URL for a default instance of the
        /// HealthVault web-service.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HealthServiceUrl" configuration
        /// value with "wildcat.ashx" removed.
        /// </remarks>
        ///
        Uri HealthVaultUrl { get; }

        /// <summary>
        /// Gets the HealthVault Shell URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "ShellUrl" configuration
        /// value.
        /// </remarks>
        ///
        Uri HealthVaultShellUrl { get; }

        /// <summary>
        /// Gets the application's unique identifier.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "ApplicationId" configuration
        /// value.
        /// </remarks>
        ///
        Guid ApplicationId { get; }

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
        int DefaultRequestTimeout { get; }

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
        int DefaultRequestTimeToLive { get; }

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
        int RetryOnInternal500Count { get; }

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
        int RetryOnInternal500SleepSeconds { get; }

        /// <summary>
        /// Gets the size in bytes of the block used to hash inlined BLOB data.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "defaultInlineBlobHashBlockSize" configuration
        /// value. The value defaults to 2MB.
        /// </remarks>
        ///
        int InlineBlobHashBlockSize { get; }

        /// <summary>
        /// Gets the type version identifiers of types supported by this application.
        /// </summary>
        ///
        /// <remarks>
        /// Although most applications don't need this configuration setting, if an application
        /// calls <see cref="HealthRecordAccessor.GetItemAsync(Guid,HealthRecordItemSections)"/> or makes any query to HealthVault
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
        IList<Guid> SupportedTypeVersions { get; }

        /// <summary>
        /// Gets a value indicating whether or not legacy type versioning support should be used.
        /// </summary>
        ///
        /// <remarks>
        /// Type versions support was initially determined by an applications base authorizations
        /// and/or the <see cref="HealthRecordView.TypeVersionFormat"/>. Some of these behaviors
        /// were unexpected which led to changes to automatically put the <see cref="HealthRecordFilter.TypeIds"/>
        /// and <see cref="HealthApplicationConfiguration.SupportedTypeVersions"/> into the
        /// <see cref="HealthRecordView.TypeVersionFormat"/> automatically for developers. This
        /// exhibits the expected behavior for most applications. However, in some rare cases
        /// applications may need to revert back to the original behavior. When this property
        /// returns true the original behavior will be observed. If false, the new behavior will
        /// be observed. This property defaults to false and can be changed in the web.config file
        /// "UseLegacyTypeVersionSupport" setting.
        /// </remarks>
        ///
        bool UseLegacyTypeVersionSupport { get; }

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
        bool MultiInstanceAware { get; }

        /// <summary>
        /// Gets the amount of time, in milliseconds, that the application's connection can
        /// remain idle before the HealthVault framework closes the connection.
        /// </summary>
        ///
        /// <remarks>
        /// This default value is 110 seconds of inactivity.
        /// <p>
        /// This setting only applies when using HTTP Persistent Connections
        /// <see cref="HealthApplicationConfiguration.ConnectionUseHttpKeepAlive"/>.
        /// </p>
        /// <p>
        /// Setting this property to -1 indicates the connection should never
        /// time out.
        /// </p>
        /// <p>
        /// This property corresponds to the "ConnectionMaxIdleTime" configuration value.
        /// </p>
        /// </remarks>
        int ConnectionMaxIdleTime { get; }

        /// <summary>
        /// Gets the amount of time, in milliseconds, that the application's connection can
        /// remain open before the HealthVault framework closes the connection.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is 5 minutes.
        /// <p>
        /// This setting only applies when using HTTP Persistent Connections
        /// <see cref="HealthApplicationConfiguration.ConnectionUseHttpKeepAlive"/>.
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
        int ConnectionLeaseTimeout { get; }

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
        bool ConnectionUseHttpKeepAlive { get; }

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
        TimeSpan ServiceInfoDefaultCacheTtl { get; }

        /// <summary>
        /// Gets the root URL for a default instance of the
        /// Rest HealthVault service
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "RestHealthServiceUrl" configuration.
        /// </remarks>
        ///
        Uri RestHealthVaultUrl { get; }

        /// <summary>
        /// Gets the application certificate password.
        /// </summary>
        string ApplicationCertificatePassword { get; }

        /// <summary>
        /// Gets the application certificate file name.
        /// </summary>
        string ApplicationCertificateFileName { get; }

        /// <summary>
        /// Gets the signature certificate store location.
        /// </summary>
        // StoreLocation SignatureCertStoreLocation { get; }

        /// <summary>
        /// Gets the certificate subject.
        /// </summary>
        string CertSubject { get; }

        /// <summary>
        /// Gets the crypto configuration.
        /// </summary>
        /// <value>
        /// The crypto configuration.
        /// </value>
        ICryptoConfiguration CryptoConfiguration { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi record application.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi record application; otherwise, <c>false</c>.
        /// </value>
        bool IsMultiRecordApp { get; set; }
    }
}