// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing that encapsulates a discharge summary.
    /// </summary>
    ///
    public class DischargeSummary : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DischargeSummary"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.DischargeSummaryUnexpectedNode);

            // <when>
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // <type>
            this.type =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "type");

            // <category>
            this.category =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "category");

            // <setting>
            this.setting =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "setting");

            // <specialty>
            this.specialty =
                XPathHelper.GetOptNavValue(itemNav, "specialty");

            // <text>
            this.text =
                XPathHelper.GetOptNavValue(itemNav, "text");

            // <primary-provider>
            this.primaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "primary-provider");

            // <primary-provider-endorsement>
            this.primaryProviderEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "primary-provider-endorsement");

            // <secondary-provider>
            this.secondaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "secondary-provider");

            // <secondary-provider-endorsement>
            this.secondaryProviderEndorsement =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "secondary-provider-endorsement");

            // <discharge-date-time>
            this.dischargeDateTime =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "discharge-date-time");

            // <admitting-diagnosis>
            this.admittingDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "admitting-diagnosis");

            // <principal-diagnosis>
            this.principalDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "principal-diagnosis");

            // <additional-diagnosis>
            this.additionalDiagnosis =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "additional-diagnosis");

            // <principal-procedure-physician>
            this.principalProcedurePhysician =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "principal-procedure-physician");

            // <principal-procedure>
            this.principalProcedure =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "principal-procedure");

            // <additional-procedure>
            this.additionalProcedure =
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
            Validator.ThrowSerializationIfNull(this.when, Resources.DischargeSummaryWhenNotSet);

            // <discharge-summary>
            writer.WriteStartElement("discharge-summary");

            // <when>
            this.when.WriteXml("when", writer);

            // <type>
            XmlWriterHelper.WriteOpt(
                writer,
                "type",
                this.Type);

            // <category>
            XmlWriterHelper.WriteOpt(
                writer,
                "category",
                this.Category);

            // <setting>
            XmlWriterHelper.WriteOpt(
                writer,
                "setting",
                this.Setting);

            // <specialty>
            XmlWriterHelper.WriteOptString(
                writer,
                "specialty",
                this.specialty);

            // <text>
            XmlWriterHelper.WriteOptString(
                writer,
                "text",
                this.text);

            // <primary-provider>
            XmlWriterHelper.WriteOpt(
                writer,
                "primary-provider",
                this.PrimaryProvider);

            // <primary-provider-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "primary-provider-endorsement",
                this.PrimaryProviderEndorsement);

            // <secondary-provider>
            XmlWriterHelper.WriteOpt(
                writer,
                "secondary-provider",
                this.SecondaryProvider);

            // <secondary-provider-endorsement>
            XmlWriterHelper.WriteOpt(
                writer,
                "secondary-provider-endorsement",
                this.SecondaryProviderEndorsement);

            // <discharge-date-time>
            XmlWriterHelper.WriteOpt(
                writer,
                "discharge-date-time",
                this.DischargeDateTime);

            // <admitting-diagnosis>
            XmlWriterHelper.WriteOpt(
                writer,
                "admitting-diagnosis",
                this.AdmittingDiagnosis);

            // <principal-diagnosis>
            XmlWriterHelper.WriteOpt(
                writer,
                "principal-diagnosis",
                this.PrincipalDiagnosis);

            // <additional-diagnosis>
            XmlWriterHelper.WriteOpt(
                writer,
                "additional-diagnosis",
                this.AdditionalDiagnosis);

            // <principal-procedure-physician>
            XmlWriterHelper.WriteOpt(
                writer,
                "principal-procedure-physician",
                this.principalProcedurePhysician);

            // <principal-procedure>
            XmlWriterHelper.WriteOpt(
                writer,
                "principal-procedure",
                this.PrincipalProcedure);

            // <additional-procedure>
            XmlWriterHelper.WriteOpt(
                writer,
                "additional-procedure",
                this.AdditionalProcedure);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

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
            get { return this.type; }
            set { this.type = value; }
        }

        private CodableValue type;

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
            get { return this.category; }
            set { this.category = value; }
        }

        private CodableValue category;

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
            get { return this.setting; }
            set { this.setting = value; }
        }

        private CodableValue setting;

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
            get { return this.specialty; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Specialty");
                this.specialty = value;
            }
        }

        private string specialty;

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
            get { return this.text; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Text");
                this.text = value;
            }
        }

        private string text;

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
            get { return this.primaryProvider; }
            set { this.primaryProvider = value; }
        }

        private PersonItem primaryProvider;

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
            get { return this.primaryProviderEndorsement; }
            set { this.primaryProviderEndorsement = value; }
        }

        private HealthServiceDateTime primaryProviderEndorsement;

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
            get { return this.secondaryProvider; }
            set { this.secondaryProvider = value; }
        }

        private PersonItem secondaryProvider;

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
            get { return this.secondaryProviderEndorsement; }
            set { this.secondaryProviderEndorsement = value; }
        }

        private HealthServiceDateTime secondaryProviderEndorsement;

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
            get { return this.dischargeDateTime; }
            set { this.dischargeDateTime = value; }
        }

        private ApproximateDateTime dischargeDateTime;

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
            get { return this.admittingDiagnosis; }
            set { this.admittingDiagnosis = value; }
        }

        private CodableValue admittingDiagnosis;

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
            get { return this.principalDiagnosis; }
            set { this.principalDiagnosis = value; }
        }

        private CodableValue principalDiagnosis;

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
            get { return this.additionalDiagnosis; }
            set { this.additionalDiagnosis = value; }
        }

        private CodableValue additionalDiagnosis;

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
            get { return this.principalProcedurePhysician; }
            set { this.principalProcedurePhysician = value; }
        }

        private PersonItem principalProcedurePhysician;

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
            get { return this.principalProcedure; }
            set { this.principalProcedure = value; }
        }

        private CodableValue principalProcedure;

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
            get { return this.additionalProcedure; }
            set { this.additionalProcedure = value; }
        }

        private CodableValue additionalProcedure;

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

            result.Append(this.When);

            if (this.PrimaryProvider != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.PrimaryProvider.ToString());
            }

            if (this.PrincipalDiagnosis != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.PrincipalDiagnosis.Text);
            }

            if (this.Text != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Text);
            }

            return result.ToString();
        }
    }
}
