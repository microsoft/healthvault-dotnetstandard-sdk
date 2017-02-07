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
    /// Represents an email address.
    /// </summary>
    /// 
    public class Email : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Email"/> class with default values.
        /// </summary>
        /// 
        public Email()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Email"/> class with the 
        /// specified email address.
        /// </summary>
        /// 
        /// <param name="address">
        /// The email address.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="address"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public Email(string address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Email"/> class with the 
        /// specified email address, description, and primary designation.
        /// </summary>
        /// 
        /// <param name="address">
        /// The email address.
        /// </param>
        /// 
        /// <param name="description">
        /// The description for the email address, such as personal or work.
        /// </param>
        /// 
        /// <param name="isPrimary">
        /// <b>true</b> if this email address is the primary email address at which 
        /// to contact the person; otherwise, <b>false</b>.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="address"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public Email(string address, string description, bool isPrimary)
            : this(address)
        {
            this.Description = description;
            this.IsPrimary = isPrimary;
        }
        
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the email information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator descNav =
                navigator.SelectSingleNode("description");

            if (descNav != null)
            {
                _description = descNav.Value;
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");

            if (isPrimaryNav != null)
            {
                _isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            _address = navigator.SelectSingleNode("address").Value;
        }

        /// <summary>
        /// Writes the XML representation of the email address into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the email address.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the email address should be 
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
        /// The <see cref="Address"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_address, "EmailAddressNotSet");

            writer.WriteStartElement(nodeName);

            if (!String.IsNullOrEmpty(_description))
            {
                writer.WriteElementString("description", _description);
            }

            if (_isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary", 
                    SDKHelper.XmlFromBool((bool)_isPrimary));
            }

            writer.WriteElementString("address", _address);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description for the email address.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the description.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the description should not be stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Description
        {
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }
        private string _description;

        /// <summary>
        /// Gets or sets a value indicating whether the email address is the 
        /// primary address for the person.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if this email address is primary; otherwise, <b>false</b>.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if this property should not be stored.
        /// </remarks>
        /// 
        public bool? IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }
        private bool? _isPrimary;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the email address.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only 
        /// whitespace during set.
        /// </exception>
        /// 
        public string Address
        {
            get { return _address; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Address");
                Validator.ThrowIfStringIsWhitespace(value, "Address");
                _address = value;
            }
        }
        private string _address;

    }

}
