// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents a thing.
    /// </summary>
    ///
    /// <remarks>
    /// A thing is a single piece of data in a health record
    /// that is accessible through the HealthVault service. Examples of health
    /// record items include a blood pressure measurement, an exercise session,
    /// or an insurance claim.
    /// <br/><br/>
    /// things are typed and have XML data that adheres to the
    /// schema for the type.
    /// </remarks>
    ///
    public class ThingBase : IThing
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
        protected internal ThingBase(Guid typeId)
        {
            TypeId = typeId;
            _tags = new TagsCollection(this);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ThingBase"/> class
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
        public ThingBase(Guid typeId, IXPathNavigable typeSpecificData)
            : this(typeId)
        {
            TypeSpecificData = typeSpecificData;
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
            writer.WriteNode(TypeSpecificData.CreateNavigator().ReadSubtree(), true);
        }

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
        /// Gets the key of the thing.
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
        /// <see cref="ThingBase.Sections"/> is set.
        /// </remarks>
        ///
        public ThingKey Key { get; internal set; }

        /// <summary>
        /// Gets the type identifier for the thing type.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the type of the item.
        /// </value>
        ///
        /// <remarks>
        /// The types available can be queried
        /// </remarks>
        ///
        public Guid TypeId { get; internal set; }

        /// <summary>
        /// Gets the thing type name.
        /// </summary>
        ///
        /// <remarks>
        /// The types and names of types available can be queried
        /// </remarks>
        ///
        public string TypeName { get; internal set; }

        #region Core information

        /// <summary>
        /// Gets or sets the date and time that the thing data was taken.
        /// </summary>
        ///
        /// <remarks>
        /// This might not be the same time that the data was entered
        /// into the system.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Core"/> bit
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
        private bool _effectiveDateDirty;

        /// <summary>
        /// Gets the state of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <remarks>
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Core"/> bit
        /// set.
        /// </remarks>
        ///
        public ThingState State { get; internal set; } = ThingState.Active;

        /// <summary>
        /// Gets the <see cref="ThingBase"/> flags.
        /// </summary>
        ///
        /// <remarks>
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Core"/> bit
        /// set.
        /// </remarks>
        ///
        public ThingFlags Flags
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

        private ThingFlags _flags;
        private bool _areFlagsDirty;

        /// <summary>
        /// Gets or sets the value indicating if the <see cref="ThingBase"/> is private.
        /// </summary>
        ///
        /// <remarks>
        /// Private items are accessible only by custodians.
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Core"/> bit
        /// set.
        /// </remarks>
        ///
        public bool IsPersonal
        {
            get { return IsFlagSet(ThingFlags.Personal); }

            set
            {
                if (value)
                {
                    SetFlag(ThingFlags.Personal);
                }
                else
                {
                    ClearFlag(ThingFlags.Personal);
                }
            }
        }

        /// <summary>
        /// Gets the value indicating if the <see cref="ThingBase"/> is down-versioned.
        /// </summary>
        ///
        /// <remarks>
        /// If this value is true then an attempt to update the <see cref="ThingBase"/>
        /// will fail.
        /// </remarks>
        ///
        public bool IsDownVersioned => IsFlagSet(ThingFlags.DownVersioned);

        /// <summary>
        /// Gets the value indicating if the <see cref="ThingBase"/> is up-versioned.
        /// </summary>
        ///
        /// <remarks>
        /// If this value is true then an application should get explicit permission to update the
        /// instance from the user. This will cause the stored instance to be converted to the
        /// type version which may cause data loss in some cases.
        /// </remarks>
        ///
        public bool IsUpVersioned => IsFlagSet(ThingFlags.UpVersioned);

        /// <summary>
        /// Gets or sets the date when ThingBase is not relevant.
        /// </summary>
        /// <remarks>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Core"/> bit
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
        private bool _updatedEndDateDirty;

        #endregion Core information

        #region Audit information

        /// <summary>
        /// Gets the audit information associated with the last update of
        /// this thing.
        /// </summary>
        ///
        /// <remarks>
        /// It is the responsibility of the application to convert the audit time
        /// to local time if necessary.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Audits"/> bit
        /// set.
        /// </remarks>
        ///
        public HealthServiceAudit LastUpdated { get; internal set; }

        /// <summary>
        /// Gets the audit information associated with the creation of
        /// this thing.
        /// </summary>
        ///
        /// <remarks>
        /// It is the responsibility of the application to convert the audit time
        /// to local time if necessary.
        /// <br/><br/>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Audits"/> bit
        /// set.
        /// </remarks>
        ///
        public HealthServiceAudit Created { get; private set; }

        #endregion Audit information

        #region EffectivePermissions information

        /// <summary>
        /// Gets the effective permissions on the item granted to the person
        /// retrieving the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// If the permissions are unknown, the value is <b>null</b>; otherwise
        /// the appropriate permissions are returned.
        /// </value>
        ///
        /// <remarks>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.EffectivePermissions"/>
        /// bit set.
        /// </remarks>
        ///
        public ThingPermissions? EffectivePermissions { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ThingBase"/>
        /// is immutable.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the <see cref="ThingBase"/> is immutable; otherwise,
        /// <b>false</b>.
        /// </value>
        /// <remarks>
        /// This data value is only available when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.EffectivePermissions"/>
        /// bit set. Returns true if either ThingBase Type is immutable or ThingBase instance is read-only.
        /// </remarks>
        ///
        public bool IsImmutable { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="ThingBase"/> is read-only.
        /// </summary>
        /// <value>
        /// <b>true</b> if the <see cref="ThingBase"/> is read-only; otherwise,
        /// <b>false</b>.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// Cannot change read-only flag to false if it is already true.
        /// </exception>
        /// <remarks>
        /// Returns true if either ThingBase Type is immutable or ThingBase instance is read-only
        /// but sets only the instance immutability.
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                return IsImmutable || IsFlagSet(ThingFlags.ReadOnly);
            }

            set
            {
                if (!value && (IsFlagSet(ThingFlags.ReadOnly) || IsImmutable))
                {
                    throw new InvalidOperationException(Resources.CannotChangeImmutableFlag);
                }

                if (value)
                {
                    SetFlag(ThingFlags.ReadOnly);
                }
            }
        }

        #endregion EffectivePermissions information

        #region Xml data information

        /// <summary>
        /// Gets or sets the XML representation of the type-specific data for the
        /// <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This data might contain data from other applications that also work
        /// with the same type of data. Take care not to overwrite
        /// existing data when making updates. Do not remove or manipulate
        /// elements in the XML that you do not understand.
        /// <br/><br/>
        /// If this property is set on a type derived from <see cref="ThingBase"/> the data
        /// is not parsed into the object model so properties of the class may still show old data.
        /// <br/><br/>
        /// This data value is only available to get when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Xml"/> bit
        /// set.
        /// </remarks>
        ///
        public IXPathNavigable TypeSpecificData { get; set; }

        /// <summary>
        /// Gets the transformed XML data of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <remarks>
        /// Transformed XML data is provided if you so specify in
        /// <see cref="HealthRecordSearcher"/>. The dictionary values are
        /// indexed by the name of the transform that was specified when the
        /// search was performed.
        /// <br/><br/>
        /// This data value is only available to get when the
        /// <see cref="ThingBase.Sections"/> show the
        /// <see cref="ThingSections.Xml"/> bit
        /// set though it will not contain data unless a transform was
        /// specified when getting the item.
        /// </remarks>
        ///
        public IDictionary<string, XDocument> TransformedXmlData => _transformedData;

        private readonly Dictionary<string, XDocument> _transformedData =
            new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the common data for the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CommonItemData"/> for the
        /// <see cref="ThingBase"/>.
        /// </value>
        ///
        /// <remarks>
        /// The common data includes notes, source, and extensions.
        /// </remarks>
        ///
        public CommonItemData CommonData { get; internal set; } = new CommonItemData();

        #endregion Xml data information

        #region Blobs

        /// <summary>
        /// Gets the BLOB store for this thing.
        /// </summary>
        ///
        /// <param name="record">
        /// The <see cref="HealthRecordAccessor"/> that any BLOB data will be written to. This parameter
        /// may be <b>null</b> if only read access is required or the BLOB data to be written is
        /// for a ConnectPackage.
        /// </param>
        ///
        /// <remarks>
        /// This method replaces the previous OtherData property. All binary data is now created,
        /// updated, and retrieved through <see cref="BlobStore"/> instance associated with the
        /// <see cref="ThingBase"/>.<br/>
        /// <br/>
        /// GetBlobStore will return an empty store on an existing <see cref="ThingBase"/>
        /// if <see cref="ThingSections.BlobPayload"/> is not specified when retrieving
        /// the item. In this case it is possible to overwrite or remove existing Blobs in the
        /// <see cref="ThingBase"/> instance stored in HealthVault by using the same name
        /// as the existing Blob. It is recommended that if you are going to be manipulating
        /// Blobs in the BlobStore, that you specify
        /// <see cref="ThingSections.BlobPayload"/> when retrieving the item.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="BlobStore"/> instance related to this thing.
        /// </returns>
        ///
        internal BlobStore GetBlobStore(HealthRecordAccessor record)
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

        private BlobStore _blobStore;

        #endregion Blobs

        #region Tags

        /// <summary>
        /// Gets the list of tags on the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A string representing the tag list.
        /// </value>
        ///
        public Collection<string> Tags => _tags;

        private TagsCollection _tags;

        private class TagsCollection : Collection<string>
        {
            internal TagsCollection(ThingBase item)
            {
                _item = item;
            }

            internal TagsCollection(ThingBase item, IList<string> list)
                : base(list)
            {
                _item = item;
            }

            private readonly ThingBase _item;

            protected override void ClearItems()
            {
                base.ClearItems();
                _item.Sections |= ThingSections.Tags;
            }

            protected override void InsertItem(int index, string itemToInsert)
            {
                base.InsertItem(index, itemToInsert);
                _item.Sections |= ThingSections.Tags;
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                _item.Sections |= ThingSections.Tags;
            }

            protected override void SetItem(int index, string itemToInsert)
            {
                base.SetItem(index, itemToInsert);
                _item.Sections |= ThingSections.Tags;
            }
        }

        #endregion Tags

        /// <summary>
        /// Gets the data sections that this ThingBase represents.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="ThingSections"/>.
        /// </value>
        ///
        public ThingSections Sections { get; internal set; } = ThingSections.Core |
            ThingSections.Xml;

        /// <summary>
        /// Gets the size of the thing which will be added to the quota used in the
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
        /// The size in bytes of the thing.
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

            byte[] bytes = Encoding.UTF8.GetBytes(thingXml.ToString());
            return bytes.Length;
        }

        /// <summary>
        /// Gets the XML representation of the thing.
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
        public string GetItemXml(string elementName = "thing")
        {
            ThingSections sections = Sections;

            // Add the tags section if there are tags present
            if (_tags != null && _tags.Count > 0)
            {
                sections |= ThingSections.Tags;
            }

            return GetItemXml(sections, elementName);
        }

        /// <summary>
        /// Gets the XML representation of the thing.
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
        public string GetItemXml(ThingSections sections, string elementName = "thing")
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

        public bool WriteItemXml(XmlWriter infoXmlWriter)
        {
            return WriteItemXml(infoXmlWriter, true);
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, string elementName)
        {
            return WriteItemXml(infoXmlWriter, true, elementName);
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, bool writeAllCore = true)
        {
            return WriteItemXml(infoXmlWriter, writeAllCore, "thing");
        }

        internal bool WriteItemXml(XmlWriter infoXmlWriter, bool writeAllCore, string elementName)
        {
            ThingSections sections = Sections & ~ThingSections.Audits;

            // Add the tags section if there are tags present
            if (_tags != null && _tags.Count > 0)
            {
                sections |= ThingSections.Tags;
            }

            return WriteItemXml(
                infoXmlWriter,
                sections,
                writeAllCore,
                elementName);
        }

        internal bool WriteItemXml(
            XmlWriter infoXmlWriter,
            ThingSections sections,
            bool writeAllCore = true,
            string elementName = "thing")
        {
            bool updateRequired;

            // First add the <__elementName__> tag
            infoXmlWriter.WriteStartElement(elementName);

            // Add the <core> element and children

            updateRequired =
                AddCorePutThingsRequestParameters(infoXmlWriter, writeAllCore, sections);

            if ((sections & ThingSections.Audits) != 0)
            {
                AddAuditThingsRequestParameters(infoXmlWriter);
            }

            if ((sections & ThingSections.Xml) != 0)
            {
                AddXmlPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & ThingSections.BlobPayload) != 0)
            {
                AddBlobPayloadPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & ThingSections.Tags) != 0)
            {
                AddTagsPutThingsRequestParameters(infoXmlWriter);
                updateRequired = true;
            }

            if ((sections & ThingSections.Core) == ThingSections.Core)
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

            // replace the data-xml node from the original xml with the data-xml node that contains the current
            // state of the object...
            XDocument fetchedDocument = SDKHelper.SafeLoadXml(_fetchedXml);
            XDocument newDocument = SDKHelper.SafeLoadXml(GetItemXml());

            string dataName = "thing/data-xml";
            fetchedDocument.Element(dataName).ReplaceWith(newDocument.Element(dataName));
            return fetchedDocument.ToString();
        }

        // The original XML that was fetched by the thing.
        private string _fetchedXml;

        /// <summary>
        /// Create a <see cref="ThingBase"/> instance from the item XML.
        /// </summary>
        ///
        /// <remarks>
        /// This method is identical to calling <see cref="ItemTypeManager.DeserializeItem(string)"/>.
        ///
        /// The item XML should come from a previous call to <see cref="ThingBase.Serialize"/>.
        /// </remarks>
        ///
        /// <returns>
        /// A instance of the <see cref="ThingBase"/> class.
        /// </returns>
        ///
        public static ThingBase Deserialize(string itemXml)
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
            ThingSections sections)
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

            if (!string.IsNullOrEmpty(TypeName))
            {
                writer.WriteAttributeString("name", TypeName);
            }

            writer.WriteValue(TypeId.ToString());
            writer.WriteEndElement();

            if ((sections & ThingSections.Core) != 0 && writeAllCore)
            {
                updateRequired = true;
                writer.WriteElementString("thing-state", State.ToString());

                writer.WriteElementString("flags", ((uint)Flags).ToString());
            }
            else
            {
                if (_areFlagsDirty)
                {
                    writer.WriteElementString("flags", ((uint)Flags).ToString());
                    updateRequired = true;
                }
            }

            if ((sections & ThingSections.Core) != 0 &&
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
        /// Populates the ThingBase elements with data from the XML.
        /// </summary>
        ///
        /// <param name="thingNavigator">
        /// The Xml Navigator from which the thing will be constructed.
        /// </param>
        ///
        /// <param name="thingXml">
        /// The XML from which the thing will be constructed.
        /// </param>
        ///
        /// <remarks>
        /// Most of the methods called use an XmlNavigator. However AddSignatureSectionValues
        /// requires an XmlReader, because it requires the exact xml passed in, and XmlNavigator
        /// can change the white space.
        /// </remarks>
        ///
        /// <exception cref="ThingDeserializationException">
        /// The derived type's <see cref="ParseXml(IXPathNavigable)"/>
        /// method throws an exception when called. The inner exception
        /// will be the exception thrown by the method.
        /// </exception>
        ///
        internal void ParseXml(XPathNavigator thingNavigator, string thingXml)
        {
            _fetchedXml = thingXml;

            Sections = ThingSections.None;

            XPathNavigator thingIdNav = thingNavigator.SelectSingleNode("thing-id");
            if (thingIdNav != null)
            {
                Guid thingId = new Guid(thingIdNav.Value);

                string versionStamp = thingIdNav.GetAttribute("version-stamp", string.Empty);

                Key = !string.IsNullOrEmpty(versionStamp) ? new ThingKey(thingId, new Guid(versionStamp)) : new ThingKey(thingId);
            }

            XPathNavigator thingTypeNavigator = thingNavigator.SelectSingleNode("type-id");

            if (thingTypeNavigator != null)
            {
                string typeId = thingTypeNavigator.Value;
                if (!string.IsNullOrEmpty(typeId))
                {
                    TypeId = new Guid(typeId);
                }

                TypeName = thingTypeNavigator.GetAttribute("name", string.Empty);
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
        }

        /// <summary>
        /// Adds the values from the core section of the thing to
        /// the specified ThingBase and updates the Sections
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
                Sections |= ThingSections.Core;
                _effectiveDate = effectiveDateNav.ValueAsDateTime;
            }

            XPathNavigator stateNav =
                thingNavigator.SelectSingleNode("thing-state");
            if (stateNav != null)
            {
                Sections |= ThingSections.Core;

                try
                {
                    State =
                        (ThingState)Enum.Parse(
                            typeof(ThingState), stateNav.Value);
                }
                catch (ArgumentException)
                {
                    State = ThingState.Unknown;
                }
            }

            XPathNavigator flagsNav = thingNavigator.SelectSingleNode("flags");
            if (flagsNav != null)
            {
                Sections |= ThingSections.Core;

                _flags = (ThingFlags)Convert.ToInt64(flagsNav.Value);
            }

            XPathNavigator updatedEndDateNav = thingNavigator.SelectSingleNode("updated-end-date");
            if (updatedEndDateNav != null)
            {
                Sections |= ThingSections.Core;
                _updatedEndDate = updatedEndDateNav.ValueAsDateTime;
            }
        }

        /// <summary>
        /// Adds the values from the created and updated sections of the
        /// thing to the specified ThingBase and updates
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
                LastUpdated = new HealthServiceAudit();
                LastUpdated.ParseXml(updatedNav);

                // Now update the sections appropriately
                Sections |= ThingSections.Audits;
            }

            // Check the "created" group
            XPathNavigator createdNav =
                thingNavigator.SelectSingleNode("created");
            if (createdNav != null)
            {
                Created = new HealthServiceAudit();
                Created.ParseXml(createdNav);

                // Now update the sections appropriately
                Sections |= ThingSections.Audits;
            }
        }

        /// <summary>
        /// Adds the values from the xml section of the thing to
        /// the specified ThingBase and updates the Sections
        /// appropriately.
        /// </summary>
        ///
        /// <param name="thingNavigator">
        /// The containing XPath navigator in which to find a child named
        /// "data-xml".
        /// </param>
        ///
        /// <exception cref="ThingDeserializationException">
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
                Sections |= ThingSections.Xml;

                string transformName =
                    dataXml.GetAttribute("transform", string.Empty);

                if (string.IsNullOrEmpty(transformName))
                {
                    XPathNavigator commonNav =
                        dataXml.SelectSingleNode("common");
                    if (commonNav != null)
                    {
                        // Parse out the common section
                        CommonData.ParseXml(commonNav);
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
                        TypeSpecificData = new XPathDocument(reader, XmlSpace.Preserve);

                        try
                        {
                            ParseXml(TypeSpecificData);
                        }
                        catch (Exception e)
                        {
                            // third-party call-out
                            throw new ThingDeserializationException(
                                Resources.ThingDeserializationFailed,
                                e);
                        }
                    }
                }
                else
                {
                    if (string.Equals(transformName, "stt", StringComparison.OrdinalIgnoreCase))
                    {
                        var row = dataXml.SelectSingleNode("row");
                        if (row != null)
                        {
                            var relatedThingsValue = row.GetAttribute("wc-relatedthings", string.Empty);
                            CommonData.ParseRelatedAttribute(relatedThingsValue);
                        }
                    }

                    // The whole data-xml section is transformed data so<healthrecorditem
                    // no elements need to be parsed by the common thing
                    // parser.
                    XDocument newDoc = SDKHelper.SafeLoadXml(dataXml.OuterXml);

                    if (!TransformedXmlData.ContainsKey(transformName))
                    {
                        TransformedXmlData.Add(transformName, newDoc);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the values from the BLOB payload section of the thing to
        /// the specified ThingBase and updates the Sections
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
                Sections |= ThingSections.BlobPayload;
            }

            XPathNavigator otherDataNav = thingNavigator.SelectSingleNode("data-other");
            if (otherDataNav != null)
            {
                _blobStore = new BlobStore(this, default(HealthRecordAccessor));
                Sections |= ThingSections.BlobPayload;
                string contentEncoding =
                    otherDataNav.GetAttribute("content-encoding", string.Empty);
                byte[] blobPayload =
                    BlobEncoder.Decode(
                        Encoding.UTF8.GetBytes(otherDataNav.Value),
                        contentEncoding);
                string contentType =
                    otherDataNav.GetAttribute("content-type", string.Empty);
                _blobStore.WriteInline(string.Empty, contentType, blobPayload);
            }
        }

        private static XPathExpression s_signaturePath =
           XPathExpression.Compile("/thing/ds:Signature");

        private static XPathExpression GetSignatureXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "ds",
                "http://www.w3.org/2000/09/xmldsig#");

            XPathExpression signaturePathClone;
            lock (s_signaturePath)
            {
                signaturePathClone = s_signaturePath.Clone();
            }

            signaturePathClone.SetContext(infoXmlNamespaceManager);

            return signaturePathClone;
        }

        /// <summary>
        /// Adds tags to the ThingBase and updates the Sections if tags are present.
        /// </summary>
        ///
        /// <param name="thingNavigator">The Xml navigator that is currently working with this ThingBase</param>
        ///
        private void AddTagsSectionValues(XPathNavigator thingNavigator)
        {
            XPathNavigator tagsNav = thingNavigator.SelectSingleNode("tags");
            if (!string.IsNullOrEmpty(tagsNav?.Value))
            {
                List<string> tagStrings =
                    new List<string>(
                        tagsNav.Value.Split(
                            new[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries));

                if (tagStrings.Count > 0)
                {
                    _tags = new TagsCollection(this, tagStrings);
                    Sections |= ThingSections.Tags;
                }
            }
        }

        /// <summary>
        /// Adds the values from the eff-permissions section of the
        /// thing to the specified ThingBase and updates
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
                Sections |= ThingSections.EffectivePermissions;

                string isImmutableString = permsNav.GetAttribute("immutable", string.Empty);
                if (!string.IsNullOrEmpty(isImmutableString))
                {
                    IsImmutable = XmlConvert.ToBoolean(isImmutableString);
                }

                XPathNodeIterator permIterator = permsNav.Select("permission");

                foreach (XPathNavigator permissionNav in permIterator)
                {
                    string permissionString = permissionNav.Value;

                    try
                    {
                        ThingPermissions permission =
                            (ThingPermissions)Enum.Parse(
                                typeof(ThingPermissions),
                                permissionString);

                        if (EffectivePermissions == null)
                        {
                            EffectivePermissions = permission;
                        }
                        else
                        {
                            EffectivePermissions |= permission;
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }

        #endregion Parsing the XML

        private void SetFlag(ThingFlags flag)
        {
            // Check if *all* bits in flag are set
            if ((_flags & flag) != flag)
            {
                _flags |= flag;
                _areFlagsDirty = true;
            }
        }

        private void ClearFlag(ThingFlags flag)
        {
            // Check if *any* bits in flag are set
            if ((_flags & flag) != 0)
            {
                _flags &= ~flag;
                _areFlagsDirty = true;
            }
        }

        private bool IsFlagSet(ThingFlags flagToCheck)
        {
            return (_flags & flagToCheck) == flagToCheck;
        }

        #endregion private helpers
    }
}
