// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
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
            Number = number;
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
            Number = number;
            Description = description;
            IsPrimary = isPrimary;
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
                _description = descNav.Value;
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");

            if (isPrimaryNav != null)
            {
                _isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            _number = navigator.SelectSingleNode("number").Value;
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Number"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_number, Resources.PhoneNumberNotSet);

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(_description))
            {
                writer.WriteElementString("description", _description);
            }

            if (_isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool((bool)_isPrimary));
            }

            writer.WriteElementString("number", _number);

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
            get { return _description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }

        private string _description;

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
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }

        private bool? _isPrimary;

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
            get { return _number; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Number");
                Validator.ThrowIfStringIsWhitespace(value, "Number");
                _number = value;
            }
        }

        private string _number;
    }
}
