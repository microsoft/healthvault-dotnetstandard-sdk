// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal class WebSessionCredentialClient : SessionCredentialClientBase, IWebSessionCredentialClient
    {
        private readonly HealthVaultConfiguration _webHealthVaultConfiguration;

        public WebSessionCredentialClient(
            HealthVaultConfiguration configuration,
            IConnectionInternal connection,
            ICertificateInfoProvider certificateInfoProvider)
        {
            Connection = connection;
            CertificateInfoProvider = certificateInfoProvider;
            _webHealthVaultConfiguration = configuration;
        }

        public ICertificateInfoProvider CertificateInfoProvider { get; set; }

        public override void WriteInfoXml(XmlWriter writer)
        {
            writer.WriteStartElement("appserver2");

            string requestXml = GetContentSection();

            // SIG
            writer.WriteStartElement("sig");
            writer.WriteAttributeString("digestMethod", HealthVaultConstants.Cryptography.DigestAlgorithm);
            writer.WriteAttributeString("sigMethod", HealthVaultConstants.Cryptography.SignatureAlgorithmName);
            writer.WriteAttributeString("thumbprint", CertificateInfoProvider.Thumbprint);
            writer.WriteString(SignRequestXml(requestXml));
            writer.WriteEndElement(); // sig

            // CONTENT
            writer.WriteRaw(requestXml);

            writer.WriteEndElement();
        }

        public string SignRequestXml(string requestXml)
        {
            var encoding = new UTF8Encoding();

            var paramBlob = encoding.GetBytes(requestXml);
            var sigBlob = CertificateInfoProvider.PrivateKey.SignData(paramBlob, new HashAlgorithmName(HealthVaultConstants.Cryptography.DigestAlgorithm), RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(sigBlob);
        }

        /// <summary>
        ///     Generate the to-be signed content for the credential.
        /// </summary>
        /// <returns>
        ///     Raw XML representing the ContentSection of the info secttion.
        /// </returns>
        internal string GetContentSection()
        {
            var requestXml = new StringBuilder(2048);
            var settings = SDKHelper.XmlUnicodeWriterSettings;

            using (var writer = XmlWriter.Create(requestXml, settings))
            {
                writer.WriteStartElement("content");

                writer.WriteStartElement("app-id");
                writer.WriteString(_webHealthVaultConfiguration.MasterApplicationId.ToString());
                writer.WriteEndElement();

                writer.WriteElementString("hmac", HealthVaultConstants.Cryptography.HmacAlgorithm);

                writer.WriteStartElement("signing-time");
                writer.WriteValue(DateTime.Now.ToUniversalTime());
                writer.WriteEndElement();

                writer.WriteEndElement(); // content
                writer.Flush();
            }

            return requestXml.ToString();
        }
    }
}