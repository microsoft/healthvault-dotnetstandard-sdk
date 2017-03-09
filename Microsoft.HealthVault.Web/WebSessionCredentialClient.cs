using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Web
{
    internal class WebSessionCredentialClient : SessionCredentialClientBase
    {
        private X509Certificate2 certificate;
        private Guid _applicationInstanceId;

        private const string DigestMethod = "RSA-SHA1";

        private const string SignMethod = "SHA1";
        private static WebConfiguration configuration = Ioc.Get<WebConfiguration>();

        public WebSessionCredentialClient(IConnectionInternal connection, X509Certificate2 cert) 
            : base(connection)
        {
            if (cert == null)
            {
                throw new ArgumentException(nameof(cert));
            }

            this.certificate = cert;
        }

        internal RSACng RsaProvider => (RSACng)this.certificate.GetRSAPrivateKey();

        public override Guid ApplicationInstanceId
        {
            get { return Guid.Parse("79aec84a-1131-435d-bdcc-40de9b870c35"); } // Hard coded for now

            set { _applicationInstanceId = value; }
        }

        public override void WriteInfoXml(XmlWriter writer)
        {
            if (this.certificate == null)
            {
                this.certificate = GetApplicationCertificate(ApplicationInstanceId,
                    StoreLocation.LocalMachine,
                    null);
            }
 
            writer.WriteStartElement("appserver2");

            string requestXml = this.GetContentSection();

            // SIG
            writer.WriteStartElement("sig");
            writer.WriteAttributeString("digestMethod", DigestMethod); // this.DigestMethod 
            writer.WriteAttributeString("sigMethod", SignMethod); // this.SignMethod // hardcoding for now
            writer.WriteAttributeString("thumbprint", this.certificate.Thumbprint);
            writer.WriteString(this.SignRequestXml(requestXml));
            writer.WriteEndElement(); // sig

            // CONTENT
            writer.WriteRaw(requestXml);

            writer.WriteEndElement();
        }

        private X509Certificate2 GetApplicationCertificate(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            var cert = this.GetApplicationCertificateFromStore(applicationId, storeLocation, certSubject);
            return cert;
        }

        private X509Certificate2 GetApplicationCertificateFromStore(Guid applicationId, StoreLocation storeLocation, string certSubject)
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

        private string GetApplicationCertificateSubject(Guid applicationId)
        {
            string result = configuration.CertSubject;

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


        /// <summary>
        /// Generate the to-be signed content for the credential.
        /// </summary>
        ///
        /// <returns>
        /// Raw XML representing the ContentSection of the info secttion.
        /// </returns>
        ///
        internal string GetContentSection()
        {
            StringBuilder requestXml = new StringBuilder(2048);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(requestXml, settings))
            {
                writer.WriteStartElement("content");

                writer.WriteStartElement("app-id");
                writer.WriteString(this.ApplicationInstanceId.ToString());
                writer.WriteEndElement();

                writer.WriteElementString("hmac", "HMACSHA256");

                writer.WriteStartElement("signing-time");
                writer.WriteValue(DateTime.Now.ToUniversalTime());
                writer.WriteEndElement();

                writer.WriteEndElement(); // content
            }

            return requestXml.ToString();
        }

        /// <summary>
        /// Compute and apply the signature to the request XML.
        /// </summary>
        ///
        private string SignRequestXml(string requestXml)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            byte[] paramBlob = encoding.GetBytes(requestXml);
            byte[] sigBlob = this.RsaProvider.SignData(paramBlob, new HashAlgorithmName(DigestMethod), RSASignaturePadding.Pkcs1); // SignData(paramBlob, DigestMethod);

            return Convert.ToBase64String(sigBlob);
        }
    }
}