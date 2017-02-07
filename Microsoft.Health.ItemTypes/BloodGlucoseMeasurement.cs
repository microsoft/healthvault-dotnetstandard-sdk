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
    /// Represents a blood glucose measurement.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, blood glucose readings have values and display values. 
    /// All values are stored in a base unit of mmol/L. An application can 
    /// take a blood glucose value using any scale the application chooses and 
    /// can store the user-entered value as the display value, but the blood
    /// glucose value must be converted to mmol/L to be stored in HealthVault.
    /// </remarks>
    /// 
    public class BloodGlucoseMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BloodGlucoseMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public BloodGlucoseMeasurement() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BloodGlucoseMeasurement"/> 
        /// class with the specified value in millimoles per liter (mmol/L).
        /// </summary>
        /// 
        /// <param name="millimolesPerLiter">
        /// The blood glucose measurement in millimoles per liter.
        /// </param>
        /// 
        public BloodGlucoseMeasurement(double millimolesPerLiter) 
            : base(millimolesPerLiter)
        {
        }


        /// <summary>
        /// Creates a new instance of the <see cref="BloodGlucoseMeasurement"/> 
        /// class with the specified value in millimoles per liter (mmol/L) 
        /// and display value.
        /// </summary>
        /// 
        /// <param name="millimolesPerLiter">
        /// The blood glucose measurement in millimoles per liter (mmol/L).
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the blood glucose measurement. This should 
        /// contain the exact measurement as entered by the user, even if it 
        /// uses some other unit of measure besides mmol/L. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public BloodGlucoseMeasurement(
            double millimolesPerLiter, 
            DisplayValue displayValue)
            : base(millimolesPerLiter, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal blood glucose measurement in 
        /// millimoles per liter (mmol/L).
        /// </summary>
        /// 
        /// <param name="value">
        /// The blood glucose measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "BloodGlucoseNotPositive");
        }

        /// <summary> 
        /// Populates the data for the blood glucose from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the blood glucose.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("mmolPerL").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the blood glucose to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the blood glucose to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "mmolPerL", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the blood glucose in the base units.
        /// </summary>
        /// <returns>
        /// The blood glucose as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
