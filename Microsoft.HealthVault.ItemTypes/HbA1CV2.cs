// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a test that
    /// measures the amount of glycosylated hemoglobin in the blood.
    /// </summary>
    ///
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1709:IdentifiersShouldBeCasedCorrectly",
        Justification = "Hb is the correct capitalization here.")]
    public class HbA1CV2 : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CV2"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/>
        /// method is called.
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
            this.When = when;
            this.Value = value;
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

            Validator.ThrowInvalidIfNull(itemNav, "HbA1CV2UnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.value = XPathHelper.GetOptNavValue<HbA1CMeasurement>(itemNav, "value");

            this.assayMethod =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "HbA1C-assay-method");

            this.deviceId =
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
            Validator.ThrowSerializationIfNull(this.value, "HbA1CV2ValueMandatory");

            // <HbA1C>
            writer.WriteStartElement("HbA1C");

            // <when>
            this.when.WriteXml("when", writer);

            // <value>
            this.value.WriteXml("value", writer);

            // <HbA1C-assay-method>
            XmlWriterHelper.WriteOpt(writer, "HbA1C-assay-method", this.assayMethod);

            // <device-id>
            XmlWriterHelper.WriteOptString(writer, "device-id", this.deviceId);

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
                return this.when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

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
                return this.value;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "HbA1CV2ValueMandatory");
                this.value = value;
            }
        }

        private HbA1CMeasurement value;

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
                return this.assayMethod;
            }

            set
            {
                this.assayMethod = value;
            }
        }

        private CodableValue assayMethod;

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
                return this.deviceId;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "DeviceId");
                this.deviceId = value;
            }
        }

        private string deviceId;

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

            if (this.Value != null)
            {
                result = this.Value.ToString();
            }

            return result;
        }
    }
}
