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
    /// Represents a flow of gas, liquid, etc. over time and a display value
    /// associated with the measurement.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, flow measurements have values and display values.
    /// All values are stored in a base unit of liters per second (L/s).
    /// An application can take a flow value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the flow value must be
    /// converted to L/s to be stored in HealthVault.
    /// </remarks>
    ///
    public class FlowMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FlowMeasurement"/> class
        /// with empty values.
        /// </summary>
        ///
        public FlowMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FlowMeasurement"/> class
        /// with the specified value in liters per second.
        /// </summary>
        ///
        /// <param name="litersPerSecond">
        /// The flow in liters per second.
        /// </param>
        ///
        public FlowMeasurement(double litersPerSecond)
            : base(litersPerSecond)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FlowMeasurement"/> class with
        /// the specified value in liters per second and optional display value.
        /// </summary>
        ///
        /// <param name="litersPerSecond">
        /// The flow in liters per second.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the flow. This should contain the
        /// exact flow as entered by the user even if it uses some
        /// other unit of measure besides liters per second. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public FlowMeasurement(double litersPerSecond, DisplayValue displayValue)
            : base(litersPerSecond, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal flow value in liters/sec.
        /// </summary>
        ///
        /// <param name="value">
        /// The flow measurement.
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
                throw new ArgumentOutOfRangeException(nameof(value), Resources.FlowNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the flow from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the flow.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("liters-per-second").ValueAsDouble;
        }

        /// <summary>
        /// Writes the flow to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the flow to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "liters-per-second",
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the flow in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The flow as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
