// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates an action plan record
    /// </summary>
    public abstract class MshItemBase : ThingBase
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
                this.WrappedTypeName = wrappedInstance.WrappedTypeName;
                this.WrappedInstanceJson = wrappedInstance.Base64EncodedJson;
                if (wrappedInstance.ThingId != Guid.Empty && wrappedInstance.VersionStamp != Guid.Empty)
                {
                    this.Key = new ThingKey(wrappedInstance.ThingId, wrappedInstance.VersionStamp);
                }
            }

            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MshItemBase"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="typeId">Unique type id</param>
        /// <param name="wrappedTypeName">Wrapped type name</param>
        /// <param name="base64EncodedJson">Base64-encoded JSON</param>
        /// <param name="thingId">ThingBase id</param>
        /// <param name="versionStamp">ThingBase versionstamp</param>
        protected MshItemBase(
            Guid typeId,
            string wrappedTypeName,
            string base64EncodedJson,
            Guid thingId,
            Guid versionStamp)
            : base(typeId)
        {
            this.WrappedTypeName = wrappedTypeName;
            this.WrappedInstanceJson = base64EncodedJson;
            if (thingId != Guid.Empty && versionStamp != Guid.Empty)
            {
                this.Key = new ThingKey(thingId, versionStamp);
            }

            this.Headers = new Dictionary<string, string>();
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
                return this.wrappedTypeName;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "WrappedTypeName", "WrappedTypeNameNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "WrappedTypeName");
                this.wrappedTypeName = value;
            }
        }

        private string wrappedTypeName;

        /// <summary>
        /// Wrapped instance in base64 encoded json format
        /// </summary>
        public string WrappedInstanceJson
        {
            get
            {
                return this.wrappedInstanceJson;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "WrappedInstanceJson", "WrappedInstanceJsonNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "WrappedInstanceJson");
                this.wrappedInstanceJson = value;
            }
        }

        private string wrappedInstanceJson;
        #endregion

        #region Overrides of ThingBase

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

            navigator = navigator.SelectSingleNode(this.RootElementName);
            Validator.ThrowInvalidIfNull(navigator, "MshRecordUnexpectedNode");

            // headers
            this.Headers.Clear();
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
                        if (!this.Headers.ContainsKey(name))
                        {
                            this.Headers.Add(name, value);
                        }
                    }
                }
            }

            // type
            var typeNode = navigator.SelectSingleNode("type");
            if (typeNode != null)
            {
                this.wrappedTypeName = typeNode.Value;
            }

            // wrapped instance
            var jsonNode = navigator.SelectSingleNode("value");
            if (jsonNode != null)
            {
                this.wrappedInstanceJson = jsonNode.Value;
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
            Validator.ThrowSerializationIfNull(this.wrappedTypeName, "WrappedTypeNameNullValue");
            Validator.ThrowSerializationIfNull(this.wrappedInstanceJson, "WrappedInstanceJsonNullValue");

            writer.WriteStartElement(this.RootElementName);

            // headers
            if (this.Headers != null && this.Headers.Count > 0)
            {
                writer.WriteStartElement("headers");
                {
                    foreach (var key in this.Headers.Keys)
                    {
                        writer.WriteElementString("name", key);
                        writer.WriteElementString("value", this.Headers[key]);
                    }
                }

                writer.WriteEndElement();
            }

            // type
            writer.WriteElementString("type", this.WrappedTypeName);

            // value: json base64 encoded
            writer.WriteStartElement("value");
            writer.WriteValue(this.WrappedInstanceJson);
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
                string.Format(
                CultureInfo.CurrentCulture,
                ResourceRetriever.GetResourceString("MshItemSummaryText"),
                this.wrappedTypeName,
                this.wrappedInstanceJson);

            return value;
        }

        #endregion
    }
}
