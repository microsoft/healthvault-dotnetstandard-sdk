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
    /// Represents a medication health record item.
    /// </summary>
    /// 
    public class Medication : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Medication"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Medication()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Medication"/> class with the specified name.
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
        public Medication(CodableValue name)
            : base(TypeId)
        {
            this.Name = name;
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("30cafccc-047d-4288-94ef-643571f7919d");

        /// <summary>
        /// Populates this medication instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the medication data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a medication node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("medication");

            Validator.ThrowInvalidIfNull(itemNav, "MedicationUnexpectedNode");

            _name = new CodableValue();
            _name.ParseXml(itemNav.SelectSingleNode("name"));

            _genericName =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "generic-name");

            _dose =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "dose");

            _strength =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "strength");

            _frequency =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "frequency");

            _route =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "route");

            _indication =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "indication");

            _dateStarted =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-started");

            _dateDiscontinued =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-discontinued");

            _prescribed =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "prescribed");

            _prescription =
                XPathHelper.GetOptNavValue<Prescription>(itemNav, "prescription");
        }

        /// <summary>
        /// Writes the medication data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the medication data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, "MedicationNameNotSet");

            // <medication>
            writer.WriteStartElement("medication");

            // <name>
            _name.WriteXml("name", writer);

            // <generic-name>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "generic-name",
                _genericName);

            // <dose>
            XmlWriterHelper.WriteOpt<GeneralMeasurement>(
                writer,
                "dose",
                _dose);

            // <strength>
            XmlWriterHelper.WriteOpt<GeneralMeasurement>(
                writer,
                "strength",
                _strength);

            // <frequency>
            XmlWriterHelper.WriteOpt<GeneralMeasurement>(
                writer,
                "frequency",
                _frequency);

            // <route>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "route",
                _route);

            // <indication>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "indication",
                _indication);

            // <date-started>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "date-started",
                _dateStarted);

            // <date-discontinued>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "date-discontinued",
                _dateDiscontinued);

            // <prescribed>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "prescribed",
                _prescribed);

            // <prescription>
            XmlWriterHelper.WriteOpt<Prescription>(
                writer,
                "prescription",
                _prescription);

            // </medication>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the medication name and clinical code.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Name", "MedicationNameMandatory");
                _name = value;
            }
        }
        private CodableValue _name;


        /// <summary>
        /// Gets or sets the generic name of the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue GenericName
        {
            get { return _genericName; }
            set { _genericName = value; }
        }
        private CodableValue _genericName;

        /// <summary>
        /// Gets or sets the dose of the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 1 tablet, 50 ml.
        /// </remarks>
        /// 
        public GeneralMeasurement Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }
        private GeneralMeasurement _dose;

        /// <summary>
        /// Gets or sets the strength of the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 500 mg, 10 mg/ml.
        /// </remarks>
        /// 
        public GeneralMeasurement Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }
        private GeneralMeasurement _strength;

        /// <summary>
        /// Gets or sets how often the medication is taken.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 1 tablet per day, 2 every 6 hours, as needed.
        /// </remarks>
        /// 
        public GeneralMeasurement Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }
        private GeneralMeasurement _frequency;

        /// <summary>
        /// Gets or sets the route by which the medication is administered.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// The preferred vocabulary for route is "medication-routes".
        /// </remarks>
        /// 
        public CodableValue Route
        {
            get { return _route; }
            set { _route = value; }
        }
        private CodableValue _route;

        /// <summary>
        /// Gets or sets the indication for the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue Indication
        {
            get { return _indication; }
            set { _indication = value; }
        }
        private CodableValue _indication;

        /// <summary>
        /// Gets or sets the date on which the person started taken the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public ApproximateDateTime DateStarted
        {
            get { return _dateStarted; }
            set { _dateStarted = value; }
        }
        private ApproximateDateTime _dateStarted;

        /// <summary>
        /// Gets or sets the date on which the medication was discontinued.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public ApproximateDateTime DateDiscontinued
        {
            get { return _dateDiscontinued; }
            set { _dateDiscontinued = value; }
        }
        private ApproximateDateTime _dateDiscontinued;


        /// <summary>
        /// Gets or sets the source of the prescription.
        /// </summary>
        /// 
        /// <remarks>
        /// A medication that is prescribed by a physician should code "prescribed"
        /// into this element. 
        /// If the value is not known, it will be set to <b>null</b>.
        /// The preferred vocabulary for prescribed is "medication-prescribed".
        /// </remarks>
        /// 
        public CodableValue Prescribed
        {
            get { return _prescribed; }
            set { _prescribed = value; }
        }
        private CodableValue _prescribed;

        /// <summary>
        /// Gets or sets the prescription for the medication.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public Prescription Prescription
        {
            get { return _prescription; }
            set { _prescription = value; }
        }
        private Prescription _prescription;

        /// <summary>
        /// Gets a string representation of the medication.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the medication.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            string space = ResourceRetriever.GetSpace("errors");

            result.Append(Name.ToString());

            if (GenericName != null)
            {
                result.Append(space);
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "MedicationToStringFormatGenericName"),
                    GenericName.ToString());
            }

            if (Strength != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Strength.ToString());
            }

            if (Dose != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Dose.ToString());
            }

            if (Frequency != null)
            {
                result.Append(space);
                result.Append(Frequency.ToString());
            }
            return result.ToString();
        }
    }
}
