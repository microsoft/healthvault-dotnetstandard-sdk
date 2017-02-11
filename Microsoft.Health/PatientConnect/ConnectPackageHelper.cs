// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.ItemTypes;

namespace Microsoft.Health.Package
{
    /// <summary>
    /// A utility class for creating HealthVault
    /// password-protected packages and the relevant HealthVault
    /// web-service requests.
    /// </summary>
    internal class ConnectPackageHelper
    {
        public ConnectPackageHelper(
            ConnectPackageCreationParameters creationParameters,
            IEnumerable<HealthRecordItem> packageContents)
        {
            if (creationParameters == null)
            {
                throw new ArgumentNullException("creationParameters");
            }

            Validator.ThrowArgumentExceptionIf(
                packageContents == null || !packageContents.Any(),
                "packageContents",
                "PackageCreateHRIMissingMandatory");

            CreationParameters = creationParameters;
            PackageContents = packageContents;
        }

        public ConnectPackageHelper(
            ConnectPackageCreationParameters creationParameters,
            PasswordProtectedPackage connectPackage,
            IEnumerable<Uri> packageContentsBlobUrls)
        {
            if (creationParameters == null)
            {
                throw new ArgumentNullException("creationParameters");
            }

            Validator.ThrowIfArgumentNull(
                connectPackage,
                "connectPackage",
                "PackageCreatePPPMissingMandatory");

            if (packageContentsBlobUrls == null)
            {
                packageContentsBlobUrls = Enumerable.Empty<Uri>();
            }

            CreationParameters = creationParameters;
            PasswordProtectedPackage = connectPackage;
            PackageContentsBlobUrls = packageContentsBlobUrls;
        }

        protected ConnectPackageCreationParameters CreationParameters { get; private set; }

        protected IEnumerable<HealthRecordItem> PackageContents { get; private set; }

        protected PasswordProtectedPackage PasswordProtectedPackage { get; private set; }

        protected IEnumerable<Uri> PackageContentsBlobUrls { get; private set; }

        /// <summary>
        /// Creates the connect package in HealthVault and returns
        /// the identity code associated with it.
        /// </summary>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// One of the items in PackageContents is signed and contains
        /// streamed blobs. This is not supported.
        /// </exception>
        /// 
        /// <returns>
        /// The identity code for the package.
        /// </returns>
        public string CreateConnectPackage()
        {
            PasswordProtectedPackage packageItem = CreatePasswordProtectedPackageItem();
            IEnumerable<Uri> streamedBlobUrls = GetPackageContentsStreamedBlobUrls();

            string parameters = GetParametersXml(packageItem, streamedBlobUrls);
            HealthServiceRequest request = GetCreateConnectPackageRequest(parameters);

            request.Execute();

            return GetIdentityCodeFromResponse(request.Response);
        }

