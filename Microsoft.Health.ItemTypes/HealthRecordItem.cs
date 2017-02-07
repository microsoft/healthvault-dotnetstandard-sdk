// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.Package;

namespace Microsoft.Health
{
    /// <summary>
    /// Represents a health record item.
    /// </summary>
    /// 
    /// <remarks>
    /// A health record item is a single piece of data in a health record 
    /// that is accessible through the HealthVault service. Examples of health 
    /// record items include a blood pressure measurement, an exercise session, 
    /// or an insurance claim.
    /// <br/><br/>
    /// Health record items are typed and have XML data that adheres to the 
    /// schema for the type.
    /// </remarks>
    /// 
    public class HealthRecordItem
    {
        /// <summary>
        /// Derived classes must call this method when their default 
        /// constructor is called.
        /// </summary>
        /// 
        /// <param name="typeId">
        /// The unique identifier of the type of which the item is an instance.
        /// </param>
        /// 
        protected internal HealthRecordItem(Guid typeId)
        {
            _typeId = typeId;
            _tags = new TagsCollection(this);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItem"/> class 
        /// with the specified type identifier and type-specific data.
        /// </summary>
        /// 
        /// <param name="typeId">
        /// The unique identifier for the item type.
        /// </param>
        /// 
        /// <param name="typeSpecificData">
        /// The type-specific data XML for the item. 
        /// </param>
        /// 
        /// <remarks>
        /// This constructor is reserved for when there are no derived
        /// classes for the item type being created. In most situations, use the
        /// derived type constructor.
        /// </remarks>
        /// 
        public HealthRecordItem(Guid typeId, IXPathNavigable typeSpecificData)
            : this(typeId)
        {
            _typeSpecificData = typeSpecificData;
        }

        /// <summary>
        /// Parses the type-specific XML data for the item.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The type-specific data XML for the item.
        /// </param>
        /// 
        /// <remarks>
        /// Derived classes should override this method and populate their
        /// members with the data from the XML.
        /// <br/><br/>
        /// The default implementation does nothing.
        /// </remarks>
        /// 
        protected virtual void ParseXml(IXPathNavigable typeSpecificXml)
        {
        }

        /// <summary>
        /// Writes the XML for the type-specific data of the item to the
        /// specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter that receives the type-specific data.
        /// </param>
        /// 
        /// <remarks>
        /// Derived classes should override this method and write the type-
        /// specific XML which goes in the data-xml section of the item.
        /// <br/><br/>
        /// The default implementation writes the XML in the 
        /// <see cref="TypeSpecificData"/> property to the specified writer.
        /// </remarks>
        /// 
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteNode(_typeSpecificData.CreateNavigator(), true);
        }

        /// <summary>
        /// The item XML used to generate the signature on this health record item.
        /// </summary>
        private string _signedItemXml;

        /// <summary>
        /// Called after an update to indicate that the instance is in sync with the platform 
        /// instance.
        /// </summary>
        /// 
        internal void ClearDirtyFlags()
        {
            _areFlagsDirty = false;
        }

        /// <summary>
        /// Signs the <see cref="HealthRecordItem"/> with a digital signature.
        /// </summary>
        /// 
        /// <param name="signingCertificate">
        /// An X509 certificate. The private key from the certificate is used to sign the
        /// <see cref="HealthRecordItem"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Creates an instance of <see cref="HealthRecordItemSignature"/> and calls its Sign method.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The specified argument is null.
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The <see cref="HealthRecordItem"/> is already signed and may only have one signature.
        /// </exception>
        ///
        /// <exception cref="XmlException">
        /// There is a load or parse error in the XML.
        /// </exception>
        ///
        /// <exception cref="SignatureFailureException">
        /// Signing failed. See the inner exception.
        /// The inner exception may be one of the following:
        /// An <see cref="XmlException"/> is thrown because there is a load or parse error loading 
        /// the xsl.
        /// A CryptographicException is thrown because the nodelist from the xsl does not contain 
        /// an <see cref="XmlDsigXsltTransform"/> object.
        /// A <see cref="CryptographicException"/> is thrown because signingCertificate.PrivateKey 
        /// is not an RSA or DSA key, or is unreadable.
        /// A <see cref="CryptographicException"/> is thrown because signingCertificate.PrivateKey 
        /// is not a DSA or RSA object.
        /// </exception>
        /// 
        public void Sign(X509Certificate2 signingCertificate)
        {
            XmlDocument thingDoc = new XmlDocument();
            thingDoc.XmlResolver = null;
            HealthRecordItemSignature thingSignature = null;

            Validator.ThrowIfArgumentNull(signingCertificate, "signingCertificate", "SigningCertificateNull");

            // Format using white spaces.
            thingDoc.PreserveWhitespace = true;

            Validator.ThrowInvalidIf(_signatures.Count > 0, "SignatureOnlyOneAllowed");

            thingSignature = new HealthRecordItemSignature();

            if (_blobStore != null && _blobStore.Count > 0)
            {
                foreach (Blob blob in _blobStore.Values)
                {
                    BlobSignatureItem bsi = new BlobSignatureItem(
                            blob.Name,
                            blob.ContentType,
                            blob.HashInfo);

                    thingSignature.AddBlobSignatureInfo(bsi);
                }
            }

            _signatures.Add(thingSignature);

            thingDoc.SafeLoadXml(GetItemXml(_sections | HealthRecordItemSections.Signature));

            XmlElement signature = thingSignature.Sign(signingCertificate, thingDoc);
            XmlNode sigNode = thingDoc.ImportNode(signature, true);
            XmlNode sigInfoNode = thingDoc.SelectSingleNode("thing/signature-info");
            XmlNode sigDataNode = sigInfoNode.FirstChild;
            sigInfoNode.InsertAfter(sigNode, sigDataNode);

            _signatures.Clear();
            _signatures.Add(thingSignature);
            _signedItemXml = thingDoc.OuterXml;
            _sections |= HealthRecordItemSections.Signature;
        }

