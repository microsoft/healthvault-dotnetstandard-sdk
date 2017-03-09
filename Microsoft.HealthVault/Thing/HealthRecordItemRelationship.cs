// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
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

            this.ItemKey = new HealthRecordItemKey(itemId);
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

            this.ItemKey = itemKey;
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
            this.ClientId = clientId;
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
            this.RelationshipType = relationshipType;
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
            this.RelationshipType = relationshipType;
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

                    this.ItemKey = new HealthRecordItemKey(itemId, versionStamp);
                }
                else
                {
                    this.ItemKey = new HealthRecordItemKey(itemId);
                }
            }
            else
            {
                XPathNavigator clientIdNav = relationshipNav.SelectSingleNode("client-thing-id");

                if (clientIdNav != null)
                {
                    this.ClientId = clientIdNav.Value;
                }
            }

            this.RelationshipType = XPathHelper.GetOptNavValue(relationshipNav, "relationship-type");
        }

        internal void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowSerializationIf(
                this.ItemKey == null && string.IsNullOrEmpty(this.ClientId),
                "RelationshipItemKeyOrClientIdNotSpecified");

            writer.WriteStartElement(nodeName);
            if (this.ItemKey != null)
            {
                writer.WriteElementString("thing-id", this.ItemKey.Id.ToString());

                if (this.ItemKey.VersionStamp != Guid.Empty)
                {
                    writer.WriteElementString("version-stamp", this.ItemKey.VersionStamp.ToString());
                }
            }
            else
            {
                writer.WriteElementString("client-thing-id", this.ClientId);
            }

            XmlWriterHelper.WriteOptString(writer, "relationship-type", this.RelationshipType);

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
        public HealthRecordItemKey ItemKey { get; set; }

        /// <summary>
        /// Gets and sets a client assigned identifier for the related <see cref="HealthRecordItem" />.
        /// </summary>
        public string ClientId { get; set; }

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
        public string RelationshipType { get; set; }
    }
}
