// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
            this.When = when;
            this.Size = size;
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

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.headers.Clear();
            foreach (XPathNavigator nav in itemNav.Select("headers"))
            {
                string name = nav.SelectSingleNode("name").Value;
                string value = nav.SelectSingleNode("value").Value;

                if (!this.headers.ContainsKey(name))
                {
                    this.headers.Add(name, new Collection<string>());
                }

                this.headers[name].Add(value);
            }

            this.size = itemNav.SelectSingleNode("size").ValueAsLong;
            this.summary = XPathHelper.GetOptNavValue(itemNav, "summary");
            this.htmlBlobName = XPathHelper.GetOptNavValue(itemNav, "html-blob-name");
            this.textBlobName = XPathHelper.GetOptNavValue(itemNav, "text-blob-name");

            this.attachments.Clear();
            foreach (XPathNavigator nav in itemNav.Select("attachments"))
            {
                MessageAttachment messageAttachment = new MessageAttachment();
                messageAttachment.ParseXml(nav);
                this.attachments.Add(messageAttachment);
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
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);

            writer.WriteStartElement("message");

            this.when.WriteXml("when", writer);

            foreach (string key in this.headers.Keys)
            {
                Collection<string> values = this.headers[key];
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

            writer.WriteElementString("size", this.size.ToString(CultureInfo.InvariantCulture));
            XmlWriterHelper.WriteOptString(writer, "summary", this.summary);
            XmlWriterHelper.WriteOptString(writer, "html-blob-name", this.htmlBlobName);
            XmlWriterHelper.WriteOptString(writer, "text-blob-name", this.textBlobName);

            foreach (MessageAttachment messageAttachment in this.attachments)
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
                return this.when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);

                this.when = value;
            }
        }

        private HealthServiceDateTime when;

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
        public Dictionary<string, Collection<string>> Headers => this.headers;

        private readonly Dictionary<string, Collection<string>> headers = new Dictionary<string, Collection<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the size of the message in bytes.
        /// </summary>
        ///
        public long Size
        {
            get
            {
                return this.size;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Size), Resources.MessageSizeOutOfRange);
                }

                this.size = value;
            }
        }

        private long size;

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
                return this.summary;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Summary");

                this.summary = value;
            }
        }

        private string summary;

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
                return this.htmlBlobName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "HtmlBlobName");

                this.htmlBlobName = value;
            }
        }

        private string htmlBlobName;

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
                return this.textBlobName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "TextBlobName");

                this.textBlobName = value;
            }
        }

        private string textBlobName;

        private string GetHeaderProperty(string headerKeyName)
        {
            if (!this.headers.ContainsKey(headerKeyName) ||
                this.headers[headerKeyName].Count == 0)
            {
                return null;
            }

            return this.headers[headerKeyName][0];
        }

        private void SetHeaderProperty(string headerKeyName, string value)
        {
            if (value == null)
            {
                this.headers.Remove(headerKeyName);
            }
            else
            {
                if (!this.headers.ContainsKey(headerKeyName))
                {
                    this.headers.Add(headerKeyName, new Collection<string>());
                }

                this.headers[headerKeyName].Clear();
                this.headers[headerKeyName].Add(value);
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
            get { return this.GetHeaderProperty("Subject"); }
            set { this.SetHeaderProperty("Subject", value); }
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
            get { return this.GetHeaderProperty("From"); }
            set { this.SetHeaderProperty("From", value); }
        }

        /// <summary>
        /// Gets the collection of attachments to the message.
        /// </summary>
        ///
        /// <remarks>
        /// If there are no attachments the collection will be empty.
        /// </remarks>
        ///
        public Collection<MessageAttachment> Attachments => this.attachments;

        private readonly Collection<MessageAttachment> attachments = new Collection<MessageAttachment>();

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

            builder.Append(this.when);

            if (this.From != null)
            {
                builder.Append(Resources.ListSeparator);
                builder.Append(this.From);
            }

            if (this.Subject != null)
            {
                builder.Append(Resources.ListSeparator);
                builder.Append(this.Subject);
            }

            return builder.ToString();
        }
    }
}
