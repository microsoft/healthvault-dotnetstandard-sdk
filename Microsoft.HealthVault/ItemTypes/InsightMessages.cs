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
    /// Represents the collection of message strings associated with this Insight.
    /// </summary>
    public class InsightMessages : ItemBase
    {
        /// <summary>
        /// Populates the data for insight messages from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the insight messages type.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.regular = XPathHelper.GetOptNavValue(navigator, "regular");
            this.@short = XPathHelper.GetOptNavValue(navigator, "short");
        }

        /// <summary>
        /// Writes the insight messages data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the insight messages type.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the insight messages type to.
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

            // <regular>
            XmlWriterHelper.WriteOptString(writer, "regular", this.regular);

            // <short>
            XmlWriterHelper.WriteOptString(writer, "short", this.@short);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of insight messages.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the insight messages type.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.regular;
        }

        /// <summary>
        /// Gets or sets the regular message for this insight.
        /// </summary>
        public string Regular
        {
            get { return this.regular; }

            set { this.regular = value; }
        }

        private string regular;

        /// <summary>
        /// Gets or sets the short message for this insight.
        /// </summary>
        public string Short
        {
            get { return this.@short; }

            set { this.@short = value; }
        }

        private string @short;
    }
}