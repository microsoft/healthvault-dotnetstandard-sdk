// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a medication thing.
    /// </summary>
    ///
    public class Medication : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Medication"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
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
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.MedicationUnexpectedNode);

            this.name = new CodableValue();
            this.name.ParseXml(itemNav.SelectSingleNode("name"));

            this.genericName =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "generic-name");

            this.dose =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "dose");

            this.strength =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "strength");

            this.frequency =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "frequency");

            this.route =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "route");

            this.indication =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "indication");

            this.dateStarted =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-started");

            this.dateDiscontinued =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-discontinued");

            this.prescribed =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "prescribed");

            this.prescription =
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
            Validator.ThrowSerializationIfNull(this.name, Resources.MedicationNameNotSet);

            // <medication>
            writer.WriteStartElement("medication");

            // <name>
            this.name.WriteXml("name", writer);

            // <generic-name>
            XmlWriterHelper.WriteOpt(
                writer,
                "generic-name",
                this.genericName);

            // <dose>
            XmlWriterHelper.WriteOpt(
                writer,
                "dose",
                this.dose);

            // <strength>
            XmlWriterHelper.WriteOpt(
                writer,
                "strength",
                this.strength);

            // <frequency>
            XmlWriterHelper.WriteOpt(
                writer,
                "frequency",
                this.frequency);

            // <route>
            XmlWriterHelper.WriteOpt(
                writer,
                "route",
                this.route);

            // <indication>
            XmlWriterHelper.WriteOpt(
                writer,
                "indication",
                this.indication);

            // <date-started>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-started",
                this.dateStarted);

            // <date-discontinued>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-discontinued",
                this.dateDiscontinued);

            // <prescribed>
            XmlWriterHelper.WriteOpt(
                writer,
                "prescribed",
                this.prescribed);

            // <prescription>
            XmlWriterHelper.WriteOpt(
                writer,
                "prescription",
                this.prescription);

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
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.MedicationNameMandatory);
                this.name = value;
            }
        }

        private CodableValue name;

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
            get { return this.genericName; }
            set { this.genericName = value; }
        }

        private CodableValue genericName;

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
            get { return this.dose; }
            set { this.dose = value; }
        }

        private GeneralMeasurement dose;

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
            get { return this.strength; }
            set { this.strength = value; }
        }

        private GeneralMeasurement strength;

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
            get { return this.frequency; }
            set { this.frequency = value; }
        }

        private GeneralMeasurement frequency;

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
            get { return this.route; }
            set { this.route = value; }
        }

        private CodableValue route;

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
            get { return this.indication; }
            set { this.indication = value; }
        }

        private CodableValue indication;

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
            get { return this.dateStarted; }
            set { this.dateStarted = value; }
        }

        private ApproximateDateTime dateStarted;

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
            get { return this.dateDiscontinued; }
            set { this.dateDiscontinued = value; }
        }

        private ApproximateDateTime dateDiscontinued;

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
            get { return this.prescribed; }
            set { this.prescribed = value; }
        }

        private CodableValue prescribed;

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
            get { return this.prescription; }
            set { this.prescription = value; }
        }

        private Prescription prescription;

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

            result.Append(this.Name);

            if (this.GenericName != null)
            {
                result.Append(" ");
                result.AppendFormat(
                    Resources.MedicationToStringFormatGenericName,
                    this.GenericName.ToString());
            }

            if (this.Strength != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Strength.ToString());
            }

            if (this.Dose != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Dose.ToString());
            }

            if (this.Frequency != null)
            {
                result.Append(" ");
                result.Append(this.Frequency);
            }

            return result.ToString();
        }
    }
}
