// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Web.Configuration;

namespace Microsoft.HealthVault.Web.Providers
{
    /// <summary>
    /// <see cref="ICertificateInfoProvider"/>
    /// </summary>
    internal class CertificateInfoProvider : ICertificateInfoProvider
    {
        private WebHealthVaultConfiguration configuration;
        private Guid applicationId;

        private StoreLocation storeLocation;
        private string certSubject;

        private X509Certificate2 x509Certificate2;

        public CertificateInfoProvider(WebHealthVaultConfiguration configuration)
        {
            this.configuration = configuration;
            this.applicationId = this.configuration.MasterApplicationId;


            this.storeLocation = StoreLocation.LocalMachine;
            this.certSubject = "CN=" + this.GetApplicationCertificateSubject();

            this.x509Certificate2 = this.GetApplicationCertificate();

            this.Thumbprint = this.x509Certificate2.Thumbprint;

            this.PrivateKey = (RSACryptoServiceProvider)this.x509Certificate2.PrivateKey;
        }

        public string Thumbprint { get; internal set; }

        public RSACryptoServiceProvider PrivateKey { get; internal set; }
        
        internal X509Certificate2 GetApplicationCertificate()
        {
            string applicationCertificateFilename = this.configuration.ApplicationCertificateFileName;

            var cert = string.IsNullOrEmpty(applicationCertificateFilename) 
                ? this.GetApplicationCertificateFromStore() 
                : this.GetApplicationCertificateFromFile(applicationCertificateFilename);

            return cert;
        }

        private X509Certificate2 GetApplicationCertificateFromFile(string certFilename)
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

                throw new ArgumentException("CertificateFileNotFound");
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

                throw new ArgumentException("ErrorLoadingCertificateFile", e);
            }

            HealthVaultPlatformTrace.LogCertLoading("Looking for private key");

            if (!cert.HasPrivateKey)
            {
                HealthVaultPlatformTrace.LogCertLoading(
                    "Certificate did not contain a private key.");

                throw new ArgumentException("CertificateMissingPrivateKey");
            }

            HealthVaultPlatformTrace.LogCertLoading(
                "Found cert with thumbprint: {0}",
                cert.Thumbprint);

            var thumbprint = cert.Thumbprint;
            var rsaProvider = (RSACryptoServiceProvider)cert.GetRSAPrivateKey();
            HealthVaultPlatformTrace.LogCertLoading("Private key found");

            if (rsaProvider == null || string.IsNullOrEmpty(thumbprint))
            {
                throw new ArgumentException("CertificateNotFound");
            }

            return cert;
        }

        internal X509Certificate2 GetApplicationCertificateFromStore()
        {
            HealthVaultPlatformTrace.LogCertLoading(
                "Opening cert store (read-only): {0}",
                storeLocation.ToString());

            RSACng rsaProvider = null;
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
                throw new SecurityException("CertificateNotFound");
            }

            return result;
        }

        private string GetApplicationCertificateSubject()
        {
            string result = this.configuration.CertSubject;

            if (result == null)
            {
                result = "WildcatApp-" + this.applicationId;

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
