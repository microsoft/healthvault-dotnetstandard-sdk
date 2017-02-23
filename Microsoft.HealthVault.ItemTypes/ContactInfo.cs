// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a contact person.
    /// </summary>
    ///
    public class ContactInfo : HealthRecordItemData
    {
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
                this.address.Add(newAddress);
            }

            XPathNodeIterator phoneIterator = navigator.Select("phone");

            foreach (XPathNavigator phoneNav in phoneIterator)
            {
                Phone newPhone = new Phone();
                newPhone.ParseXml(phoneNav);
                this.phone.Add(newPhone);
            }

            XPathNodeIterator emailIterator = navigator.Select("email");

            foreach (XPathNavigator emailNav in emailIterator)
            {
                Email newEmail = new Email();
                newEmail.ParseXml(emailNav);
                this.email.Add(newEmail);
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

            foreach (Address address in this.address)
            {
                address.WriteXml("address", writer);
            }

            foreach (Phone phone in this.phone)
            {
                phone.WriteXml("phone", writer);
            }

            foreach (Email email in this.email)
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
                if (this.primaryAddress == null)
                {
                    for (int index = 0; index < this.Address.Count; ++index)
                    {
                        if (this.Address[index].IsPrimary != null &&
                            (bool)this.Address[index].IsPrimary)
                        {
                            this.primaryAddress = this.Address[index];
                            break;
                        }
                    }
                }

                return this.primaryAddress;
            }
        }

        private Address primaryAddress;

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
                if (this.primaryPhone == null)
                {
                    for (int index = 0; index < this.Phone.Count; ++index)
                    {
                        if (this.Phone[index].IsPrimary != null &&
                            (bool)this.Phone[index].IsPrimary)
                        {
                            this.primaryPhone = this.Phone[index];
                            break;
                        }
                    }
                }

                return this.primaryPhone;
            }
        }

        private Phone primaryPhone;

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
                if (this.primaryEmail == null)
                {
                    for (int index = 0; index < this.Email.Count; ++index)
                    {
                        if (this.Email[index].IsPrimary != null &&
                            (bool)this.Email[index].IsPrimary)
                        {
                            this.primaryEmail = this.Email[index];
                            break;
                        }
                    }
                }

                return this.primaryEmail;
            }
        }

        private Email primaryEmail;

        /// <summary>
        /// Gets the addresses for the contact.
        /// </summary>
        ///
        /// <value>
        /// A collection of addresses.
        /// </value>
        ///
        public Collection<Address> Address => this.address;

        private readonly Collection<Address> address = new Collection<Address>();

        /// <summary>
        /// Gets the telephone numbers for the contact.
        /// </summary>
        ///
        /// <value>
        /// A collection of phone numbers.
        /// </value>
        ///
        public Collection<Phone> Phone => this.phone;

        private readonly Collection<Phone> phone = new Collection<Phone>();

        /// <summary>
        /// Gets the email addresses for the contact.
        /// </summary>
        ///
        /// <value>
        /// A collection of email addresses.
        /// </value>
        ///
        public Collection<Email> Email => this.email;

        private readonly Collection<Email> email = new Collection<Email>();

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
            string result = string.Empty;

            if (this.PrimaryPhone != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "ContactInfoFormatPrimary"),
                        this.PrimaryPhone.Number);
            }
            else if (this.Phone.Count > 0)
            {
                result = this.Phone[0].Number;
            }
            else if (this.Address.Count > 0)
            {
                result = this.Address[0].City;

                if (!string.IsNullOrEmpty(this.Address[0].County))
                {
                    result +=
                         ResourceRetriever.GetResourceString(
                            "ListSeparator") +
                         this.Address[0].County;
                }

                if (!string.IsNullOrEmpty(this.Address[0].State))
                {
                    result +=
                         ResourceRetriever.GetResourceString(
                            "ListSeparator") +
                         this.Address[0].State;
                }
            }
            else if (this.PrimaryEmail != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "ContactInfoFormatPrimary"),
                        this.PrimaryEmail.Address);
            }
            else if (this.Email.Count > 0)
            {
                result = this.Email[0].Address;
            }

            return result;
        }
    }
}
