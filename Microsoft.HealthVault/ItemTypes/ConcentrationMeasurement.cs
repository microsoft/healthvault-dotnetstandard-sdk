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
    /// Represents a concentration measurement (volume / volume).
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, concentration readings have values and display values.
    /// All values are stored in a base unit of mmol/L. An application can
    /// take a concentration measurement like Cholesterol using any scale the
    /// application chooses and can store the user-entered value as the display value,
    /// but the concentration value must be converted to mmol/L to be stored in HealthVault.
    /// </remarks>
    ///
    public class ConcentrationMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConcentrationMeasurement"/>
        /// class with empty values.
        /// </summary>
        ///
        public ConcentrationMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConcentrationMeasurement"/>
        /// class with the specified value in millimoles per liter (mmol/L).
        /// </summary>
        ///
        /// <param name="millimolesPerLiter">
        /// The concentration value in millimoles per liter.
        /// </param>
        ///
        public ConcentrationMeasurement(double millimolesPerLiter)
            : base(millimolesPerLiter)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConcentrationMeasurement"/>
        /// class with the specified value in millimoles per liter (mmol/L)
        /// and display value.
        /// </summary>
        ///
        /// <param name="millimolesPerLiter">
        /// The concentration value in millimoles per liter.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the concentration measurement. This should
        /// contain the exact measurement as entered by the user, even if it
        /// uses some other unit of measure besides mmol/L. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public ConcentrationMeasurement(
            double millimolesPerLiter,
            DisplayValue displayValue)
            : base(millimolesPerLiter, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal concentration measurement in
        /// millimoles per liter (mmol/L).
        /// </summary>
        ///
        /// <param name="value">
        /// The concentration measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.BloodGlucoseNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the concentration value from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the concentration value.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("mmolPerL").ValueAsDouble;
        }

        /// <summary>
        /// Writes the concentration value to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the concentration value to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "mmolPerL", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the concentration value in the base units.
        /// </summary>
        /// <returns>
        /// The concentration value as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}