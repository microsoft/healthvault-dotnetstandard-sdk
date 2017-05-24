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

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a length value and display.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, lengths have values and display values. All values are
    /// stored in a base unit of meters. An application can take a length
    /// value using any scale the application chooses and can store the
    /// user-entered value as the display value, but the length value must be
    /// converted to meters to be stored in HealthVault.
    /// </remarks>
    ///
    public class Length : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with empty values.
        /// </summary>
        ///
        public Length()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with the
        /// specified value in meters.
        /// </summary>
        ///
        /// <param name="meters">
        /// The length in meters.
        /// </param>
        ///
        public Length(double meters)
            : base(meters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with the
        /// specified value in meters and optional display value.
        /// </summary>
        ///
        /// <param name="meters">
        /// The length in meters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the length. This should contain the
        /// exact length as entered by the user even if it uses some
        /// other unit of measure besides meters. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public Length(double meters, DisplayValue displayValue)
            : base(meters, displayValue)
        {
        }

        /// <summary>
        /// Gets or sets the value of the length in meters.
        /// </summary>
        ///
        /// <value>
        /// A number representing the length.
        /// </value>
        ///
        /// <remarks>
        /// The value must be in meters. The <see cref="DisplayValue"/> can
        /// be used to store the user-entered value in a scale other than
        /// metric.
        /// </remarks>
        ///
        public double Meters
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Verifies that the value is a legal length value in meters.
        /// </summary>
        ///
        /// <param name="value">
        /// The length measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            if (value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.LengthNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the length from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the length.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("m").ValueAsDouble;
        }

        /// <summary>
        /// Writes the length to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the length to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "m", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the length in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The length as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return string.Format(
                Resources.LengthToStringFormatMeters,
                value.ToString(CultureInfo.CurrentCulture));
        }
    }
}
