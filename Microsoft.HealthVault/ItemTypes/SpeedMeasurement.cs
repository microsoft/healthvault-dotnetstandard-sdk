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
    /// Represents a speed measurement and display.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, speed measurements have values and display values.
    /// All values are stored in a base unit of meters per second (m/s).
    /// An application can take a speed value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the speed value must be
    /// converted to m/s to be stored in HealthVault.
    /// </remarks>
    ///
    public class SpeedMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SpeedMeasurement"/> class
        /// with empty values.
        /// </summary>
        ///
        public SpeedMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SpeedMeasurement"/> class
        /// with the specified value in meters per second.
        /// </summary>
        ///
        /// <param name="metersPerSecond">
        /// The speed in meters per second.
        /// </param>
        ///
        public SpeedMeasurement(double metersPerSecond)
            : base(metersPerSecond)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SpeedMeasurement"/> class with
        /// the specified value in meters per second and optional display value.
        /// </summary>
        ///
        /// <param name="metersPerSecond">
        /// The speed in meters per second.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the speed. This should contain the
        /// exact speed as entered by the user even if it uses some
        /// other unit of measure besides meters per second. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public SpeedMeasurement(double metersPerSecond, DisplayValue displayValue)
            : base(metersPerSecond, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal speed value in meters.
        /// </summary>
        ///
        /// <param name="value">
        /// The speed measurement.
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
                throw new ArgumentOutOfRangeException(nameof(value), Resources.SpeedNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the speed from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the speed.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("meters-per-second").ValueAsDouble;
        }

        /// <summary>
        /// Writes the speed to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the speed to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "meters-per-second",
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the speed in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The speed as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
