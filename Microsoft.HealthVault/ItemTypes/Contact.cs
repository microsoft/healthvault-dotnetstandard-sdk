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
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
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
            this.ContactInformation = contactInfo;
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

            this.contactInfo = new ContactInfo();
            this.contactInfo.ParseXml(contactNav.SelectSingleNode("contact"));
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="ContactInformation"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.contactInfo, Resources.ContactInformationNotSet);

            // <contact>
            writer.WriteStartElement("contact");

            this.contactInfo.WriteXml("contact", writer);

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
            get { return this.contactInfo; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ContactInformation), Resources.ContactInformationMandatory);
                this.contactInfo = value;
            }
        }

        private ContactInfo contactInfo = new ContactInfo();

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
            if (this.ContactInformation.Address.Count > 0 ||
                this.ContactInformation.Email.Count > 0 ||
                this.ContactInformation.Phone.Count > 0)
            {
                return this.ContactInformation.ToString();
            }

            if (!string.IsNullOrEmpty(this.CommonData.Note))
            {
                return this.CommonData.Note;
            }

            return Resources.ContactInformationToStringSeeDetails;
        }
    }
}
