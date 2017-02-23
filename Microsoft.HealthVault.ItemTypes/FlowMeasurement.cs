// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

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
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "FlowNotPositive");
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
            this.Value = navigator.SelectSingleNode("liters-per-second").ValueAsDouble;
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
                XmlConvert.ToString(this.Value));
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
