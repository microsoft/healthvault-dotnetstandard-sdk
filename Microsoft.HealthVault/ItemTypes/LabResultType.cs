// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a clinical value within lab result.
    /// </summary>
    ///
    public class LabResultType : ItemBase
    {
        /// <summary>
        /// Populates the data for the lab result type from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the lab result type.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.value = XPathHelper.GetOptNavValueAsDouble(navigator, "value");
            this.unit = XPathHelper.GetOptNavValue<CodableValue>(navigator, "unit");
            this.referenceRange = XPathHelper.GetOptNavValue<DoubleRange>(navigator, "reference-range");
            this.toxicRange = XPathHelper.GetOptNavValue<DoubleRange>(navigator, "toxic-range");
            this.textValue = XPathHelper.GetOptNavValue(navigator, "text-value");

            this.flag.Clear();
            XPathNodeIterator flagIterator = navigator.Select("flag");
            foreach (XPathNavigator flagNav in flagIterator)
            {
                CodableValue flagValue = new CodableValue();
                flagValue.ParseXml(flagNav);
                this.flag.Add(flagValue);
            }
        }

        /// <summary>
        /// Writes the lab result type data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the lab result type.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lab result type to.
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOptDouble(writer, "value", this.value);
            XmlWriterHelper.WriteOpt(writer, "unit", this.unit);
            XmlWriterHelper.WriteOpt(writer, "reference-range", this.referenceRange);
            XmlWriterHelper.WriteOpt(writer, "toxic-range", this.toxicRange);
            XmlWriterHelper.WriteOptString(writer, "text-value", this.textValue);

            foreach (CodableValue flagValue in this.flag)
            {
                flagValue.WriteXml("flag", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the value for the lab result.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="double"/> representing the value of the lab result, or <b>null</b> if
        /// no lab result is available.
        /// </value>
        ///
        public double? Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private double? value;

        /// <summary>
        /// Gets or sets the unit of measure for the <see cref="Value"/>.
        /// </summary>
        ///
        /// <remarks>
        /// The unit of measure may defined by the HealthVault dictionary or the
        /// <see cref="CodableValue.Text"/> property may be set to any unit of measure.
        /// </remarks>
        ///
        public CodableValue Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }

        private CodableValue unit;

        /// <summary>
        /// Gets or sets the reference range for the lab result type.
        /// </summary>
        ///
        /// <value>
        /// The "normal" range of values for this lab result type or <b>null</b> if the reference
        /// range is not available.
        /// </value>
        ///
        public DoubleRange ReferenceRange
        {
            get { return this.referenceRange; }
            set { this.referenceRange = value; }
        }

        private DoubleRange referenceRange;

        /// <summary>
        /// Gets or sets the toxic range for the lab result type.
        /// </summary>
        ///
        /// <value>
        /// The toxic range of values for this lab result type or <b>null</b> if the toxic
        /// range is not available.
        /// </value>
        ///
        public DoubleRange ToxicRange
        {
            get { return this.toxicRange; }
            set { this.toxicRange = value; }
        }

        private DoubleRange toxicRange;

        /// <summary>
        /// Gets or set the text representation of the value.
        /// </summary>
        ///
        /// <value>
        /// A string representation of the <see cref="Value"/> or <b>null</b> if the text value is
        /// not available.
        /// </value>
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
        /// Gets a collection of the flags associated with the lab result type.
        /// </summary>
        ///
        /// <value>
        /// The flags for the lab result type are generally values like "normal", "critical", "high",
        /// "low", etc. which are defined by the "lab-result-flag" HealthVault vocabulary.
        /// </value>
        ///
        public Collection<CodableValue> Flags => this.flag;

        private readonly Collection<CodableValue> flag = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the lab result type.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the lab result type.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (this.Value != null && this.Unit != null)
            {
                result.AppendFormat(
                    Resources.LabResultTypeToStringFormatValueAndUnit,
                    this.Value,
                    this.Unit.ToString());
            }
            else if (this.Value != null)
            {
                result.Append(this.Value);
            }

            if (!string.IsNullOrEmpty(this.TextValue))
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.Append(this.TextValue);
            }

            foreach (CodableValue flag in this.flag)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.Append(flag);
            }

            return result.ToString();
        }
    }
}
