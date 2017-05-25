// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            StatusType = statusType;
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.StatusUnexpectedNode);

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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="StatusType"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_statusType, Resources.StatusTypeNotSet);
            Validator.ThrowSerializationIfNull(_statusType.Text, Resources.CodableValueNullText);

            writer.WriteStartElement("status");

            _statusType.WriteXml("status-type", writer);

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
        public CodableValue StatusType
        {
            get { return _statusType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(StatusType), Resources.StatusTypeMandatory);
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
                   Resources.ListSeparator +
                   _text;
            }

            return value;
        }
    }
}
