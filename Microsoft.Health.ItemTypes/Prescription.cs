// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Information related to a medication prescription.
    /// </summary>
    /// 
    public class Prescription : HealthRecordItemData
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
            _prescribedBy = new PersonItem();
            _prescribedBy.ParseXml(navigator.SelectSingleNode("prescribed-by"));

            // <date-prescribed>
            _datePrescribed = 
                XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "date-prescribed");

            // <amount-prescribed>
            _amountPrescribed =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(navigator, "amount-prescribed");

            // <substitution>
            _substitution =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "substitution");

            // <refills>
            _refills =
                XPathHelper.GetOptNavValueAsInt(navigator, "refills");

            // <days-supply>
            _daysSupply =
                XPathHelper.GetOptNavValueAsInt(navigator, "days-supply");

            // <prescription-expiration>
            _expiration =
                XPathHelper.GetOptNavValue<HealthServiceDate>(navigator, "prescription-expiration");

            // <instructions>
            _instructions =
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
            Validator.ThrowSerializationIfNull(_prescribedBy, "PrescriptionPrescribedByNotSet");
            
            // <prescription>
            writer.WriteStartElement(nodeName);

            _prescribedBy.WriteXml("prescribed-by", writer);

            // <date-prescribed>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "date-prescribed",
                _datePrescribed);

            // <amount-prescribed>
            XmlWriterHelper.WriteOpt<GeneralMeasurement>(
                writer,
                "amount-prescribed",
                _amountPrescribed);

            // <substitution>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "substitution",
                _substitution);

            // <refills>
            XmlWriterHelper.WriteOptInt(
                writer,
                "refills",
                _refills);

            // <days-supply>
            XmlWriterHelper.WriteOptInt(
                writer,
                "days-supply",
                _daysSupply);

            // <prescription-expiration>
            XmlWriterHelper.WriteOpt<HealthServiceDate>(
                writer,
                "prescription-expiration",
                _expiration);

            // <instructions>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "instructions",
                _instructions);

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
            get { return _prescribedBy; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "PrescribedBy", "PrescriptionPrescribedByNameMandatory");
                _prescribedBy = value; 
            }
        }
        private PersonItem _prescribedBy;

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
            get { return _datePrescribed; }
            set { _datePrescribed = value; }
        }
        private ApproximateDateTime _datePrescribed;

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
            get { return _amountPrescribed; }
            set { _amountPrescribed = value; }
        }
        private GeneralMeasurement _amountPrescribed;

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
            get { return _substitution; }
            set { _substitution = value; }
        }
        private CodableValue _substitution;

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
            get { return _refills; }
            set { _refills = value; }
        }
        private int? _refills;

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
            get { return _daysSupply; }
            set { _daysSupply = value; }
        }
        private int? _daysSupply;

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
            get { return _expiration; }
            set { _expiration = value; }
        }
        private HealthServiceDate _expiration;

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
            get { return _instructions; }
            set { _instructions = value; }
        }
        private CodableValue _instructions;

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
            string result = String.Empty;

            if (PrescribedBy != null)
            {
                result = PrescribedBy.ToString();
            }

            return result;
        }
     }

}
