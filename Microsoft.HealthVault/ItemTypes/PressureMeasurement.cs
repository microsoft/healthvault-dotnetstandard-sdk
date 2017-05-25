// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a pressure measurement and a display value
    /// associated with the measurement.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, pressure measurements have values and display values.
    /// All values are stored in a standard SI unit of pascal (Pa).
    /// An application can take a pressure value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the pressure value must be converted to pascals to be stored in HealthVault.
    /// </remarks>
    ///
    public class PressureMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PressureMeasurement"/> class
        /// with empty values.
        /// </summary>
        ///
        public PressureMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PressureMeasurement"/> class
        /// with the specified value in pascal.
        /// </summary>
        ///
        /// <param name="pascals">
        /// The pressure value in pascal.
        /// </param>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "pascals is a valid element name.")]
        public PressureMeasurement(double pascals)
            : base(pascals)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PressureMeasurement"/> class with
        /// the specified value in pascals and an optional display value.
        /// </summary>
        ///
        /// <param name="pascals">
        /// The pressure in pascal.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the pressure. This should contain the
        /// exact pressure as entered by the user even if it uses some
        /// other unit of measure besides pascal. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "pascals is a valid element name.")]
        public PressureMeasurement(double pascals, DisplayValue displayValue)
            : base(pascals, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal pressure value in Pa.
        /// </summary>
        ///
        /// <param name="value">
        /// The pressure measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.PressureNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the pressure from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the pressure.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("pascals").ValueAsDouble;
        }

        /// <summary>
        /// Writes the pressure to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the pressure to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "pascals",
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the pressure in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The pressure as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
