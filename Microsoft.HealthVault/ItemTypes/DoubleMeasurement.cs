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
    /// Represents a measurement of type double and display.
    /// </summary>
    ///
    public class DoubleMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DoubleMeasurement"/> class using
        /// default values.
        /// </summary>
        ///
        public DoubleMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DoubleMeasurement"/> class
        /// with the specified value.
        /// </summary>
        ///
        /// <param name="value">
        /// The value of the measurement.
        /// </param>
        ///
        public DoubleMeasurement(double value)
            : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DoubleMeasurement"/> class
        /// with the specified value and optional display value.
        /// </summary>
        ///
        /// <param name="value">
        /// The value of the measurement.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the measurement. This should contain the
        /// exact measurement as entered by the user even if it uses some
        /// other unit of measure. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user entered value.
        /// </param>
        ///
        public DoubleMeasurement(
            double value,
            DisplayValue displayValue)
            : base(value, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal value.
        /// </summary>
        ///
        /// <param name="value">
        /// The value of the measurement.
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
                throw new ArgumentOutOfRangeException(nameof(value), Resources.ValueNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the measurement from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the measurement.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode(ValueElementName).ValueAsDouble;
        }

        /// <summary>
        /// Writes the measurement to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the measurement to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                ValueElementName,
                Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Retrieves a string representation of the measurement in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The measurement as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets or sets the name of the element that defines the value for
        /// the measurement.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the element name.
        /// </returns>
        ///
        /// <remarks>
        /// The default value for the element name is "value" but should be
        /// overridden in derived classes to provide a specific element name
        /// where appropriate.
        /// </remarks>
        ///
        protected virtual string ValueElementName { get; set; } = "value";
    }
}
