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
    /// Represents a health record item that encapsulates a discharge summary.
    /// </summary>
    /// 
    public class DischargeSummary : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DischargeSummary"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public DischargeSummary()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DischargeSummary"/> class 
        /// specifying the mandatory values.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date and time of the discharge summary.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public DischargeSummary(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("02EF57A2-A620-425A-8E92-A301542CCA54");

        /// <summary>
        /// Populates this discharge summary instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the discharge summary data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node of the <paramref name="typeSpecificXml"/> parameter is not
        /// a discharge summary node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("discharge-summary");

            Validator.ThrowInvalidIfNull(itemNav, "DischargeSummaryUnexpectedNode");

            // <when>
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <type>
            _type = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "type");

            // <category>
            _category = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "category");

            // <setting>
            _setting =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "setting");

            // <specialty>
            _specialty =
                XPathHelper.GetOptNavValue(itemNav, "specialty");

            // <text>
            _text =
                XPathHelper.GetOptNavValue(itemNav, "text");
            
            // <primary-provider>
            _primaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav, 
                    "primary-provider");

            // <primary-provider-endorsement>
            _primaryProviderEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "primary-provider-endorsement");

            // <secondary-provider>
            _secondaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "secondary-provider");

            // <secondary-provider-endorsement>
            _secondaryProviderEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "secondary-provider-endorsement");
            
            // <discharge-date-time>
            _dischargeDateTime =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "discharge-date-time"); 
            

            // <admitting-diagnosis>
            _admittingDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav, 
                    "admitting-diagnosis");

            // <principal-diagnosis>
            _principalDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "principal-diagnosis"); 
            
            // <additional-diagnosis>
            _additionalDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "additional-diagnosis");
            
            // <principal-procedure-physician>
            _principalProcedurePhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "principal-procedure-physician");

            // <principal-procedure>
            _principalProcedure =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "principal-procedure");

            // <additional-procedure>
            _additionalProcedure =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "additional-procedure");
        }

        /// <summary>
        /// Writes the discharge summary data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the discharge summary data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "DischargeSummaryWhenNotSet");

            // <discharge-summary>
            writer.WriteStartElement("discharge-summary");

            // <when>
            _when.WriteXml("when", writer);

            // <type>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "type",
                Type);
            
            // <category>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "category",
                Category);
            
            // <setting>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "setting",
                Setting);

            // <specialty>
            XmlWriterHelper.WriteOptString(
                writer,
                "specialty",
                _specialty); 

            // <text>
            XmlWriterHelper.WriteOptString(
                writer,
                "text",
                _text); 

            // <primary-provider>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "primary-provider",
                PrimaryProvider); 
            
            // <primary-provider-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "primary-provider-endorsement",
                PrimaryProviderEndorsement);

            // <secondary-provider>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "secondary-provider",
                SecondaryProvider);

            // <secondary-provider-endorsement>
            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "secondary-provider-endorsement",
                SecondaryProviderEndorsement);
            
            // <discharge-date-time>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "discharge-date-time",
                DischargeDateTime); 
            
            // <admitting-diagnosis>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "admitting-diagnosis",
                AdmittingDiagnosis); 
            
            // <principal-diagnosis>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "principal-diagnosis",
                PrincipalDiagnosis); 
            
            // <additional-diagnosis>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "additional-diagnosis",
                AdditionalDiagnosis); 

            // <principal-procedure-physician>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "principal-procedure-physician",
                _principalProcedurePhysician); 

            // <principal-procedure>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "principal-procedure",
                PrincipalProcedure); 

            // <additional-procedure>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "additional-procedure",
                AdditionalProcedure); 
            
            // </discharge-summary>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time when the discharge summary occurred.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date 
        /// and time.
        /// </returns>
        /// 
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
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
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the type for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the type.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the type should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private CodableValue _type;

        /// <summary>
        /// Gets or sets the category for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the category.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the category should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue Category
        {
            get { return _category; }
            set { _category = value; }
        }
        private CodableValue _category;
        
        /// <summary>
        /// Gets or sets the setting for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the setting.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the setting should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }
        private CodableValue _setting;

        /// <summary>
        /// Gets or sets the medical specialty for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the specialty.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the medical specialty should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Specialty
        {
            get { return _specialty; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Specialty");
                _specialty = value;
            }
        }
        private string _specialty;

        /// <summary>
        /// Gets or sets the textual content for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the text content.
        /// </returns>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the medical specialty should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Text
        {
            get { return _text; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Text");
                _text = value;
            }
        }
        private string _text;

        /// <summary>
        /// Gets or sets the primary provider contact information.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the primary provider.
        /// </returns>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the primary provider contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem PrimaryProvider
        {
            get { return _primaryProvider; }
            set { _primaryProvider = value; }
        }
        private PersonItem _primaryProvider;

        /// <summary>
        /// Gets or sets the date and time for the primary provider endorsement details.
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
        public HealthServiceDateTime PrimaryProviderEndorsement
        {
            get { return _primaryProviderEndorsement; }
            set { _primaryProviderEndorsement = value; }
        }
        private HealthServiceDateTime _primaryProviderEndorsement;

        /// <summary>
        /// Gets or sets the secondary provider contact information.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the secondary provider.
        /// </returns>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the secondary provider contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem SecondaryProvider
        {
            get { return _secondaryProvider; }
            set { _secondaryProvider = value; }
        }
        private PersonItem _secondaryProvider;
        
        /// <summary>
        /// Gets or sets the date and time for the secondary provider endorsement details.
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
        public HealthServiceDateTime SecondaryProviderEndorsement
        {
            get { return _secondaryProviderEndorsement; }
            set { _secondaryProviderEndorsement = value; }
        }
        private HealthServiceDateTime _secondaryProviderEndorsement;

        /// <summary>
        /// Gets or sets the approximate date and time discharged.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="ApproximateDateTime"/> instance representing the date and time.
        /// </returns> 
        /// 
        /// <remarks>
        /// An approximate date must have a year and may also have the month, 
        /// day, or both.
        /// </remarks>
        /// 
        public ApproximateDateTime DischargeDateTime
        {
            get { return _dischargeDateTime; }
            set { _dischargeDateTime = value; }
        }
        private ApproximateDateTime _dischargeDateTime;

        /// <summary>
        /// Gets or sets the admitting diagnosis for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the diagnosis.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the admitting diagnosis should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue AdmittingDiagnosis
        {
            get { return _admittingDiagnosis; }
            set { _admittingDiagnosis = value; }
        }
        private CodableValue _admittingDiagnosis;

        /// <summary>
        /// Gets or sets the principal diagnosis for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the principal diagnosis.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the principal diagnosis should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue PrincipalDiagnosis
        {
            get { return _principalDiagnosis; }
            set { _principalDiagnosis = value; }
        }
        private CodableValue _principalDiagnosis;

        /// <summary>
        /// Gets or sets the additional diagnosis for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the additional diagnosis.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the additional diagnosis should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue AdditionalDiagnosis
        {
            get { return _additionalDiagnosis; }
            set { _additionalDiagnosis = value; }
        }
        private CodableValue _additionalDiagnosis;
            
        /// <summary>
        /// Gets or sets the principal procedure physician contact information.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="PersonItem"/> instance representing the contact information.
        /// </returns>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the principal procedure physician contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem PrincipalProcedurePhysician
        {
            get { return _principalProcedurePhysician; }
            set { _principalProcedurePhysician = value; }
        }
        private PersonItem _principalProcedurePhysician;

        /// <summary>
        /// Gets or sets the principal procedure for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the principal procedure.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the principal procedure should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue PrincipalProcedure
        {
            get { return _principalProcedure; }
            set { _principalProcedure = value; }
        }
        private CodableValue _principalProcedure;

        /// <summary>
        /// Gets or sets the additional procedure for the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the additional 
        /// procedure.
        /// </returns> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the additional procedure should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue AdditionalProcedure
        {
            get { return _additionalProcedure; }
            set { _additionalProcedure = value; }
        }
        private CodableValue _additionalProcedure;

        /// <summary>
        /// Gets a string representation of the discharge summary.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the discharge summary.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(When.ToString());

            if (PrimaryProvider != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    PrimaryProvider.ToString());
            }

            if (PrincipalDiagnosis != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    PrincipalDiagnosis.Text);
            }

            if (Text != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Text);
            }

            return result.ToString();
        }
    }
}
