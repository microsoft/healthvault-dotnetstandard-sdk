// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// The Message type is used to store a multipart mail message, including message text and attachments.
    /// </summary>
    ///
    /// <remarks>
    /// The message is stored in two forms. The "FullMessage" blob contains the message in the native format.
    /// The text of the message is available in the blobs denoted by the 'html-blob-name" and "text-blob-name"
    /// element. Any attachments to the message are described in the "attachments" element.
    ///
    /// The data stored is intended to be compatible with the SendMail Multipart MIME format.
    /// </remarks>
    ///
    public class Message : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Message"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Message()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Message"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time of the message.
        /// </param>
        /// <param name="size">
        /// The size of the message in bytes.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// </exception>
        ///
        public Message(
            HealthServiceDateTime when,
            long size)
        : base(TypeId)
        {
            When = when;
            Size = size;
        }

        /// <summary>
        /// Retrieves the unique identifier for this type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("72dc49e1-1486-4634-b651-ef560ed051e5");

        /// <summary>
        /// Populates the <see cref="Message"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the Message data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a Message node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("message");

            Validator.ThrowInvalidIfNull(itemNav, Resources.MessageUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _headers.Clear();
            foreach (XPathNavigator nav in itemNav.Select("headers"))
            {
                string name = nav.SelectSingleNode("name").Value;
                string value = nav.SelectSingleNode("value").Value;

                if (!_headers.ContainsKey(name))
                {
                    _headers.Add(name, new Collection<string>());
                }

                _headers[name].Add(value);
            }

            _size = itemNav.SelectSingleNode("size").ValueAsLong;
            _summary = XPathHelper.GetOptNavValue(itemNav, "summary");
            _htmlBlobName = XPathHelper.GetOptNavValue(itemNav, "html-blob-name");
            _textBlobName = XPathHelper.GetOptNavValue(itemNav, "text-blob-name");

            _attachments.Clear();
            foreach (XPathNavigator nav in itemNav.Select("attachments"))
            {
                MessageAttachment messageAttachment = new MessageAttachment();
                messageAttachment.ParseXml(nav);
                _attachments.Add(messageAttachment);
            }
        }

        /// <summary>
        /// Writes the XML representation of the Message into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the Message should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);

            writer.WriteStartElement("message");

            _when.WriteXml("when", writer);

            foreach (string key in _headers.Keys)
            {
                Collection<string> values = _headers[key];
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        writer.WriteStartElement("headers");
                        {
                            writer.WriteElementString("name", key);
                            writer.WriteElementString("value", value);
                        }

                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteElementString("size", _size.ToString(CultureInfo.InvariantCulture));
            XmlWriterHelper.WriteOptString(writer, "summary", _summary);
            XmlWriterHelper.WriteOptString(writer, "html-blob-name", _htmlBlobName);
            XmlWriterHelper.WriteOptString(writer, "text-blob-name", _textBlobName);

            foreach (MessageAttachment messageAttachment in _attachments)
            {
                messageAttachment.WriteXml("attachments", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the message.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return _when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);

                _when = value;
            }
        }

        private HealthServiceDateTime _when;

        /// <summary>
        /// Gets the header information associated with this message.
        /// </summary>
        ///
        /// <remarks>
        /// The header information is stored in a dictionary of collections. For example,
        /// Headers["To"] returns the collection of all the "To" headers in the message, or null
        /// if there were not such headers associated with the message.
        /// </remarks>
        ///
        public Dictionary<string, Collection<string>> Headers => _headers;

        private readonly Dictionary<string, Collection<string>> _headers = new Dictionary<string, Collection<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the size of the message in bytes.
        /// </summary>
        ///
        public long Size
        {
            get
            {
                return _size;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(Size), Resources.MessageSizeOutOfRange);
                }

                _size = value;
            }
        }

        private long _size;

        /// <summary>
        /// Gets or sets a summary of the message.
        /// </summary>
        ///
        /// <remarks>
        /// The summary contains the first 512 characters of the message in text format. This information
        /// is used to display the start of the message without having to fetch the blobs that store the
        /// whole message.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Summary
        {
            get
            {
                return _summary;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Summary");

                _summary = value;
            }
        }

        private string _summary;

        /// <summary>
        /// Gets or sets the name of the blob that stores the message in HTML format.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string HtmlBlobName
        {
            get
            {
                return _htmlBlobName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "HtmlBlobName");

                _htmlBlobName = value;
            }
        }

        private string _htmlBlobName;

        /// <summary>
        /// Gets or sets the name of the blob that stores the message in text format.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string TextBlobName
        {
            get
            {
                return _textBlobName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "TextBlobName");

                _textBlobName = value;
            }
        }

        private string _textBlobName;

        private string GetHeaderProperty(string headerKeyName)
        {
            if (!_headers.ContainsKey(headerKeyName) ||
                _headers[headerKeyName].Count == 0)
            {
                return null;
            }

            return _headers[headerKeyName][0];
        }

        private void SetHeaderProperty(string headerKeyName, string value)
        {
            if (value == null)
            {
                _headers.Remove(headerKeyName);
            }
            else
            {
                if (!_headers.ContainsKey(headerKeyName))
                {
                    _headers.Add(headerKeyName, new Collection<string>());
                }

                _headers[headerKeyName].Clear();
                _headers[headerKeyName].Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        ///
        /// <remarks>
        /// The Subject property is equivalent to Headers["Subject"].
        ///
        /// The value of the property is null if there is no subject in the
        /// header collection.
        /// </remarks>
        ///
        public string Subject
        {
            get { return GetHeaderProperty("Subject"); }
            set { SetHeaderProperty("Subject", value); }
        }

        /// <summary>
        /// Gets or sets the origin of the message.
        /// </summary>
        ///
        /// <remarks>
        /// The From property is equivalent to Headers["From"].
        ///
        /// The value of the property is null if there is no from in the
        /// header collection.
        /// </remarks>
        ///
        public string From
        {
            get { return GetHeaderProperty("From"); }
            set { SetHeaderProperty("From", value); }
        }

        /// <summary>
        /// Gets the collection of attachments to the message.
        /// </summary>
        ///
        /// <remarks>
        /// If there are no attachments the collection will be empty.
        /// </remarks>
        ///
        public Collection<MessageAttachment> Attachments => _attachments;

        private readonly Collection<MessageAttachment> _attachments = new Collection<MessageAttachment>();

        /// <summary>
        /// Gets a string representation of the Message.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Message.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(_when);

            if (From != null)
            {
                builder.Append(Resources.ListSeparator);
                builder.Append(From);
            }

            if (Subject != null)
            {
                builder.Append(Resources.ListSeparator);
                builder.Append(Subject);
            }

            return builder.ToString();
        }
    }
}
