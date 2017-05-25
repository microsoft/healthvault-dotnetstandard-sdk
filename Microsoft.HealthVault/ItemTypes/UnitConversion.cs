// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Unit conversion representation.
    /// </summary>
    ///
    public class UnitConversion : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="UnitConversion"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the UnitConversion data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _multiplier = XPathHelper.GetOptNavValueAsDouble(navigator, "multiplier");
            _offset = XPathHelper.GetOptNavValueAsDouble(navigator, "offset");
        }

        /// <summary>
        /// Writes the XML representation of the UnitConversion into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the unit conversion.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the UnitConversion should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            writer.WriteStartElement("unit-conversion");

            XmlWriterHelper.WriteOptDouble(writer, "multiplier", _multiplier);
            XmlWriterHelper.WriteOptDouble(writer, "offset", _offset);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the multiplier.
        /// </summary>
        ///
        public double? Multiplier
        {
            get
            {
                return _multiplier;
            }

            set
            {
                _multiplier = value;
            }
        }

        private double? _multiplier;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        ///
        public double? Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                _offset = value;
            }
        }

        private double? _offset;

        /// <summary>
        /// Gets a string representation of the UnitConversion.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the UnitConversion.
        /// </returns>
        ///
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentUICulture,
                Resources.UnitConversionFormat,
                _multiplier,
                _offset);
        }

        /// <summary>
        /// Convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>the value using the selected conversion</returns>
        public double Convert(double value)
        {
            if (_multiplier.HasValue)
            {
                value = value * _multiplier.Value;
            }

            if (_offset.HasValue)
            {
                value = value + _offset.Value;
            }

            return value;
        }

        /// <summary>
        /// Reverse convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>the value using the reverse of this conversion</returns>
        public double ReverseConvert(double value)
        {
            if (_offset.HasValue)
            {
                value = value - _offset.Value;
            }

            if (_multiplier.HasValue)
            {
                value = value / _multiplier.Value;
            }

            return value;
        }
    }
}
