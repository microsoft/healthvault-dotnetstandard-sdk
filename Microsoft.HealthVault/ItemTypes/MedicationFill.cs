// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
    /// Represents a thing about filling a medication.
    /// </summary>
    ///
    public class MedicationFill : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicationFill"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public MedicationFill()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicationFill"/> class with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the medication.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public MedicationFill(CodableValue name)
            : base(TypeId)
        {
            this.Name = name;
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("167ecf6b-bb54-43f9-a473-507b334907e0");

        /// <summary>
        /// Populates this medication fill instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the medication fill data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a medication-fill node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("medication-fill");

            Validator.ThrowInvalidIfNull(itemNav, Resources.MedicationFillUnexpectedNode);

            this.name = new CodableValue();
            this.name.ParseXml(itemNav.SelectSingleNode("name"));

            this.dateFilled =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-filled");

            this.daysSupply =
                XPathHelper.GetOptNavValueAsInt(itemNav, "days-supply");

            this.nextRefillDate =
                XPathHelper.GetOptNavValue<HealthServiceDate>(itemNav, "next-refill-date");

            this.refillsLeft =
                XPathHelper.GetOptNavValueAsInt(itemNav, "refills-left");

            this.pharmacy =
                XPathHelper.GetOptNavValue<Organization>(itemNav, "pharmacy");

            this.prescriptionNumber =
                XPathHelper.GetOptNavValue(itemNav, "prescription-number");

            this.lotNumber =
                XPathHelper.GetOptNavValue(itemNav, "lot-number");
        }

        /// <summary>
        /// Writes the medication fill data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the medication fill data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, Resources.MedicationFillNameNotSet);

            writer.WriteStartElement("medication-fill");

            this.name.WriteXml("name", writer);

            XmlWriterHelper.WriteOpt(writer, "date-filled", this.dateFilled);
            XmlWriterHelper.WriteOptInt(writer, "days-supply", this.daysSupply);
            XmlWriterHelper.WriteOpt(writer, "next-refill-date", this.nextRefillDate);
            XmlWriterHelper.WriteOptInt(writer, "refills-left", this.refillsLeft);
            XmlWriterHelper.WriteOpt(writer, "pharmacy", this.pharmacy);
            XmlWriterHelper.WriteOptString(writer, "prescription-number", this.prescriptionNumber);
            XmlWriterHelper.WriteOptString(writer, "lot-number", this.lotNumber);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the medication name and clinical code.
        /// </summary>
        ///
        /// <remarks>
        /// The name should be understandable to the person taking the medication, such as the
        /// brand name.
        /// The preferred vocabularies for medication name are "RxNorm" or "NDC".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.MedicationFillNameMandatory);
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets the date the prescription was filled.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime DateFilled
        {
            get { return this.dateFilled; }
            set { this.dateFilled = value; }
        }

        private ApproximateDateTime dateFilled;

        /// <summary>
        /// Gets or sets the number of days supply of the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public int? DaysSupply
        {
            get { return this.daysSupply; }
            set { this.daysSupply = value; }
        }

        private int? daysSupply;

        /// <summary>
        /// Gets or sets the date on which the prescription can be refilled.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public HealthServiceDate NextRefillDate
        {
            get { return this.nextRefillDate; }
            set { this.nextRefillDate = value; }
        }

        private HealthServiceDate nextRefillDate;

        /// <summary>
        /// Gets or sets the number of medication refills left.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public int? RefillsLeft
        {
            get { return this.refillsLeft; }
            set { this.refillsLeft = value; }
        }

        private int? refillsLeft;

        /// <summary>
        /// Gets or sets the pharmacy.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public Organization Pharmacy
        {
            get { return this.pharmacy; }
            set { this.pharmacy = value; }
        }

        private Organization pharmacy;

        /// <summary>
        /// Gets or sets the free form prescription number.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PrescriptionNumber
        {
            get { return this.prescriptionNumber; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PrescriptionNumber");
                this.prescriptionNumber = value;
            }
        }

        private string prescriptionNumber;

        /// <summary>
        /// Gets or sets the lot number for the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string LotNumber
        {
            get { return this.lotNumber; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "LotNumber");
                this.lotNumber = value;
            }
        }

        private string lotNumber;

        /// <summary>
        /// Gets a string representation of the medication fill.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the medication fill.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = this.Name.ToString();

            if (this.dateFilled != null)
            {
                result =
                    string.Format(
                        Resources.MedicationFillToStringFormat,
                        this.Name.ToString(),
                        this.dateFilled.ToString());
            }

            return result;
        }
    }
}
