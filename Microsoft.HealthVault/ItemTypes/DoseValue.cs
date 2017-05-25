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
    /// Represents a medicinal dose value.
    /// </summary>
    ///
    public class DoseValue : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DoseValue"/> class with empty values.
        /// </summary>
        ///
        public DoseValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DoseValue"/> class with
        /// the specified exact dosage.
        /// </summary>
        ///
        /// <param name="exactDosage">
        /// The exact dosage.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="exactDosage"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public DoseValue(double exactDosage)
        {
            ExactDose = exactDosage;
        }

        /// <summary>
        /// Populates the data for the dose value from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the dose value.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is null.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _description =
                XPathHelper.GetOptNavValue(navigator, "description");

            _exactDose =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "exact-dose");

            _minDose =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "min-dose");

            _maxDose =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "max-dose");
        }

        /// <summary>
        /// Writes the dose to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the dose.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the dose to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is null or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is null.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOptString(
                writer,
                "description",
                _description);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "exact-dose",
                _exactDose);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "min-dose",
                _minDose);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "max-dose",
                _maxDose);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets a description of the dosage.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no description then the value may be set to null.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get { return _description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }

        private string _description;

        /// <summary>
        /// Gets or sets an exact value for the dosage.
        /// </summary>
        ///
        /// <value>
        /// A number representing the exact dosage value.
        /// </value>
        ///
        /// <remarks>
        /// If there is no exact value, then the value can be set to null.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public double? ExactDose
        {
            get { return _exactDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ExactDose), Resources.DoseValueMustBePositive);
                }

                _exactDose = value;
            }
        }

        private double? _exactDose;

        /// <summary>
        /// Gets or sets a minimum value for the dosage.
        /// </summary>
        ///
        /// <value>
        /// A number representing the minimum dosage value.
        /// </value>
        ///
        /// <remarks>
        /// If there is no minimum value, then the value can be set to null.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public double? MinDose
        {
            get { return _minDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinDose), Resources.DoseValueMustBePositive);
                }

                _minDose = value;
            }
        }

        private double? _minDose;

        /// <summary>
        /// Gets or sets a maximum value for the dosage.
        /// </summary>
        ///
        /// <value>
        /// A number representing the maximum dosage value.
        /// </value>
        ///
        /// <remarks>
        /// If there is no maximum value, then the value can be set to null.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public double? MaxDose
        {
            get { return _maxDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxDose), Resources.DoseValueMustBePositive);
                }

                _maxDose = value;
            }
        }

        private double? _maxDose;

        /// <summary>
        /// Gets a string representation of the dose value.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the dose value.
        /// </returns>
        ///
        public override string ToString()
        {
            if (Description != null)
            {
                return Description;
            }

            if (ExactDose != null)
            {
                return ExactDose.Value.ToString();
            }

            if (MinDose != null && MaxDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMinAndMax,
                    MinDose.Value,
                    MaxDose.Value);
            }

            if (MinDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMin,
                    MinDose.Value);
            }

            if (MaxDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMax,
                    MaxDose.Value);
            }

            return string.Format(
                    Resources.DoseValueTOStringDoseNeither);
        }
    }
}
