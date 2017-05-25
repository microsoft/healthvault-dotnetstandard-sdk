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
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores a body dimension.
    /// </summary>
    ///
    /// <remarks>
    /// Examples: Waist size, head circumference, length (pediatric).
    /// </remarks>
    ///
    public class BodyDimension : ThingBase
    {
        /// <summary>
        /// Stores a body dimension.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Waist size, head circumference, length (pediatric).
        /// </remarks>
        ///
        public BodyDimension()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Stores a body dimension.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Waist size, head circumference, length (pediatric).
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time of the body dimension measurement.
        /// </param>
        ///
        /// <param name="measurementName">
        /// The name of the body dimension measurement.
        /// </param>
        ///
        /// <param name="value">
        /// The value of the body dimension measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/>, <paramref name="measurementName"/> or
        /// <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public BodyDimension(
            ApproximateDateTime when,
            CodableValue measurementName,
            Length value)
            : base(TypeId)
        {
            When = when;
            MeasurementName = measurementName;
            Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("dd710b31-2b6f-45bd-9552-253562b9a7c1");

        /// <summary>
        /// Populates this <see cref="BodyDimension"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the body dimension data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "body-dimension" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("body-dimension");

            Validator.ThrowInvalidIfNull(itemNav, Resources.BodyDimensionUnexpectedNode);

            // when (approxi-date-time, mandatory)
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            _measurementName = new CodableValue();
            _measurementName.ParseXml(itemNav.SelectSingleNode("measurement-name"));

            // value (Length, mandatory)
            _value = new Length();
            _value.ParseXml(itemNav.SelectSingleNode("value"));
        }

        /// <summary>
        /// Writes the body dimension data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the body dimension data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/>, <see cref="MeasurementName"/> or <see cref="Value"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(_measurementName, Resources.BodyDimensionMeasurementNameNotSet);
            Validator.ThrowSerializationIfNull(_value, Resources.BodyDimensionValueNotSet);

            // <body-dimension>
            writer.WriteStartElement("body-dimension");

            // <when>
            _when.WriteXml("when", writer);

            // <measurement-name>
            _measurementName.WriteXml("measurement-name", writer);

            // <value>
            _value.WriteXml("value", writer);

            // </body-dimension>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the body dimension measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date
        /// and time.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets the name of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the name
        /// of this measurement.
        /// </value>
        ///
        /// <remarks>
        /// Choose the appropriate preferred vocabulary based on your scenario.
        /// The preferred vocabularies are "body-dimension-measurement-names"
        /// and "body-dimension-measurement-names-pediatrics".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue MeasurementName
        {
            get { return _measurementName; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(MeasurementName), Resources.BodyDimensionMeasurementNameNullValue);
                _measurementName = value;
            }
        }

        private CodableValue _measurementName;

        /// <summary>
        /// Gets or sets the value of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> instance representing the value of
        /// this measurement.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Length Value
        {
            get { return _value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.BodyDimensionValueNullValue);
                _value = value;
            }
        }

        private Length _value;

        /// <summary>
        /// Gets the representation of a body dimension instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the body dimension instance.
        /// </returns>
        ///
        public override string ToString()
        {
            if (_measurementName != null && _value != null)
            {
                return string.Format(
                        Resources.NameAndValue,
                        _measurementName.ToString(),
                        _value.ToString());
            }

            if (_measurementName != null)
            {
                return _measurementName.ToString();
            }

            if (_value != null)
            {
                return _value.ToString();
            }

            return string.Empty;
        }
    }
}
