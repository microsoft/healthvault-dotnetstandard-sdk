// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents Insight attribution information.
    /// </summary>
    public class InsightAttribution : ItemBase
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="InsightAttribution"/> class with
        /// default values.
        /// </summary>
        ///
        public InsightAttribution()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="InsightAttribution"/> with the specified name
        /// </summary>
        /// <param name="name"></param>
        ///
        public InsightAttribution(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Populates the data for insight attribution from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the insight attribution type.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.name = navigator.SelectSingleNode("name").Value;

            this.attributionRequired = XPathHelper.GetOptNavValueAsBool(navigator, "attribution-required");
        }

        /// <summary>
        /// Writes the insight attribution data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the insight attribution type.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the insight attribution type to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, Resources.InsightAttributionNameNullValue);

            writer.WriteStartElement(nodeName);

            // <name>
            writer.WriteElementString("name", this.name);

            // <attributon-required>
            XmlWriterHelper.WriteOptBool(writer, "attributon-required", this.attributionRequired);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of insight attribution.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the insight attribution type.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// Gets or sets attribution name.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is null or contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.InsightAttributionNameNullValue);
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets attribution required boolean value.
        /// </summary>
        ///
        public bool? AttributionRequired
        {
            get
            {
                return this.attributionRequired;
            }

            set
            {
                this.attributionRequired = value;
            }
        }

        private bool? attributionRequired;
    }
}