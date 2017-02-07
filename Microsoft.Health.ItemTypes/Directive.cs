// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates an advance directive.
    /// </summary>
    ///
    /// <remarks>
    /// An advance directive is a legal document that provides directions for future
    /// health care decisions in case the patient becomes incapacitated.
    /// </remarks>
    /// 
    public class Directive : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Directive"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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
            _startDate = new ApproximateDateTime();
            _startDate.ParseXml(itemNav.SelectSingleNode("start-date"));

            // <stop-date>
            _stopDate = new ApproximateDateTime();
            _stopDate.ParseXml(itemNav.SelectSingleNode("stop-date"));

            // <description>
            _description = 
                XPathHelper.GetOptNavValue(itemNav, "description");


            // <full-resuscitation>
            _fullResuscitation =
                XPathHelper.GetOptNavValueAsBool(itemNav, "full-resuscitation");

            // <prohibited-interventions>
            _prohibitedInterventions =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "prohibited-interventions");

            // <additional-instructions>
            _additionalInstructions =
                XPathHelper.GetOptNavValue(itemNav, "additional-instructions");

            // <attending-physician>
            _attendingPhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "attending-physician");

            // <attending-physician-endorsement>
            _attendingPhysicianEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "attending-physician-endorsement");

            // <attending-nurse>
            _attendingNurse =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "attending-nurse");

            // <attending-nurse-endorsement" type="d:date-time">
            _attendingNurseEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "attending-nurse-endorsement");

            // <expiration-date>
            _expirationDate =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "expiration-date");

            // <discontinuation-date>
            _discontinuationDate =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "discontinuation-date"); 

            // <discontinuation-physician>
            _discontinuationPhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "discontinuation-physician"); 
            
            // <discontinuation-physician-endorsement>
            _discontinuationPhysicianEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "discontinuation-physician-endorsement"); 


            // <discontinuation-nurse>
            _discontinuationNurse =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "discontinuation-nurse"); 

            // <discontinuation-nurse-endorsement>
            _discontinuationNurseEndorsement =
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
            Validator.ThrowSerializationIfNull(_startDate, "DirectiveStartDateNotSet");
            Validator.ThrowSerializationIfNull(_stopDate, "DirectiveStopDateNotSet");
            
            // <directive>
            writer.WriteStartElement("directive");

            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "start-date",
                _startDate);

            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "stop-date",
                _stopDate);

            XmlWriterHelper.WriteOptString(
                writer,
                "description", 
                _description);


            // <full-resuscitation>
            XmlWriterHelper.WriteOptBool(
                writer,
                "full-resuscitation",
                _fullResuscitation);


            // <prohibited-interventions>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "prohibited-interventions", 
                _prohibitedInterventions);
            
            // <additional-instructions>
            XmlWriterHelper.WriteOptString(
                writer,
                "additional-instructions", 
                _additionalInstructions);

            // <attending-physician>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "attending-physician", 
                _attendingPhysician);
            
            // <attending-physician-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "attending-physician-endorsement", 
                _attendingPhysicianEndorsement);

            // <attending-nurse>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "attending-nurse",
                _attendingNurse);

            // <attending-nurse-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "attending-nurse-endorsement",
                _attendingNurseEndorsement);
            
            // <expiration-date>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "expiration-date", 
                _expirationDate);

            // <discontinuation-date>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "discontinuation-date", 
                _discontinuationDate);

            // <discontinuation-physician>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "discontinuation-physician",
                _discontinuationPhysician);

            // <discontinuation-physician-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "discontinuation-physician-endorsement",
                _discontinuationPhysicianEndorsement);

            // <discontinuation-nurse>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "discontinuation-nurse",
                _discontinuationNurse);

            // <discontinuation-nurse-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "discontinuation-nurse-endorsement",
                _discontinuationNurseEndorsement);

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
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }
        private string _description;

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
            get { return _startDate; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "StartDate", "DirectiveStartDateMandatory");
                _startDate = value;
            }
        }
        private ApproximateDateTime _startDate;
        

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
            get { return _stopDate; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "StopDate", "DirectiveStopDateMandatory");
                _stopDate = value;
            }
        }
        private ApproximateDateTime _stopDate;

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
            get { return _fullResuscitation; }
            set { _fullResuscitation = value; }
        }
        private bool? _fullResuscitation;

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
            get { return _prohibitedInterventions; }
            set { _prohibitedInterventions = value; }
        }
        private CodableValue _prohibitedInterventions;

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
            get { return _additionalInstructions; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "AdditionalInstructions");
                _additionalInstructions = value;
            }
        }
        private string _additionalInstructions;

        /// <summary>
        /// Gets or sets the attending physician endorsement details.
        /// </summary>
        /// 
        public PersonItem AttendingPhysician
        {
            get { return _attendingPhysician; }
            set { _attendingPhysician = value; }
        }
        private PersonItem _attendingPhysician; 

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
            get { return _attendingPhysicianEndorsement; }
            set { _attendingPhysicianEndorsement = value; }
        }
        private HealthServiceDateTime _attendingPhysicianEndorsement;
         
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
            get { return _attendingNurse; }
            set { _attendingNurse = value; }
        }
        private PersonItem _attendingNurse;

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
            get { return _attendingNurseEndorsement; }
            set { _attendingNurseEndorsement = value; }
        }
        private HealthServiceDateTime _attendingNurseEndorsement;

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
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }
        private HealthServiceDateTime _expirationDate;

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
            get { return _discontinuationDate; }
            set {_discontinuationDate = value; }
        }
        private ApproximateDateTime _discontinuationDate;

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
            get { return _discontinuationPhysician; }
            set { _discontinuationPhysician = value; }
        }
        private PersonItem _discontinuationPhysician; 

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
            get { return _discontinuationPhysicianEndorsement; }
            set { _discontinuationPhysicianEndorsement = value; }
        }
        private HealthServiceDateTime _discontinuationPhysicianEndorsement;
        
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
            get { return _discontinuationNurse; }
            set { _discontinuationNurse = value; }
        }
        private PersonItem _discontinuationNurse; 

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
            get { return _discontinuationNurseEndorsement; }
            set { _discontinuationNurseEndorsement = value; }
        }
        private HealthServiceDateTime _discontinuationNurseEndorsement;

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
            return Description ?? String.Empty;
        }
    }
}
