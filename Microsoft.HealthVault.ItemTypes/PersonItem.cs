// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents non-identifying information about a person.
    /// </summary>
    ///
    public class PersonItem : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PersonItem"/> class with default
        /// values.
        /// </summary>
        ///
        public PersonItem()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonItem"/> class
        /// with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the person.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public PersonItem(Name name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonItem"/> class
        /// with the specified name and type.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the person.
        /// </param>
        ///
        /// <param name="personType">
        /// The type of the person: emergency contact, provider, etc.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="personType"/>
        /// is <b>null</b>.
        /// </exception>
        ///
        public PersonItem(Name name, CodableValue personType)
        {
            this.Name = name;
            this.PersonType = personType;
        }

        /// <summary>
        /// Populates this Person instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the goal information.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="navigator"/> is not
        /// a person node.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // <name>
            this.name = new Name();
            this.name.ParseXml(navigator.SelectSingleNode("name"));

            // <organization>
            XPathNavigator orgNav =
                navigator.SelectSingleNode("organization");

            if (orgNav != null)
            {
                this.organization = orgNav.Value;
            }

            // <professional-training>
            XPathNavigator professionalTrainingNav =
                navigator.SelectSingleNode("professional-training");

            if (professionalTrainingNav != null)
            {
                this.professionalTraining = professionalTrainingNav.Value;
            }

            // <id>
            XPathNavigator idNav =
                navigator.SelectSingleNode("id");

            if (idNav != null)
            {
                this.id = idNav.Value;
            }

            // <contact>
            XPathNavigator contactNav =
                navigator.SelectSingleNode("contact");

            if (contactNav != null)
            {
                this.contactInfo = new ContactInfo();
                this.contactInfo.ParseXml(contactNav);
            }

            // <type>
            XPathNavigator typeNav =
                navigator.SelectSingleNode("type");

            if (typeNav != null)
            {
                this.personType = new CodableValue();
                this.personType.ParseXml(navigator.SelectSingleNode("type"));
            }
        }

        /// <summary>
        /// Writes the person data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the person data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the person data to.
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
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, "PersonNameNotSet");

            // <person>
            writer.WriteStartElement(nodeName);

            this.name.WriteXml("name", writer);

            if (!string.IsNullOrEmpty(this.organization))
            {
                writer.WriteElementString("organization", this.organization);
            }

            // <professional-training>
            if (!string.IsNullOrEmpty(this.professionalTraining))
            {
                writer.WriteElementString("professional-training", this.professionalTraining);
            }

            if (!string.IsNullOrEmpty(this.id))
            {
                writer.WriteElementString("id", this.id);
            }

            if (this.contactInfo != null)
            {
                this.contactInfo.WriteXml("contact", writer);
            }

            if (this.personType != null &&
                !string.IsNullOrEmpty(this.personType.Text))
            {
                this.personType.WriteXml("type", writer);
            }

            // </person>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the person's name.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Name"/> instance.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public Name Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "PersonNameMandatory");
                this.name = value;
            }
        }

        private Name name;

        /// <summary>
        /// Gets or sets the organization the person belongs to.
        /// </summary>
        ///
        /// <value>
        /// A string representing the organization.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the organization should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Organization
        {
            get { return this.organization; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Organization");
                this.organization = value;
            }
        }

        private string organization;

        /// <summary>
        /// Gets or sets the professional training for the provider.
        /// </summary>
        ///
        /// <value>
        /// A string representing the training.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string ProfessionalTraining
        {
            get { return this.professionalTraining; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "ProfessionalTraining");
                this.professionalTraining = value;
            }
        }

        private string professionalTraining;

        /// <summary>
        /// Gets or sets the ID number for the person in the organization.
        /// </summary>
        ///
        /// <value>
        /// A string representing the ID.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the ID should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PersonId
        {
            get { return this.id; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PersonId");
                this.id = value;
            }
        }

        private string id;

        /// <summary>
        /// Gets or sets the contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="ContactInfo"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the contact information should not be
        /// stored.
        /// </remarks>
        ///
        public ContactInfo ContactInformation
        {
            get { return this.contactInfo; }
            set { this.contactInfo = value; }
        }

        private ContactInfo contactInfo;

        /// <summary>
        /// Gets or sets the type of person, such as provider, emergency
        /// contact, and so on.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the type.
        /// </value>
        ///
        public CodableValue PersonType
        {
            get { return this.personType; }
            set { this.personType = value; }
        }

        private CodableValue personType = new CodableValue();

        /// <summary>
        /// Gets a string representation of the person item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the person item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (this.Name != null)
            {
                result.Append(this.Name);
            }

            if (this.Organization != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Organization);
            }

            if (this.ProfessionalTraining != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.ProfessionalTraining);
            }

            if (this.PersonId != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.PersonId);
            }

            if (this.ContactInformation != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.ContactInformation.ToString());
            }

            if (this.PersonType != null &&
                !string.IsNullOrEmpty(this.PersonType.Text))
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.PersonType.Text);
            }

            return result.ToString();
        }
    }
}
