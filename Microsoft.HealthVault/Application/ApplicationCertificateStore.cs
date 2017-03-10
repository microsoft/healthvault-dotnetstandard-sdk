using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Application
{
    internal sealed class ApplicationCertificateStore
    {
        private static readonly object InstanceLock = new object();
        private HealthVaultConfiguration configuration = Ioc.Get<HealthVaultConfiguration>();

        /// <summary>
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public static ApplicationCertificateStore Current
        {
            get
            {
                lock (InstanceLock)
                {
                    return current ?? (current = new ApplicationCertificateStore());
                }
            }

            internal set
            {
                lock (InstanceLock)
                {
                    current = value;
                }
            }
        }

        private static ApplicationCertificateStore current;

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
        public X509Certificate2 ApplicationCertificate => this.applicationCertificate ??
                                                                  (this.applicationCertificate =
                                                                      this.GetApplicationCertificate(this.configuration.MasterApplicationId));

        private volatile X509Certificate2 applicationCertificate;

        // This is here for compatibility with the previous relase and for testability.
        internal X509Certificate2 GetApplicationCertificate(Guid applicationId)
        {
            Validator.ThrowArgumentExceptionIf(
                applicationId == Guid.Empty,
                "appId",
                "GuidParameterEmpty");

            // TODO: What to do here?

            //return this.GetApplicationCertificate(
            //    applicationId,
            //    HealthApplicationConfiguration.Current.SignatureCertStoreLocation,
            //    "CN=" + this.GetApplicationCertificateSubject(applicationId));

            return null;
        }

        internal X509Certificate2 GetApplicationCertificateFromStore(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            if (certSubject == null)
            {
                certSubject = "CN=" + this.GetApplicationCertificateSubject(applicationId);
            }

            HealthVaultPlatformTrace.LogCertLoading(
                "Opening cert store (read-only): {0}",
                storeLocation.ToString());

            RSACng rsaProvider = null;
            string thumbprint = null;

            X509Certificate2 result = null;

            // Note- .NET SDK invokes X509Store(storeLocation),
            // as .NET standard doesn't have similar method yet (tho, available in 2.0) 
            // we will create the store with store name "MY" which is similar
            // to .NET SDK version
            X509Store store = new X509Store(StoreName.My, storeLocation);

            store.Open(OpenFlags.ReadOnly);

            try
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Looking for matching cert with subject: {0}",
                    certSubject);

                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (string.Equals(
                            cert.Subject,
                            certSubject,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        HealthVaultPlatformTrace.LogCertLoading(
                            "Found matching cert subject with thumbprint: {0}",
                            cert.Thumbprint);

                        thumbprint = cert.Thumbprint;

                        HealthVaultPlatformTrace.LogCertLoading("Looking for private key");
                        rsaProvider = (RSACng)cert.GetRSAPrivateKey();
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
                store.Dispose();
            }

            if (rsaProvider == null || string.IsNullOrEmpty(thumbprint))
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

            certFilename = Environment.ExpandEnvironmentVariables(certFilename);

            if (!File.Exists(certFilename))
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Cert file not found: {0}",
                    certFilename);

                throw Validator.SecurityException("CertificateFileNotFound");
            }

            string password = this.configuration.ApplicationCertificatePassword;
            X509Certificate2 cert;

            try
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Loading certificate from file {0}",
                    string.IsNullOrEmpty(password) ? "without a password" : "with a password");

                cert = new X509Certificate2(certFilename, password, X509KeyStorageFlags.MachineKeySet);
            }
            catch (CryptographicException e)
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Failed to load certificate: {0}",
                    e.ToString());

                throw Validator.SecurityException("ErrorLoadingCertificateFile", e);
            }

            HealthVaultPlatformTrace.LogCertLoading("Looking for private key");

            if (!cert.HasPrivateKey)
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Certificate did not contain a private key.");

                throw Validator.SecurityException("CertificateMissingPrivateKey");
            }

            HealthVaultPlatformTrace.LogCertLoading(
                "Found cert with thumbprint: {0}",
                cert.Thumbprint);

            var thumbprint = cert.Thumbprint;
            var rsaProvider = (RSACryptoServiceProvider)cert.GetRSAPrivateKey();
            HealthVaultPlatformTrace.LogCertLoading("Private key found");

            if (rsaProvider == null || string.IsNullOrEmpty(thumbprint))
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
            string applicationCertificateFilename = this.configuration.ApplicationCertificateFileName;
            var cert = applicationCertificateFilename != null ? 
                this.GetApplicationCertificateFromFile(applicationCertificateFilename) : this.GetApplicationCertificateFromStore(applicationId, storeLocation, certSubject);

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
            string result = this.configuration.CertSubject;

            if (result == null)
            {
                result = "WildcatApp-" + applicationId;

                HealthVaultPlatformTrace.LogCertLoading(
                    "Using default cert subject: {0}",
                    result);
            }
            else
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Using custom cert subject: {0}",
                    result);
            }

            return result;
        }
    }
}
