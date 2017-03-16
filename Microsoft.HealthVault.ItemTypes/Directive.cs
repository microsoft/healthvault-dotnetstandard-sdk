// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates an advance directive.
    /// </summary>
    ///
    /// <remarks>
    /// An advance directive is a legal document that provides directions for future
    /// health care decisions in case the patient becomes incapacitated.
    /// </remarks>
    ///
    public class Directive : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Directive"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Directive()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Directive"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="startDate">
        /// The date the directive takes effect.
        /// </param>
        ///
        /// <param name="stopDate">
        /// The date the directive stops being effective.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="startDate"/> parameter or <paramref name="stopDate"/>
        /// is <b>null</b> or empty.
        /// </exception>
        ///
        public Directive(
            ApproximateDateTime startDate,
            ApproximateDateTime stopDate)
            : base(TypeId)
        {
            this.StartDate = startDate;
            this.StopDate = stopDate;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("822a5e5a-14f1-4d06-b92f-8f3f1b05218f");

        /// <summary>
        /// Populates this <see cref="Directive"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the directive data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node of the <paramref name="typeSpecificXml"/>
        /// parameter is not a directive node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("directive");

            Validator.ThrowInvalidIfNull(itemNav, "DirectiveUnexpectedNode");

            // <start-date>
            this.startDate = new ApproximateDateTime();
            this.startDate.ParseXml(itemNav.SelectSingleNode("start-date"));

            // <stop-date>
            this.stopDate = new ApproximateDateTime();
            this.stopDate.ParseXml(itemNav.SelectSingleNode("stop-date"));

            // <description>
            this.description =
                XPathHelper.GetOptNavValue(itemNav, "description");

            // <full-resuscitation>
            this.fullResuscitation =
                XPathHelper.GetOptNavValueAsBool(itemNav, "full-resuscitation");

            // <prohibited-interventions>
            this.prohibitedInterventions =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "prohibited-interventions");

            // <additional-instructions>
            this.additionalInstructions =
                XPathHelper.GetOptNavValue(itemNav, "additional-instructions");

            // <attending-physician>
            this.attendingPhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "attending-physician");

            // <attending-physician-endorsement>
            this.attendingPhysicianEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "attending-physician-endorsement");

            // <attending-nurse>
            this.attendingNurse =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "attending-nurse");

            // <attending-nurse-endorsement" type="d:date-time">
            this.attendingNurseEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "attending-nurse-endorsement");

            // <expiration-date>
            this.expirationDate =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "expiration-date");

            // <discontinuation-date>
            this.discontinuationDate =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "discontinuation-date");

            // <discontinuation-physician>
            this.discontinuationPhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "discontinuation-physician");

            // <discontinuation-physician-endorsement>
            this.discontinuationPhysicianEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "discontinuation-physician-endorsement");

            // <discontinuation-nurse>
            this.discontinuationNurse =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "discontinuation-nurse");

            // <discontinuation-nurse-endorsement>
            this.discontinuationNurseEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "discontinuation-nurse-endorsement");
        }

        /// <summary>
        /// Writes the directive data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the directive data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="StartDate"/> or <see cref="StopDate"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.startDate, "DirectiveStartDateNotSet");
            Validator.ThrowSerializationIfNull(this.stopDate, "DirectiveStopDateNotSet");

            // <directive>
            writer.WriteStartElement("directive");

            XmlWriterHelper.WriteOpt(
                writer,
                "start-date",
                this.startDate);

            XmlWriterHelper.WriteOpt(
                writer,
                "stop-date",
                this.stopDate);

            XmlWriterHelper.WriteOptString(
                writer,
                "description",
                this.description);

            // <full-resuscitation>
            XmlWriterHelper.WriteOptBool(
                writer,
                "full-resuscitation",
                this.fullResuscitation);

            // <prohibited-interventions>
            XmlWriterHelper.WriteOpt(
                writer,
                "prohibited-interventions",
                this.prohibitedInterventions);

            // <additional-instructions>
            XmlWriterHelper.WriteOptString(
                writer,
                "additional-instructions",
                this.additionalInstructions);

            // <attending-physician>
            XmlWriterHelper.WriteOpt(
                writer,
                "attending-physician",
                this.attendingPhysician);

            // <attending-physician-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "attending-physician-endorsement",
                this.attendingPhysicianEndorsement);

            // <attending-nurse>
            XmlWriterHelper.WriteOpt(
                writer,
                "attending-nurse",
                this.attendingNurse);

            // <attending-nurse-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "attending-nurse-endorsement",
                this.attendingNurseEndorsement);

            // <expiration-date>
            XmlWriterHelper.WriteOpt(
                writer,
                "expiration-date",
                this.expirationDate);

            // <discontinuation-date>
            XmlWriterHelper.WriteOpt(
                writer,
                "discontinuation-date",
                this.discontinuationDate);

            // <discontinuation-physician>
            XmlWriterHelper.WriteOpt(
                writer,
                "discontinuation-physician",
                this.discontinuationPhysician);

            // <discontinuation-physician-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "discontinuation-physician-endorsement",
                this.discontinuationPhysicianEndorsement);

            // <discontinuation-nurse>
            XmlWriterHelper.WriteOpt(
                writer,
                "discontinuation-nurse",
                this.discontinuationNurse);

            // <discontinuation-nurse-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "discontinuation-nurse-endorsement",
                this.discontinuationNurseEndorsement);

            // </directive>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description of the directive.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the directive description.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace when set.
        /// </exception>
        ///
        public string Description
        {
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets the approximate date of the directive is effective.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateDateTime"/> instance representing the
        /// effective date of the directive.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime StartDate
        {
            get { return this.startDate; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "StartDate", "DirectiveStartDateMandatory");
                this.startDate = value;
            }
        }

        private ApproximateDateTime startDate;

        /// <summary>
        /// Gets or sets the approximate date the directive is no longer to
        /// be considered.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateDateTime"/> instance representing the
        /// stop date of the directive.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime StopDate
        {
            get { return this.stopDate; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "StopDate", "DirectiveStopDateMandatory");
                this.stopDate = value;
            }
        }

        private ApproximateDateTime stopDate;

        /// <summary>
        /// Gets or sets a value indicating the resuscitation status.
        /// </summary>
        ///
        /// <returns>
        /// <b>true</b> for full resuscitation; otherwise, <b>false</b>.
        /// </returns>
        ///
        public bool? FullResuscitation
        {
            get { return this.fullResuscitation; }
            set { this.fullResuscitation = value; }
        }

        private bool? fullResuscitation;

        /// <summary>
        /// Gets or sets the list of prohibited interventions in this directive.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the list.
        /// </returns>
        ///
        public CodableValue ProhibitedInterventions
        {
            get { return this.prohibitedInterventions; }
            set { this.prohibitedInterventions = value; }
        }

        private CodableValue prohibitedInterventions;

        /// <summary>
        /// Gets or sets additional directive instructions.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the instructions.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the manufacturer should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AdditionalInstructions
        {
            get { return this.additionalInstructions; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AdditionalInstructions");
                this.additionalInstructions = value;
            }
        }

        private string additionalInstructions;

        /// <summary>
        /// Gets or sets the attending physician endorsement details.
        /// </summary>
        ///
        public PersonItem AttendingPhysician
        {
            get { return this.attendingPhysician; }
            set { this.attendingPhysician = value; }
        }

        private PersonItem attendingPhysician;

        /// <summary>
        /// Gets or sets the date and time for the attending physician
        /// endorsement details.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        public HealthServiceDateTime AttendingPhysicianEndorsement
        {
            get { return this.attendingPhysicianEndorsement; }
            set { this.attendingPhysicianEndorsement = value; }
        }

        private HealthServiceDateTime attendingPhysicianEndorsement;

        /// <summary>
        /// Gets or sets the attending nurse endorsement details.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the nurse
        /// endorsement details.
        /// </returns>
        ///
        public PersonItem AttendingNurse
        {
            get { return this.attendingNurse; }
            set { this.attendingNurse = value; }
        }

        private PersonItem attendingNurse;

        /// <summary>
        /// Gets or sets the date and time for the attending nurse endorsement details.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        public HealthServiceDateTime AttendingNurseEndorsement
        {
            get { return this.attendingNurseEndorsement; }
            set { this.attendingNurseEndorsement = value; }
        }

        private HealthServiceDateTime attendingNurseEndorsement;

        /// <summary>
        /// Gets or sets the date and time when the patient expired.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        public HealthServiceDateTime ExpirationDate
        {
            get { return this.expirationDate; }
            set { this.expirationDate = value; }
        }

        private HealthServiceDateTime expirationDate;

        /// <summary>
        /// Gets or sets the date/time when clinical support was discontinued.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateDateTime"/> instance representing the date and time.
        /// </returns>
        ///
        public ApproximateDateTime DiscontinuationDate
        {
            get { return this.discontinuationDate; }
            set { this.discontinuationDate = value; }
        }

        private ApproximateDateTime discontinuationDate;

        /// <summary>
        /// Gets or sets the attending physician discontinuation details.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the details.
        /// </returns>
        ///
        /// <remarks>
        /// This type provides discontinuation details including name, identity,
        /// and signature date and time of the attending physician.
        /// </remarks>
        ///
        public PersonItem DiscontinuationPhysician
        {
            get { return this.discontinuationPhysician; }
            set { this.discontinuationPhysician = value; }
        }

        private PersonItem discontinuationPhysician;

        /// <summary>
        /// Gets or sets the date and time for the attending physician
        /// discontinuation endorsement.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the
        /// date and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        public HealthServiceDateTime DiscontinuationPhysicianEndorsement
        {
            get { return this.discontinuationPhysicianEndorsement; }
            set { this.discontinuationPhysicianEndorsement = value; }
        }

        private HealthServiceDateTime discontinuationPhysicianEndorsement;

        /// <summary>
        /// Gets or sets the attending nurse discontinuation details.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the details.
        /// </returns>
        ///
        /// <remarks>
        /// This type provides discontinuation details including name, identity,
        /// and signature date and time of the attending nurse.
        /// </remarks>
        ///
        public PersonItem DiscontinuationNurse
        {
            get { return this.discontinuationNurse; }
            set { this.discontinuationNurse = value; }
        }

        private PersonItem discontinuationNurse;

        /// <summary>
        /// Gets or sets the date and time for the attending nurse
        /// discontinuation endorsement.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        ///
        public HealthServiceDateTime DiscontinuationNurseEndorsement
        {
            get { return this.discontinuationNurseEndorsement; }
            set { this.discontinuationNurseEndorsement = value; }
        }

        private HealthServiceDateTime discontinuationNurseEndorsement;

        /// <summary>
        /// Gets a string representation of the directive item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the directive item.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Description ?? string.Empty;
        }
    }
}
