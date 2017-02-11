// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a power measurement and display.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, power measurements have values and display values. 
    /// All values are stored in a base unit of watts. 
    /// An application can take a pace value using any scale the application 
    /// chooses and can store the user-entered value as the display value, 
    /// but the power value must be 
    /// converted to watts to be stored in HealthVault.
    /// </remarks>
    /// 
    public class PowerMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PowerMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public PowerMeasurement() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PowerMeasurement"/> 
        /// class with the specified value in watts.
        /// </summary>
        /// 
        /// <param name="watts">
        /// The power in watts.
        /// </param>
        /// 
        public PowerMeasurement(double watts) 
            : base(watts)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PowerMeasurement"/> 
        /// class with the specified value in watts and optional display value.
        /// </summary>
        /// 
        /// <param name="watts">
        /// The power in watts.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the power. This should contain the
        /// exact power as entered by the user even if it uses some
        /// other unit of measure besides watts. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public PowerMeasurement(
            double watts, 
            DisplayValue displayValue)
            : base(watts, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal power value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The power measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "PowerNotPositive");
        }

        /// <summary> 
        /// Populates the data for the power from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the power.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("watts").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the power to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the power to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "watts", 
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the power in the base units.
        /// </summary>
        /// 
        /// <returns>
        /// The power as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
