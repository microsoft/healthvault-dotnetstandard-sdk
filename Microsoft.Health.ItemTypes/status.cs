// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Stores status information about a specific health record item. 
    /// </summary>
    /// 
    /// <remarks>
    /// Each status is related to one or more health record items which are stored as related items.
    ///
    /// For example, the HealthVault shell creates a status item to indicate that a CCR or CCD document has 
    /// been reconciled, and that document is linked using a related item. 
    /// </remarks>
    /// 
    public class Status : HealthRecordItem
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
            StatusType = statusType;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
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

            _statusType = new CodableValue();
            _statusType.ParseXml(itemNav.SelectSingleNode("status-type"));

            _text =
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="StatusType"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_statusType, "StatusTypeNotSet");
            Validator.ThrowSerializationIfNull(_statusType.Text, "CodableValueNullText");

            writer.WriteStartElement("status");

            _statusType.WriteXml("status-type",writer);

            XmlWriterHelper.WriteOptString(
                writer,
                "text",
                _text);

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
        // FXCop thinks that CodableValue is a collection, so it throws this error. 
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public CodableValue StatusType
        {
            get { return _statusType; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "StatusType", "StatusTypeMandatory");
                _statusType = value;
            }
        }
        private CodableValue _statusType = new CodableValue();

        /// <summary>
        /// Gets or sets additional information about the status.
        /// </summary>
        /// 
        public string Text
        {
            get { return _text; }
            set 
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Text");
                _text = value;
            }
        }
        private string _text;

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
            string value = _statusType.Text;

            if (_text != null)
            {
                value += 
                   ResourceRetriever.GetResourceString("ListSeparator") +
                   _text;
            }

            return value;
        }
    }
}
