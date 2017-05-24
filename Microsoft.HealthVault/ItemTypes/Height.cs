// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's height.
    /// </summary>
    ///
    public class Height : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Height()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified height in meters.
        /// </summary>
        ///
        /// <param name="meters">
        /// The height in meters.
        /// </param>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Height(double meters)
            : base(TypeId)
        {
            _value.Value = meters;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified date and time of measurement and the height in meters.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the height measurement occurred.
        /// </param>
        ///
        /// <param name="height">
        /// The person's height.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="height"/> parameter is
        /// <b>null</b>.
        /// </exception>
        ///
        public Height(HealthServiceDateTime when, Length height)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(when, nameof(when), Resources.WhenNullValue);
            Validator.ThrowIfArgumentNull(height, nameof(height), Resources.HeightValueMandatory);

            _when = when;
            _value = height;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified height in meters and display value.
        /// </summary>
        ///
        /// <param name="meters">
        /// The height in meters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The height value as entered by the user. Typically, the display value is
        /// used when the user-entered height units are not in meters.
        /// </param>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Height(double meters, DisplayValue displayValue)
            : base(TypeId)
        {
            _value.Value = meters;
            _value.DisplayValue = displayValue;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("40750a6a-89b2-455c-bd8d-b420a4cb500b");

        /// <summary>
        /// Populates this <see cref="Height"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the height data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a height node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator heightNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("height");

            Validator.ThrowInvalidIfNull(heightNav, Resources.HeightUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(heightNav.SelectSingleNode("when"));

            _value = new Length();
            _value.ParseXml(heightNav.SelectSingleNode("value"));
        }

        /// <summary>
        /// Writes the height data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the height data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Value"/> has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_value, Resources.HeightValueNotSet);

            // <height>
            writer.WriteStartElement("height");

            // <when>
            _when.WriteXml("when", writer);

            _value.WriteXml("value", writer);

            // </height>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the height measurement
        /// was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date. The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's height.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> value representing the person's height.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public Length Value
        {
            get { return _value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.HeightValueMandatory);
                _value = value;
            }
        }

        private Length _value = new Length();

        /// <summary>
        /// Gets a string representation of the height item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the height item.
        /// </returns>
        ///
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