        /// <summary>
        /// Checks if the health record item's signature is valid.
        /// </summary>
        /// 
        /// <remarks>
        /// Verifies that the signature on the item is valid for the XML representation of the 
        /// item as retrieved from the HealthVault service.
        /// <br/><br/>
        /// This method will always verify against the underlying XML of this item as returned 
        /// from the service, even if local modifications are made to the item. In the case 
        /// of new items that have not yet been created in the HealthVault service, this method 
        /// validates the signature against the XML of the item at the time of signing.
        /// <br /><br />
        /// For more information about XML digital signatures see: 
        /// <see cref="System.Security.Cryptography.Xml"/>. 
        /// </remarks>
        /// 
        /// <returns>
        /// <b>true</b> if the signature is valid against the XML representation of the item 
        /// returned from the service, or for new items, if the signature is valid against
        /// the XML of the item at the time the item was signed. Returns <b>false</b> if the 
        /// signature could not be validated.
        /// </returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The signature could not be validated because the <see cref="HealthRecordItem"/> is not 
        /// signed.
        /// </exception>
        /// 
        /// <exception cref="SignatureFailureException">
        /// Signature validation failed becaue either the 
        /// <see cref="HealthRecordItemSignatureMethod"/> of this item is unknown and cannot be
        /// validated, or the integrity of the signature could not be verified in which case the 
        /// inner exception contains details on the reasons why.
        /// The inner exception is <see cref="CryptographicException"/>, thrown because of one of:
        /// The SignatureAlgorithm property of the public key in the 
        /// signature does not match the SignatureMethod property.
        /// The signature description could not be created.
        /// The hash algorithm could not be created. 
        /// </exception>
        /// 
        [SecurityCritical]
        public bool IsSignatureValid()
        {
            bool returnValue = true;

            Validator.ThrowInvalidIf(_signatures.Count < 1, "SignatureNoSignature");

            HealthRecordItemSignature signature = _signatures[0];

            if (signature.Method == HealthRecordItemSignatureMethod.Unknown)
            {
                throw new SignatureFailureException(
                    ResourceRetriever.FormatResourceString(
                        "SignatureMethodUnsupported",
                        signature.Method));
            }

            XmlDocument thingDoc = new XmlDocument();
            thingDoc.XmlResolver = null;
            thingDoc.PreserveWhitespace = true;            

            string signedThingXml = GetSignedItemXml(signature);

            if (String.IsNullOrEmpty(signedThingXml))
            {
                return false;
            }

            thingDoc.SafeLoadXml(signedThingXml);

            if (!signature.CheckSignature(thingDoc))
            {
                returnValue = false;
            }

            return returnValue;
        }

        private string GetSignedItemXml(HealthRecordItemSignature signature)
        {
            String signedItemXml = null;

            if (signature.Method == HealthRecordItemSignatureMethod.HV1)
            {
                signedItemXml = FetchV1SignedItemXml();
            }
            else if (signature.Method == HealthRecordItemSignatureMethod.HV2)
            {
                if (_signedItemXml != null)
                {
                    signedItemXml = _signedItemXml;
                }
                else
                {
                    signedItemXml = GetItemXml();
                }
            }

            return signedItemXml;
        }

        /// <summary>
        /// Fetches item xml in V1 signature format. 
        /// </summary>
        /// <returns></returns>
        internal string FetchV1SignedItemXml()
        {
            string itemXml;
            if (_signedItemXml != null)
            {
                itemXml = _signedItemXml;
            }
            else
            {
                itemXml = GetItemXml();
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.PreserveWhitespace = true;
            doc.SafeLoadXml(itemXml);

            XPathNavigator thingNav = doc.CreateNavigator();

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartElement("thing");

                XPathNavigator dxNav = thingNav.SelectSingleNode("thing/data-xml");
                if (dxNav != null)
                {
                    dxNav.WriteSubtree(writer);
                }

                OtherItemData otherItemData = null;
                XPathNavigator blobPayloadNav = thingNav.SelectSingleNode("thing/blob-payload");
                if (blobPayloadNav != null)
                {
                    BlobStore blobStore = new BlobStore(this, default(HealthRecordAccessor));
                    blobStore.ParseXml(blobPayloadNav);
                    if (blobStore.Count == 1 && blobStore.ContainsKey(String.Empty))
                    {
                        Blob b = blobStore[String.Empty];
                        otherItemData = GetEncodedBlob(b);
                        otherItemData.WriteXml(writer);
                    }
                }

                XmlNamespaceManager nsManager =
                    HealthRecordItemSignature.GetSignatureInfoNamespaceManager(
                        thingNav.NameTable);

                XPathNavigator sigNav = thingNav.SelectSingleNode(
                    "thing/signature-info/ds:Signature", nsManager);

                if (sigNav != null)
                {
                    sigNav.WriteSubtree(writer);
                }

                writer.WriteEndElement(); // thing

                writer.Flush();
            }

            return sb.ToString();
        }

        private static OtherItemData GetEncodedBlob(Blob b)
        {
            if (string.IsNullOrEmpty(b.ContentEncoding) &&
                    !string.IsNullOrEmpty(b.LegacyContentEncoding))
            {
                // Convert blob data to original encoding.

                byte[] blobData = b.ReadAllBytes();
                byte[] encodedBlobData = BlobEncoder.Encode(blobData, b.LegacyContentEncoding);

                string blobPayload = Encoding.UTF8.GetString(encodedBlobData);
                return new OtherItemData(blobPayload, b.LegacyContentEncoding, b.ContentType);
            }
            else
            {
                string blobPayload = b.ReadAsString();
                return new OtherItemData(blobPayload, b.ContentEncoding, b.ContentType);
            }
        }

        /// <summary>
        /// Checks if the certificates are valid.
        /// </summary>
        /// 
        /// <remarks>
        /// Validates the certificates of each signature on the <see cref="HealthRecordItem"/>.
        /// </remarks>
        /// 
        /// <exception cref="CertificateValidationException">
        /// Certificate validation failed.
        /// There may be an inner exception is <see cref="CryptographicException"/>, thrown because
        /// of:
        /// The certificate is unreadable. 
        /// If there is no inner exception, there will be a string with info about the certificate
        /// and the error.
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The certificate could not be validated because the <see cref="HealthRecordItem"/> is 
        /// not signed.
        /// </exception>
        /// 
        [SecurityCritical]
        public void ValidateCertificate()
        {
            Validator.ThrowInvalidIf(_signatures.Count < 1, "SignatureNoSignature");

            for (int i = 0; i < _signatures.Count; i++)
            {
                _signatures[i].CheckCertificate();
            }
        }

        /// <summary>
        /// Gets the key of the health record item.
        /// </summary>
        /// 
        /// <value>
        /// A globally unique identifier for the item issued when the item is 
        /// created and a globally unique version stamp updated every time 
        /// the item is changed.
        /// </value>
        /// 
        /// <remarks>
        /// This is the only property that
        /// is guaranteed to be available regardless of how 
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> is set.
        /// </remarks>
        /// 
        public HealthRecordItemKey Key
        {
            get { return _thingKey; }
            internal set { _thingKey = value; }
        }
        private HealthRecordItemKey _thingKey;

        /// <summary>
        /// Gets the type identifier for the health record item type.
        /// </summary>
        /// 
        /// <value>
        /// A globally unique identifier for the type of the item.
        /// </value>
        /// 
        /// <remarks> 
        /// The types available can be queried using  
        /// <see 
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinition(Guid,HealthServiceConnection)"/>
        /// .
        /// </remarks>
        /// 
        public Guid TypeId
        {
            get { return _typeId; }
            internal set { _typeId = value; }
        }
        private Guid _typeId;

        /// <summary>
        /// Gets the health record item type name.
        /// </summary>
        /// 
        /// <remarks> 
        /// The types and names of types available can be queried using 
        /// <see 
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinition(Guid,HealthServiceConnection)"/>
        /// .
        /// </remarks>
        /// 
        public string TypeName
        {
            get { return _typeName; }
            internal set { _typeName = value; }

        }
        private string _typeName;

