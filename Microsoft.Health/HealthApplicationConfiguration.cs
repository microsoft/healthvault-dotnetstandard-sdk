// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Health
{
    /// <summary>
    /// Gives access to the configuration file for the application and
    /// exposes some of the settings directly.
    /// </summary>
    /// 
    public class HealthApplicationConfiguration
    {
        #region configuration key constants
        // Url configuration keys
        private const string ConfigKeyHealthServiceUrl = "HealthServiceUrl";
        private const string ConfigKeyRestHealthServiceUrl = "RestHealthServiceUrl";
        private const string ConfigKeyShellUrl = "ShellUrl";

        // Application related configuration keys
        private const string ConfigKeyAppId = "ApplicationId";
        private const string ConfigKeyApplicationCertificateFileName = "ApplicationCertificateFilename";
        private const string ConfigKeyApplicationCertificatePassword = "ApplicationCertificatePassword";
        private const string ConfigKeyCertSubject = "AppCertSubject";
        private const string ConfigKeySignatureCertStoreLocation = "SignatureCertStoreLocation";
        private const string ConfigKeySupportedType = "SupportedTypeVersions";
        private const string ConfigKeyUseLegacyTypeVersionSupport = "UseLegacyTypeVersionSupport";
        private const string ConfigKeyMultiInstanceAware = "MultiInstanceAware";
        private const string ConfigKeyServiceInfoDefaultCacheTtlMilliseconds = "ServiceInfoDefaultCacheTtlMilliseconds";

        // Request/Response related configuration keys
        private const string ConfigKeyDefaultRequestTimeout = "DefaultRequestTimeout";
        private const string ConfigKeyDefaultRequestTimeToLive = "DefaultRequestTimeToLive";
        private const string ConfigKeyRequestRetryOnInternal500Count = "RequestRetryOnInternal500Count";
        private const string ConfigKeyRequestRetryOnInternal500SleepSeconds = "RequestRetryOnInternal500SleepSeconds";
        private const string ConfigKeyRequestCompressionThreshold = "RequestCompressionThreshold";
        private const string ConfigKeyRequestCompressionMethod = "RequestCompressionMethod";
        private const string ConfigKeyResponseCompressionMethods = "ResponseCompressionMethods";
        private const string ConfigKeyDefaultInlineBlobHashBlockSize = "DefaultInlineBlobHashBlockSize";

        // Security related keys
        private const string ConfigKeyHmacAlgorithmName = "HmacAlgorithmName";
        private const string ConfigKeyHashAlgorithmName = "HashAlgorithmName";
        private const string ConfigKeySignatureHashAlgorithmName = "SignatureHashAlgorithmName";
        private const string ConfigKeySignatureAlgorithmName = "SignatureAlgorithmName";
        private const string ConfigSymmetricAlgorithmName = "SymmetricAlgorithmName";

        // Connection related keys
        private const string ConfigKeyConnectionUseHttpKeepAlive = "ConnectionUseHttpKeepAlive";
        private const string ConfigKeyConnectionLeaseTimeout = "ConnectionLeaseTimeout";
        private const string ConfigKeyConnectionMaxIdleTime = "ConnectionMaxIdleTime";

        #endregion

        /// <summary>
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public static HealthApplicationConfiguration Current
        {
            get { return _current; }
            set { _current = value; }
        }

        private static volatile HealthApplicationConfiguration _current =
            new HealthApplicationConfiguration();

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
        public virtual Uri HealthVaultUrl
        {
            get
            {
                if (_healthVaultRootUrl == null)
                {
                    _healthVaultRootUrl = GetConfigurationUrl(ConfigKeyHealthServiceUrl, true);
                }

                return _healthVaultRootUrl;
            }
        }
        private volatile Uri _healthVaultRootUrl;

        /// <summary>
        /// Gets the HealthVault method request URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HealthServiceUrl" configuration
        /// value.
        /// </remarks>
        /// 
        public Uri HealthVaultMethodUrl
        {
            get
            {
                string newUri = HealthVaultUrl.AbsoluteUri;
                if (!newUri.EndsWith("/", StringComparison.Ordinal))
                {
                    newUri = newUri + "/wildcat.ashx";
                }
                else
                {
                    newUri = newUri + "wildcat.ashx";
                }

                return new Uri(newUri);
            }
        }

        /// <summary>
        /// Gets the HealthVault type schema root URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HealthServiceUrl" configuration
        /// value.
        /// </remarks>
        /// 
        public Uri HealthVaultTypeSchemaUrl
        {
            get
            {
                return new Uri(HealthVaultUrl, "type-xsd/");
            }
        }

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
        public virtual Uri HealthVaultShellUrl
        {
            get
            {
                if (_shellUrl == null)
                {
                    _shellUrl = GetConfigurationUrl(ConfigKeyShellUrl, true);
                }

                return _shellUrl;
            }
        }
        private volatile Uri _shellUrl;

        /// <summary>
        /// Gets the URL to/from which BLOBs get streamed, for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks>
        /// 
        public virtual Uri BlobStreamUrl
        {
            get
            {
                if (HealthVaultUrl != null)
                {
                    return new Uri(
                        HealthVaultUrl.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped) + "/streaming/wildcatblob.ashx");
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the HealthVault client service URL for
        /// the configured default instance of the HealthVault web-service,
        /// from the application or web configuration file.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks> 
        /// 
        public virtual Uri HealthClientServiceUrl
        {
            get
            {
                if (HealthVaultUrl != null)
                {
                    return UrlPathAppend(HealthVaultUrl, "hvclientservice.ashx");
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a certificate containing the application's private key.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "SignatureCertStoreLocation", "AppCertSubject",
        /// "ApplicationCertificateFilename", and "ApplicationCertificatePassword" configuration
        /// values.
        /// </remarks>
        /// 
        public virtual X509Certificate2 ApplicationCertificate
        {
            get
            {
                if (_applicationCertificate == null)
                {
                    _applicationCertificate = GetApplicationCertificate(ApplicationId);
                }

                return _applicationCertificate;
            }
        }
        private volatile X509Certificate2 _applicationCertificate;

        // This is here for compatibility with the previous relase and for testability.
        internal X509Certificate2 GetApplicationCertificate(Guid applicationId)
        {
            Validator.ThrowArgumentExceptionIf(
                applicationId == Guid.Empty,
                "appId",
                "GuidParameterEmpty");

            return GetApplicationCertificate(
                applicationId,
                GetSignatureCertStoreLocation(),
                "CN=" + GetApplicationCertificateSubject(applicationId));
        }

        internal X509Certificate2 GetApplicationCertificateFromStore(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            if (certSubject == null)
            {
                certSubject = "CN=" + GetApplicationCertificateSubject(applicationId);
            }

            HealthVaultPlatformTrace.LogCertLoading(
                "Opening cert store (read-only): {0}",
                storeLocation.ToString());

            RSACryptoServiceProvider rsaProvider = null;
            string thumbprint = null;

            X509Certificate2 result = null;
            X509Store store = new X509Store(storeLocation);

            store.Open(OpenFlags.ReadOnly);

            try
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Looking for matching cert with subject: {0}",
                    certSubject);

                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (String.Equals(
                            cert.Subject,
                            certSubject,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        HealthVaultPlatformTrace.LogCertLoading(
                            "Found matching cert subject with thumbprint: {0}",
                            cert.Thumbprint);

                        thumbprint = cert.Thumbprint;

                        HealthVaultPlatformTrace.LogCertLoading("Looking for private key");
                        rsaProvider = (RSACryptoServiceProvider)cert.PrivateKey;
                        HealthVaultPlatformTrace.LogCertLoading("Private key found");

                        result = cert;
                        break;
                    }
                }
            }
            catch (CryptographicException e)
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Failed to retrieve private key for certificate: {0}",
                    e.ToString());
            }
            finally
            {
                store.Close();
            }

            if (rsaProvider == null || String.IsNullOrEmpty(thumbprint))
            {
                throw new SecurityException(
                        ResourceRetriever.FormatResourceString(
                            "CertificateNotFound",
                            certSubject,
                            storeLocation));
            }

            return result;
        }

        private X509Certificate2 GetApplicationCertificateFromFile(
            string certFilename)
        {
            HealthVaultPlatformTrace.LogCertLoading(
                "Attempting to load certificate from file: {0}",
                certFilename);

            RSACryptoServiceProvider rsaProvider = null;
            string thumbprint = null;

            certFilename = Environment.ExpandEnvironmentVariables(certFilename);

            if (!System.IO.File.Exists(certFilename))
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Cert file not found: {0}",
                    certFilename);

                throw Validator.SecurityException("CertificateFileNotFound");
            }

            string password = GetConfigurationString(ConfigKeyApplicationCertificatePassword);
            X509Certificate2 cert;

            try
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Loading certificate from file {0}",
                    String.IsNullOrEmpty(password) ? "without a password" : "with a password");

                cert = new X509Certificate2(certFilename, password, X509KeyStorageFlags.MachineKeySet);
            }
            catch (CryptographicException e)
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Failed to load certificate: {0}",
                    e.ToString());

                throw Validator.SecurityException("ErrorLoadingCertificateFile", e);
            }

            if (cert != null)
            {
                HealthVaultPlatformTrace.LogCertLoading("Looking for private key");

                if (cert.PrivateKey == null)
                {
                    HealthVaultPlatformTrace.LogCertLoading(
                        "Certificate did not contain a private key.");

                    throw Validator.SecurityException("CertificateMissingPrivateKey");
                }

                HealthVaultPlatformTrace.LogCertLoading(
                    "Found cert with thumbprint: {0}",
                    cert.Thumbprint);

                thumbprint = cert.Thumbprint;
                rsaProvider = (RSACryptoServiceProvider)cert.PrivateKey;
                HealthVaultPlatformTrace.LogCertLoading("Private key found");
            }

            if (rsaProvider == null || String.IsNullOrEmpty(thumbprint))
            {
                throw Validator.SecurityException("CertificateNotFound");
            }
            return cert;
        }

        private X509Certificate2 GetApplicationCertificate(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            X509Certificate2 cert = null;

            string applicationCertificateFilename = GetConfigurationString(ConfigKeyApplicationCertificateFileName);
            if (applicationCertificateFilename != null)
            {
                cert = GetApplicationCertificateFromFile(applicationCertificateFilename);
            }
            else
            {
                cert = GetApplicationCertificateFromStore(applicationId, storeLocation, certSubject);
            }
            return cert;
        }

        /// <summary>
        /// Gets the subject name of the certificate containing the applications private key.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The application identifier to get the certificate subject name for. This is only used
        /// to default the name if the certificate subject is specified in the web.config. The default
        /// name is "WildcatApp-" + <paramref name="applicationId"/>.
        /// </param>
        /// 
        /// <remarks>
        /// This value is retrieved from the application configuration file using the configuration
        /// key named "AppCertSubject".
        /// </remarks>
        /// 
        private string GetApplicationCertificateSubject(Guid applicationId)
        {
            string result = GetConfigurationString(ConfigKeyCertSubject);

            if (result == null)
            {
                result = "WildcatApp-" + applicationId.ToString();

                HealthVaultPlatformTrace.LogCertLoading(
                    "Using default cert subject: {0}",
                    result);
            }
            else
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Cert subject retrieved from configuration file key '{0}': {1}",
                    ConfigKeyCertSubject,
                    result);
            }

            return result;
        }

        private StoreLocation GetSignatureCertStoreLocation()
        {
            string signatureCertStoreLocation = GetConfigurationString(ConfigKeySignatureCertStoreLocation, DefaultSignatureCertStoreLocation);

            StoreLocation result = StoreLocation.LocalMachine;
            try
            {
                result = (StoreLocation)Enum.Parse(
                        typeof(StoreLocation),
                        signatureCertStoreLocation,
                        true);
            }
            catch (Exception)
            {
                if (String.IsNullOrEmpty(signatureCertStoreLocation))
                {
                    throw Validator.InvalidConfigurationException("SignatureCertStoreLocationMissing");
                }
            }

            return result;
        }
        private const string DefaultSignatureCertStoreLocation = "LocalMachine";

        /// <summary>
        /// Gets the application's unique identifier.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "ApplicationId" configuration
        /// value.
        /// </remarks>
        /// 
        public virtual Guid ApplicationId
        {
            get
            {
                if (_appId == Guid.Empty)
                {
                    _appId = GetConfigurationGuid(ConfigKeyAppId);
                }

                return _appId;
            }
        }
        private Guid _appId;

        /// <summary>
        /// Gets the name of the algorithm used to ensure communication with HealthVault
        /// isn't tampered with.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HmacAlgorithmName" configuration
        /// value. The value defaults to "HMACSHA256".
        /// </remarks>
        /// 
        public virtual string HmacAlgorithmName
        {
            get
            {
                if (_hmacAlgorithmName == null)
                {
                    _hmacAlgorithmName = GetConfigurationString(ConfigKeyHmacAlgorithmName, DefaultHmacAlgorithmName);
                }

                return _hmacAlgorithmName;
            }
        }
        private volatile string _hmacAlgorithmName;
        /// <summary>
        /// The default HMAC algorithm name.
        /// </summary>
        protected const string DefaultHmacAlgorithmName = "HMACSHA256";

        /// <summary>
        /// Gets the name of the hashing algorithm to use when communicating with HealthVault.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HashAlgorithmName" configuration
        /// value. The value defaults to "SHA256".
        /// </remarks>
        /// 
        /// <summary>
        /// Gets the name of the hashing algorithm to use when communicating with HealthVault.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "HashAlgorithmName" configuration
        /// value. The value defaults to "SHA256".
        /// </remarks>
        /// 
        public virtual string HashAlgorithmName
        {
            get
            {
                if (_hashAlgorithmName == null)
                {
                    _hashAlgorithmName = GetConfigurationString(ConfigKeyHashAlgorithmName, DefaultHashAlgorithmName);
                }

                return _hashAlgorithmName;
            }
        }
        private volatile string _hashAlgorithmName;
        /// <summary>
        /// The default hash algorithm name.
        /// </summary>
        protected const string DefaultHashAlgorithmName = "SHA256";

        /// <summary>
        /// Gets the name of the signature hash algorithm.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "SignatureHashAlgorithmName" configuration
        /// value. The value defaults to "SHA1".
        /// </remarks>
        /// 
        public virtual string SignatureHashAlgorithmName
        {
            get
            {
                if (_signatureHashAlgorithmName == null)
                {
                    _signatureHashAlgorithmName = GetConfigurationString(ConfigKeySignatureHashAlgorithmName, DefaultSignatureHashAlgorithmName);
                }

                return _signatureHashAlgorithmName;
            }
        }
        private volatile string _signatureHashAlgorithmName;
        /// <summary>
        /// The default signature hash algorithm name.
        /// </summary>
        protected const string DefaultSignatureHashAlgorithmName = "SHA1";

        /// <summary>
        /// Gets the name of the signature algorithm.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "SignatureAlgorithmName" configuration
        /// value. The value defaults to "RSA-SHA1".
        /// </remarks>
        /// 
        public virtual string SignatureAlgorithmName
        {
            get
            {
                if (_signatureAlgorithmName == null)
                {
                    _signatureAlgorithmName = GetConfigurationString(ConfigKeySignatureAlgorithmName, DefaultSignatureAlgorithmName);
                }

                return _signatureAlgorithmName;
            }
        }
        private volatile string _signatureAlgorithmName;
        /// <summary>
        /// The default signature algorithm name.
        /// </summary>
        protected const string DefaultSignatureAlgorithmName = "RSA-SHA1";

        /// <summary>
        /// Gets the name of the symmetric algorithm.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "SymmetricAlgorithmName" configuration
        /// value. The value defaults to "AES256".
        /// </remarks>
        /// 
        public virtual string SymmetricAlgorithmName
        {
            get
            {
                if (_symmetricAlgorithmName == null)
                {
                    _symmetricAlgorithmName = GetConfigurationString(ConfigSymmetricAlgorithmName, DefaultSymmetricAlgorithmName);
                }

                return _symmetricAlgorithmName;
            }
        }
        private volatile string _symmetricAlgorithmName;
        /// <summary>
        /// The default symmetric algorithm name.
        /// </summary>
        protected const string DefaultSymmetricAlgorithmName = "AES256";

        #region web request/response configuration

        /// <summary>
        /// Gets the request timeout in seconds.
        /// </summary>
        /// 
        /// <remarks>
        /// This value is used to set the <see cref="HttpWebRequest.Timeout"/> property 
        /// when making the request to HealthVault. The timeout is the number of seconds that a 
        /// request will wait for a response from HealtVault. If the method response is not
        /// returned within the time-out period the request will throw a <see cref="System.Net.WebException"/>
        /// with the <see cref="System.Net.WebException.Status">Status</see> property set to
        /// <see cref="System.Net.WebExceptionStatus.Timeout"/>.
        /// This property corresponds to the "defaultRequestTimeout" configuration
        /// value. The value defaults to 30 seconds.
        /// </remarks>
        /// 
        public virtual int DefaultRequestTimeout
        {
            get
            {
                if (!_configurationRequestTimeoutInitialized)
                {
                    int tempRequestTimeout = GetConfigurationInt32(ConfigKeyDefaultRequestTimeout, DefaultDefaultRequestTimeout);

                    // Note, -1 signifies an infinite timeout so that is OK.
                    if (tempRequestTimeout < -1)
                    {
                        tempRequestTimeout = DefaultDefaultRequestTimeout;
                    }

                    _configuredRequestTimeout = tempRequestTimeout;
                    _configurationRequestTimeoutInitialized = true;
                }

                return _configuredRequestTimeout;
            }
        }
        private volatile int _configuredRequestTimeout;
        private volatile bool _configurationRequestTimeoutInitialized;
        /// <summary>
        /// The default request time out value.
        /// </summary>
        protected const int DefaultDefaultRequestTimeout = 30;

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
                if (!_configuredRequestTimeToLiveInitialized)
                {
                    int tempRequestTimeToLive = GetConfigurationInt32(ConfigKeyDefaultRequestTimeToLive, DefaultDefaultRequestTimeToLive);

                    if (tempRequestTimeToLive < -1)
                    {
                        tempRequestTimeToLive = DefaultDefaultRequestTimeToLive;
                    }

                    _configuredRequestTimeToLive = tempRequestTimeToLive;
                    _configuredRequestTimeToLiveInitialized = true;
                }

                return _configuredRequestTimeToLive;
            }
        }
        private volatile int _configuredRequestTimeToLive;
        private volatile bool _configuredRequestTimeToLiveInitialized;
        /// <summary>
        /// The default request time to live value.
        /// </summary>
        protected const int DefaultDefaultRequestTimeToLive = 30 * 60;

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
                if (!_retryOnInternal500CountInitialized)
                {
                    _retryOnInternal500Count = GetConfigurationInt32(ConfigKeyRequestRetryOnInternal500Count, DefaultRetryOnInternal500Count);
                    _retryOnInternal500CountInitialized = true;
                }

                return _retryOnInternal500Count;
            }
        }
        private volatile int _retryOnInternal500Count;
        private volatile bool _retryOnInternal500CountInitialized;
        /// <summary>
        /// The default number of internal retries.
        /// </summary>
        protected const int DefaultRetryOnInternal500Count = 2;

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
                if (!_retryOnInternal500SleepSecondsInitialized)
                {
                    _retryOnInternal500SleepSeconds = GetConfigurationInt32(ConfigKeyRequestRetryOnInternal500SleepSeconds, DefaultRetryOnInternal500SleepSeconds);
                    _retryOnInternal500SleepSecondsInitialized = true;
                }

                return _retryOnInternal500SleepSeconds;
            }
        }
        private volatile int _retryOnInternal500SleepSeconds;
        private volatile bool _retryOnInternal500SleepSecondsInitialized;

        /// <summary>
        /// Default sleep duration in seconds.
        /// </summary>
        protected const int DefaultRetryOnInternal500SleepSeconds = 1;

        /// <summary>
        /// Gets the size in kilobytes above which requests will be compressed.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "requestCompressionThreshold" configuration
        /// value. The value defaults to 1KB.
        /// </remarks>
        /// 
        public virtual int RequestCompressionThreshold
        {
            get
            {
                if (!_requestCompressionThresholdInitialized)
                {
                    _requestCompressionThreshold = GetConfigurationInt32(ConfigKeyRequestCompressionThreshold, DefaultRequestCompressionThreshold);
                    _requestCompressionThresholdInitialized = true;
                }

                return _requestCompressionThreshold;
            }
        }
        private volatile int _requestCompressionThreshold;
        private volatile bool _requestCompressionThresholdInitialized;
        /// <summary>
        /// Default size for compression threshold
        /// </summary>
        protected const int DefaultRequestCompressionThreshold = 1; // kilobyte

        /// <summary>
        /// Gets the method used to compress requests.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "requestCompressionMethod" configuration
        /// value. The value defaults is to not compress requests.
        /// </remarks>
        /// 
        public virtual string RequestCompressionMethod
        {
            get
            {
                if (_requestCompressionMethod == null)
                {
                    string tempCompressionMethod = GetConfigurationString(ConfigKeyRequestCompressionMethod, String.Empty);

                    if (!String.IsNullOrEmpty(tempCompressionMethod))
                    {
                        if (!String.Equals(
                                tempCompressionMethod,
                                "gzip",
                                StringComparison.OrdinalIgnoreCase) &&
                            !String.Equals(
                                tempCompressionMethod,
                                "deflate",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw Validator.InvalidConfigurationException("InvalidRequestCompressionMethod");
                        }
                    }

                    _requestCompressionMethod = tempCompressionMethod;
                }

                return _requestCompressionMethod;
            }
        }
        private volatile string _requestCompressionMethod;

        /// <summary>
        /// Gets the application's supported compression methods that can be sent back 
        /// from HealtVault during a method response.
        /// </summary>
        /// 
        /// <remarks>
        /// This property corresponds to the "responseCompressionMethods" configuration
        /// value. The value defaults to not compress responses.
        /// </remarks>
        /// 
        public virtual string ResponseCompressionMethods
        {
            get
            {
                if (_responseCompressionMethods == null)
                {
                    GetResponseCompressionMethods();
                }

                return _responseCompressionMethods;
            }
        }
        private volatile string _responseCompressionMethods;

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need the lowercase for backward compat.")]
        private void GetResponseCompressionMethods()
        {
            string tempCompressionMethods = GetConfigurationString(ConfigKeyResponseCompressionMethods, String.Empty);

            if (!String.IsNullOrEmpty(tempCompressionMethods))
            {
                string[] methods = SDKHelper.SplitAndTrim(tempCompressionMethods.ToLowerInvariant(), ',');

                for (int i = 0; i < methods.Length; ++i)
                {
                    if (!String.Equals(
                            methods[i],
                            "gzip",
                            StringComparison.Ordinal) &&
                        !String.Equals(
                            methods[i],
                            "deflate",
                            StringComparison.Ordinal))
                    {
                        throw Validator.HealthServiceException("InvalidResponseCompressionMethods");
                    }
                }

                tempCompressionMethods = String.Join(",", methods);
            }

            _responseCompressionMethods = tempCompressionMethods;
        }

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
                if (!_configuredInlineBlobHashBlockSizeInitilialized)
                {
                    int tempBlobHashSize = GetConfigurationInt32(
                        ConfigKeyDefaultInlineBlobHashBlockSize,
                        BlobHasher.DefaultInlineBlobHashBlockSizeBytes);

                    if (tempBlobHashSize < 1)
                    {
                        tempBlobHashSize =
                            BlobHasher.DefaultInlineBlobHashBlockSizeBytes;
                    }

                    _configuredInlineBlobHashBlockSize = tempBlobHashSize;
                    _configuredInlineBlobHashBlockSizeInitilialized = true;
                }

                return _configuredInlineBlobHashBlockSize;
            }
        }
        private volatile int _configuredInlineBlobHashBlockSize;
        private volatile bool _configuredInlineBlobHashBlockSizeInitilialized;

        /// <summary>
        /// Gets the type version identifiers of types supported by this application.
        /// </summary>
        /// 
        /// <remarks>
        /// Although most applications don't need this configuration setting, if an application
        /// calls <see cref="HealthRecordAccessor.GetItem(Guid)"/> or makes any query to HealthVault
        /// that doesn't specify the type identifier in the filter, this configuration setting
        /// will tell HealthVault the format of the type to reply with. For example, if a web 
        /// application has two servers and makes a call to GetItem for EncounterV1 and the 
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
            get
            {
                if (_supportedTypeVersions == null)
                {
                    _supportedTypeVersions = GetSupportedTypeVersions();
                }

                return _supportedTypeVersions;
            }
        }
        private volatile IList<Guid> _supportedTypeVersions;

        private IList<Guid> GetSupportedTypeVersions()
        {
            Collection<Guid> supportedTypeVersions = new Collection<Guid>();

            string typeVersionsConfig = GetConfigurationString(ConfigKeySupportedType, String.Empty);
            string[] typeVersions =
                typeVersionsConfig.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string typeVersionClassName in typeVersions)
            {
                if (ItemTypeManager.TypeHandlersByClassName.ContainsKey(typeVersionClassName))
                {
                    supportedTypeVersions.Add(ItemTypeManager.TypeHandlersByClassName[typeVersionClassName].TypeId);
                }
                else
                {
                    throw Validator.InvalidConfigurationException("InvalidSupportedTypeVersions");
                }
            }

            return supportedTypeVersions;
        }

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
        public virtual bool UseLegacyTypeVersionSupport
        {
            get
            {
                if (!_useLegacyTypeVersionSupportInitialized)
                {
                    _useLegacyTypeVersionSupport = GetConfigurationBoolean(ConfigKeyUseLegacyTypeVersionSupport, false);
                    _useLegacyTypeVersionSupportInitialized = true;
                }
                return _useLegacyTypeVersionSupport;
            }
        }
        private volatile bool _useLegacyTypeVersionSupport;
        private volatile bool _useLegacyTypeVersionSupportInitialized;

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
                if (!_multiInstanceAwareInitialized)
                {
                    _multiInstanceAware = GetConfigurationBoolean(ConfigKeyMultiInstanceAware, true);
                    _multiInstanceAwareInitialized = true;
                }

                return _multiInstanceAware;
            }
        }

        private volatile bool _multiInstanceAware;
        private volatile bool _multiInstanceAwareInitialized;

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
        public virtual int ConnectionMaxIdleTime
        {
            get
            {
                if (!_connectionMaxIdleTimeInitialized)
                {
                    _connectionMaxIdleTime = GetConfigurationInt32(ConfigKeyConnectionMaxIdleTime, 110 * 1000);

                    if (_connectionMaxIdleTime < -1)
                    {
                        _connectionMaxIdleTime = -1;
                    }

                    _connectionMaxIdleTimeInitialized = true;
                }

                return _connectionMaxIdleTime;
            }
        }

        private volatile int _connectionMaxIdleTime;
        private volatile bool _connectionMaxIdleTimeInitialized;

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
        public virtual int ConnectionLeaseTimeout
        {
            get
            {
                if (!_connectionLeaseTimeoutInitialized)
                {
                    _connectionLeaseTimeout = GetConfigurationInt32(ConfigKeyConnectionLeaseTimeout, 5 * 60 * 1000);

                    if (_connectionLeaseTimeout < -1)
                    {
                        _connectionLeaseTimeout = -1;
                    }

                    _connectionLeaseTimeoutInitialized = true;
                }

                return _connectionLeaseTimeout;
            }
        }

        private volatile int _connectionLeaseTimeout;
        private volatile bool _connectionLeaseTimeoutInitialized;

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
                if (!_connectionUseHttpKeepAliveInitialized)
                {
                    _connectionUseHttpKeepAlive = GetConfigurationBoolean(ConfigKeyConnectionUseHttpKeepAlive, true);
                    _connectionUseHttpKeepAliveInitialized = true;
                }

                return _connectionUseHttpKeepAlive;
            }
        }

        private volatile bool _connectionUseHttpKeepAlive;
        private volatile bool _connectionUseHttpKeepAliveInitialized;

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

                lock (_serviceInfoDefaultCacheTtlInitLock)
                {
                    if (!_serviceInfoDefaultCacheTtlInitialized)
                    {
                        _serviceInfoDefaultCacheTtl =
                            GetConfigurationTimeSpanMilliseconds(
                                ConfigKeyServiceInfoDefaultCacheTtlMilliseconds,
                                TimeSpan.FromDays(1));

                        _serviceInfoDefaultCacheTtlInitialized = true;
                    }

                    return _serviceInfoDefaultCacheTtl;
                }
            }
        }

        private TimeSpan _serviceInfoDefaultCacheTtl;

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
                if (_restHealthVaultRootUrl == null)
                {
                    _restHealthVaultRootUrl = GetConfigurationUrl(ConfigKeyRestHealthServiceUrl, true /* appendSlash */);
                }

                return _restHealthVaultRootUrl;
            }
        }
        private volatile Uri _restHealthVaultRootUrl;

        private bool _serviceInfoDefaultCacheTtlInitialized;
        private readonly object _serviceInfoDefaultCacheTtlInitLock = new object();

        #region configuration retrieval helpers
        /// <summary>
        /// Gets the string configuration value given the key
        /// </summary>
        /// <param name="configurationKey">Key to look up the configuration item.</param>
        /// <returns>String value of the configuration item, should return null if key not found.</returns>
        protected virtual string GetConfigurationString(string configurationKey)
        {
            return ApplicationConfiguration.GetConfigurationValue(configurationKey);
        }

        /// <summary>
        /// Retrieves the specified setting for strings.
        /// </summary>
        /// 
        /// <param name="key">
        /// A string specifying the name of the setting.
        /// </param>
        /// 
        /// <param name="defaultValue">
        /// A string representing the default string value.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the settings.
        /// </returns>
        /// 
        private string GetConfigurationString(string key, string defaultValue)
        {
            string result = GetConfigurationString(key);

            if (result == null)
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Bool value from configuration if exists or the specified default value.</returns>
        private bool GetConfigurationBoolean(string key, bool defaultValue)
        {
            bool result = defaultValue;
            string resultString = GetConfigurationString(key);
            if (!String.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = bool.Parse(resultString);
            }

            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Int value from configuration if exists or the specified default value.</returns>
        private int GetConfigurationInt32(string key, int defaultValue)
        {
            int result = defaultValue;

            string resultString = GetConfigurationString(key);
            if (resultString != null)
            {
                if (!Int32.TryParse(resultString, out result))
                {
                    result = defaultValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the URL value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="appendSlash">If set to true, append a '/' character at the end of URL.</param>
        /// <returns>URL value from configuration if exists, null if not found.</returns>
        private Uri GetConfigurationUrl(string key, bool appendSlash)
        {
            string resultString = GetConfigurationString(key);
            if (String.IsNullOrEmpty(resultString))
            {
                return null;
            }
            else
            {
                if (appendSlash)
                {
                    return new Uri(resultString.EndsWith("/", StringComparison.Ordinal) ? resultString : (resultString + "/"));
                }
                else
                {
                    return new Uri(resultString);
                }
            }
        }

        /// <summary>
        /// Retrieves the specified setting for GUIDs.
        /// </summary>
        /// 
        /// <param name="key">
        /// A string specifying the name of the setting.
        /// </param>
        /// 
        /// <returns>
        /// The GUID of the setting.
        /// </returns>
        /// 
        private Guid GetConfigurationGuid(string key)
        {
            Guid result = Guid.Empty;
            string resultString = GetConfigurationString(key);
            if (!String.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = new Guid(resultString);
            }

            return result;
        }

        private TimeSpan GetConfigurationTimeSpanMilliseconds(string key, TimeSpan defaultValue)
        {
            int resultInMs = GetConfigurationInt32(key, (int)defaultValue.TotalMilliseconds);
            return TimeSpan.FromMilliseconds(resultInMs);
        }

        /// <summary>
        /// Appends the specified path to the URL after trimming the path.
        /// </summary>
        /// <param name="baseUrl">The base URL to trim and append the path to.
        /// </param>
        /// <param name="path">The path to append to the URL.</param>
        /// <returns>The combined URL and path.</returns>
        private static Uri UrlPathAppend(Uri baseUrl, string path)
        {
            return new Uri(baseUrl, path);
        }

        #endregion
    }
}

