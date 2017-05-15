// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class HbA1C : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HbA1C"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public HbA1C()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HbA1C"/> class with the
        /// specified date and amount.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the HbA1C was taken.
        /// </param>
        ///
        /// <param name="value">
        /// The amount of glycosylated hemoglobin in the blood as a percentage.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero or greater than one.
        /// </exception>
        ///
        public HbA1C(HealthServiceDateTime when, double value)
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
            new Guid("227F55FB-1001-4D4E-9F6A-8D893E07B451");

        /// <summary>
        /// Populates this HbA1C instance from the data in the XML.
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.HbA1CUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _value = itemNav.SelectSingleNode("value").ValueAsDouble;

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

            // <HbA1C>
            writer.WriteStartElement("HbA1C");

            // <when>
            _when.WriteXml("when", writer);

            // <value>
            writer.WriteElementString(
                "value",
                Value.ToString(CultureInfo.InvariantCulture));

            // <HbA1C-assay-method>
            if (_assayMethod != null)
            {
                _assayMethod.WriteXml("HbA1C-assay-method", writer);
            }

            // <device-id>
            if (!string.IsNullOrEmpty(_deviceId))
            {
                writer.WriteElementString("device-id", _deviceId);
            }

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
            get { return _when; }

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
        /// A number representing the amount as a fraction between 0 and 1.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public double Value
        {
            get { return _value; }

            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Value), Resources.HbA1CValueRange);
                }

                _value = value;
            }
        }

        private double _value;

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
            get { return _assayMethod; }
            set { _assayMethod = value; }
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
            get { return _deviceId; }

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
            return
                string.Format(
                    Resources.HbA1CToStringFormatPercent,
                    (Value * 100.0).ToString(CultureInfo.CurrentCulture));
        }
    }
}
