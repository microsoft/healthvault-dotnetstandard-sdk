// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates an action plan record
    /// </summary>
    public abstract class MshItemBase : HealthRecordItem
    {
        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="MshItemBase"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="wrappedInstance">Wrapped instance</param>
        /// <param name="typeId">Unique type id</param>
        protected MshItemBase(IMshItem wrappedInstance, Guid typeId)
            : base(typeId)
        {
            if (wrappedInstance != null)
            {
                WrappedTypeName = wrappedInstance.WrappedTypeName;
                WrappedInstanceJson = wrappedInstance.Base64EncodedJson;
                if (wrappedInstance.ThingId != Guid.Empty && wrappedInstance.VersionStamp != Guid.Empty)
                {
                    Key = new HealthRecordItemKey(wrappedInstance.ThingId, wrappedInstance.VersionStamp);
                }
            }

            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MshItemBase"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="typeId">Unique type id</param>
        /// <param name="wrappedTypeName">Wrapped type name</param>
        /// <param name="base64EncodedJson">Base64-encoded JSON</param>
        /// <param name="thingId">Thing id</param>
        /// <param name="versionStamp">Thing versionstamp</param>
        protected MshItemBase(
            Guid typeId,
            string wrappedTypeName,
            string base64EncodedJson,
            Guid thingId,
            Guid versionStamp)
            : base(typeId)
        {
            WrappedTypeName = wrappedTypeName;
            WrappedInstanceJson = base64EncodedJson;
            if (thingId != Guid.Empty && versionStamp != Guid.Empty)
            {
                Key = new HealthRecordItemKey(thingId, versionStamp);
            }

            Headers = new Dictionary<string, string>();
        }

        #endregion

        #region abstract

        /// <summary>
        /// override root element name within thing xml
        /// </summary>
        protected abstract string RootElementName { get; }
        #endregion

        #region props
        /// <summary>
        /// Gets the header information associated with this item.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Full type name of wrapped object.
        /// </summary>
        public string WrappedTypeName
        {
            get
            {
                return _wrappedTypeName;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "WrappedTypeName", "WrappedTypeNameNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "WrappedTypeName");
                _wrappedTypeName = value;
            }
        }

        private string _wrappedTypeName;

        /// <summary>
        /// Wrapped instance in base64 encoded json format
        /// </summary>
        public string WrappedInstanceJson
        {
            get
            {
                return _wrappedInstanceJson;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "WrappedInstanceJson", "WrappedInstanceJsonNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "WrappedInstanceJson");
                _wrappedInstanceJson = value;
            }
        }

        private string _wrappedInstanceJson;
        #endregion

        #region Overrides of HealthRecordItem

        /// <summary>
        /// Populates the <see cref="Message"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the Message data from.
        /// </param>
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, "typeSpecificXml", "ParseXmlNavNull");

            XPathNavigator navigator = typeSpecificXml.CreateNavigator();
            Validator.ThrowInvalidIfNull(navigator, "MshRecordUnexpectedNode");

            navigator = navigator.SelectSingleNode(RootElementName);
            Validator.ThrowInvalidIfNull(navigator, "MshRecordUnexpectedNode");

            // headers
            Headers.Clear();
            var headersNav = navigator.SelectSingleNode("headers");
            if (headersNav != null)
            {
                foreach (XPathNavigator nav in headersNav.Select("header"))
                {
                    var nameNode = nav.SelectSingleNode("name");
                    var valueNode = nav.SelectSingleNode("value");
                    if (nameNode != null && valueNode != null)
                    {
                        string name = nameNode.Value;
                        string value = valueNode.Value;
                        if (!Headers.ContainsKey(name))
                        {
                            Headers.Add(name, value);
                        }
                    }
                }
            }

            // type
            var typeNode = navigator.SelectSingleNode("type");
            if (typeNode != null)
            {
                _wrappedTypeName = typeNode.Value;
            }

            // wrapped instance
            var jsonNode = navigator.SelectSingleNode("value");
            if (jsonNode != null)
            {
                _wrappedInstanceJson = jsonNode.Value;
            }
        }

        /// <summary>
        /// Writes the XML representation of the Message into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// xml writer
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_wrappedTypeName, "WrappedTypeNameNullValue");
            Validator.ThrowSerializationIfNull(_wrappedInstanceJson, "WrappedInstanceJsonNullValue");

            writer.WriteStartElement(RootElementName);

            // headers
            if (Headers != null && Headers.Count > 0)
            {
                writer.WriteStartElement("headers");
                {
                    foreach (var key in Headers.Keys)
                    {
                        writer.WriteElementString("name", key);
                        writer.WriteElementString("value", Headers[key]);
                    }
                }

                writer.WriteEndElement();
            }

            // type
            writer.WriteElementString("type", WrappedTypeName);

            // value: json base64 encoded
            writer.WriteStartElement("value");
            writer.WriteValue(WrappedInstanceJson);
            writer.WriteEndElement();

            writer.WriteEndElement(); // action-plan
        }

        /// <summary>
        /// Gets a string representation of the msh item.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the msh item.
        /// </returns>
        ///
        public override string ToString()
        {
            string value =
                String.Format(
                CultureInfo.CurrentCulture,
                ResourceRetriever.GetResourceString("MshItemSummaryText"),
                _wrappedTypeName,
                _wrappedInstanceJson);

            return value;
        }

        #endregion
    }
}
