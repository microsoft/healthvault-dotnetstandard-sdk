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
    /// Represents a group membership.
    /// </summary>
    /// 
    public class GroupMembershipType : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="GroupMembershipType"/> class with 
        /// default values.
        /// </summary>
        /// 
        public GroupMembershipType()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupMembershipType"/> class with the specified
        /// name.
        /// </summary>
        /// 
        /// <param name="name">
        /// The group name.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is empty.
        /// </exception>
        /// 
        public GroupMembershipType(CodableValue name)
            : base()
        {

            Name = name;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="GroupMembershipType"/> class with the specified
        /// name and value.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the group type.
        /// </param>
        /// 
        /// <param name="value">
        /// The value the member has for the group type.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is empty.
        /// </exception>
        /// 
        public GroupMembershipType(CodableValue name, string value)
            : base()
        {

            Name = name;
            Value = value;
        }

        /// <summary> 
        /// Populates the data for the group membership type from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the group membership type.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            CodableValue name = new CodableValue();
            name.ParseXml(navigator.SelectSingleNode("name"));
            _name = name;
            _value = navigator.SelectSingleNode("value").Value;
        }

        /// <summary> 
        /// Writes the group membership type data to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the group membership type.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the group membership type to.
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
        /// If <see cref="Name"/>  or <see cref="Value"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, "GroupMembershipTypeNameNotSet");

            Validator.ThrowSerializationIf(
                String.IsNullOrEmpty(_value),
                "GroupMembershipTypeValueNotSet");

            writer.WriteStartElement(nodeName);

            _name.WriteXml("name", writer);
            writer.WriteElementString("value", _value);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the group membership name.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is empty.
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "GroupMembershipTypeNameMandatory");
                Validator.ThrowIfStringNullOrEmpty(value.Text, "Name");
                _name = value;
            }
        }
        private CodableValue _name;

        /// <summary>
        /// Gets or sets the group membership value.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        /// 
        public string Value
        {
            get { return _value; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "GroupMembershipTypeValueMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "Value");
                _value = value;
            }
        }
        private string _value;

        /// <summary>
        /// Gets a string representation of the group membership type.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the group membership type.
        /// </returns>
        /// 
        public override string ToString()
        {
            return
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "NameEqualsValue"),
                    Name.ToString(),
                    Value);
        }
    }
}
