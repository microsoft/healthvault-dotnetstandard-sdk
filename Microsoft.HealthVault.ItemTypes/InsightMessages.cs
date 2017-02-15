// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the collection of message strings associated with this Insight.
    /// </summary>
    public class InsightMessages : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="InsightMessages"/> class with
        /// default values.
        /// </summary>
        ///
        public InsightMessages()
            : base()
        {
        }

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

            _regular = XPathHelper.GetOptNavValue(navigator, "regular");
            _short = XPathHelper.GetOptNavValue(navigator, "short");
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
            XmlWriterHelper.WriteOptString(writer, "regular", _regular);

            // <short>
            XmlWriterHelper.WriteOptString(writer, "short", _short);

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
            return _regular;
        }

        /// <summary>
        /// Gets or sets the regular message for this insight.
        /// </summary>
        public string Regular
        {
            get { return _regular; }

            set { _regular = value; }
        }

        private string _regular;

        /// <summary>
        /// Gets or sets the short message for this insight.
        /// </summary>
        public string Short
        {
            get { return _short; }

            set { _short = value; }
        }

        private string _short;
    }
}