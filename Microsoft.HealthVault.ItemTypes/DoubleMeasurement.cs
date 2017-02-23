// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

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
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "ValueNotPositive");
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
            this.Value = navigator.SelectSingleNode(this.ValueElementName).ValueAsDouble;
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
                this.ValueElementName,
                this.Value.ToString(CultureInfo.InvariantCulture));
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
