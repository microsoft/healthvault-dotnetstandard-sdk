// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
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
            Name = name;
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
            Name = name;
            PersonType = personType;
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.PersonNameNotSet);

            // <person>
            writer.WriteStartElement(nodeName);

            _name.WriteXml("name", writer);

            if (!string.IsNullOrEmpty(_organization))
            {
                writer.WriteElementString("organization", _organization);
            }

            // <professional-training>
            if (!string.IsNullOrEmpty(_professionalTraining))
            {
                writer.WriteElementString("professional-training", _professionalTraining);
            }

            if (!string.IsNullOrEmpty(_id))
            {
                writer.WriteElementString("id", _id);
            }

            if (_contactInfo != null)
            {
                _contactInfo.WriteXml("contact", writer);
            }

            if (_personType != null &&
                !string.IsNullOrEmpty(_personType.Text))
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
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.PersonNameMandatory);
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
                result.Append(Name);
            }

            if (Organization != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    Organization);
            }

            if (ProfessionalTraining != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    ProfessionalTraining);
            }

            if (PersonId != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    PersonId);
            }

            if (ContactInformation != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    ContactInformation.ToString());
            }

            if (PersonType != null &&
                !string.IsNullOrEmpty(PersonType.Text))
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    PersonType.Text);
            }

            return result.ToString();
        }
    }
}
