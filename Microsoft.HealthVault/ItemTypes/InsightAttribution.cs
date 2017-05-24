// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
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
        /// Constructs a new instance of the <see cref="InsightAttribution"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="InsightAttribution"/>.</param>
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
            Validator.ThrowSerializationIfNull(_name, Resources.InsightAttributionNameNullValue);

            writer.WriteStartElement(nodeName);

            // <name>
            writer.WriteElementString("name", _name);

            // <attribution-required>
            XmlWriterHelper.WriteOptBool(writer, "attribution-required", _attributionRequired);

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
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.InsightAttributionNameNullValue);
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