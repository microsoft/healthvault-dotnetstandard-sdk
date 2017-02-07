// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a contact.
    /// </summary>
    /// 
    public class Contact : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Contact"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(contactNav, "ContactInformationUnexpectedNode");

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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="ContactInformation"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_contactInfo, "ContactInformationNotSet");

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
                Validator.ThrowIfArgumentNull(value, "ContactInformation", "ContactInformationMandatory");
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
            string result = null;
            if (ContactInformation.Address.Count > 0 ||
                ContactInformation.Email.Count > 0 ||
                ContactInformation.Phone.Count > 0)
            {
                result = ContactInformation.ToString();
            }
            else if (!String.IsNullOrEmpty(CommonData.Note))
            {
                result = CommonData.Note;
            }
            else
            {
                result = 
                    ResourceRetriever.GetResourceString(
                        "ContactInformationToStringSeeDetails");
            }
            return result;
        }
    }

}
