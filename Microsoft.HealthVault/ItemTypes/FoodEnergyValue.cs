// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
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
        public FoodEnergyValue(double calories)
            : base(calories)
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
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
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
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.WeightNotPositive);
            }
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
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.FoodEnergyValueFormat,
                Calories.ToString(CultureInfo.CurrentCulture));
        }
    }
}
