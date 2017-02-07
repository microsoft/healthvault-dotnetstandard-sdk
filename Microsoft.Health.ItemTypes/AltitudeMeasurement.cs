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
    /// Represents a altitude measurement and display.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, altitude measurements have values and display values. 
    /// All values are stored in a base unit of meters. 
    /// An application can take an altitude value using any scale the application 
    /// chooses and can store the user-entered value as the display value, 
    /// but the altitude value must be 
    /// converted to meters to be stored in HealthVault.
    /// </remarks>
    /// 
    public class AltitudeMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AltitudeMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public AltitudeMeasurement() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AltitudeMeasurement"/> 
        /// class with the specified value in meters.
        /// </summary>
        /// 
        /// <param name="meters">
        /// The altitude in meters.
        /// </param>
        /// 
        public AltitudeMeasurement(double meters)
            : base(meters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AltitudeMeasurement"/> 
        /// class with the specified value in meters and optional display value.
        /// </summary>
        /// 
        /// <param name="meters">
        /// The altitude in meters.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the altitude. This should contain the
        /// exact altitude as entered by the user even if it uses some
        /// other unit of measure besides meters. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public AltitudeMeasurement(double meters, DisplayValue displayValue)
            : base(meters, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal altitude value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The altitude measurement.
        /// </param>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
        }

        /// <summary> 
        /// Populates the data for the altitude from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the altitude.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("m").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the altitude to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the altitude to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "m", 
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the altitude in the base units.
        /// </summary>
        /// 
        /// <returns>
        /// The altitude as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
