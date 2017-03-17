// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a link to a URL.
    /// </summary>
    ///
    public class Link : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Link"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Link()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Link"/> class with the
        /// specified URL.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the link.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="url"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Link(Uri url)
            : base(TypeId)
        {
            this.Url = url;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("d4b48e6b-50fa-4ba8-ac73-7d64a68dc328");

        /// <summary>
        /// Populates this <see cref="Link"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the link data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a link node.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The url element in the XML is not properly formatted.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator linkNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("link");

            Validator.ThrowInvalidIfNull(linkNav, Resources.LinkUnexpectedNode);

            this.url = new Uri(linkNav.SelectSingleNode("url").Value);

            XPathNavigator titleNav =
                linkNav.SelectSingleNode("title");

            if (titleNav != null)
            {
                this.title = titleNav.Value;
            }
        }

        /// <summary>
        /// Writes the file data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the link data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Url"/> property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.url, Resources.LinkUrlNotSet);

            // <link>
            writer.WriteStartElement("link");

            writer.WriteElementString("url", this.url.OriginalString);

            if (!string.IsNullOrEmpty(this.title))
            {
                writer.WriteElementString(
                    "title", this.title);
            }

            // </link>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the URL of the link.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Uri"/>.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Uri Url
        {
            get { return this.url; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Url), Resources.LinkUrlMandatory);
                this.url = value;
            }
        }

        private Uri url;

        /// <summary>
        /// Gets or sets the link title.
        /// </summary>
        ///
        /// <value>
        /// A string representing the link title.
        /// </value>
        ///
        /// <remarks>
        /// The link title is a friendly name for the link.<br/>
        /// <br/>
        /// This property must be set before the item is created or updated.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Title
        {
            get { return this.title; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Title");
                this.title = value;
            }
        }

        private string title;

        /// <summary>
        /// Gets the display text for the link.
        /// </summary>
        ///
        /// <value>
        /// A string representing the link display text.
        /// </value>
        ///
        /// <remarks>
        /// If the <see cref="Title"/> property is set, its value is returned.
        /// If not, the <see cref="Url"/> value is returned.
        /// </remarks>
        ///
        public string DisplayText
        {
            get
            {
                string result = this.url.ToString();
                if (!string.IsNullOrEmpty(this.title))
                {
                    result = this.title;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a string representation of the link.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the link.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Url.ToString();
        }
    }
}
