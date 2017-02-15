// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a measurement of type string and display.
    /// </summary>
    ///
    public class StringMeasurement : Measurement<string>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StringMeasurement"/>
        /// class with empty values.
        /// </summary>
        ///
        public StringMeasurement()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StringMeasurement"/>
        /// class with the specified value.
        /// </summary>
        ///
        /// <param name="value">
        /// The value of the measurement.
        /// </param>
        ///
        public StringMeasurement(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StringMeasurement"/>
        /// class with the specified value and optional display value.
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
        public StringMeasurement(string value, DisplayValue displayValue)
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
        protected override void AssertMeasurementValue(string value)
        {
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
            Value = navigator.SelectSingleNode(ValueElementName).Value;
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
                ValueElementName,
                Value);
        }

        /// <summary>
        /// Retrieves a string representation of the measurement in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The measurement as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(string value)
        {
            return value;
        }

        /// <summary>
        /// Gets or sets the name of the element that defines the value for the measurement.
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
        protected virtual string ValueElementName
        {
            get { return _valueElementName; }
            set { _valueElementName = value; }
        }
        private string _valueElementName = "value";
    }
}
