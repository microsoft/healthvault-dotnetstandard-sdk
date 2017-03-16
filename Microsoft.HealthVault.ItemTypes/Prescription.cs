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
    /// Information related to a medication prescription.
    /// </summary>
    ///
    public class Prescription : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Prescription"/> class with default
        /// values.
        /// </summary>
        ///
        public Prescription()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Prescription"/> class
        /// with the specified prescriber.
        /// </summary>
        ///
        /// <param name="prescribedBy">
        /// The person that prescribed the medication.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="person"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Prescription(PersonItem prescribedBy)
        {
            this.PrescribedBy = prescribedBy;
        }

        /// <summary>
        /// Populates this Prescription instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the prescription information.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a prescription node.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // <prescribed-by>
            this.prescribedBy = new PersonItem();
            this.prescribedBy.ParseXml(navigator.SelectSingleNode("prescribed-by"));

            // <date-prescribed>
            this.datePrescribed =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "date-prescribed");

            // <amount-prescribed>
            this.amountPrescribed =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(navigator, "amount-prescribed");

            // <substitution>
            this.substitution =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "substitution");

            // <refills>
            this.refills =
                XPathHelper.GetOptNavValueAsInt(navigator, "refills");

            // <days-supply>
            this.daysSupply =
                XPathHelper.GetOptNavValueAsInt(navigator, "days-supply");

            // <prescription-expiration>
            this.expiration =
                XPathHelper.GetOptNavValue<HealthServiceDate>(navigator, "prescription-expiration");

            // <instructions>
            this.instructions =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "instructions");
        }

        /// <summary>
        /// Writes the prescription data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the prescription data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the prescription data to.
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
        /// The <see cref="PrescribedBy"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.prescribedBy, "PrescriptionPrescribedByNotSet");

            // <prescription>
            writer.WriteStartElement(nodeName);

            this.prescribedBy.WriteXml("prescribed-by", writer);

            // <date-prescribed>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-prescribed",
                this.datePrescribed);

            // <amount-prescribed>
            XmlWriterHelper.WriteOpt(
                writer,
                "amount-prescribed",
                this.amountPrescribed);

            // <substitution>
            XmlWriterHelper.WriteOpt(
                writer,
                "substitution",
                this.substitution);

            // <refills>
            XmlWriterHelper.WriteOptInt(
                writer,
                "refills",
                this.refills);

            // <days-supply>
            XmlWriterHelper.WriteOptInt(
                writer,
                "days-supply",
                this.daysSupply);

            // <prescription-expiration>
            XmlWriterHelper.WriteOpt(
                writer,
                "prescription-expiration",
                this.expiration);

            // <instructions>
            XmlWriterHelper.WriteOpt(
                writer,
                "instructions",
                this.instructions);

            // </prescription>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the person that prescribed the medication.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Person"/> instance.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public PersonItem PrescribedBy
        {
            get { return this.prescribedBy; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "PrescribedBy", "PrescriptionPrescribedByNameMandatory");
                this.prescribedBy = value;
            }
        }

        private PersonItem prescribedBy;

        /// <summary>
        /// Gets or sets the date the medication was prescribed.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime DatePrescribed
        {
            get { return this.datePrescribed; }
            set { this.datePrescribed = value; }
        }

        private ApproximateDateTime datePrescribed;

        /// <summary>
        /// Gets or sets the amount of medication prescribed.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public GeneralMeasurement AmountPrescribed
        {
            get { return this.amountPrescribed; }
            set { this.amountPrescribed = value; }
        }

        private GeneralMeasurement amountPrescribed;

        /// <summary>
        /// Gets or sets whether a substitution is permitted.
        /// </summary>
        ///
        /// <remarks>
        /// Example: Dispense as written, substitution allowed.
        /// If the value is not known, it will be set to <b>null</b>.
        /// The preferred vocabulary for substitution is "medication-substitution".
        /// </remarks>
        ///
        public CodableValue Substitution
        {
            get { return this.substitution; }
            set { this.substitution = value; }
        }

        private CodableValue substitution;

        /// <summary>
        /// Gets or sets the number of refills of the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public int? Refills
        {
            get { return this.refills; }
            set { this.refills = value; }
        }

        private int? refills;

        /// <summary>
        /// Gets or sets the number of days supply of medication.
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
        /// Gets or sets the date the prescription expires.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public HealthServiceDate PrescriptionExpiration
        {
            get { return this.expiration; }
            set { this.expiration = value; }
        }

        private HealthServiceDate expiration;

        /// <summary>
        /// Gets or sets the medication instructions.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue Instructions
        {
            get { return this.instructions; }
            set { this.instructions = value; }
        }

        private CodableValue instructions;

        /// <summary>
        /// Gets a string representation of the prescription item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the prescription item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.PrescribedBy != null)
            {
                result = this.PrescribedBy.ToString();
            }

            return result;
        }
    }
}
