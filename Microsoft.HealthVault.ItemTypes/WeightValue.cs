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
    /// Represents a weight value and display.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, weights have values and display values. All values are
    /// stored in a base unit of kilograms. An application can take a length
    /// value using any scale the application chooses and can store the user-
    /// entered value as the display value, but the length value must be
    /// converted to kilograms to be stored in HealthVault.
    /// </remarks>
    ///
    public class WeightValue : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WeightValue"/> class
        /// with empty values.
        /// </summary>
        ///
        public WeightValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WeightValue"/> class with
        /// the specified value in kilograms.
        /// </summary>
        ///
        /// <param name="kilograms">
        /// The weight value in kilograms.
        /// </param>
        ///
        public WeightValue(double kilograms)
            : base(kilograms)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WeightValue"/> class with
        /// the specified value in kilograms and the specified display value.
        /// </summary>
        ///
        /// <param name="kilograms">
        /// The weight value in kilograms.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the weight. This should contain the
        /// exact weight as entered by the user, even if it uses some
        /// other unit of measure besides kilograms. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public WeightValue(double kilograms, DisplayValue displayValue)
            : base(kilograms, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal weight value in kilograms (kg).
        /// </summary>
        ///
        /// <param name="value">
        /// The weight measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value < 0.0, "value", "WeightNotPositive");
        }

        /// <summary>
        /// Populates the data for the weight from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the weight.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            this.Value = navigator.SelectSingleNode("kg").ValueAsDouble;
        }

        /// <summary>
        /// Writes the weight to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the weight to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "kg", XmlConvert.ToString(this.Value));
        }

        /// <summary>
        /// Gets a string representation of the weight in the base units.
        /// </summary>
        ///
        /// <value>
        /// A number representing the weight.
        /// </value>
        ///
        /// <returns>
        /// The weight as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets or sets the value of the weight in kilograms.
        /// </summary>
        ///
        /// <value>
        /// A number representing the weight.
        /// </value>
        ///
        /// <remarks>
        /// The value must be in kilograms. The <see cref="DisplayValue"/> can
        /// be used to store the user-entered value in a scale other than
        /// metric.
        /// </remarks>
        ///
        public double Kilograms
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WeightValue"/> class by subtracting
        /// the operand from this weight value.
        /// </summary>
        ///
        /// <param name="value1">
        /// The weight value from which <paramref name="value2"/> is to
        /// be subtracted.
        /// </param>
        ///
        /// <param name="value2">
        /// The weight value to subtract from <paramref name="value1"/>.
        /// </param>
        ///
        /// <remarks>
        /// This constructor creates a <see cref="WeightValue"/> that is
        /// the difference between this weight value and the operand. The value is
        /// always subtracted. If both operands have a
        /// <see cref="DisplayValue"/> set and they have the same
        /// <see cref="DisplayValue.UnitsCode"/>,
        /// then the result sets DisplayValue with the same units code and the
        /// value is the difference between the operands.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="value2"/> parameter is larger than the <paramref name="value1"/> parameter.
        /// </exception>
        ///
        public static WeightValue operator -(
            WeightValue value1,
            WeightValue value2)
        {
            WeightValue newValue = new WeightValue();

            Validator.ThrowInvalidIf(
                value2.Value > value1.Value, "WeightResultNotPositive");

            newValue.Value = value1.Value - value2.Value;

            if (value1.DisplayValue != null && value2.DisplayValue != null)
            {
                if (value1.DisplayValue.UnitsCode == value2.DisplayValue.UnitsCode ||
                    value1.DisplayValue.Units == value2.DisplayValue.Units)
                {
                    newValue.DisplayValue = new DisplayValue();
                    newValue.DisplayValue.UnitsCode = value1.DisplayValue.UnitsCode;
                    newValue.DisplayValue.Value =
                        value1.DisplayValue.Value - value2.DisplayValue.Value;
                    newValue.DisplayValue.Units = value1.DisplayValue.Units;
                }
            }

            return newValue;
        }
    }
}
