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
    /// Represents a lab test result range value.
    /// </summary>
    ///
    /// <remarks>
    /// A range consists of minimum and/or maximum values.
    /// An open-ended range may be created by omitting the minimum or maximum value.
    ///
    /// For example, "greater than 3.5" is created by setting the minimum value to 3.5 and
    /// the maximum value to null.
    /// </remarks>
    ///
    ///
    public class TestResultRangeValue : ItemBase
    {
        /// <summary>
        /// Populates the data for the range from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the range.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _minimum = XPathHelper.GetOptNavValueAsDouble(navigator, "minimum-range");
            _maximum = XPathHelper.GetOptNavValueAsDouble(navigator, "maximum-range");
        }

        /// <summary>
        /// Writes the range data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the range data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the range data to.
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

            // minimum
            XmlWriterHelper.WriteOptDouble(
                writer,
                "minimum-range",
                _minimum);

            // maximum
            XmlWriterHelper.WriteOptDouble(
                writer,
                "maximum-range",
                _maximum);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        ///
        /// <value>
        /// A value of type double? that represents the minimum value of the range.
        /// </value>
        public double? Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }

        private double? _minimum;

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        ///
        /// <value>
        /// A value of type double? that represents the maximum value of the range.
        /// </value>
        public double? Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }

        private double? _maximum;

        /// <summary>
        /// Gets a string representation of the test result range value.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the test result range value item.
        /// </returns>
        ///
        public override string ToString()
        {
            string minimumString = _minimum != null ? _minimum.ToString() : string.Empty;
            string maximumString = _maximum != null ? _maximum.ToString() : string.Empty;

            return string.Format(
                Resources.TestResultRangeValueToStringFormat,
                minimumString,
                maximumString);
        }
    }
}
