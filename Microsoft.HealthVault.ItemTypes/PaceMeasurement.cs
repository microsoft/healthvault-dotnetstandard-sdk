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
    /// Represents a pace measurement and display.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, pace measurements have values and display values.
    /// All values are stored in a base unit of seconds per 100 meters.
    /// An application can take a pace value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the pace value must be converted to seconds per 100 meters to be
    /// stored in HealthVault.
    /// </remarks>
    ///
    public class PaceMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PaceMeasurement"/>
        /// class with empty values.
        /// </summary>
        ///
        public PaceMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PaceMeasurement"/>
        /// class with the specified value in seconds per 100 meters.
        /// </summary>
        ///
        /// <param name="secondsPerHundredMeters">
        /// The pace in seconds per 100 meters.
        /// </param>
        ///
        public PaceMeasurement(double secondsPerHundredMeters)
            : base(secondsPerHundredMeters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PaceMeasurement"/>
        /// class with the specified value in seconds per 100 meters and
        /// optional display value.
        /// </summary>
        ///
        /// <param name="secondsPerHundredMeters">
        /// The pace in seconds per 100 meters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the pace. This should contain the
        /// exact pace as entered by the user even if it uses some
        /// other unit of measure besides seconds per 100 meters. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public PaceMeasurement(double secondsPerHundredMeters, DisplayValue displayValue)
            : base(secondsPerHundredMeters, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal pace value.
        /// </summary>
        ///
        /// <param name="value">
        /// The pace measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "PaceNotPositive");
        }

        /// <summary>
        /// Populates the data for the pace from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the pace.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            this.Value = navigator.SelectSingleNode("seconds-per-hundred-meters").ValueAsDouble;
        }

        /// <summary>
        /// Writes the pace to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the pace to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "seconds-per-hundred-meters",
                XmlConvert.ToString(this.Value));
        }

        /// <summary>
        /// Gets a string representation of the pace in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The pace as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
