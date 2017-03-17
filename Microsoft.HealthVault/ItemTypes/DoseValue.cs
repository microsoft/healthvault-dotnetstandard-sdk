// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
            this.ExactDose = exactDosage;
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

            this.description =
                XPathHelper.GetOptNavValue(navigator, "description");

            this.exactDose =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "exact-dose");

            this.minDose =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "min-dose");

            this.maxDose =
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
                this.description);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "exact-dose",
                this.exactDose);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "min-dose",
                this.minDose);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "max-dose",
                this.maxDose);

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
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

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
            get { return this.exactDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.ExactDose), Resources.DoseValueMustBePositive);
                }

                this.exactDose = value;
            }
        }

        private double? exactDose;

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
            get { return this.minDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.MinDose), Resources.DoseValueMustBePositive);
                }

                this.minDose = value;
            }
        }

        private double? minDose;

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
            get { return this.maxDose; }

            set
            {
                if (value != null && value.Value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.MaxDose), Resources.DoseValueMustBePositive);
                }

                this.maxDose = value;
            }
        }

        private double? maxDose;

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
            if (this.Description != null)
            {
                return this.Description;
            }

            if (this.ExactDose != null)
            {
                return this.ExactDose.Value.ToString();
            }

            if (this.MinDose != null && this.MaxDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMinAndMax,
                    this.MinDose.Value,
                    this.MaxDose.Value);
            }

            if (this.MinDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMin,
                    this.MinDose.Value);
            }

            if (this.MaxDose != null)
            {
                return string.Format(
                        Resources.DoseValueToStringDoseMax,
                    this.MaxDose.Value);
            }

            return string.Format(
                    Resources.DoseValueTOStringDoseNeither);
        }
    }
}