        #region Core information

        /// <summary>
        /// Gets or sets the date and time that the health record item data was taken.
        /// </summary>
        /// 
        /// <remarks>
        /// This might not be the same time that the data was entered
        /// into the system.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Core"/> bit
        /// set.       
        /// </remarks>
        /// 
        public DateTime EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                if (_effectiveDate != value)
                {
                    _effectiveDate = value;
                    _effectiveDateDirty = true;
                }
            }
        }
        private DateTime _effectiveDate = SDKHelper.DateUnspecified;
        private bool _effectiveDateDirty = false;

        /// <summary>
        /// Gets the state of the <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Core"/> bit
        /// set.       
        /// </remarks>
        /// 
        public HealthRecordItemState State
        {
            get { return _state; }
            internal set { _state = value; }
        }
        private HealthRecordItemState _state = HealthRecordItemState.Active;

        /// <summary>
        /// Gets the <see cref="HealthRecordItem"/> flags.
        /// </summary>
        /// 
        /// <remarks>
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Core"/> bit
        /// set.       
        /// </remarks>
        /// 
        public HealthRecordItemFlags Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                if (_flags != value)
                {
                    _areFlagsDirty = true;
                    _flags = value;
                }
            }
        }
        private HealthRecordItemFlags _flags;
        private bool _areFlagsDirty;

        /// <summary>
        /// Gets or sets the value indicating if the <see cref="HealthRecordItem"/> is private.
        /// </summary>
        /// 
        /// <remarks>
        /// Private items are accessible only by custodians.
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Core"/> bit
        /// set.       
        /// </remarks>
        /// 
        public bool IsPersonal
        {
            get { return IsFlagSet(HealthRecordItemFlags.Personal); }
            set
            {
                if (value)
                {
                    SetFlag(HealthRecordItemFlags.Personal);
                }
                else
                {
                    ClearFlag(HealthRecordItemFlags.Personal);
                }
            }
        }

        /// <summary>
        /// Gets the value indicating if the <see cref="HealthRecordItem"/> is down-versioned.
        /// </summary>
        /// 
        /// <remarks>
        /// If this value is true then an attempt to update the <see cref="HealthRecordItem"/>
        /// will fail.
        /// </remarks>
        /// 
        public bool IsDownVersioned
        {
            get { return IsFlagSet(HealthRecordItemFlags.DownVersioned); }
        }

        /// <summary>
        /// Gets the value indicating if the <see cref="HealthRecordItem"/> is up-versioned.
        /// </summary>
        /// 
        /// <remarks>
        /// If this value is true then an application should get explicit permission to update the
        /// instance from the user. This will cause the stored instance to be converted to the 
        /// type version which may cause data loss in some cases.
        /// </remarks>
        /// 
        public bool IsUpVersioned
        {
            get { return IsFlagSet(HealthRecordItemFlags.UpVersioned); }
        }

        /// <summary>
        /// Gets or sets the date when HealthRecordItem is not relevant.
        /// </summary>
        /// <remarks>
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Core"/> bit
        /// set.
        /// </remarks>
        public DateTime UpdatedEndDate
        {
            get
            {
                return _updatedEndDate;
            }
            set
            {
                if (_updatedEndDate != value)
                {
                    _updatedEndDate = value;
                    _updatedEndDateDirty = true;
                }
            }
        }

        private DateTime _updatedEndDate = DateTime.MaxValue;
        private bool _updatedEndDateDirty = false;

        #endregion Core information

        #region Audit information

        /// <summary>
        /// Gets the audit information associated with the last update of
        /// this health record item.
        /// </summary>
        /// 
        /// <remarks> 
        /// It is the responsibility of the application to convert the audit time
        /// to local time if necessary.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Audits"/> bit
        /// set.
        /// </remarks>
        /// 
        public HealthServiceAudit LastUpdated
        {
            get { return _lastUpdated; }
            internal set { _lastUpdated = value; }
        }
        private HealthServiceAudit _lastUpdated;

        /// <summary>
        /// Gets the audit information associated with the creation of
        /// this health record item.
        /// </summary>
        /// 
        /// <remarks> 
        /// It is the responsibility of the application to convert the audit time
        /// to local time if necessary.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Audits"/> bit
        /// set.
        /// </remarks>
        /// 
        public HealthServiceAudit Created
        {
            get { return _created; }
            private set { _created = value; }
        }
        private HealthServiceAudit _created;

        #endregion Audit information

        #region EffectivePermissions information

        /// <summary>
        /// Gets the effective permissions on the item granted to the person 
        /// retrieving the <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <value>
        /// If the permissions are unknown, the value is <b>null</b>; otherwise
        /// the appropriate permissions are returned.
        /// </value>
        /// 
        /// <remarks> 
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.EffectivePermissions"/> 
        /// bit set.
        /// </remarks>
        /// 
        public HealthRecordItemPermissions? EffectivePermissions
        {
            get { return _permissions; }
            internal set { _permissions = value; }
        }
        private HealthRecordItemPermissions? _permissions;

        /// <summary>
        /// Gets a value indicating whether the <see cref="HealthRecordItem"/> 
        /// is immutable.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the <see cref="HealthRecordItem"/> is immutable; otherwise,
        /// <b>false</b>.
        /// </value>
        /// <remarks> 
        /// This data value is only available when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.EffectivePermissions"/> 
        /// bit set. Returns true if either Thing Type is immutable or Thing instance is read-only.
        /// </remarks>
        /// 
        public bool IsImmutable
        {
            get { return _isImmutable; }
            internal set { _isImmutable = value; }
        }
        private bool _isImmutable;

        /// <summary>
        /// Gets a value indicating whether <see cref="HealthRecordItem"/> is read-only.
        /// </summary>
        /// <value>
        /// <b>true</b> if the <see cref="HealthRecordItem"/> is read-only; otherwise,
        /// <b>false</b>.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// Cannot change read-only flag to false if it is already true.
        /// </exception>
        /// <remarks>
        /// Returns true if either Thing Type is immutable or Thing instance is read-only 
        /// but sets only the instance immutability.
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                return _isImmutable || IsFlagSet(HealthRecordItemFlags.ReadOnly);
            }
            set
            {
                Validator.ThrowInvalidIf(
                    !value
                    && (IsFlagSet(HealthRecordItemFlags.ReadOnly) || _isImmutable),
                    "CannotChangeImmutableFlag");

                if (value)
                {
                    SetFlag(HealthRecordItemFlags.ReadOnly);
                }
            }
        }

        #endregion EffectivePermissions information

        #region Xml data information

        /// <summary>
        /// Gets or sets the XML representation of the type-specific data for the 
        /// <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <remarks> 
        /// This data might contain data from other applications that also work
        /// with the same type of data. Take care not to overwrite
        /// existing data when making updates. Do not remove or manipulate
        /// elements in the XML that you do not understand.
        /// <br/><br/>
        /// If this property is set on a type derived from <see cref="HealthRecordItem"/> the data
        /// is not parsed into the object model so properties of the class may still show old data.
        /// <br/><br/>
        /// This data value is only available to get when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Xml"/> bit
        /// set.
        /// </remarks>
        /// 
        public IXPathNavigable TypeSpecificData
        {
            get { return _typeSpecificData; }
            set { _typeSpecificData = value; }
        }
        private IXPathNavigable _typeSpecificData;

        /// <summary>
        /// Gets the transformed XML data of the <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <remarks> 
        /// Transformed XML data is provided if you so specify in 
        /// <see cref="HealthRecordSearcher"/>. The dictionary values are 
        /// indexed by the name of the transform that was specified when the 
        /// search was performed.
        /// <br/><br/>
        /// This data value is only available to get when the
        /// <see cref="Microsoft.Health.HealthRecordItem.Sections"/> show the
        /// <see cref="Microsoft.Health.HealthRecordItemSections.Xml"/> bit
        /// set though it will not contain data unless a transform was 
        /// specified when getting the item.
        /// </remarks>
        /// 
        public IDictionary<string, XmlDocument> TransformedXmlData
        {
            get { return _transformedData; }
        }
        private Dictionary<string, XmlDocument> _transformedData =
            new Dictionary<string, XmlDocument>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the common data for the <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CommonItemData"/> for the 
        /// <see cref="HealthRecordItem"/>.
        /// </value>
        /// 
        /// <remarks>
        /// The common data includes notes, source, and extensions.
        /// </remarks>
        /// 
        public CommonItemData CommonData
        {
            get { return _common; }
            internal set { _common = value; }
        }
        private CommonItemData _common = new CommonItemData();

        #endregion Xml data information

        #region Blobs

        /// <summary>
        /// Gets the BLOB store for this health record item.
        /// </summary>
        /// 
        /// <param name="record">
        /// The <see cref="HealthRecordAccessor"/> that any BLOB data will be written to. This parameter
        /// may be <b>null</b> if only read access is required or the BLOB data to be written is 
        /// for a <see cref="Package.ConnectPackage"/>.
        /// </param>
        /// 
        /// <remarks>
        /// This method replaces the previous OtherData property. All binary data is now created,
        /// updated, and retrieved through <see cref="BlobStore"/> instance associated with the
        /// <see cref="HealthRecordItem"/>.<br/>
        /// <br/>
        /// GetBlobStore will return an empty store on an existing <see cref="HealthRecordItem"/>
        /// if <see cref="HealthRecordItemSections.BlobPayload"/> is not specified when retrieving
        /// the item. In this case it is possible to overwrite or remove existing Blobs in the
        /// <see cref="HealthRecordItem"/> instance stored in HealthVault by using the same name
        /// as the existing Blob. It is recommended that if you are going to be manipulating 
        /// Blobs in the BlobStore, that you specify
        /// <see cref="HealthRecordItemSections.BlobPayload"/> when retrieving the item.
        /// </remarks>
        /// 
        /// <returns>
        /// A <see cref="BlobStore"/> instance related to this health record item.
        /// </returns>
        /// 
        public BlobStore GetBlobStore(HealthRecordAccessor record)
        {
            if (_blobStore == null)
            {
                _blobStore = new BlobStore(this, record);
            }
            else
            {
                _blobStore.Record = record;
            }
            return _blobStore;
        }

        /// <summary>
        /// Gets the BLOB store for this health record item.
        /// </summary>
        /// 
        /// <param name="connectPackageParameters">
        /// The <see cref="ConnectPackageCreationParameters"/> that define 
        /// the <see cref="Package.ConnectPackage"/> to which any BLOB data will be written to.
        /// </param>
        /// 
        /// <remarks>
        /// This method replaces the previous OtherData property. All binary data is now created,
        /// updated, and retrieved through <see cref="BlobStore"/> instance associated with the
        /// <see cref="HealthRecordItem"/>.<br/>
        /// <br/>
        /// GetBlobStore will return an empty store on an existing <see cref="HealthRecordItem"/>
        /// if <see cref="HealthRecordItemSections.BlobPayload"/> is not specified when retrieving
        /// the item. In this case it is possible to overwrite or remove existing Blobs in the
        /// <see cref="HealthRecordItem"/> instance stored in HealthVault by using the same name
        /// as the existing Blob. It is recommended that if you are going to be manipulating 
        /// Blobs in the BlobStore, that you specify
        /// <see cref="HealthRecordItemSections.BlobPayload"/> when retrieving the item.
        /// </remarks>
        /// 
        /// <returns>
        /// A <see cref="BlobStore"/> instance related to this health record item.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connectPackageParameters"/> is <b>null</b>.
        /// </exception>
        /// 
        public BlobStore GetBlobStore(ConnectPackageCreationParameters connectPackageParameters)
        {
            if (_blobStore == null)
            {
                _blobStore = new BlobStore(this, connectPackageParameters);
            }
            else
            {
                _blobStore.ConnectPackageParameters = connectPackageParameters;
            }
            return _blobStore;
        }
        private BlobStore _blobStore;

        #endregion Blobs

        #region Tags

        /// <summary>
        /// Gets the list of tags on the <see cref="HealthRecordItem"/>.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the tag list.
        /// </value>
        /// 
        public Collection<string> Tags
        {
            get { return _tags; }
        }
        private TagsCollection _tags;

        private class TagsCollection : Collection<string>
        {
            internal TagsCollection(HealthRecordItem item)
            {
                _item = item;
            }

            internal TagsCollection(HealthRecordItem item, IList<string> list)
                : base(list)
            {
                _item = item;
            }
            private HealthRecordItem _item;

            protected override void ClearItems()
            {
                base.ClearItems();
                _item.Sections |= HealthRecordItemSections.Tags;
            }

            protected override void InsertItem(int index, string item)
            {
                base.InsertItem(index, item);
                _item.Sections |= HealthRecordItemSections.Tags;
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                _item.Sections |= HealthRecordItemSections.Tags;
            }

            protected override void SetItem(int index, string item)
            {
                base.SetItem(index, item);
                _item.Sections |= HealthRecordItemSections.Tags;
            }
        }

        #endregion Tags

        #region Signatures

        /// <summary>
        /// Gets the signatures for the <see cref="HealthRecordItem"/>. 
        /// </summary>
        /// 
        /// <value>
        /// An collection of <see cref="HealthRecordItemSignature"/>.
        /// </value>
        /// 
        public Collection<HealthRecordItemSignature> HealthRecordItemSignatures
        {
            get { return _signatures; }
        }
        private Collection<HealthRecordItemSignature> _signatures = new Collection<HealthRecordItemSignature>();

        #endregion Signatures

        /// <summary>
        /// Gets the data sections that this HealthRecordItem represents.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="HealthRecordItemSections"/>.
        /// </value>
        /// 
        public HealthRecordItemSections Sections
        {
            get { return _sections; }
            internal set { _sections = value; }
        }
        private HealthRecordItemSections _sections =
            HealthRecordItemSections.Core |
            HealthRecordItemSections.Xml;

        /// <summary>
        /// Gets the size of the health record item which will be added to the quota used in the
        /// person's health record.
        /// </summary>
        /// 
        /// <remarks>
        /// This size may be used to determine if there is sufficient room left in the person's health
        /// record. It is recommended that this be used only for large items that may cause the
        /// person to exceed their quota. This measurement is an approximation. The HealthVault
        /// service may evaluate the item to have slightly more or fewer bytes depending on how the
        /// white space is transformed between the client and server. The person's health record
        /// quota usage may also change on the HealthVault service due to data being added by other
        /// applications.
        /// </remarks>
        /// 
        /// <returns>
        /// The size in bytes of the health record item.
        /// </returns>
        /// 
        public int GetSizeInBytes()
        {
            StringBuilder thingXml = new StringBuilder(512);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(thingXml, settings))
            {
                AddXmlPutThingsRequestParameters(writer);
                AddBlobPayloadPutThingsRequestParameters(writer);
                AddTagsPutThingsRequestParameters(writer);
            }

            Byte[] bytes = Encoding.UTF8.GetBytes(thingXml.ToString());
            return bytes.Length;
        }

        /// <summary>
        /// Gets the XML representation of the health record item.
        /// </summary>
        /// 
        /// <returns>
        /// A string containing the XML representation of the item.
        /// </returns>
        /// 
        public string GetItemXml()
        {
            return GetItemXml("thing");
        }

        /// <summary>
        /// Gets the XML representation of the health record item.
        /// </summary>
        /// 
        /// <param name="sections">
        /// The sections of the item to write to the XML.
        /// </param>
        /// 
        /// <returns>
        /// A string containing the XML representation of the item wrapped by a "thing" element. 
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="sections"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public string GetItemXml(HealthRecordItemSections sections)
        {
            return GetItemXml(sections, "thing");
        }

        /// <summary>
        /// Gets the XML representation of the health record item.
        /// </summary>
        /// 
        /// <param name="elementName">
        /// The element that will wrap the thing's contents.
        /// </param>
        /// 
        /// <returns>
        /// A string containing the XML representation of the item wrapped by the specified 
        /// <paramref name="elementName"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="elementName"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public string GetItemXml(string elementName)
        {
            HealthRecordItemSections sections = _sections;

            // Add the tags section if there are tags present
            if (_tags != null && _tags.Count > 0)
            {
                sections |= HealthRecordItemSections.Tags;
            }
            return GetItemXml(sections, elementName);
        }

        /// <summary>
        /// Gets the XML representation of the health record item.
        /// </summary>
        /// 
        /// <param name="sections">
        /// The sections of the item to write to the XML.
        /// </param>
        /// 
        /// <param name="elementName">
        /// The element that will wrap the thing's contents.
        /// </param>
        /// 
        /// <returns>
        /// A string containing the XML representation of the item wrapped by the specified 
        /// <paramref name="elementName"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="elementName"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public string GetItemXml(HealthRecordItemSections sections, string elementName)
        {
            Validator.ThrowIfStringNullOrEmpty(elementName, "elementName");

            StringBuilder thingXml = new StringBuilder(256);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(thingXml, settings))
            {
                WriteItemXml(writer, sections, true, elementName);
            }
            return thingXml.ToString();
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter)
        {
            return WriteItemXml(infoXmlWriter, true);
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, HealthRecordItemSections sections)
        {
            return WriteItemXml(infoXmlWriter, sections, true, "thing");
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, string elementName)
        {
            return WriteItemXml(infoXmlWriter, true, elementName);
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, bool writeAllCore)
        {
            return WriteItemXml(infoXmlWriter, writeAllCore, "thing");
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, bool writeAllCore, string elementName)
        {
            HealthRecordItemSections sections = _sections & ~HealthRecordItemSections.Audits;

            // Add the tags section if there are tags present
            if (_tags != null && _tags.Count > 0)
            {
                sections |= HealthRecordItemSections.Tags;
            }

            return WriteItemXml(
                infoXmlWriter,
                sections,
                writeAllCore,
                elementName);
        }

        internal bool WriteItemXml(
            XmlWriter infoXmlWriter,
            HealthRecordItemSections sections,
            bool writeAllCore,
            string elementName)
        {
            bool updateRequired = false;

            // First add the <__elementName__> tag  
            infoXmlWriter.WriteStartElement(elementName);

            // Add the <core> element and children

            updateRequired =
                AddCorePutThingsRequestParameters(infoXmlWriter, writeAllCore, sections);

            if ((sections & HealthRecordItemSections.Audits) != 0)
            {
                AddAuditThingsRequestParameters(infoXmlWriter);
            }

            if ((sections & HealthRecordItemSections.Xml) != 0)
            {
                AddXmlPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & HealthRecordItemSections.BlobPayload) != 0)
            {
                AddBlobPayloadPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & HealthRecordItemSections.Tags) != 0)
            {
                AddTagsPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & HealthRecordItemSections.Signature) != 0)
            {
                AddSignaturesPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & HealthRecordItemSections.Core) == HealthRecordItemSections.Core)
            {
                updateRequired |= AddUpdatedEndDate(infoXmlWriter, writeAllCore);
            }

            // Close the <__elementName__> tag
            infoXmlWriter.WriteEndElement();

            return updateRequired;
        }

        /// <summary>
        /// Gets the XML representation of the item for serialization.
        /// </summary>
        /// 
        /// <remarks>
        /// There are two ways to obtain an XML representation of the item.
        /// GetItemXml() returns only the XML for the type-specific information of the item.
        /// Serialize() returns the full XML.
        /// 
        /// Use GetItemXml() if you want to save the XML representation to pass to the HealthVault platform for a 
        /// new or update operation.
        /// Use Serialize if you want to serialize and deserialize the item and have the deserialized item be
        /// identical to the serialized one.
        /// </remarks>
        /// 
        /// <returns>
        /// A string containing the XML representation of the item.
        /// </returns>
        /// 
        public string Serialize()
        {
            if (_fetchedXml == null)
            {
                return GetItemXml();
            }
            else
            {
                // replace the data-xml node from the original xml with the data-xml node that contains the current
                // state of the object...
                XmlDocument fetchedDocument = new XmlDocument();
                fetchedDocument.XmlResolver = null;
                fetchedDocument.SafeLoadXml(_fetchedXml);

                XmlDocument newDocument = new XmlDocument();
                newDocument.XmlResolver = null;
                newDocument.SafeLoadXml(GetItemXml());

                XmlNode fetchedDataNode = fetchedDocument.SelectSingleNode("thing/data-xml");
                XmlNode newDataNode = newDocument.SelectSingleNode("thing/data-xml");

                fetchedDataNode.InnerXml = newDataNode.InnerXml;

                return fetchedDocument.OuterXml;
            }
        }

        // The original XML that was fetched by the thing.
        private string _fetchedXml;

        /// <summary>
        /// Create a <see cref="HealthRecordItem"/> instance from the item XML. 
        /// </summary>
        /// 
        /// <remarks>
        /// This method is identical to calling <see cref="ItemTypeManager.DeserializeItem(string)"/>.
        /// 
        /// The item XML should come from a previous call to <see cref="HealthRecordItem.Serialize"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// A instance of the <see cref="HealthRecordItem"/> class.
        /// </returns>
        /// 
        public static HealthRecordItem Deserialize(string itemXml)
        {
            return ItemTypeManager.DeserializeItem(itemXml);
        }

        #region private helpers

        /// <summary>
        /// Adds the XML data section to the thing XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to use to write the XML data section to the thing XML.
        /// </param>
        /// 
        private void AddXmlPutThingsRequestParameters(XmlWriter writer)
        {
            // <data-xml>
            writer.WriteStartElement("data-xml");

            WriteXml(writer);
            CommonData.WriteXml(writer);

            // </data-xml>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the BLOB payload section to the thing XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to use to write the BLOB payload section to the thing XML.
        /// </param>
        /// 
        private void AddBlobPayloadPutThingsRequestParameters(
            XmlWriter writer)
        {
            if (_blobStore != null && (_blobStore.Count > 0 || _blobStore.RemovedBlobs.Count > 0))
            {
                _blobStore.WriteXml("blob-payload", writer);
            }
        }

        /// <summary>
        /// Adds the signature section to the thing XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to use to write the signature section to the thing XML.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Signing failed. The thing was already signed and another signature is not allowed.
        /// </exception>
        /// 
        private void AddSignaturesPutThingsRequestParameters(
            XmlWriter writer)
        {
            Validator.ThrowInvalidIf(_signatures.Count > 1, "SignatureOnlyOneAllowed");

            if (_signatures.Count > 0)
            {
                _signatures[0].WriteXml(writer);
            }
        }

        /// <summary>
        /// Adds the tags section to the thing XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to use to write the tags section to the thing XML.
        /// </param>
        /// 
        private void AddTagsPutThingsRequestParameters(
            XmlWriter writer)
        {
            if (_tags != null && _tags.Count != 0)
            {
                StringBuilder tagsString = new StringBuilder(512);
                tagsString.Append(_tags[0]);
                for (int i = 1; i < _tags.Count; i++)
                {
                    tagsString.Append(",");
                    tagsString.Append(_tags[i]);
                }
                writer.WriteElementString("tags", tagsString.ToString());
            }
        }

        /// <summary>
        /// Adds the XML representing the core section to the specified
        /// XML writer.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to add the core section's element to.
        /// </param>
        /// 
        /// <param name="writeAllCore">
        /// If true, thing-state and eff-date elements will be written.
        /// </param>
        /// 
        /// <param name="sections">
        /// The sections to be written to the XML.
        /// </param>
        /// 
        /// <returns>
        /// True if the system sets or effective date needs to be updated.
        /// </returns>
        /// 
        private bool AddCorePutThingsRequestParameters(
            XmlWriter writer,
            bool writeAllCore,
            HealthRecordItemSections sections)
        {
            bool updateRequired = false;

            if (Key != null)
            {
                // Since it's not a new thing we need to specify the ID
                writer.WriteStartElement("thing-id");
                writer.WriteAttributeString("version-stamp", Key.VersionStamp.ToString());
                writer.WriteValue(Key.Id.ToString());
                writer.WriteEndElement();
            }

            // Write the type-id element
            writer.WriteStartElement("type-id");

            if (!String.IsNullOrEmpty(TypeName))
            {
                writer.WriteAttributeString("name", TypeName);
            }
            writer.WriteValue(TypeId.ToString());
            writer.WriteEndElement();

            if ((sections & HealthRecordItemSections.Core) != 0 && writeAllCore)
            {
                updateRequired = true;
                writer.WriteElementString("thing-state", State.ToString());

                writer.WriteElementString("flags", ((UInt32)Flags).ToString());
                if (_areFlagsDirty)
                {
                    updateRequired = true;
                }
            }
            else
            {
                if (_areFlagsDirty)
                {
                    writer.WriteElementString("flags", ((UInt32)Flags).ToString());
                    updateRequired = true;
                }
            }

            if ((sections & HealthRecordItemSections.Core) != 0 &&
                EffectiveDate != SDKHelper.DateUnspecified && (_effectiveDateDirty || writeAllCore))
            {
                // <eff-date>
                writer.WriteStartElement("eff-date");
                writer.WriteValue(SDKHelper.XmlFromDateTime(EffectiveDate));
                writer.WriteEndElement();
                updateRequired = true;
            }

            return updateRequired;
        }

        /// <summary>
        /// Adds the XML representing the audits section to the specified
        /// XML writer.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to add the audits section's element to.
        /// </param>
        /// 
        private void AddAuditThingsRequestParameters(XmlWriter writer)
        {
            if (Created != null)
            {
                writer.WriteStartElement("created");
                Created.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (LastUpdated != null)
            {
                writer.WriteStartElement("updated");
                LastUpdated.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds the Updated End Date to the thing XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XML writer to use to write the XML data section to the thing XML.
        /// </param>
        /// <param name="writeAllCore">
        /// If true, updated-end-date element will also be written.
        /// </param>
        private bool AddUpdatedEndDate(XmlWriter writer, bool writeAllCore)
        {
            bool updateRequired = false;

            if (_updatedEndDateDirty || (writeAllCore && UpdatedEndDate != DateTime.MaxValue))
            {
                writer.WriteStartElement("updated-end-date");
                writer.WriteValue(SDKHelper.XmlFromDateTime(UpdatedEndDate));
                writer.WriteEndElement();
                updateRequired = true;
            }

            return updateRequired;
        }

        #region Parsing the XML

        /// <summary>
        /// Populates the HealthRecordItem elements with data from the XML.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The XML from which the health record item will be constructed.
        /// </param>
        /// 
        /// <param name="thingXml">
        /// The XML from which the health record item will be constructed.
        /// </param>
        /// 
        /// <remarks>
        /// Most of the methods called use an XmlNavigator. However AddSignatureSectionValues
        /// requires an XmlReader, because it requires the exact xml passed in, and XmlNavigator
        /// can change the white space.
        /// </remarks>
        /// 
        /// <exception cref="HealthRecordItemDeserializationException">
        /// The derived type's <see cref="ParseXml(IXPathNavigable)"/> 
        /// method throws an exception when called. The inner exception 
        /// will be the exception thrown by the method.
        /// </exception>
        /// 
        internal void ParseXml(XPathNavigator thingNavigator, string thingXml)
        {
            _fetchedXml = thingXml;

            _sections = HealthRecordItemSections.None;

            XPathNavigator thingIdNav = thingNavigator.SelectSingleNode("thing-id");
            if (thingIdNav != null)
            {
                Guid thingId = new Guid(thingIdNav.Value);

                string versionStamp = thingIdNav.GetAttribute("version-stamp", String.Empty);

                if (!String.IsNullOrEmpty(versionStamp))
                {
                    _thingKey = new HealthRecordItemKey(thingId, new Guid(versionStamp));
                }
                else
                {
                    _thingKey = new HealthRecordItemKey(thingId);
                }
            }

            XPathNavigator thingTypeNavigator = thingNavigator.SelectSingleNode("type-id");

            if (thingTypeNavigator != null)
            {
                string typeId = thingTypeNavigator.Value;
                if (!String.IsNullOrEmpty(typeId))
                {
                    _typeId = new Guid(typeId);
                }
                _typeName = thingTypeNavigator.GetAttribute("name", String.Empty);
            }

            AddCoreSectionValues(thingNavigator);
            AddAuditsSectionValues(thingNavigator);
            AddEffectivePermissionsSectionValues(thingNavigator);
            AddBlobPayloadSectionValues(thingNavigator);

            // Do the data-xml section last so that the type specific 
            // extensions have access to all the data, and so that failures
            // in the derived type can be handled more cleanly.
            AddXmlSectionValues(thingNavigator);

            AddTagsSectionValues(thingNavigator);
            AddSignatureSectionValues(thingNavigator, thingXml);
        }

        /// <summary>
        /// Adds the values from the core section of the health record item to 
        /// the specified HealthRecordItem and updates the Sections 
        /// appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "core".
        /// </param>
        /// 
        private void AddCoreSectionValues(XPathNavigator thingNavigator)
        {
            XPathNavigator effectiveDateNav =
                thingNavigator.SelectSingleNode("eff-date");

            if (effectiveDateNav != null)
            {
                _sections |= HealthRecordItemSections.Core;
                _effectiveDate = effectiveDateNav.ValueAsDateTime;
            }

            XPathNavigator stateNav =
                thingNavigator.SelectSingleNode("thing-state");
            if (stateNav != null)
            {
                _sections |= HealthRecordItemSections.Core;

                try
                {
                    _state =
                        (HealthRecordItemState)Enum.Parse(
                            typeof(HealthRecordItemState), stateNav.Value);
                }
                catch (ArgumentException)
                {
                    _state = HealthRecordItemState.Unknown;
                }
            }

            XPathNavigator flagsNav = thingNavigator.SelectSingleNode("flags");
            if (flagsNav != null)
            {
                _sections |= HealthRecordItemSections.Core;

                _flags = (HealthRecordItemFlags)Convert.ToInt64(flagsNav.Value);
            }

            XPathNavigator updatedEndDateNav = thingNavigator.SelectSingleNode("updated-end-date");
            if (updatedEndDateNav != null)
            {
                _sections |= HealthRecordItemSections.Core;
                _updatedEndDate = updatedEndDateNav.ValueAsDateTime;
            }
        }

        /// <summary>
        /// Adds the values from the created and updated sections of the 
        /// health record item to the specified HealthRecordItem and updates 
        /// the Sections appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "created" and "updated".
        /// </param>
        /// 
        private void AddAuditsSectionValues(XPathNavigator thingNavigator)
        {
            // Check the "updated" group
            XPathNavigator updatedNav =
                thingNavigator.SelectSingleNode("updated");
            if (updatedNav != null)
            {
                _lastUpdated = new HealthServiceAudit();
                _lastUpdated.ParseXml(updatedNav);

                // Now update the sections appropriately
                _sections |= HealthRecordItemSections.Audits;
            }

            // Check the "created" group
            XPathNavigator createdNav =
                thingNavigator.SelectSingleNode("created");
            if (createdNav != null)
            {
                _created = new HealthServiceAudit();
                _created.ParseXml(createdNav);

                // Now update the sections appropriately
                _sections |= HealthRecordItemSections.Audits;
            }
        }

        /// <summary>
        /// Adds the values from the xml section of the health record item to 
        /// the specified HealthRecordItem and updates the Sections 
        /// appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "data-xml".
        /// </param>
        /// 
        /// <exception cref="HealthRecordItemDeserializationException">
        /// If derived type's <see cref="ParseXml(IXPathNavigable)"/> throws 
        /// an exception when called. The inner exception will be the 
        /// exception thrown by the method.
        /// </exception>
        /// 
        private void AddXmlSectionValues(XPathNavigator thingNavigator)
        {
            // Check for the "data-xml" data-group

            XPathNodeIterator dataXmlIterator =
                thingNavigator.Select("data-xml");

            foreach (XPathNavigator dataXml in dataXmlIterator)
            {
                // Now update the sections appropriately
                _sections |= HealthRecordItemSections.Xml;

                string transformName =
                    dataXml.GetAttribute("transform", String.Empty);

                if (String.IsNullOrEmpty(transformName))
                {
                    XPathNavigator commonNav =
                        dataXml.SelectSingleNode("common");
                    if (commonNav != null)
                    {
                        // Parse out the common section
                        _common.ParseXml(commonNav);
                    }

                    // The first child is always the type specific data 
                    // element, of which there is only one.
                    dataXml.MoveToFollowing(XPathNodeType.Element);

                    // I am making a document out the type specific data
                    // so that the derived classes can't muck with the
                    // rest of the thing data.
                    XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
                    settings.IgnoreWhitespace = false;

                    using (XmlReader reader = SDKHelper.GetXmlReaderForXml(dataXml.OuterXml, settings))
                    {
                        _typeSpecificData = new XPathDocument(reader, XmlSpace.Preserve);

                        try
                        {
                            ParseXml(_typeSpecificData);
                        }
                        catch (Exception e) // third-party call-out
                        {
                            throw new HealthRecordItemDeserializationException(
                                ResourceRetriever.GetResourceString(
                                    "ThingDeserializationFailed"),
                                e);
                        }
                    }
                }
                else
                {
                    if (String.Equals(transformName, "stt", StringComparison.OrdinalIgnoreCase))
                    {
                        var row = dataXml.SelectSingleNode("row");
                        if (row != null)
                        {
                            var relatedThingsValue = row.GetAttribute("wc-relatedthings", String.Empty);
                            _common.ParseRelatedAttribute(relatedThingsValue);
                        }
                    }

                    // The whole data-xml section is transformed data so
                    // no elements need to be parsed by the common thing
                    // parser.
                    XmlDocument newDoc = new XmlDocument();
                    newDoc.XmlResolver = null;
                    newDoc.SafeLoadXml(dataXml.OuterXml);

                     if (!TransformedXmlData.ContainsKey(transformName))
                     {
                        TransformedXmlData.Add(transformName, newDoc);
                     }
                }
            }
        }

        /// <summary>
        /// Adds the values from the BLOB payload section of the health record item to 
        /// the specified HealthRecordItem and updates the Sections 
        /// appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "blob-payload".
        /// </param>
        /// 
        private void AddBlobPayloadSectionValues(XPathNavigator thingNavigator)
        {
            // Check for the "blob-payload"
            XPathNavigator blobPayloadNav =
                thingNavigator.SelectSingleNode("blob-payload");

            if (blobPayloadNav != null)
            {
                _blobStore = new BlobStore(this, default(HealthRecordAccessor));
                _blobStore.ParseXml(blobPayloadNav);
                _sections |= HealthRecordItemSections.BlobPayload;
            }

            XPathNavigator otherDataNav = thingNavigator.SelectSingleNode("data-other");
            if (otherDataNav != null)
            {
                _blobStore = new BlobStore(this, default(HealthRecordAccessor));
                _sections |= HealthRecordItemSections.BlobPayload;
                string contentEncoding =
                    otherDataNav.GetAttribute("content-encoding", string.Empty);
                Byte[] blobPayload =
                    BlobEncoder.Decode(
                        Encoding.UTF8.GetBytes(otherDataNav.Value),
                        contentEncoding);
                string contentType =
                    otherDataNav.GetAttribute("content-type", string.Empty);
                _blobStore.WriteInline(String.Empty, contentType, blobPayload);
            }
        }

        private static XPathExpression _signaturePath =
           XPathExpression.Compile("/thing/ds:Signature");

        private static XPathExpression GetSignatureXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "ds",
                "http://www.w3.org/2000/09/xmldsig#");

            XPathExpression signaturePathClone = null;
            lock (_signaturePath)
            {
                signaturePathClone = _signaturePath.Clone();
            }

            signaturePathClone.SetContext(infoXmlNamespaceManager);

            return signaturePathClone;
        }

        /// <summary>
        /// Adds the values from the Signature section of the health record item to 
        /// the specified HealthRecordItem and updates the Sections 
        /// appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "Signature".
        /// </param>        
        /// 
        /// <param name="thingXml">
        /// The XML string of the thing used to create this health record item.
        /// </param>
        /// <exception cref="HealthRecordItemDeserializationException">
        /// The signature xml could not be parsed. The inner exception may be:
        /// <see cref="XmlException"/>: There is a load or parse error in the XML.
        /// <see cref="ArgumentNullException"/>: The <see cref="Signature"/> section of the
        /// document was not found.
        /// <see cref="CryptographicException"/>: The <see cref="Signature"/> section of the 
        /// document does not contain a valid SignatureValue property.
        /// The <see cref="Signature"/> section of the document does not contain a valid 
        /// <see cref="SignedInfo"/> property.
        /// </exception>
        ///
        private void AddSignatureSectionValues(XPathNavigator thingNavigator, string thingXml)
        {
            // Check for the "Signature"

            XPathNavigator signatureInfoNav = thingNavigator.SelectSingleNode("signature-info");

            if (signatureInfoNav != null)
            {
                HealthRecordItemSignature hriSignature = new HealthRecordItemSignature();
                try
                {
                    hriSignature.ParseXml(signatureInfoNav, thingXml);
                }
                catch (XmlException e)
                {
                    throw new HealthRecordItemDeserializationException(
                        ResourceRetriever.FormatResourceString(
                            "SignatureDeserializationFailed",
                            e.Message),
                        e);
                }
                catch (ArgumentNullException e)
                {
                    throw new HealthRecordItemDeserializationException(
                        ResourceRetriever.FormatResourceString(
                            "SignatureDeserializationFailed",
                            e.Message),
                        e);
                }
                catch (CryptographicException e)
                {
                    throw new HealthRecordItemDeserializationException(
                        ResourceRetriever.FormatResourceString(
                            "SignatureDeserializationFailed",
                            e.Message),
                        e);
                }

                _signatures.Add(hriSignature);
                _signedItemXml = thingXml;

                _sections |= HealthRecordItemSections.Signature;
            }
            else
            {
                XPathExpression signatureXPath = GetSignatureXPathExpression(thingNavigator);
                XPathNavigator signatureNav = thingNavigator.SelectSingleNode(signatureXPath);
                if (signatureNav != null)
                {
                    HealthRecordItemSignature hriSignature = new HealthRecordItemSignature();
                    hriSignature.ParseV1SignatureXml(thingXml);
                    _signatures.Add(hriSignature);
                    _signedItemXml = thingXml;
                    _sections |= HealthRecordItemSections.Signature;
                }
            }

        }

        /// <summary>
        /// Adds tags to the HealthRecordItem and updates the Sections if tags are present.
        /// </summary>
        /// 
        /// <param name="thingNavigator"></param>
        /// 
        private void AddTagsSectionValues(XPathNavigator thingNavigator)
        {
            XPathNavigator tagsNav = thingNavigator.SelectSingleNode("tags");
            if (tagsNav != null && !String.IsNullOrEmpty(tagsNav.Value))
            {
                List<string> tagStrings =
                    new List<string>(
                        tagsNav.Value.Split(
                            new Char[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries));

                if (tagStrings.Count > 0)
                {
                    _tags = new TagsCollection(this, tagStrings);
                    _sections |= HealthRecordItemSections.Tags;
                }
            }
        }

        /// <summary>
        /// Adds the values from the eff-permissions section of the 
        /// health record item to the specified HealthRecordItem and updates 
        /// the Sections appropriately.
        /// </summary>
        /// 
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "eff-permissions".
        /// </param>
        /// 
        private void AddEffectivePermissionsSectionValues(XPathNavigator thingNavigator)
        {
            // Check for the "eff-permissions" data-group

            XPathNavigator permsNav = thingNavigator.SelectSingleNode("eff-permissions");
            if (permsNav != null)
            {
                // Now update the sections appropriately
                _sections |= HealthRecordItemSections.EffectivePermissions;

                string isImmutableString = permsNav.GetAttribute("immutable", String.Empty);
                if (!String.IsNullOrEmpty(isImmutableString))
                {
                    _isImmutable = XmlConvert.ToBoolean(isImmutableString);
                }

                XPathNodeIterator permIterator = permsNav.Select("permission");

                foreach (XPathNavigator permissionNav in permIterator)
                {
                    string permissionString = permissionNav.Value;

                    try
                    {
                        HealthRecordItemPermissions permission =
                            (HealthRecordItemPermissions)
                            Enum.Parse(
                                typeof(HealthRecordItemPermissions),
                                permissionString);

                        if (_permissions == null)
                        {
                            _permissions = permission;
                        }
                        else
                        {
                            _permissions |= permission;
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }

        #endregion Parsing the XML

        private void SetFlag(HealthRecordItemFlags flag)
        {
            // Check if *all* bits in flag are set 
            if ((_flags & flag) != flag)
            {
                _flags |= flag;
                _areFlagsDirty = true;
            }
        }

        private void ClearFlag(HealthRecordItemFlags flag)
        {
            // Check if *any* bits in flag are set
            if ((_flags & flag) != 0)
            {
                _flags &= ~flag;
                _areFlagsDirty = true;
            }
        }

        private bool IsFlagSet(HealthRecordItemFlags flagToCheck)
        {
            return (_flags & flagToCheck) == flagToCheck;
        }
        #endregion private helpers
    }
}
