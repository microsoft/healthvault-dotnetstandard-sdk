// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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

            // mass-value (t:weigth-value)
            this.massValue = XPathHelper.GetOptNavValue<WeightValue>(navigator, "mass-value");

            // percent-value (t:percentage)
            XPathNavigator percentValueNav = navigator.SelectSingleNode("percent-value");
            if (percentValueNav != null)
            {
                this.percentValue = percentValueNav.ValueAsDouble;
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
                this.massValue);

            // percent-value
            XmlWriterHelper.WriteOptDouble(
                writer,
                "percent-value",
                this.percentValue);

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
            get { return this.massValue; }
            set { this.massValue = value; }
        }

        private WeightValue massValue;

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
            get { return this.percentValue; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    (value > 1.0) || (value < 0.0),
                    "PercentValue",
                    "BodyCompositionValuePercentValueOutOfRange");
                this.percentValue = value;
            }
        }

        private double? percentValue;

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
            if (this.massValue != null && this.percentValue != null)
            {
                return string.Format(
                        ResourceRetriever.GetResourceString(
                            "BodyCompositionValueToStringFormatMassAndPercent"),
                        this.massValue.ToString(),
                        this.percentValue * 100);
            }

            if (this.massValue != null)
            {
                return this.massValue.ToString();
            }

            if (this.percentValue != null)
            {
                return string.Format(
                    ResourceRetriever.GetResourceString(
                        "Percent"),
                    this.percentValue * 100);
            }

            return string.Empty;
        }
    }
}
