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
    /// Represents a torque measurement and display.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, torque measurements have values and display values.
    /// All values are stored in a base unit of newton meters.
    /// An application can take a torque value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the torque value must be
    /// converted to newton meters to be stored in HealthVault.
    /// </remarks>
    ///
    public class TorqueMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TorqueMeasurement"/>
        /// class with empty values.
        /// </summary>
        ///
        public TorqueMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TorqueMeasurement"/>
        /// class with the specified value in newton meters.
        /// </summary>
        ///
        /// <param name="newtonMeters">
        /// The torque in newton meters.
        /// </param>
        ///
        public TorqueMeasurement(double newtonMeters)
            : base(newtonMeters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TorqueMeasurement"/>
        /// class with the specified value in newton meters and optional display value.
        /// </summary>
        ///
        /// <param name="newtonMeters">
        /// The torque in newton meters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the torque. This should contain the
        /// exact torque as entered by the user even if it uses some
        /// other unit of measure besides newton meters. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public TorqueMeasurement(
            double newtonMeters,
            DisplayValue displayValue)
            : base(newtonMeters, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal torque value.
        /// </summary>
        ///
        /// <param name="value">
        /// The torque measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "TorqueNotPositive");
        }

        /// <summary>
        /// Populates the data for the torque from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the torque.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            this.Value = navigator.SelectSingleNode("newton-meters").ValueAsDouble;
        }

        /// <summary>
        /// Writes the torque to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the torque to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "newton-meters",
                XmlConvert.ToString(this.Value));
        }

        /// <summary>
        /// Gets a string representation of the torque in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The torque as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
