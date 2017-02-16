// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents a digital signature of the HealthRecordItem.
    /// </summary>
    ///
    /// <remarks>
    /// A digital signature contains key information, which may be an X509 certificate,
    /// and a cryptographic hash. These may be used to verify the identity of the signer
    /// and the contents of the HealthRecordItem.
    /// </remarks>
    ///
    public class HealthRecordItemSignature
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemSignature"/> class
        /// using default values.
        /// </summary>
        ///
        public HealthRecordItemSignature()
        {
            _method = HealthRecordItemSignatureMethod.HV2;
            _methodString = c_HVSignatureMethod2Name;
            _algorithmName = RsaSha1W3cAlgorithmName;
        }

        /// <summary>
        /// The type of the certificate used to sign the signature.
        /// </summary>
        ///
        public CertificateType CertType
        {
            get
            {
                if (_signedXml == null)
                {
                    return CertificateType.None;
                }
                else
                {
                    IEnumerator keyInfoClauseEnumerator = _signedXml.KeyInfo.GetEnumerator();
                    if (keyInfoClauseEnumerator != null)
                    {
                        if (keyInfoClauseEnumerator.MoveNext())
                        {
                            KeyInfoX509Data keyInfoX509Data =
                                keyInfoClauseEnumerator.Current as KeyInfoX509Data;
                            if (keyInfoX509Data != null)
                            {
                                return CertificateType.X509Certificate;
                            }
                        }
                    }
                }
                return CertificateType.Unknown;
            }
        }

        /// <summary>
        /// Get's the X509Certificate2 used for the signature.
        /// </summary>
        ///
        /// <remarks>
        /// This method is deprecated. Please use <see cref="X509Certificate2" /> to
        /// access the signing certificate information.
        /// </remarks>
        public T GetCertificate<T>() where T : X509Certificate2
        {
            T cert = X509Certificate2 as T;
            return cert;
        }

        /// <summary>
        /// Get's the X509Certificate2 used for the signature.
        /// </summary>
        ///
        public X509Certificate2 X509Certificate2
        {
            get
            {
                X509Certificate2 cert = null;
                KeyInfoX509Data keyInfoX509Data = null;

                // We only support exactly 1 KeyInfo clause
                if ((_signedXml != null) &&
                    (_signedXml.KeyInfo != null) &&
                    (_signedXml.KeyInfo.Count == 1))
                {
                    IEnumerator keyInfoClauseEnumerator =
                        _signedXml.KeyInfo.GetEnumerator();
                    if (keyInfoClauseEnumerator != null)
                    {
                        if (keyInfoClauseEnumerator.MoveNext())
                        {
                            keyInfoX509Data = keyInfoClauseEnumerator.Current as KeyInfoX509Data;

                            // We only support a single certificate passed in
                            if ((keyInfoX509Data != null) &&
                                (keyInfoX509Data.Certificates.Count == 1))
                            {
                                cert = keyInfoX509Data.Certificates[0] as X509Certificate2;
                            }
                        }
                    }
                }
                return cert;
            }
        }

        /// <summary>
        /// Signs the XML document.
        /// </summary>
        ///
        /// <param name="signingCertificate">
        /// The X509 certificate.
        /// </param>
        ///
        /// <param name="thingDoc">
        /// The XML document to sign.
        /// </param>
        ///
        /// <remarks>
        /// This method uses the .Net SignedXml class to sign the thing. It passes in
        /// an XSLT transform to select the data-xml section the value of the data-other
        /// section.
        /// </remarks>
        ///
        /// <returns>
        /// An XmlElement containing the signature xml in the http://www.w3.org/2000/09/xmldsig
        /// namespace.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The specified argument is null.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The HealthRecordItem is already signed and may only have one signature.
        /// </exception>
        ///
        /// <exception cref="SignatureFailureException">
        /// Signing failed. See the inner exception.
        /// The inner exception may be one of the following:
        /// A <see cref="XmlException"/> is thrown because there is a load or parse error loading
        /// the xsl.
        /// A <see cref="CryptographicException"/> is thrown because the nodelist from the xsl does
        /// not contain an XmlDsigXsltTransform object.
        /// A <see cref="CryptographicException"/> is thrown because signingCertificate.PrivateKey
        /// is not an RSA or DSA key, or is unreadable.
        /// A <see cref="CryptographicException"/> is thrown because signingCertificate.PrivateKey
        /// is not a DSA or RSA object.
        /// </exception>
        ///
        internal XmlElement Sign(
            X509Certificate2 signingCertificate,
            XmlDocument thingDoc)
        {
            Validator.ThrowIfArgumentNull(signingCertificate, "signingCertificate", "SigningCertificateNull");
            Validator.ThrowIfArgumentNull(thingDoc, "thingDoc", "ThingDocNull");
            Validator.ThrowInvalidIf(_signedXml != null, "SignatureOnlyOneAllowed");

            try
            {
                _signedXml = new SignedXml(thingDoc);
                _signedXml.SigningKey = signingCertificate.GetRSAPrivateKey();
                KeyInfo ki = new KeyInfo();
                KeyInfoX509Data kix509d = new KeyInfoX509Data(signingCertificate);
                ki.AddClause(kix509d);
                _signedXml.KeyInfo = ki;

                Reference reference;
                reference = new Reference();
                reference.Uri = String.Empty;

                XmlDsigXsltTransform XsltTransform = CreateXsltTransform(SigningMethod2XSLT);

                // Add the transform to the reference.
                reference.AddTransform(XsltTransform);

                XmlDsigC14NTransform c14NTransform = new XmlDsigC14NTransform();
                reference.AddTransform(c14NTransform);

                _signedXml.AddReference(reference);
                _signedXml.ComputeSignature();

                return _signedXml.GetXml();
            }
            catch (CryptographicException e)
            {
                throw new SignatureFailureException(
                    ResourceRetriever.FormatResourceString("SignatureSigningFailed", e.Message),
                    e);
            }
        }

        /// <summary>
        /// Checks the certificate in the signature of the XML document.
        /// </summary>
        ///
        /// <remarks>
        /// Calls
        /// <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2.Verify"/>.
        /// </remarks>
        ///
        /// <exception cref="CertificateValidationException">
        /// Certificate validation failed.
        /// The InnerException will contain a <see cref="CryptographicException"/> if the
        /// certificate is unreadable.
        /// If the InnerException is <b>null</b> then the Message will contain information about
        /// the certificate and the error.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The certificate could not be validated because the <see cref="HealthRecordItem"/> is
        /// not signed.
        /// </exception>
        ///
        internal void CheckCertificate()
        {
            X509Certificate2 cert = X509Certificate2;

            Validator.ThrowInvalidIfNull(cert, "CertificateNoCertificate");

            try
            {
                bool isValid = cert.Verify();
                if (!isValid)
                {
                    throw new CertificateValidationException(
                        ResourceRetriever.FormatResourceString(
                            "CertificateValidationFailed",
                            this.GetCertificateFailureInfo(cert)));
                }
            }
            catch (CryptographicException e)
            {
                throw new CertificateValidationException(
                    ResourceRetriever.FormatResourceString(
                        "CertificateValidationFailed",
                        e.Message),
                    e);
            }
        }

        /// <summary>
        /// Creates and returns a string with info about why a certificate failed to validate.
        /// </summary>
        ///
        /// <remarks>
        /// Calls <see cref="X509Chain.Build"/> to create and validate a certificate chain.
        /// </remarks>
        ///
        /// <exception ref="CryptographicException">
        /// The certificate is unreadable.
        /// </exception>
        [SecuritySafeCritical]
        private string GetCertificateFailureInfo(X509Certificate2 cert)
        {
            string certificateInfo = string.Empty;
            // Get chain information of the selected certificate.
            X509Chain ch = new X509Chain();
            ch.Build(cert);

            foreach (X509ChainElement element in ch.ChainElements)
            {
                certificateInfo = certificateInfo.Insert(
                    certificateInfo.Length,
                    ResourceRetriever.FormatResourceString(
                    "CertificateIssuer",
                    element.Certificate.Issuer));

                certificateInfo = certificateInfo.Insert(
                    certificateInfo.Length,
                    ResourceRetriever.FormatResourceString(
                        "CertificateExpireDate",
                        element.Certificate.NotAfter));

                certificateInfo = certificateInfo.Insert(
                    certificateInfo.Length,
                    ResourceRetriever.FormatResourceString(
                        "CertificateIsValid",
                        element.Certificate.Verify()));

                for (int index = 0; index < element.ChainElementStatus.Length; index++)
                {
                    object status = element.ChainElementStatus[index].Status;
                    certificateInfo = certificateInfo.Insert(
                        certificateInfo.Length,
                        ResourceRetriever.FormatResourceString(
                            "CertficateStatus",
                            status.ToString()));
                }
            }
            return certificateInfo;
        }

        /// <summary>
        /// Checks the signature of the XML document.
        /// </summary>
        ///
        /// <param name="thingDoc">
        /// The signed XML document.
        /// This is passed in so that the signature validation can take updates into account.
        /// </param>
        ///
        /// <remarks>
        /// Calls the <see cref="CheckSignature"/> method of <see cref="SignedXml"/>.
        /// </remarks>
        ///
        /// <exception cref="SignatureFailureException">
        /// Signature validation failed. See the inner exception.
        /// The inner exception is <see cref="CryptographicException"/>, thrown because of one of:
        /// The SignatureAlgorithm property of the public key in the signature does
        /// not match the SignatureMethod property.
        /// The signature description could not be created.
        /// The hash algorithm could not be created.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The signature could not be validated because the <see cref="HealthRecordItem"/> is not
        /// signed.
        /// </exception>
        ///
        internal bool CheckSignature(XmlDocument thingDoc)
        {
            bool isSignatureValid = false;
            if (_signedXml == null)
            {
                throw new SignatureFailureException(
                    ResourceRetriever.GetResourceString("SignatureNoSignature"));
            }

            if (thingDoc != null)
            {
                // Load the updated doc.
                _signedXml = new SignedXml(thingDoc);

                XmlNamespaceManager nsManager =
                    GetSignatureInfoNamespaceManager(thingDoc.NameTable);

                string sigNodeXPath = null;
                if (_method == HealthRecordItemSignatureMethod.HV2)
                {
                    sigNodeXPath = "thing/signature-info/ds:Signature";
                }
                else if (_method == HealthRecordItemSignatureMethod.HV1)
                {
                    sigNodeXPath = "thing/ds:Signature";
                }

                XmlNode node = thingDoc.SelectSingleNode(sigNodeXPath, nsManager);

                // Load the signature node.
                _signedXml.LoadXml((XmlElement)node);
            }

            try
            {
                isSignatureValid = _signedXml.CheckSignature();
            }
            catch (CryptographicException e)
            {
                throw new SignatureFailureException(
                    ResourceRetriever.FormatResourceString(
                        "SignatureValidationFailed",
                        e.Message),
                    e);
            }

            return isSignatureValid;
        }

        internal void ParseXml(XPathNavigator sigInfoNav, string thingXml)
        {
            XPathNavigator sigMethodNav =
                sigInfoNav.SelectSingleNode("sig-data/hv-signature-method");
            ParseSignatureMethod(sigMethodNav.Value);

            XPathNavigator blobSigInfoNav =
                sigInfoNav.SelectSingleNode("sig-data/blob-signature-info");
            if (blobSigInfoNav != null)
            {
                ParseBlobSignatureInfo(blobSigInfoNav);
            }

            _algorithmName = sigInfoNav.SelectSingleNode("sig-data/algorithm-tag").Value;

            XPathExpression signaturePath = GetSignatureXPathExpression(sigInfoNav);
            XPathNavigator signatureNav = sigInfoNav.SelectSingleNode(signaturePath);

            if (signatureNav != null)
            {
                ParseSignatureXml(sigInfoNav, thingXml);
            }
        }

        private static XPathExpression _signaturePath =
            XPathExpression.Compile("ds:Signature");

        internal static XmlNamespaceManager GetSignatureInfoNamespaceManager(
            XmlNameTable nameTable)
        {
            XmlNamespaceManager sigInfoNamespaceManager =
                new XmlNamespaceManager(nameTable);

            sigInfoNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

            return sigInfoNamespaceManager;
        }

        private static XPathExpression GetSignatureXPathExpression(XPathNavigator sigInfoNav)
        {
            XmlNamespaceManager sigInfoNSManager =
                GetSignatureInfoNamespaceManager(sigInfoNav.NameTable);

            XPathExpression signaturePathClone = null;
            lock (_signaturePath)
            {
                signaturePathClone = _signaturePath.Clone();
            }

            signaturePathClone.SetContext(sigInfoNSManager);

            return signaturePathClone;
        }

        private void ParseSignatureMethod(string sigMethodStr)
        {
            switch (sigMethodStr)
            {
                case c_HVSignatureMethod1Name:
                    _method = HealthRecordItemSignatureMethod.HV1;
                    break;
                case c_HVSignatureMethod2Name:
                    _method = HealthRecordItemSignatureMethod.HV2;
                    break;
                default:
                    _method = HealthRecordItemSignatureMethod.Unknown;
                    break;
            }

            _methodString = sigMethodStr;
        }

        private void ParseBlobSignatureInfo(XPathNavigator blobSigInfoNav)
        {
            XPathNodeIterator blobSigItemIterator = blobSigInfoNav.Select("item");

            foreach (XPathNavigator blobSigItemNav in blobSigItemIterator)
            {
                string name = blobSigItemNav.SelectSingleNode("blob-info/name").Value;
                string contenttype =
                    blobSigItemNav.SelectSingleNode("blob-info/content-type").Value;

                XPathNavigator hashNav = blobSigItemNav.SelectSingleNode("blob-info/hash-info");
                BlobHashInfo hashInfo = new BlobHashInfo();
                hashInfo.Parse(hashNav);

                BlobSignatureItem bsi = new BlobSignatureItem(name, contenttype, hashInfo);
                _blobSignatureItems.Add(bsi);
            }
        }

        /// <summary>
        /// Parse the Signature xml from the thingReader.
        /// </summary>
        ///
        /// <remarks>
        /// Creates an instance <see cref="SignedXml"/> using a document created from thingReader
        /// and the signature section of that document.
        /// </remarks>
        ///
        /// <exception cref="XmlException">
        /// There is a load or parse error in the XML.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <see cref="Signature"/> section of the document was not found.
        /// </exception>
        ///
        /// <exception cref="CryptographicException">
        /// The <see cref="Signature"/> section of the document does not contain a valid
        /// SignatureValue property.
        /// The <see cref="Signature"/>  section of the document does not contain a valid
        /// SignedInfo property.
        /// </exception>
        ///
        internal void ParseSignatureXml(
            XPathNavigator sigInfoNav,
            string thingXml)
        {
            XmlDocument thingDoc = new XmlDocument();
            thingDoc.XmlResolver = null;

            // Format using white spaces.
            thingDoc.PreserveWhitespace = true;
            thingDoc.SafeLoadXml(thingXml);

            _signedXml = new SignedXml(thingDoc);

            XmlNamespaceManager nsManager =
                GetSignatureInfoNamespaceManager(sigInfoNav.NameTable);

            XmlNode node = thingDoc.SelectSingleNode(
                "thing/signature-info/ds:Signature",
                nsManager);

            // Load the signature node.
            _signedXml.LoadXml((XmlElement)node);
        }

        internal void ParseV1SignatureXml(
            string thingString)
        {
            _method = HealthRecordItemSignatureMethod.HV1;
            _algorithmName = RsaSha1W3cAlgorithmName;

            XmlDocument thingDoc = new XmlDocument();
            thingDoc.XmlResolver = null;

            // Format using white spaces.
            thingDoc.PreserveWhitespace = true;
            thingDoc.SafeLoadXml(thingString);

            _signedXml = new SignedXml(thingDoc);

            // Find the "Signature" node and create a new XmlNode object.
            XmlNamespaceManager nsManager = GetSignatureInfoNamespaceManager(thingDoc.NameTable);
            XmlNode node = thingDoc.SelectSingleNode("thing/ds:Signature", nsManager);

            // Load the signature node.
            _signedXml.LoadXml((XmlElement)node);
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("signature-info");
            writer.WriteStartElement("sig-data");
            writer.WriteElementString("hv-signature-method", _methodString);

            if (_blobSignatureItems.Count > 0)
            {
                writer.WriteStartElement("blob-signature-info");

                // order is important!
                for (Int32 i = 0; i < _blobSignatureItems.Count; i++)
                {
                    BlobSignatureItem item = _blobSignatureItems[i];

                    writer.WriteStartElement("item");
                    writer.WriteStartElement("blob-info");
                    writer.WriteElementString("name", item.Name);
                    writer.WriteElementString("content-type", item.ContentType);
                    if (item.HashInfo != null)
                    {
                        item.HashInfo.Write(writer);
                    }
                    writer.WriteEndElement(); // </blob-info>

                    writer.WriteEndElement(); // </item>
                }

                writer.WriteEndElement(); // </blob-signature-info>
            }

            writer.WriteElementString("algorithm-tag", _algorithmName);

            writer.WriteEndElement(); // </sig-data>

            if (_signedXml != null)
            {
                XmlElement xmlElement = _signedXml.GetXml();
                xmlElement.WriteTo(writer);
            }

            writer.WriteEndElement(); // </signature-info>
        }

        /// <summary>
        /// Create the XML that represents the transform.
        /// Copied from the sample in the docs.
        /// </summary>
        ///
        /// <exception cref="XmlException">
        /// There is a load or parse error loading the xsl parameter.
        /// </exception>
        ///
        /// <exception cref="CryptographicException">
        /// The nodelist from the xsl parameter does not contain an XmlDsigXsltTransform object.
        /// </exception>
        ///
        private static XmlDsigXsltTransform CreateXsltTransform(string xsl)
        {
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.SafeLoadXml(xsl);

            XmlDsigXsltTransform xform = new XmlDsigXsltTransform();
            xform.LoadInnerXml(doc.ChildNodes);

            return xform;
        }

        /// <summary>
        /// Gets the HealthVault signature method for this signature.
        /// </summary>
        public HealthRecordItemSignatureMethod Method
        {
            get { return _method; }
        }
        private HealthRecordItemSignatureMethod _method;

        private string _methodString;

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{BlobSignatureItem}" /> objects describing
        /// the blobs that are part of this signature.
        /// </summary>
        public ReadOnlyCollection<BlobSignatureItem> BlobSignatureItems
        {
            get { return new ReadOnlyCollection<BlobSignatureItem>(_blobSignatureItems); }
        }
        private Collection<BlobSignatureItem> _blobSignatureItems =
            new Collection<BlobSignatureItem>();

        internal void AddBlobSignatureInfo(BlobSignatureItem blobSignatureItem)
        {
            _blobSignatureItems.Add(blobSignatureItem);
        }

        /// <summary>
        /// Gets a string identifying the signature algorithm for this signature.
        /// </summary>
        /// <remarks>
        /// This value is currently <see cref="RsaSha1W3cAlgorithmName"/>.
        /// </remarks>
        public string AlgorithmName
        {
            get { return _algorithmName; }
        }
        private string _algorithmName;

        private SignedXml _signedXml;

        /// <summary>
        /// Represents the signature algorithm defined by:
        /// "http://www.w3.org/2000/09/xmldsig#rsa-sha1".
        /// </summary>
        public const string RsaSha1W3cAlgorithmName = "rsa-sha1";

        private const string c_HVSignatureMethod1Name = "HVSignatureMethod1";
        private const string c_HVSignatureMethod2Name = "HVSignatureMethod2";

        private const string XSLString =
            @"<xs:transform xmlns:xs='http://www.w3.org/1999/XSL/Transform' version='1.0'>" +
                @"<xs:template match='thing'>" +
                    @"<xs:copy-of select='data-xml'/>" +
                    @"<xs:value-of select='data-other'/>" +
                @"</xs:template>" +
            @"</xs:transform>";

        private const string SigningMethod2XSLT =
            "<xs:stylesheet xmlns:xs='http://www.w3.org/1999/XSL/Transform' version='1.0'>" +
            "<xs:template match='thing'>" +
                "<hv:signed-thing-data xmlns:hv='urn:com.microsoft.wc.thing.signing.2.xsl'>" +
                    "<xs:copy-of select='data-xml'/>" +
                    "<xs:copy-of select='signature-info/sig-data'/>" +
                "</hv:signed-thing-data>" +
            "</xs:template>" +
            "</xs:stylesheet>";
    }
}
