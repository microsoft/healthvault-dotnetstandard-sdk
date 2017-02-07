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
    /// Represents a length value and display.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, lengths have values and display values. All values are
    /// stored in a base unit of meters. An application can take a length
    /// value using any scale the application chooses and can store the 
    /// user-entered value as the display value, but the length value must be 
    /// converted to meters to be stored in HealthVault.
    /// </remarks>
    /// 
    public class Length : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with empty values.
        /// </summary>
        /// 
        public Length() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with the 
        /// specified value in meters.
        /// </summary>
        /// 
        /// <param name="meters">
        /// The length in meters.
        /// </param>
        /// 
        public Length(double meters) : base(meters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Length"/> class with the 
        /// specified value in meters and optional display value.
        /// </summary>
        /// 
        /// <param name="meters">
        /// The length in meters.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the length. This should contain the
        /// exact length as entered by the user even if it uses some
        /// other unit of measure besides meters. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public Length(double meters, DisplayValue displayValue)
            : base(meters, displayValue)
        {
        }

        /// <summary>
        /// Gets or sets the value of the length in meters.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the length.
        /// </value>
        /// 
        /// <remarks>
        /// The value must be in meters. The <see cref="DisplayValue"/> can
        /// be used to store the user-entered value in a scale other than
        /// metric.
        /// </remarks>
        /// 
        public double Meters
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Verifies that the value is a legal length value in meters.
        /// </summary>
        /// 
        /// <param name="value">
        /// The length measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "LengthNotPositive");
        }

        /// <summary> 
        /// Populates the data for the length from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the length.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("m").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the length to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the length to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "m", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the length in the base units.
        /// </summary>
        /// 
        /// <returns>
        /// The length as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return String.Format(
                ResourceRetriever.GetResourceString("LengthToStringFormatMeters"),
                value.ToString(CultureInfo.CurrentCulture));
        }
    }
}
