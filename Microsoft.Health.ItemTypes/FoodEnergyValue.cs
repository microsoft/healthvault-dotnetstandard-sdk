// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Information related to the amount of nutrient consumed.
    /// </summary>
    ///
    public class FoodEnergyValue : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FoodEnergyValue"/> class with default values.
        /// </summary>
        ///
        public FoodEnergyValue()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="FoodEnergyValue"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="calories">
        /// The amount of calories consumed.
        /// </param>
        ///
        public FoodEnergyValue(double calories) : base(calories)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FoodEnergyValue"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="calories">
        /// The amount of calories consumed.
        /// </param>
        /// <param name="displayValue">
        /// The display value of the energy. This should contain the
        /// exact value as entered by the user, even if it uses some
        /// other unit of measure besides calories. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public FoodEnergyValue(double calories, DisplayValue displayValue)
            : base(calories, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal calories value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The calorie measurement.
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
        /// Populates the data for the energy value from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the energy value.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("calories").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the energy value to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the energy value to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString("calories", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets or sets the amount of calories consumed.
        /// </summary>
        /// 
        /// <remarks>
        /// Calories are measured in kilocalories (kCal).
        /// </remarks>
        ///
        public double Calories
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Gets a string representation of the calories in the base units.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the calories.
        /// </value>
        /// 
        /// <returns>
        /// The calories as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return String.Format(
                CultureInfo.CurrentCulture,
                ResourceRetriever.GetResourceString("FoodEnergyValueFormat"),
                Calories.ToString(CultureInfo.CurrentCulture));
        }
    }
}
