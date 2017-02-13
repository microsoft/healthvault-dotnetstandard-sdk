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

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents an insulin injection measurement.
    /// </summary>
    /// 
    /// <remarks>
    /// In HealthVault, insulin injection readings have values and display values. 
    /// All values are stored in a base unit of IE. An application can 
    /// take an insulin injection value using any scale the application chooses and 
    /// can store the user-entered value as the display value, but the insulin
    /// injection value must be converted to IE to be stored in HealthVault.
    /// </remarks>
    /// 
    public class InsulinInjectionMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public InsulinInjectionMeasurement() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/> 
        /// class with the specified value in IE (1/100mL).
        /// </summary>
        /// 
        /// <param name="iE">
        /// The insulin injection measurement in 1/100mL.
        /// </param>
        /// 
        public InsulinInjectionMeasurement(double iE) 
            : base(iE)
        {
        }


        /// <summary>
        /// Creates a new instance of the <see cref="InsulinInjectionMeasurement"/> 
        /// class with the specified value in IE (1/100mL) and display value.
        /// </summary>
        /// 
        /// <param name="iE">
        /// The insulin injection measurement in IE (1/100mL).
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the insulin injection measurement. This should 
        /// contain the exact measurement as entered by the user even if it 
        /// uses some other unit of measure besides IE. The display value
        /// <see cref="DisplayValue.Units"/> and 
        /// <see cref="DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public InsulinInjectionMeasurement(double iE, DisplayValue displayValue)
            : base(iE, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal insulin injection measurement in 
        /// IE (1/100mL).
        /// </summary>
        /// 
        /// <param name="value">
        /// The insulin injection measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
            Validator.ThrowArgumentOutOfRangeIf(value <= 0.0, "value", "InsulinInjectionNotPositive");
        }

        /// <summary> 
        /// Populates the data for the insulin injection from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the insulin injection.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("IE").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the insulin injection to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the insulin injection to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "IE", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the insulin injection in the base 
        /// units.
        /// </summary>
        /// 
        /// <returns>
        /// The insulin injection as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
