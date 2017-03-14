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
    /// Represents a lab test result range value.
    /// </summary>
    ///
    /// <remarks>
    /// A range consists of minimum and/or maximum values.
    /// An open-ended range may be created by omitting the minimum or maximum value.
    ///
    /// For example, "greater than 3.5" is created by setting the minimum value to 3.5 and
    /// the maximum value to null.
    /// </remarks>
    ///
    ///
    public class TestResultRangeValue : ItemBase
    {
        /// <summary>
        /// Populates the data for the range from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the range.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.minimum = XPathHelper.GetOptNavValueAsDouble(navigator, "minimum-range");
            this.maximum = XPathHelper.GetOptNavValueAsDouble(navigator, "maximum-range");
        }

        /// <summary>
        /// Writes the range data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the range data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the range data to.
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

            // minimum
            XmlWriterHelper.WriteOptDouble(
                writer,
                "minimum-range",
                this.minimum);

            // maximum
            XmlWriterHelper.WriteOptDouble(
                writer,
                "maximum-range",
                this.maximum);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        ///
        /// <value>
        /// A value of type double? that represents the minimum value of the range.
        /// </value>
        public double? Minimum
        {
            get { return this.minimum; }
            set { this.minimum = value; }
        }

        private double? minimum;

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        ///
        /// <value>
        /// A value of type double? that represents the maximum value of the range.
        /// </value>
        public double? Maximum
        {
            get { return this.maximum; }
            set { this.maximum = value; }
        }

        private double? maximum;

        /// <summary>
        /// Gets a string representation of the test result range value.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the test result range value item.
        /// </returns>
        ///
        public override string ToString()
        {
            string minimumString = this.minimum != null ? this.minimum.ToString() : string.Empty;
            string maximumString = this.maximum != null ? this.maximum.ToString() : string.Empty;

            return string.Format(
                ResourceRetriever.GetResourceString(
                        "TestResultRangeValueToStringFormat"),
                minimumString,
                maximumString);
        }
    }
}
