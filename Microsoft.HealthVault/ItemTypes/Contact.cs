// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a contact.
    /// </summary>
    ///
    public class Contact : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Contact"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Contact()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Contact"/> class with the
        /// specified contact information.
        /// </summary>
        ///
        /// <param name="contactInfo">
        /// The information about the contact.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="contactInfo"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Contact(ContactInfo contactInfo)
            : base(TypeId)
        {
            ContactInformation = contactInfo;
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
            new Guid("162dd12d-9859-4a66-b75f-96760d67072b");

        /// <summary>
        /// Populates this <see cref="Contact"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the contact data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a contact node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator contactNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("contact");

            Validator.ThrowInvalidIfNull(contactNav, Resources.ContactInformationUnexpectedNode);

            _contactInfo = new ContactInfo();
            _contactInfo.ParseXml(contactNav.SelectSingleNode("contact"));
        }

        /// <summary>
        /// Writes the contact data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the contact data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="ContactInformation"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_contactInfo, Resources.ContactInformationNotSet);

            // <contact>
            writer.WriteStartElement("contact");

            _contactInfo.WriteXml("contact", writer);

            // </contact>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="ContactInfo"/> containing the information.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public ContactInfo ContactInformation
        {
            get { return _contactInfo; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ContactInformation), Resources.ContactInformationMandatory);
                _contactInfo = value;
            }
        }

        private ContactInfo _contactInfo = new ContactInfo();

        /// <summary>
        /// Gets a string representation of the contact item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the contact item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (ContactInformation.Address.Count > 0 ||
                ContactInformation.Email.Count > 0 ||
                ContactInformation.Phone.Count > 0)
            {
                return ContactInformation.ToString();
            }

            if (!string.IsNullOrEmpty(CommonData.Note))
            {
                return CommonData.Note;
            }

            return Resources.ContactInformationToStringSeeDetails;
        }
    }
}
