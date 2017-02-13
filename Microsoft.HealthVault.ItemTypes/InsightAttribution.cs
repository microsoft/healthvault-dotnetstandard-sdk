// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents Insight attribution information.
    /// </summary>
    public class InsightAttribution : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="InsightAttribution"/> class with 
        /// default values.
        /// </summary>
        /// 
        public InsightAttribution() 
            : base()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="InsightAttribution"/> with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// 
        public InsightAttribution(string name)
        {
            Name = name;
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

            _name = navigator.SelectSingleNode("name").Value;

            _attributionRequired = XPathHelper.GetOptNavValueAsBool(navigator, "attribution-required");
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
            Validator.ThrowSerializationIfNull(_name, "InsightAttributionNameNullValue");

            writer.WriteStartElement(nodeName);

            // <name>
            writer.WriteElementString("name", _name);

            // <attributon-required>
            XmlWriterHelper.WriteOptBool(writer, "attributon-required", _attributionRequired);

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
            return _name;
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
                return _name; 
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "InsightAttributionNameNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");
                _name = value;
            }
        }

        private string _name;

        /// <summary>
        /// Gets or sets attribution required boolean value.
        /// </summary>
        /// 
        public bool? AttributionRequired
        {
            get 
            { 
                return _attributionRequired; 
            }

            set
            {
                _attributionRequired = value;
            }
        }

        private bool? _attributionRequired;
    }
}