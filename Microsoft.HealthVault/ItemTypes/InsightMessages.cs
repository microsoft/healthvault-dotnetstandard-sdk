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
    /// Represents the collection of message strings associated with this Insight.
    /// </summary>
    public class InsightMessages : ItemBase
    {
        /// <summary>
        /// Populates the data for insight messages from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the insight messages type.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _regular = XPathHelper.GetOptNavValue(navigator, "regular");
            _short = XPathHelper.GetOptNavValue(navigator, "short");
        }

        /// <summary>
        /// Writes the insight messages data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the insight messages type.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the insight messages type to.
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

            writer.WriteStartElement(nodeName);

            // <regular>
            XmlWriterHelper.WriteOptString(writer, "regular", _regular);

            // <short>
            XmlWriterHelper.WriteOptString(writer, "short", _short);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of insight messages.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the insight messages type.
        /// </returns>
        ///
        public override string ToString()
        {
            return _regular;
        }

        /// <summary>
        /// Gets or sets the regular message for this insight.
        /// </summary>
        public string Regular
        {
            get { return _regular; }

            set { _regular = value; }
        }

        private string _regular;

        /// <summary>
        /// Gets or sets the short message for this insight.
        /// </summary>
        public string Short
        {
            get { return _short; }

            set { _short = value; }
        }

        private string _short;
    }
}