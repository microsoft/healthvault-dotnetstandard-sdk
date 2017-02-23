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
    /// An organization entity.
    /// </summary>
    ///
    /// <remarks>
    /// An entity such as a hospital, a pharmacy,  or a doctor's office.
    /// </remarks>
    ///
    public class Organization : HealthRecordItemData
    {
        /// <summary>
        /// It creates a new instance of the <see cref="Organization"/>
        /// class with default values.
        /// </summary>
        ///
        public Organization()
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="Organization"/>
        /// class with specific values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the organization is mandatory.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public Organization(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Populates the organization information from the
        /// specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the organization information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// If <paramref name="Website"/> is empty.
        /// -or-
        /// The scheme specified in <paramref name="Website"/> is invalid.
        /// -or-
        /// <paramref name="Website"/> contains too many slashes.
        /// -or-
        /// The host name specified in <paramref name="Website"/> is invalid.
        /// -or-
        /// The file name specified in <paramref name="Website"/> is invalid.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.name = navigator.SelectSingleNode("name").Value;

            this.contact = XPathHelper.GetOptNavValue<ContactInfo>(
                navigator,
                "contact");

            this.type =
                XPathHelper.GetOptNavValue<CodableValue>(
                navigator,
                "type");

            XPathNavigator websiteNav =
                navigator.SelectSingleNode("website");

            if (websiteNav != null)
            {
                this.website = new Uri(websiteNav.Value, UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Writes the organization data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the organization information.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the organization information should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, "OrganizationNameNotSet");

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("name", this.Name);

            XmlWriterHelper.WriteOpt(writer, "contact", this.contact);
            XmlWriterHelper.WriteOpt(writer, "type", this.type);

            if (this.website != null)
            {
                writer.WriteElementString("website", this.website.OriginalString);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="Name"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the contact information of the organization.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ContactInfo Contact
        {
            get { return this.contact; }
            set { this.contact = value; }
        }

        private ContactInfo contact;

        /// <summary>
        /// Gets or sets the type of the organization.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Type
        {
            get { return this.type; }

            set
            {
                this.type = value;
            }
        }

        private CodableValue type;

        private Uri website;

        /// <summary>
        /// Gets or sets the website URL of the organization.
        /// </summary>
        ///
        /// <value>
        /// The URL for the organization website.
        /// </value>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public Uri Website
        {
            get { return this.website; }
            set { this.website = value; }
        }

        /// <summary>
        /// Gets a string representation of the organization information.
        /// </summary>
        ///
        public override string ToString()
        {
            if (this.Name != null)
            {
                return this.Name;
            }

            return string.Empty;
        }
    }
}
