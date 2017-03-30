// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a vital sign result type.
    /// </summary>
    ///
    public class VitalSignsResultType : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VitalSignsResultType"/> class with
        /// default values.
        /// </summary>
        ///
        public VitalSignsResultType()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VitalSignsResultType"/> class
        /// with the specified name.
        /// </summary>
        ///
        /// <param name="title">
        /// The name of the vital sign result.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="title"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public VitalSignsResultType(CodableValue title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the vital sign result information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.title = new CodableValue();
            this.title.ParseXml(navigator.SelectSingleNode("title"));

            this.value =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "value");

            // <unit>
            this.unit =
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator,
                    "unit");

            // <reference-minimum>
            this.referenceMinimum =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "reference-minimum");

            // <reference-maximum>
            this.referenceMaximum =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "reference-maximum");

            // <text-value>
            this.textValue =
                XPathHelper.GetOptNavValue(
                    navigator,
                    "text-value");

            // <flag>
            this.flag =
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator,
                    "flag");
        }

        /// <summary>
        /// Writes the XML representation of the vital sign result into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the vital sign result.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the vital sign result should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Title"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.title, Resources.VitalSignResultTitleNotSet);
            Validator.ThrowSerializationIfNull(this.title.Text, Resources.CodableValueNullText);

            writer.WriteStartElement(nodeName);

            // <title>
            this.title.WriteXml("title", writer);

            // <value>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "value",
                this.value);

            // <unit>
            XmlWriterHelper.WriteOpt(
                writer,
                "unit",
                this.unit);

            // <reference-minimum>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "reference-minimum",
                this.referenceMinimum);

            // <reference-maximum>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "reference-maximum",
                this.referenceMaximum);

            // <text-value>
            XmlWriterHelper.WriteOptString(
                writer,
                "text-value",
                this.textValue);

            // <flag>
            XmlWriterHelper.WriteOpt(
                writer,
                "flag",
                this.flag);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the title for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing
        /// the title.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Title
        {
            get { return this.title; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Title), Resources.VitalSignResultTitleMandatory);
                this.title = value;
            }
        }

        private CodableValue title = new CodableValue();

        /// <summary>
        /// Gets or sets the vital sign value.
        /// </summary>
        ///
        /// <value>
        /// A number representing the vital sign value.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the value should not
        /// be stored.
        /// </remarks>
        ///
        public double? Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private double? value;

        /// <summary>
        /// Gets or sets the unit for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing
        /// the unit.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the unit should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }

        private CodableValue unit;

        /// <summary>
        /// Gets or sets the reference minimum for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A number representing the reference minimum.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the reference minimum should not be
        /// stored.
        /// </remarks>
        ///
        public double? ReferenceMinimum
        {
            get { return this.referenceMinimum; }
            set { this.referenceMinimum = value; }
        }

        private double? referenceMinimum;

        /// <summary>
        /// Gets or sets the reference maximum for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A number representing the reference maximum.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the reference maximum should not be
        /// stored.
        /// </remarks>
        ///
        public double? ReferenceMaximum
        {
            get { return this.referenceMaximum; }
            set { this.referenceMaximum = value; }
        }

        private double? referenceMaximum;

        /// <summary>
        /// Gets or sets the text value for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A string representing the text value.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the text value should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string TextValue
        {
            get { return this.textValue; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "TextValue");
                this.textValue = value;
            }
        }

        private string textValue;

        /// <summary>
        /// Gets or sets the flag for the vital signs.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing
        /// the flag.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the flag should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue Flag
        {
            get { return this.flag; }
            set { this.flag = value; }
        }

        private CodableValue flag;

        /// <summary>
        /// Gets a string representation of the vital signs result type.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the vital signs result type.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            if (this.title != null)
            {
                result.Append(this.title);
            }

            if (this.value != null)
            {
                result.AppendFormat(
                    Resources.VitalSignResultToStringFormatValue,
                    this.value.Value);
            }

            if (this.unit != null)
            {
                result.Append(" ");

                result.AppendFormat(
                    Resources.VitalSignResultToStringFormatUnit,
                    this.unit.ToString());
            }

            return result.ToString();
        }
    }
}
