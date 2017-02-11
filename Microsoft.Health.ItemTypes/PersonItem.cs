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
    /// Represents non-identifying information about a person.
    /// </summary>
    /// 
    public class PersonItem : HealthRecordItemData
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
            _name = new Name();
            _name.ParseXml(navigator.SelectSingleNode("name"));

            // <organization>
            XPathNavigator orgNav =
                navigator.SelectSingleNode("organization");

            if (orgNav != null)
            {
                _organization = orgNav.Value;
            }

            // <professional-training>
            XPathNavigator professionalTrainingNav =
                navigator.SelectSingleNode("professional-training");

            if (professionalTrainingNav != null)
            {
                _professionalTraining = professionalTrainingNav.Value;
            }

            // <id>
            XPathNavigator idNav =
                navigator.SelectSingleNode("id");

            if (idNav != null)
            {
                _id = idNav.Value;
            }

            // <contact>
            XPathNavigator contactNav =
                navigator.SelectSingleNode("contact");

            if (contactNav != null)
            {
                _contactInfo = new ContactInfo();
                _contactInfo.ParseXml(contactNav);
            }

            // <type>
            XPathNavigator typeNav =
                navigator.SelectSingleNode("type");

            if (typeNav != null)
            {
                _personType = new CodableValue();
                _personType.ParseXml(navigator.SelectSingleNode("type"));
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
            Validator.ThrowSerializationIfNull(_name, "PersonNameNotSet");
            
            // <person>
            writer.WriteStartElement(nodeName);

            _name.WriteXml("name", writer);

            if (!String.IsNullOrEmpty(_organization))
            {
                writer.WriteElementString("organization", _organization);
            }

            // <professional-training>
            if (!String.IsNullOrEmpty(_professionalTraining))
            {
                writer.WriteElementString("professional-training", _professionalTraining);
            }
            
            if (!String.IsNullOrEmpty(_id))
            {
                writer.WriteElementString("id", _id);
            }

            if (_contactInfo != null)
            {
                _contactInfo.WriteXml("contact", writer);
            }

            if (_personType != null &&
                !String.IsNullOrEmpty(_personType.Text))
            {
                _personType.WriteXml("type", writer);
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
            get { return _name; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Name", "PersonNameMandatory");
                _name = value; 
            }
        }
        private Name _name;

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
            get { return _organization; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Organization");
                _organization = value;
            }
        }
        private string _organization;

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
            get { return _professionalTraining; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "ProfessionalTraining");
                _professionalTraining = value;
            }
        }
        private string _professionalTraining;
        
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
            get { return _id; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "PersonId");
                _id = value;
            }
        }
        private string _id;

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
            get { return _contactInfo; }
            set { _contactInfo = value; }
        }
        private ContactInfo _contactInfo;

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
            get { return _personType; }
            set { _personType = value; }
        }
        private CodableValue _personType = new CodableValue();

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

            if (Name != null)
            {
                result.Append(Name.ToString());
            }

            if (Organization != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Organization);
            }

            if (ProfessionalTraining != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    ProfessionalTraining);
            }

            if (PersonId != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    PersonId);
            }

            if (ContactInformation != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    ContactInformation.ToString());
            }

            if (PersonType != null &&
                !String.IsNullOrEmpty(PersonType.Text))
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    PersonType.Text);
            }

            return result.ToString();
        }
     }

}
