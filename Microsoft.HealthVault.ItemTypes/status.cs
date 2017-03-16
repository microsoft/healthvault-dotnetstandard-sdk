// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores status information about a specific thing.
    /// </summary>
    ///
    /// <remarks>
    /// Each status is related to one or more things which are stored as related items.
    ///
    /// For example, the HealthVault shell creates a status item to indicate that a CCR or CCD document has
    /// been reconciled, and that document is linked using a related item.
    /// </remarks>
    ///
    public class Status : ThingBase
    {
        /// <summary>
        /// Initializes an instance of the <see cref="Status"/> class,
        /// with default values.
        /// </summary>
        ///
        public Status()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="Status"/> class,
        /// with a specified status type.
        /// </summary>
        ///
        /// <param name="statusType">
        /// The specific type of status.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="statusType"/> is <b>null</b>.
        /// </exception>
        ///
        public Status(CodableValue statusType)
            : base(TypeId)
        {
            this.StatusType = statusType;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("d33a32b2-00de-43b8-9f2a-c4c7e9f580ec");

        /// <summary>
        /// Populates this <see cref="Status"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the status data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a status node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("status");

            Validator.ThrowInvalidIfNull(itemNav, "StatusUnexpectedNode");

            this.statusType = new CodableValue();
            this.statusType.ParseXml(itemNav.SelectSingleNode("status-type"));

            this.text =
                XPathHelper.GetOptNavValue(itemNav, "text");
        }

        /// <summary>
        /// Writes the status data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the status data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="StatusType"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.statusType, "StatusTypeNotSet");
            Validator.ThrowSerializationIfNull(this.statusType.Text, "CodableValueNullText");

            writer.WriteStartElement("status");

            this.statusType.WriteXml("status-type", writer);

            XmlWriterHelper.WriteOptString(
                writer,
                "text",
                this.text);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the status type of a status.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        ///
        public CodableValue StatusType
        {
            get { return this.statusType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "StatusType", "StatusTypeMandatory");
                this.statusType = value;
            }
        }

        private CodableValue statusType = new CodableValue();

        /// <summary>
        /// Gets or sets additional information about the status.
        /// </summary>
        ///
        public string Text
        {
            get { return this.text; }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Text");
                this.text = value;
            }
        }

        private string text;

        /// <summary>
        /// Gets a string representation of the status instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the status item.
        /// </returns>
        ///
        public override string ToString()
        {
            string value = this.statusType.Text;

            if (this.text != null)
            {
                value +=
                   ResourceRetriever.GetResourceString("ListSeparator") +
                   this.text;
            }

            return value;
        }
    }
}
