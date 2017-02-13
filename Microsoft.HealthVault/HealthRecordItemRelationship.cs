// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents a loose relationship between health record item instances.
    /// </summary>
    /// 
    /// <remarks>
    /// The relationship defined is not maintained by HealthVault. It is completely up to the 
    /// application defining and consuming the relationship to ensure the related item exists
    /// and is in the same health record.
    /// </remarks>
    /// 
    public class HealthRecordItemRelationship
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemRelationship"/>
        /// instance with default values.
        /// </summary>
        /// 
        public HealthRecordItemRelationship()
        {
        }

        /// <summary>
        /// Constructs a <see cref="HealthRecordItemRelationship" /> instance for a relationship
        /// to the item with the specified ID.
        /// </summary>
        /// 
        /// <param name="itemId">
        /// The unique identifier of the health record item to related to.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="itemId"/> is <see cref="System.Guid.Empty"/>.
        /// </exception>
        /// 
        public HealthRecordItemRelationship(Guid itemId)
        {
            Validator.ThrowArgumentExceptionIf(
                itemId == Guid.Empty,
                "itemId",
                "RelationshipItemIDNotSpecified");

            _itemKey = new HealthRecordItemKey(itemId);
        }

        /// <summary>
        /// Constructs a <see cref="HealthRecordItemRelationship" /> instance for a relationship
        /// to the item with the specified key.
        /// </summary>
        /// 
        /// <param name="itemKey">
        /// The unique key of the health record item to related to, including the item ID and
        /// optionally the item version stamp.
        /// </param>
        ///     
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemKey"/> is <b>null</b>.
        /// </exception>
        /// 
        public HealthRecordItemRelationship(HealthRecordItemKey itemKey)
        {
            Validator.ThrowIfArgumentNull(itemKey, "itemKey", "RelationshipItemKeyNotSpecified");

            _itemKey = itemKey;
        }

        /// <summary>
        /// Constructs a <see cref="HealthRecordItemRelationship" /> instance for a relationship
        /// to the item with the specified key.
        /// </summary>
        /// 
        /// <param name="clientId">
        /// A client assigned ID for the health record item to relate to.
        /// </param>
        ///     
        /// <exception cref="ArgumentException">
        /// If <paramref name="clientId"/> is <b>null</b> or <b>empty</b>.
        /// </exception>
        /// 
        public HealthRecordItemRelationship(string clientId)
        {
            Validator.ThrowIfStringNullOrEmpty(clientId, "clientId");
            _clientId = clientId;
        }

        /// <summary>
        /// Constructs a <see cref="HealthRecordItemRelationship" /> instance for a relationship
        /// to the item with the specified key and relationship type.
        /// </summary>
        /// 
        /// <param name="itemKey">
        /// The unique key of the health record item to related to, including the item ID and
        /// optionally the item version stamp.
        /// </param>
        ///  
        /// <param name="relationshipType">
        /// The application defined type for the relationship. This is usually a descriptive tagging
        /// for the relationship. For example, a Annotation item may have a
        /// "annotation" relationship with a Problem item.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemKey"/> is <b>null</b>.
        /// </exception>
        /// 
        public HealthRecordItemRelationship(HealthRecordItemKey itemKey, string relationshipType)
            : this(itemKey)
        {
            _relationshipType = relationshipType;
        }

        /// <summary>
        /// Constructs a <see cref="HealthRecordItemRelationship" /> instance for a relationship
        /// to the item with the specified client ID and relationship type.
        /// </summary>
        /// 
        /// <param name="clientId">
        /// A client assigned ID of the health record item to be related to.
        /// </param>
        ///  
        /// <param name="relationshipType">
        /// The application defined type for the relationship. This is usually a descriptive tagging
        /// for the relationship. For example, an Annotation item may have a
        /// "annotation" relationship with a Problem item.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="clientId"/> is <b>null</b>.
        /// </exception>
        /// 
        public HealthRecordItemRelationship(string clientId, string relationshipType)
            : this(clientId)
        {
            _relationshipType = relationshipType;
        }

        internal void ParseXml(XPathNavigator relationshipNav)
        {
            XPathNavigator thingIdNav = relationshipNav.SelectSingleNode("thing-id");
            if (thingIdNav != null)
            {
                Guid itemId = new Guid(thingIdNav.Value);

                XPathNavigator versionStampNav = relationshipNav.SelectSingleNode("version-stamp");
                if (versionStampNav != null)
                {
                    Guid versionStamp = new Guid(versionStampNav.Value);

                    _itemKey = new HealthRecordItemKey(itemId, versionStamp);
                }
                else
                {
                    _itemKey = new HealthRecordItemKey(itemId);
                }
            }
            else
            {
                XPathNavigator clientIdNav = relationshipNav.SelectSingleNode("client-thing-id");

                if (clientIdNav != null)
                {
                    _clientId = clientIdNav.Value;
                }
            }
            _relationshipType = XPathHelper.GetOptNavValue(relationshipNav, "relationship-type");
        }

        internal void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowSerializationIf(
                _itemKey == null && String.IsNullOrEmpty(_clientId),
                "RelationshipItemKeyOrClientIdNotSpecified");

            writer.WriteStartElement(nodeName);
            if (_itemKey != null)
            {
                writer.WriteElementString("thing-id", _itemKey.Id.ToString());

                if (_itemKey.VersionStamp != Guid.Empty)
                {
                    writer.WriteElementString("version-stamp", _itemKey.VersionStamp.ToString());
                }
            }
            else
            {
                writer.WriteElementString("client-thing-id", _clientId);
            }

            XmlWriterHelper.WriteOptString(writer, "relationship-type", _relationshipType);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the key for the related <see cref="HealthRecordItem" />.
        /// </summary>
        /// 
        /// <value>
        /// An instance of the <see cref="HealthRecordItemKey"/> class with the item ID specified
        /// and optionally the item version stamp.
        /// </value>
        /// 
        public HealthRecordItemKey ItemKey
        {
            get { return _itemKey; }
            set { _itemKey = value; }
        }
        private HealthRecordItemKey _itemKey;

        /// <summary>
        /// Gets and sets a client assigned identifier for the related <see cref="HealthRecordItem" />.
        /// </summary>
        public String ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }
        private String _clientId;

        /// <summary>
        /// Gets or sets the type of relationship between the items.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional.
        /// A relationship type is application defined and should be somewhat descriptive about the
        /// relationship. For example, an Annotation item may have an
        /// "annotation" relationship to a Problem item.
        /// </remarks>
        /// 
        public string RelationshipType
        {
            get { return _relationshipType; }
            set { _relationshipType = value; }
        }
        private string _relationshipType;
    }
}

