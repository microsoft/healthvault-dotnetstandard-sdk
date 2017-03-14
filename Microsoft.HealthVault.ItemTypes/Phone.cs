// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a telephone number.
    /// </summary>
    ///
    public class Phone : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Phone"/> class with default values.
        /// </summary>
        ///
        public Phone()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Phone"/> class with the
        /// specified phone number.
        /// </summary>
        ///
        /// <param name="number">
        /// The phone number.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="number"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Phone(string number)
        {
            this.Number = number;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Phone"/> class with the
        /// specified phone number, description, primary designation.
        /// </summary>
        ///
        /// <param name="number">
        /// The phone number.
        /// </param>
        ///
        /// <param name="description">
        /// The description of the phone number.
        /// </param>
        ///
        /// <param name="isPrimary">
        /// <b>true</b> if the phone number is the primary phone number for the
        /// person; otherwise, <b>false</b>.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="number"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Phone(string number, string description, bool isPrimary)
        {
            this.Number = number;
            this.Description = description;
            this.IsPrimary = isPrimary;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the phone information.
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
                this.description = descNav.Value;
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");

            if (isPrimaryNav != null)
            {
                this.isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            this.number = navigator.SelectSingleNode("number").Value;
        }

        /// <summary>
        /// Writes the XML representation of the telephone number into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the phone.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the telephone number should be
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
        /// The <see cref="Number"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.number, "PhoneNumberNotSet");

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(this.description))
            {
                writer.WriteElementString("description", this.description);
            }

            if (this.isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool((bool)this.isPrimary));
            }

            writer.WriteElementString("number", this.number);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description for the telephone number.
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
        public string Description
        {
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets a value indicating whether the telephone number is the
        /// primary number for the person.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the this number is the primary phone
        /// number for the person; otherwise, <b>false</b>.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if this property should not be stored.
        /// </remarks>
        ///
        public bool? IsPrimary
        {
            get { return this.isPrimary; }
            set { this.isPrimary = value; }
        }

        private bool? isPrimary;

        /// <summary>
        /// Gets or sets the telephone number.
        /// </summary>
        ///
        /// <value>
        /// A string representing the phone number.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        ///
        public string Number
        {
            get { return this.number; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Number");
                Validator.ThrowIfStringIsWhitespace(value, "Number");
                this.number = value;
            }
        }

        private string number;
    }
}
