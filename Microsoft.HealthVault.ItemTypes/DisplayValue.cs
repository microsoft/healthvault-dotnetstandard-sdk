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
    /// Represents the display value for a length, weight, or other type
    /// of measurement.
    /// </summary>
    ///
    /// <remarks>
    /// A display value differs from a value in that it is the value as entered
    /// by the user rather than the value converted to a base unit. The unit
    /// used can also reference the unit through the HealthVault dictionary code.
    ///
    /// Single-valued display values ("15 pounds") should be expressed using the
    /// <see cref="Value"/> and <see cref="Units"/> properties.
    ///
    /// Multi-valued display values ("12 pounds 3 ounces") can be expressed using the
    /// <see cref="Text"/> property, but should also be expressed using value and
    /// units ("12.25 pounds").
    /// </remarks>
    ///
    public class DisplayValue : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DisplayValue"/> class with empty values.
        /// </summary>
        ///
        public DisplayValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DisplayValue"/> class
        /// with the specified value.
        /// </summary>
        ///
        /// <param name="value">
        /// The value as it was entered.
        /// </param>
        ///
        public DisplayValue(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DisplayValue"/> class
        /// with the specified value and units.
        /// </summary>
        ///
        /// <param name="value">
        /// The value as it was entered.
        /// </param>
        ///
        /// <param name="units">
        /// The units of the <paramref name="value"/> as it was
        /// entered.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="units"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public DisplayValue(double value, string units)
            : this(value)
        {
            this.Units = units;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DisplayValue"/> class with
        /// the specified value, units, and units code.
        /// </summary>
        ///
        /// <param name="value">
        /// The value as it was entered.
        /// </param>
        ///
        /// <param name="units">
        /// The units of the <paramref name="value"/> as it was
        /// entered.
        /// </param>
        ///
        /// <param name="unitsCode">
        /// The Health Lexicon vocabulary code for the unit of measure.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="units"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public DisplayValue(double value, string units, string unitsCode)
            : this(value, units)
        {
            this.UnitsCode = unitsCode;
        }

        /// <summary>
        /// Populates the data for the display value from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the display value.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.units = navigator.GetAttribute("units", string.Empty);
            this.unitsCode = navigator.GetAttribute("units-code", string.Empty);
            if (string.IsNullOrEmpty(this.unitsCode))
            {
                this.unitsCode = null;
            }

            this.text = navigator.GetAttribute("text", string.Empty);
            if (string.IsNullOrEmpty(this.text))
            {
                this.text = null;
            }

            this.value = navigator.ValueAsDouble;
        }

        /// <summary>
        /// Writes the display value to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the display value.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the display value to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Units"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.units, "DisplayValueUnitsMandatory");

            writer.WriteStartElement(nodeName);

            writer.WriteAttributeString("units", this.units);

            if (!string.IsNullOrEmpty(this.unitsCode))
            {
                writer.WriteAttributeString("units-code", this.unitsCode);
            }

            if (!string.IsNullOrEmpty(this.text))
            {
                writer.WriteAttributeString("text", this.text);
            }

            writer.WriteValue(XmlConvert.ToString(this.value));

            writer.WriteEndElement();
        }

        /// <summary>
        /// Retrieves a string representation of the display value.
        /// </summary>
        ///
        /// <returns>
        /// The value with the optional units.
        /// </returns>
        ///
        public override string ToString()
        {
            string result;

            if (this.text != null)
            {
                return this.text;
            }

            result = this.Value.ToString(CultureInfo.CurrentCulture);

            if (!string.IsNullOrEmpty(this.Units))
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "DisplayValueToStringFormatWithUnits"),
                        result,
                        this.Units);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the code used in the HealthVault Dictionary to represent
        /// the units.
        /// </summary>
        ///
        /// <value>
        /// A string representing the code.
        /// </value>
        ///
        /// <remarks>
        /// The code is used to abstract the units from the application so
        /// that the application can retrieve the appropriate value for the
        /// culture desired.
        /// </remarks>
        ///
        public string UnitsCode
        {
            get { return this.unitsCode; }
            set { this.unitsCode = value; }
        }

        private string unitsCode;

        /// <summary>
        /// Gets or sets the units of measure as defined by the user.
        /// </summary>
        ///
        /// <value>
        /// A string representing the units.
        /// </value>
        ///
        /// <remarks>
        /// When the units are different from the base units for the particular
        /// type of measurement, the units should be set and if possible the
        /// <see cref="UnitsCode"/> should also be set.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> set is <b>null</b>, empty.
        /// </exception>
        ///
        public string Units
        {
            get { return this.units; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Units", "DisplayValueUnitsMandatory");
                this.units = value;
            }
        }

        private string units;

        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        ///
        /// <value>
        /// A number representing the display value.
        /// </value>
        ///
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private double value;

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        ///
        /// <value>
        /// A string representing the display value.
        /// </value>
        ///
        /// <remarks>
        /// If the display value cannot be properly expressed as "number units", the
        /// Text property can be used to express the display value. An
        /// example of this would be expressing a height as "5 feet 8 inches".
        ///
        /// Applications that use the Text property should still express the value
        /// in the "number units" format for applications that predate the introduction
        /// of this property.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> set is <b>null</b> or empty.
        /// </exception>
        ///
        public string Text
        {
            get { return this.text; }

            set
            {
                if (value != null)
                {
                    Validator.ThrowIfStringNullOrEmpty(value, "Text");
                    Validator.ThrowIfStringIsWhitespace(value, "Text");
                }

                this.text = value;
            }
        }

        private string text;
    }
}