        /// <summary>
        /// Gets the URLs for all streamed blobs of the package contents. 
        /// </summary>
        ///
        /// <exception cref="NotSupportedException">
        /// One of the items in PackageContents is signed and contains
        /// streamed blobs. This is not supported.
        /// </exception>
        public virtual IEnumerable<Uri> GetPackageContentsStreamedBlobUrls()
        {
            if (PackageContentsBlobUrls != null)
            {
                return PackageContentsBlobUrls;
            }

            var results = new List<Uri>();
            foreach (HealthRecordItem item in PackageContents)
            {
                foreach (Blob blob
                    in item.GetBlobStore(
                        default(HealthRecordAccessor)).Values)
                {
                    if (blob.Url != null)
                    {
                        if (item.HealthRecordItemSignatures.Count > 0)
                        {
                            throw Validator.NotSupportedException("PackageCreateSignedWithStreamedBlobsNotSupported");
                        }

                        results.Add(blob.Url);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Creates and returns a password-protected package item with
        /// the contents encrypted and committed as a blob.
        /// </summary>
        public virtual PasswordProtectedPackage CreatePasswordProtectedPackageItem()
        {
            if (PasswordProtectedPackage != null)
            {
                return PasswordProtectedPackage;
            }

            // Obtain the data to encrypt
            var xmlContents = new StringBuilder();

            Validator.ThrowArgumentExceptionIf(
                PackageContents == null || !PackageContents.Any(),
                "packageContents",
                "PackageCreateHRIMissingMandatory");

            foreach (HealthRecordItem item in PackageContents)
            {
                Validator.ThrowArgumentExceptionIf(
                    item == null,
                    "packageContents",
                    "PackageCreateHRIMissingMandatory");

                xmlContents.Append(item.GetItemXml());
            }

            string dataToEncrypt = xmlContents.ToString();

            // Uses AES256 by default
            var connectPackage =
                new PasswordProtectedPackage(
                    CreationParameters.PasswordProtectAlgorithm,
                    CreationParameters.Salt,
                    CreationParameters.ConnectPackageEncryptionAlgorithm.KeySize);

            // encrypt the item XMLs
            byte[] encryptedData =
                SignAndEncrypt(
                    dataToEncrypt,
                    CreationParameters.ConnectPackageHMACAlgorithm,
                    CreationParameters.ConnectPackageEncryptionAlgorithm);

            // write the encrypted item XMLs as the package blob
            BlobStore store = connectPackage.GetBlobStore(default(HealthRecordAccessor));
            store.WriteInline(String.Empty, "application/octet-stream", encryptedData);

            return connectPackage;
        }

        /// <summary>
        /// Signs and encrypts the package contents.
        /// </summary>
        private static byte[] SignAndEncrypt(
            string dataToEncrypt,
            HMAC hmacAlgorithm,
            SymmetricAlgorithm encryptionAlgorithm)
        {
            byte[] rawOtherData = Encoding.UTF8.GetBytes(dataToEncrypt);

            // Compute the hash
            byte[] hashedData = hmacAlgorithm.ComputeHash(rawOtherData, 0, rawOtherData.Length);

            // Encrypt the data
            byte[] encryptedData = new byte[hashedData.Length + rawOtherData.Length];
            Array.Copy(hashedData, encryptedData, hashedData.Length);
            Array.Copy(rawOtherData, 0, encryptedData, hashedData.Length, rawOtherData.Length);
            using (ICryptoTransform transform = encryptionAlgorithm.CreateEncryptor())
            {
                encryptedData =
                    transform.TransformFinalBlock(
                        encryptedData,
                        0,
                        encryptedData.Length);
            }

            return encryptedData;
        }

        /// <summary>
        /// Gets the XML for a CreateConnectPackage request.
        /// </summary>
        /// <returns></returns>
        private string GetParametersXml(
            PasswordProtectedPackage package,
            IEnumerable<Uri> streamdBlobUrls)
        {
            var xml = new StringBuilder(512);

            var settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(xml, settings))
            {
                WriteBasicParameters(writer);
                WritePackageItem(writer, package);
                WritePackageStreamedBlobRefUrls(writer, streamdBlobUrls);
            }

            return xml.ToString();
        }

        /// <summary>
        /// Writes the basic parameters of the package for the
        /// CreateConnectPackage request.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteBasicParameters(XmlWriter writer)
        {
            if (CreationParameters.IdentityCode != null)
            {
                writer.WriteElementString("identity-code", CreationParameters.IdentityCode);
            }

            writer.WriteElementString("friendly-name", CreationParameters.FriendlyName);
            writer.WriteElementString("question", CreationParameters.SecurityQuestion);
            writer.WriteElementString("external-id", CreationParameters.ApplicationPatientId);
        }

        /// <summary>
        /// Writes the password-protected package item as XML.
        /// </summary>
        protected virtual void WritePackageItem(XmlWriter writer, PasswordProtectedPackage package)
        {
            writer.WriteRaw(package.GetItemXml("package"));
        }

        /// <summary>
        /// Writes the XML for the URLs of the streamed blobs of the package contents.
        /// </summary>
        protected virtual void WritePackageStreamedBlobRefUrls(XmlWriter writer, IEnumerable<Uri> streamdBlobUrls)
        {
            if (streamdBlobUrls.Any())
            {
                writer.WriteStartElement("streamed-package-blobs");

                foreach (Uri blobUrl in streamdBlobUrls)
                {
                    writer.WriteElementString(
                        "blob-in-package-ref-url",
                        blobUrl.OriginalString);
                }

                // </streamed-package-blobs>
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Creates a CreateConnectPackage request with specified parameters.
        /// </summary>
        protected virtual HealthServiceRequest GetCreateConnectPackageRequest(string parameters)
        {
            var request = new HealthServiceRequest(CreationParameters.Connection, "CreateConnectPackage", 2);
            request.Parameters = parameters;

            return request;
        }

        /// <summary>
        /// Parses the CreateConnectPackage response and returns
        /// the identity code.
        /// </summary>
        protected virtual string GetIdentityCodeFromResponse(HealthServiceResponseData response)
        {
            return GetIdentityCodeFromResponse(response, "CreateConnectPackage");
        }

        /// <summary>
        /// Parses the CreateConnectPackage response and returns
        /// the identity code, using the standard response namespace
        /// for the specified method name.
        /// </summary>
        protected static string GetIdentityCodeFromResponse(HealthServiceResponseData response, string methodNsName)
        {
            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    response.InfoNavigator,
                    methodNsName);

            XPathNavigator infoNav = response.InfoNavigator.SelectSingleNode(infoPath);
            return infoNav.SelectSingleNode("identity-code").Value;
        }
    }
}
