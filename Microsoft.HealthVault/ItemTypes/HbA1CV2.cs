// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a test that
    /// measures the amount of glycosylated hemoglobin in the blood.
    /// </summary>
    ///
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1709:IdentifiersShouldBeCasedCorrectly",
        Justification = "Hb is the correct capitalization here.")]
    public class HbA1CV2 : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CV2"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public HbA1CV2()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CV2"/> class with the
        /// specified date and amount.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the HbA1C was taken.
        /// </param>
        ///
        /// <param name="value">
        /// The amount of glycosylated hemoglobin in the blood in millimoles per mole (mmol/mol).
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HbA1CV2(HealthServiceDateTime when, HbA1CMeasurement value)
            : base(TypeId)
        {
            When = when;
            Value = value;
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
            new Guid("62160199-b80f-4905-a55a-ac4ba825ceae");

        /// <summary>
        /// Populates this <see cref="HbA1CV2"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the HbA1C data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an HbA1C node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "HbA1C");

            Validator.ThrowInvalidIfNull(itemNav, Resources.HbA1CV2UnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _value = XPathHelper.GetOptNavValue<HbA1CMeasurement>(itemNav, "value");

            _assayMethod =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "HbA1C-assay-method");

            _deviceId =
                XPathHelper.GetOptNavValue(itemNav, "device-id");
        }

        /// <summary>
        /// Writes the HbA1C data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the HbA1C data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_value, Resources.HbA1CV2ValueMandatory);

            // <HbA1C>
            writer.WriteStartElement("HbA1C");

            // <when>
            _when.WriteXml("when", writer);

            // <value>
            _value.WriteXml("value", writer);

            // <HbA1C-assay-method>
            XmlWriterHelper.WriteOpt(writer, "HbA1C-assay-method", _assayMethod);

            // <device-id>
            XmlWriterHelper.WriteOptString(writer, "device-id", _deviceId);

            // </HbA1C>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the HbA1C measurement was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The value defaults to the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return _when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the amount of glycosylated hemoglobin in the blood.
        /// </summary>
        ///
        /// <value>
        /// A number representing the amount in millimoles/mole (mmol/mol).
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is null.
        /// </exception>
        ///
        public HbA1CMeasurement Value
        {
            get
            {
                return _value;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.HbA1CV2ValueMandatory);
                _value = value;
            }
        }

        private HbA1CMeasurement _value;

        /// <summary>
        /// Gets or sets the assay method.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the method.
        /// </value>
        ///
        /// <remarks>
        /// The preferred vocabulary for this value is "HbA1C-assay-method".
        /// </remarks>
        ///
        public CodableValue AssayMethod
        {
            get
            {
                return _assayMethod;
            }

            set
            {
                _assayMethod = value;
            }
        }

        private CodableValue _assayMethod;

        /// <summary>
        /// Gets or sets the ID of the device that took the reading.
        /// </summary>
        ///
        /// <value>
        /// A string representing the ID.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string DeviceId
        {
            get
            {
                return _deviceId;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "DeviceId");
                _deviceId = value;
            }
        }

        private string _deviceId;

        /// <summary>
        /// Gets a string representation of the HbA1C value.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the HbA1C value.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (Value != null)
            {
                result = Value.ToString();
            }

            return result;
        }
    }
}
