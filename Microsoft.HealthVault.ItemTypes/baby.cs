// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a baby associated with a pregnancy or delivery.
    /// </summary>
    ///
    public class Baby : ItemBase
    {
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the baby information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.name = XPathHelper.GetOptNavValue<Name>(navigator, "name");
            this.gender = XPathHelper.GetOptNavValue<CodableValue>(navigator, "gender");
            this.weight = XPathHelper.GetOptNavValue<WeightValue>(navigator, "weight");
            this.length = XPathHelper.GetOptNavValue<Length>(navigator, "length");
            this.head = XPathHelper.GetOptNavValue<Length>(navigator, "head-circumference");
            this.note = XPathHelper.GetOptNavValue(navigator, "note");
        }

        /// <summary>
        /// Writes the XML representation of the baby information into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the baby information.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the baby information should be
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt(writer, "name", this.name);
            XmlWriterHelper.WriteOpt(writer, "gender", this.gender);
            XmlWriterHelper.WriteOpt(writer, "weight", this.weight);
            XmlWriterHelper.WriteOpt(writer, "length", this.length);
            XmlWriterHelper.WriteOpt(writer, "head-circumference", this.head);
            XmlWriterHelper.WriteOptString(writer, "note", this.note);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the baby.
        /// </summary>
        ///
        /// <remarks>
        /// If the name is not known the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Name Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private Name name;

        /// <summary>
        /// Gets or sets the gender of the baby.
        /// </summary>
        ///
        /// <remarks>
        /// If the gender is not known the value should be set to <b>null</b>.
        /// The preferred vocabulary for this value is "gender-types".
        /// </remarks>
        ///
        public CodableValue Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }

        private CodableValue gender;

        /// <summary>
        /// Gets or sets the birth weight of the baby.
        /// </summary>
        ///
        /// <remarks>
        /// If the weight is not known the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Weight
        {
            get { return this.weight; }
            set { this.weight = value; }
        }

        private WeightValue weight;

        /// <summary>
        /// Gets or sets the birth length of the baby.
        /// </summary>
        ///
        /// <remarks>
        /// If the length is not known the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Length Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        private Length length;

        /// <summary>
        /// Gets or sets the circumference of the baby's head.
        /// </summary>
        ///
        /// <remarks>
        /// If the head circumference is not known the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Length HeadCircumference
        {
            get { return this.head; }
            set { this.head = value; }
        }

        private Length head;

        /// <summary>
        /// Gets or sets additional information about the baby.
        /// </summary>
        ///
        /// <remarks>
        /// If there are no additional notes the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Note
        {
            get { return this.note; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                this.note = value;
            }
        }

        private string note;

        /// <summary>
        /// Gets a string representation of the baby information.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the baby information.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (this.Name != null)
            {
                result.Append(this.Name);
            }

            if (this.Weight != null && this.Length != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatWeightAndLength"),
                    this.Weight.ToString(),
                    this.Length.ToString());
            }
            else if (this.Weight != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatWeight"),
                    this.Weight.ToString());
            }
            else if (this.Length != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatLength"),
                    this.Length.ToString());
            }

            return result.ToString();
        }
    }
}
