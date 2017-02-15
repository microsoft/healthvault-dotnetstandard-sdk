// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
            Validator.ThrowArgumentOutOfRangeIf(value < 0.0, "value", "BloodGlucoseNotPositive");
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