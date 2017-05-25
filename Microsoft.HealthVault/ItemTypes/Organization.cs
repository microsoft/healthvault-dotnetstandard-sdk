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
    public class Organization : ItemBase
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
            Name = name;
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

            _name = navigator.SelectSingleNode("name").Value;

            _contact = XPathHelper.GetOptNavValue<ContactInfo>(
                navigator,
                "contact");

            _type =
                XPathHelper.GetOptNavValue<CodableValue>(
                navigator,
                "type");

            XPathNavigator websiteNav =
                navigator.SelectSingleNode("website");

            if (websiteNav != null)
            {
                _website = new Uri(websiteNav.Value, UriKind.RelativeOrAbsolute);
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.OrganizationNameNotSet);

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("name", Name);

            XmlWriterHelper.WriteOpt(writer, "contact", _contact);
            XmlWriterHelper.WriteOpt(writer, "type", _type);

            if (_website != null)
            {
                writer.WriteElementString("website", _website.OriginalString);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }

        private string _name;

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
            get { return _contact; }
            set { _contact = value; }
        }

        private ContactInfo _contact;

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
            get { return _type; }

            set
            {
                _type = value;
            }
        }

        private CodableValue _type;

        private Uri _website;

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
            get { return _website; }
            set { _website = value; }
        }

        /// <summary>
        /// Gets a string representation of the organization information.
        /// </summary>
        ///
        public override string ToString()
        {
            if (Name != null)
            {
                return Name;
            }

            return string.Empty;
        }
    }
}
