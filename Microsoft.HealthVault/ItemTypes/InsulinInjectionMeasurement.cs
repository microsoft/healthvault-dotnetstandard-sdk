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
    /// Represents an insulin injection measurement.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, insulin injection readings have values and display values.
    /// All values are stored in a base unit of IE. An application can
    /// take an insulin injection value using any scale the application chooses and
    /// can store the user-entered value as the display value, but the insulin
    /// injection value must be converted to IE to be stored in HealthVault.
    /// </remarks>
    ///
    public class InsulinInjectionMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/>
        /// class with empty values.
        /// </summary>
        ///
        public InsulinInjectionMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/>
        /// class with the specified value in IE (1/100mL).
        /// </summary>
        ///
        /// <param name="iE">
        /// The insulin injection measurement in 1/100mL.
        /// </param>
        ///
        public InsulinInjectionMeasurement(double iE)
            : base(iE)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/>
        /// class with the specified value in IE (1/100mL) and display value.
        /// </summary>
        ///
        /// <param name="iE">
        /// The insulin injection measurement in IE (1/100mL).
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the insulin injection measurement. This should
        /// contain the exact measurement as entered by the user even if it
        /// uses some other unit of measure besides IE. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public InsulinInjectionMeasurement(double iE, DisplayValue displayValue)
            : base(iE, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal insulin injection measurement in
        /// IE (1/100mL).
        /// </summary>
        ///
        /// <param name="value">
        /// The insulin injection measurement.
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
                throw new ArgumentOutOfRangeException(nameof(value), Resources.InsulinInjectionNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the insulin injection from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the insulin injection.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("IE").ValueAsDouble;
        }

        /// <summary>
        /// Writes the insulin injection to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the insulin injection to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "IE", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the insulin injection in the base
        /// units.
        /// </summary>
        ///
        /// <returns>
        /// The insulin injection as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
