// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A single measurement of body composition.
    /// </summary>
    ///
    public class BodyCompositionValue : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="BodyCompositionValue"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the body composition value data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // mass-value (t:weight-value)
            _massValue = XPathHelper.GetOptNavValue<WeightValue>(navigator, "mass-value");

            // percent-value (t:percentage)
            XPathNavigator percentValueNav = navigator.SelectSingleNode("percent-value");
            if (percentValueNav != null)
            {
                _percentValue = percentValueNav.ValueAsDouble;
            }
        }

        /// <summary>
        /// Writes the body composition value data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the body composition value item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the body composition value data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <body-composition-value>
            writer.WriteStartElement(nodeName);

            // mass-value
            XmlWriterHelper.WriteOpt(
                writer,
                "mass-value",
                _massValue);

            // percent-value
            XmlWriterHelper.WriteOptDouble(
                writer,
                "percent-value",
                _percentValue);

            // </body-composition-value>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets a body composition measurement stored as a mass.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: 45 Kg, 15 lbs.
        /// </remarks>
        ///
        public WeightValue MassValue
        {
            get { return _massValue; }
            set { _massValue = value; }
        }

        private WeightValue _massValue;

        /// <summary>
        /// Gets or sets a body composition measurement stored as a percentage.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: 0.37 (37%), 0.075 (7.5%).
        /// </remarks>
        ///
        public double? PercentValue
        {
            get { return _percentValue; }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PercentValue), Resources.BodyCompositionValuePercentValueOutOfRange);
                }

                _percentValue = value;
            }
        }

        private double? _percentValue;

        /// <summary>
        /// Gets a string representation of BodyCompositionValue.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the BodyCompositionValue.
        /// </returns>
        ///
        public override string ToString()
        {
            if (_massValue != null && _percentValue != null)
            {
                return string.Format(
                        Resources.BodyCompositionValueToStringFormatMassAndPercent,
                        _massValue.ToString(),
                        _percentValue * 100);
            }

            if (_massValue != null)
            {
                return _massValue.ToString();
            }

            if (_percentValue != null)
            {
                return string.Format(
                    Resources.Percent,
                    _percentValue * 100);
            }

            return string.Empty;
        }
    }
}
