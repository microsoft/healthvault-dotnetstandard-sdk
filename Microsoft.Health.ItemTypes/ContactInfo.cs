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

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents information about a contact person.
    /// </summary>
    /// 
    public class ContactInfo : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContactInfo"/> class with default values.
        /// </summary>
        /// 
        public ContactInfo()
        {
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the contact information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNodeIterator addressIterator = navigator.Select("address");

            foreach (XPathNavigator addressNav in addressIterator)
            {
                Address newAddress = new Address();
                newAddress.ParseXml(addressNav);
                _address.Add(newAddress);
            }

            XPathNodeIterator phoneIterator = navigator.Select("phone");

            foreach (XPathNavigator phoneNav in phoneIterator)
            {
                Phone newPhone = new Phone();
                newPhone.ParseXml(phoneNav);
                _phone.Add(newPhone);
            }

            XPathNodeIterator emailIterator = navigator.Select("email");

            foreach (XPathNavigator emailNav in emailIterator)
            {
                Email newEmail = new Email();
                newEmail.ParseXml(emailNav);
                _email.Add(newEmail);
            }
        }

        /// <summary>
        /// Writes the XML representation of the contact information into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the contact information.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the contact information should be 
        /// written.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// A mandatory property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            
            writer.WriteStartElement(nodeName);

            foreach (Address address in _address)
            {
                address.WriteXml("address", writer);
            }

            foreach (Phone phone in _phone)
            {
                phone.WriteXml("phone", writer);
            }

            foreach (Email email in _email)
            {
                email.WriteXml("email", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the first address that is marked as IsPrimary".
        /// </summary>
        /// 
        /// <value>
        /// The first <see cref="Address"/> value that is marked as IsPrimary, 
        /// or <b>null</b> if no primary addresses were found.
        /// </value>
        /// 
        public Address PrimaryAddress
        {
            get
            {
                if (_primaryAddress == null)
                {
                    for (int index = 0; index < Address.Count; ++index)
                    {
                        if (Address[index].IsPrimary != null &&
                            (bool)Address[index].IsPrimary)
                        {
                            _primaryAddress = Address[index];
                            break;
                        }
                    }
                }
                return _primaryAddress;
            }
        }
        private Address _primaryAddress;

        /// <summary>
        /// Gets the first telephone number that is marked as IsPrimary.
        /// </summary>
        /// 
        /// <value>
        /// The first value of <see cref="Phone"/> that is marked as 
        /// IsPrimary, or <b>null</b> if no primary telephone numbers 
        /// were found.
        /// </value>
        /// 
        public Phone PrimaryPhone
        {
            get
            {
                if (_primaryPhone == null)
                {
                    for (int index = 0; index < Phone.Count; ++index)
                    {
                        if (Phone[index].IsPrimary != null &&
                            (bool)Phone[index].IsPrimary)
                        {
                            _primaryPhone = Phone[index];
                            break;
                        }
                    }
                }
                return _primaryPhone;
            }
        }
        private Phone _primaryPhone;

        /// <summary>
        /// Gets the first email that is marked as IsPrimary.
        /// </summary>
        /// 
        /// <value>
        /// The first value of <see cref="Email"/> that is marked as IsPrimary, 
        /// or <b>null</b> if no primary email addresses were found.
        /// </value>
        /// 
        public Email PrimaryEmail
        {
            get
            {
                if (_primaryEmail == null)
                {
                    for (int index = 0; index < Email.Count; ++index)
                    {
                        if (Email[index].IsPrimary != null &&
                            (bool)Email[index].IsPrimary)
                        {
                            _primaryEmail = Email[index];
                            break;
                        }
                    }
                }
                return _primaryEmail;
            }
        }
        private Email _primaryEmail;


        /// <summary>
        /// Gets the addresses for the contact.
        /// </summary>
        /// 
        /// <value>
        /// A collection of addresses.
        /// </value>
        /// 
        public Collection<Address> Address
        {
            get { return _address; }
        }
        private Collection<Address> _address = new Collection<Address>();

        /// <summary>
        /// Gets the telephone numbers for the contact.
        /// </summary>
        /// 
        /// <value>
        /// A collection of phone numbers.
        /// </value>
        /// 
        public Collection<Phone> Phone
        {
            get { return _phone; }
        }
        private Collection<Phone> _phone = new Collection<Phone>();

        /// <summary>
        /// Gets the email addresses for the contact.
        /// </summary>
        /// 
        /// <value>
        /// A collection of email addresses.
        /// </value>
        /// 
        public Collection<Email> Email
        {
            get { return _email; }
        }
        private Collection<Email> _email = new Collection<Email>();

        /// <summary>
        /// Gets a string representation of the contact information.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the contact information.
        /// </returns>
        /// 
        public override string ToString()
        {
            String result = String.Empty;

            if (PrimaryPhone != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "ContactInfoFormatPrimary"),
                        PrimaryPhone.Number);
            }
            else if (Phone.Count > 0)
            {
                result = Phone[0].Number;
            }
            else if (Address.Count > 0)
            {
                result = Address[0].City;

                if (!String.IsNullOrEmpty(Address[0].County))
                {
                    result += 
                         ResourceRetriever.GetResourceString(
                            "ListSeparator") +
                         Address[0].County;
                }

                if (!String.IsNullOrEmpty(Address[0].State))
                {
                    result += 
                         ResourceRetriever.GetResourceString(
                            "ListSeparator") +
                         Address[0].State;
                }
            }
            else if (PrimaryEmail != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "ContactInfoFormatPrimary"),
                        PrimaryEmail.Address);
            }
            else if (Email.Count > 0)
            {
                result = Email[0].Address;
            }

            return result;
        }
    }

}
