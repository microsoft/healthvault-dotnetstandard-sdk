// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's
    /// medication and supplement usage for a day.
    /// </summary>
    ///
    public class DailyMedicationUsage : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DailyMedicationUsage"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/>
        /// method is called.
        /// </remarks>
        ///
        public DailyMedicationUsage()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DailyMedicationUsage"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date when the medication/supplement was consumed.
        /// </param>
        ///
        /// <param name="drugName">
        /// The name of the drug.
        /// </param>
        ///
        /// <param name="dosesConsumed">
        /// The number of doses consumed by the person.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="drugName"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public DailyMedicationUsage(
            HealthServiceDate when,
            CodableValue drugName,
            int dosesConsumed)
            : base(TypeId)
        {
            this.When = when;
            this.DrugName = drugName;
            this.DosesConsumed = dosesConsumed;
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
            new Guid("A9A76456-0357-493e-B840-598BBB9483FD");

        /// <summary>
        /// Populates this <see cref="DailyMedicationUsage"/> instance from the
        /// data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the medication usage data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a <see cref="DailyMedicationUsage"/> node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "daily-medication-usage");

            Validator.ThrowInvalidIfNull(itemNav, Resources.DailyMedicationUsageUnexpectedNode);

            this.when = new HealthServiceDate();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.drugName = new CodableValue();
            this.drugName.ParseXml(itemNav.SelectSingleNode("drug-name"));

            this.dosesConsumed =
                itemNav.SelectSingleNode("number-doses-consumed-in-day").ValueAsInt;

            this.purposeOfUse =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "purpose-of-use");

            this.intendedDoses =
                XPathHelper.GetOptNavValueAsInt(itemNav, "number-doses-intended-in-day");

            this.usageSchedule =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "medication-usage-schedule");

            this.drugForm =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "drug-form");

            this.prescriptionType =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "prescription-type");

            this.singleDoseDescription =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "single-dose-description");
        }

        /// <summary>
        /// Writes the daily medication usage data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the daily medication usage data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="DrugName"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.drugName, Resources.DailyMedicationUsageDrugNameNotSet);

            // <daily-medication-usage>
            writer.WriteStartElement("daily-medication-usage");

            // <when>
            this.when.WriteXml("when", writer);

            this.drugName.WriteXml("drug-name", writer);

            writer.WriteElementString(
                "number-doses-consumed-in-day",
                this.dosesConsumed.ToString(CultureInfo.InvariantCulture));

            XmlWriterHelper.WriteOpt(
                writer,
                "purpose-of-use",
                this.purposeOfUse);

            XmlWriterHelper.WriteOptInt(
                writer,
                "number-doses-intended-in-day",
                this.intendedDoses);

            XmlWriterHelper.WriteOpt(
                writer,
                "medication-usage-schedule",
                this.usageSchedule);

            XmlWriterHelper.WriteOpt(
                writer,
                "drug-form",
                this.drugForm);

            XmlWriterHelper.WriteOpt(
                writer,
                "prescription-type",
                this.prescriptionType);

            XmlWriterHelper.WriteOpt(
                writer,
                "single-dose-description",
                this.singleDoseDescription);

            // </daily-medication-usage>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date of the medication usage.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="HealthServiceDate"/> representing the date.
        /// </value>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDate When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDate when = new HealthServiceDate();

        /// <summary>
        /// Gets or sets the name of the drug.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the drug name.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue DrugName
        {
            get { return this.drugName; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(DrugName), Resources.DailyMedicationUsageDrugNameNull);
                this.drugName = value;
            }
        }

        private CodableValue drugName;

        /// <summary>
        /// Gets or sets the number of doses of the drug consumed in the day.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the number of doses consumed by the person
        /// in the specified day.
        /// </value>
        ///
        public int DosesConsumed
        {
            get { return this.dosesConsumed; }
            set { this.dosesConsumed = value; }
        }

        private int dosesConsumed;

        /// <summary>
        /// Gets or sets the purpose of the medication or supplement.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the purpose of
        /// the medication or supplement.
        /// </value>
        ///
        public CodableValue PurposeOfUse
        {
            get { return this.purposeOfUse; }
            set { this.purposeOfUse = value; }
        }

        private CodableValue purposeOfUse;

        /// <summary>
        /// Gets or sets the intended number of doses the person should take in a day.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the intended number of doses the person
        /// should take in a day. If this value is unknown, it can be set to <b>null</b>.
        /// </value>
        ///
        public int? IntendedDoses
        {
            get { return this.intendedDoses; }
            set { this.intendedDoses = value; }
        }

        private int? intendedDoses;

        /// <summary>
        /// Gets or sets the usage schedule.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the schedule of usage.
        /// </value>
        ///
        public CodableValue UsageSchedule
        {
            get { return this.usageSchedule; }
            set { this.usageSchedule = value; }
        }

        private CodableValue usageSchedule;

        /// <summary>
        /// Gets or sets the form by which the drug/supplement is taken.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the form by which
        /// the drug/supplement is taken.
        /// </value>
        ///
        public CodableValue DrugForm
        {
            get { return this.drugForm; }
            set { this.drugForm = value; }
        }

        private CodableValue drugForm;

        /// <summary>
        /// Gets or sets the means by which the drug was determined to be needed.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the means by which
        /// the drug was determined to be needed.
        /// </value>
        ///
        public CodableValue PrescriptionType
        {
            get { return this.prescriptionType; }
            set { this.prescriptionType = value; }
        }

        private CodableValue prescriptionType;

        /// <summary>
        /// Gets or sets a description of a single dose.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing a description
        /// of a single dose.
        /// </value>
        ///
        public CodableValue SingleDoseDescription
        {
            get { return this.singleDoseDescription; }
            set { this.singleDoseDescription = value; }
        }

        private CodableValue singleDoseDescription;

        /// <summary>
        /// Gets a string representation of the daily medication usage.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the daily medication usage item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            result.AppendFormat(
                Resources.DailyMedUsageToStringFormatDrug,
                this.DrugName.Text);

            if (this.PurposeOfUse != null)
            {
                result.AppendFormat(
                    Resources.DailyMedUsageToStringFormatPurpose,
                    this.PurposeOfUse.Text);
            }

            if (this.IntendedDoses != null)
            {
                result.AppendFormat(
                    Resources.DailyMedUsageToStringFormatDosesWithIntended,
                    this.DosesConsumed,
                    this.IntendedDoses.Value);
            }
            else
            {
                result.Append(
                    Resources.DailyMedUsageToStringFormatDosesWithIntendedUnknown);
            }

            if (this.SingleDoseDescription != null)
            {
                result.AppendFormat(
                    Resources.DailyMedUsageToStringFormatSingleDoseDescription,
                    this.SingleDoseDescription);
            }

            return result.ToString();
        }
    }
}
