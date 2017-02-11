// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
{
    /// <summary>
    /// Represents data that is common for all types of health record items.
    /// </summary>
    /// 
    /// <remarks>
    /// The common data for all health record item types includes data 
    /// such as notes, source, and extensions.
    /// </remarks>
    /// 
    public class CommonItemData
    {
        /// <summary>
        /// Creates an empty instance of the CommonItemData class.
        /// </summary>
        /// 
        public CommonItemData()
        {
        }

        internal void ParseXml(XPathNavigator commonNav)
        {
            XPathNavigator sourceNav = commonNav.SelectSingleNode("source");
            if (sourceNav != null)
            {
                _source = sourceNav.Value;
            }

            XPathNavigator noteNav = commonNav.SelectSingleNode("note");
            if (noteNav != null)
            {
                _note = noteNav.Value;
            }

            // Please leave this code until the data-xml/common/tags gets removed.
            XPathNavigator tagsNav = commonNav.SelectSingleNode("tags");
            if (tagsNav != null)
            {
                _tags = tagsNav.Value;
            }

            XPathNodeIterator extensionIterator = commonNav.Select("extension");

            foreach (XPathNavigator extensionNav in extensionIterator)
            {
                HealthRecordItemExtension extension =
                    ItemTypeManager.DeserializeExtension(extensionNav);

                if (extension != null)
                {
                    _extensions.Add(extension);
                }
            }

            XPathNodeIterator relationshipIterator =
                commonNav.Select("related-thing[./thing-id != '' or ./client-thing-id != '']");

            foreach (XPathNavigator relationshipNav in relationshipIterator)
            {
                HealthRecordItemRelationship relationship =
                    new HealthRecordItemRelationship();

                relationship.ParseXml(relationshipNav);

                _relatedItems.Add(relationship);
            }

            XPathNavigator clientIdNav = commonNav.SelectSingleNode("client-thing-id");
            if (clientIdNav != null)
            {
                _clientId = clientIdNav.Value;
            }
        }

        internal void ParseRelatedAttribute(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                var relThings = value.Split(';');
                foreach (var relThing in relThings)
                {
                    Guid thingId;
                    if (Guid.TryParse(relThing.Split(',')[0], out thingId))
                    {
                        _relatedItems.Add(new HealthRecordItemRelationship(thingId));
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            // <common>
            writer.WriteStartElement("common");

            if (!String.IsNullOrEmpty(_source))
            {
                writer.WriteElementString("source", _source);
            }

            if (!String.IsNullOrEmpty(_note))
            {
                writer.WriteElementString("note", _note);
            }

            // Please leave this code until the data-xml/common/tags gets removed.
            if (!String.IsNullOrEmpty(_tags))
            {
                writer.WriteElementString("tags", _tags);
            }

            foreach (HealthRecordItemExtension extension in _extensions)
            {
                extension.WriteExtensionXml(writer);
            }

            foreach (HealthRecordItemRelationship relationship in _relatedItems)
            {
                relationship.WriteXml("related-thing", writer);
            }

            if (!String.IsNullOrEmpty(_clientId))
            {
                writer.WriteElementString("client-thing-id", _clientId);
            }

            // </common>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the source of the health record item.
        /// </summary>
        /// 
        /// 
        /// <value>
        /// A string representing the item source.
        /// </value>
        /// 
        /// <remarks>
        /// The source is the description of the device or application
        /// from which the health record item came.
        /// </remarks>
        /// 
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        private string _source;

        /// <summary>
        /// Gets or sets a note on the health record item.
        /// </summary>
        /// 
        /// <value>
        /// A string.
        /// </value>
        /// 
        /// <remarks>
        /// Notes are general annotations about the health record item.
        /// </remarks>
        /// 
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }
        private string _note;

        /// <summary>
        /// Gets or sets a comma-separated list of tags on 
        /// the health record item.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the tag list.
        /// </value>
        /// 
        /// <remarks>
        /// Tags enable users to group information freely. Applications
        /// must parse the tag list for individual tags.
        /// </remarks>
        /// 
        [Obsolete("This property will be soon removed. Please use HealthRecordItem.Tags instead.")]
        public string Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }
        private string _tags;

        /// <summary>
        /// Gets the collection representing the extension data of the 
        /// health record item.
        /// </summary>
        /// 
        /// <value>
        /// A collection of <see cref="HealthRecordItemExtension"/> objects.
        /// </value>
        /// 
        /// <remarks>
        /// To add extensions to the health record item, add an instance of the
        /// <see cref="HealthRecordItemExtension"/> or derived class to this 
        /// collection.
        /// </remarks>
        /// 
        public Collection<HealthRecordItemExtension> Extensions
        {
            get { return _extensions; }
        }
        private Collection<HealthRecordItemExtension> _extensions =
            new Collection<HealthRecordItemExtension>();

        /// <summary>
        /// Gets the collection representing the health record items related to this one.
        /// </summary>
        /// 
        /// <value>
        /// A collection of <see cref="HealthRecordItemRelationship"/> objects.
        /// </value>
        /// 
        /// <remarks>
        /// The relationships between this item and the health record items defined in this collection
        /// are not maintained by HealthVault. It is solely the responsibility of applications to
        /// ensure that the referenced items exist and are in the same health record.
        /// </remarks>
        ///
        public Collection<HealthRecordItemRelationship> RelatedItems
        {
            get { return _relatedItems; }
        }
        private Collection<HealthRecordItemRelationship> _relatedItems =
            new Collection<HealthRecordItemRelationship>();

        /// <summary>
        /// Gets and sets a client assigned identifier to be associated with the <see cref="HealthRecordItem" />.
        /// </summary>
        public string ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }
        private string _clientId;
    }
}
