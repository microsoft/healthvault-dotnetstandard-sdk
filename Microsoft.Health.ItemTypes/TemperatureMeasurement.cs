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
    /// Represents a temperature measurement and display.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, temperature measurements have values and display values. 
    /// All values are stored in a base unit of degrees Celsius (C). 
    /// An application can take a pace value using any scale the application 
    /// chooses and can store the user-entered value as the display value, 
    /// but the temperature value must be converted to degrees Celsius to be stored in 
    /// HealthVault.
    /// </remarks>
    /// 
    public class TemperatureMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TemperatureMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public TemperatureMeasurement() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TemperatureMeasurement"/> 
        /// class with the specified value in degrees Celsius.
        /// </summary>
        /// 
        /// <param name="celsius">
        /// The temperature in degrees Celsius.
        /// </param>
        /// 
        public TemperatureMeasurement(double celsius)
            : base(celsius)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TemperatureMeasurement"/> 
        /// class with the specified value in degrees Celsius and optional display value.
        /// </summary>
        /// 
        /// <param name="celsius">
        /// The temperature in degrees Celsius.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the temperature. This should contain the
        /// exact temperature as entered by the user even if it uses some
        /// other unit of measure besides degrees Celsius. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public TemperatureMeasurement(double celsius, DisplayValue displayValue)
            : base(celsius, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal temperature value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The temperature measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "TemperatureNotPositive");
        }

        /// <summary> 
        /// Populates the data for the temperature from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the temperature.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("celsius").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the temperature to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the temperature to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "celsius", 
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the temperature in the base units.
        /// </summary>
        /// 
        /// <returns>
        /// The temperature as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
