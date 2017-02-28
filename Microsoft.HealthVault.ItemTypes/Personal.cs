// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents health record item type that encapsulates identifying
    /// information about a person.
    /// </summary>
    ///
    public class Personal : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Personal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Personal()
            : base(TypeId)
        {
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
            new Guid("92ba621e-66b3-4a01-bd73-74844aed4f5b");

        /// <summary>
        /// Populates this <see cref="Personal"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the personal data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a personal node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("personal");

            Validator.ThrowInvalidIfNull(itemNav, "PersonalUnexpectedNode");

            this.name =
                XPathHelper.GetOptNavValue<Name>(itemNav, "name");

            this.birthDate =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "birthdate");

            this.bloodtype =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "blood-type");

            this.ethnicity =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "ethnicity");

            this.ssn =
                XPathHelper.GetOptNavValue(itemNav, "ssn");

            this.maritalStatus =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "marital-status");

            this.employmentStatus =
                XPathHelper.GetOptNavValue(itemNav, "employment-status");

            // <is-deceased>
            this.isDeceased =
                XPathHelper.GetOptNavValueAsBool(
                    itemNav,
                    "is-deceased");

            // <date-of-death>
            this.dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "date-of-death");

            // <religion>
            this.religion =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "religion");

            // <is-veteran>
            this.isVeteran =
                XPathHelper.GetOptNavValueAsBool(itemNav, "is-veteran");

            // <highest-education-level>
            this.highestEducationLevel =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "highest-education-level");

            // <is-disabled>
            this.isDisabled =
                XPathHelper.GetOptNavValueAsBool(itemNav, "is-disabled");

            // <organ-donor>
            this.organDonor =
                XPathHelper.GetOptNavValue(itemNav, "organ-donor");
        }

        /// <summary>
        /// Writes the personal data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the personal data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <personal>
            writer.WriteStartElement("personal");

            XmlWriterHelper.WriteOpt(
                writer,
                "name",
                this.name);

            XmlWriterHelper.WriteOpt(
                writer,
                "birthdate",
                this.birthDate);

            XmlWriterHelper.WriteOpt(
                writer,
                "blood-type",
                this.bloodtype);

            XmlWriterHelper.WriteOpt(
                writer,
                "ethnicity",
                this.ethnicity);

            XmlWriterHelper.WriteOptString(
                writer,
                "ssn",
                this.ssn);

            XmlWriterHelper.WriteOpt(
                writer,
                "marital-status",
                this.maritalStatus);

            XmlWriterHelper.WriteOptString(
                writer,
                "employment-status",
                this.employmentStatus);

            // <is-deceased>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-deceased",
                this.isDeceased);

            // <date-of-death>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-of-death",
                this.dateOfDeath);

            // <religion>
            XmlWriterHelper.WriteOpt(
                writer,
                "religion",
                this.religion);

            // <is-veteran>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-veteran",
                this.isVeteran);

            // <highest-education-level>
            XmlWriterHelper.WriteOpt(
                writer,
                "highest-education-level",
                this.highestEducationLevel);

            // <is-disabled>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-disabled",
                this.isDisabled);

            // <organ-donor>
            XmlWriterHelper.WriteOptString(
                writer,
                "organ-donor",
                this.organDonor);

            // </personal>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        ///
        /// <value>
        /// The name of the person.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the name should not be stored.
        /// </remarks>
        ///
        public Name Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private Name name;

        /// <summary>
        /// Gets or sets the birth date.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the birth date should not be stored.
        /// </remarks>
        ///
        public HealthServiceDateTime BirthDate
        {
            get { return this.birthDate; }
            set { this.birthDate = value; }
        }

        private HealthServiceDateTime birthDate;

        /// <summary>
        /// Gets or sets the ABO and Rhesus +/- blood type for the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the blood type.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the blood type should not be stored.
        /// </remarks>
        ///
        public CodableValue BloodType
        {
            get { return this.bloodtype; }
            set { this.bloodtype = value; }
        }

        private CodableValue bloodtype;

        /// <summary>
        /// Gets or sets the ethnicity of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the ethnicity.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the ethnicity should not be stored.
        /// </remarks>
        ///
        public CodableValue Ethnicity
        {
            get { return this.ethnicity; }
            set { this.ethnicity = value; }
        }

        private CodableValue ethnicity;

        /// <summary>
        /// Gets or sets the deployment-specific national identifier for
        /// the person.
        /// </summary>
        ///
        /// <value>
        /// An instance representing the
        /// deployment-specific national identifier.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the deployment-specific national
        /// identifier should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SocialSecurityNumber
        {
            get { return this.ssn; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SocialSecurityNumber");
                this.ssn = value;
            }
        }

        private string ssn;

        /// <summary>
        /// Gets or sets the marital status of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the marital status.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the marital status should not be stored.
        /// </remarks>
        ///
        public CodableValue MaritalStatus
        {
            get { return this.maritalStatus; }
            set { this.maritalStatus = value; }
        }

        private CodableValue maritalStatus;

        /// <summary>
        /// Gets or sets the employment status of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the employment
        /// status.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the employment status should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string EmploymentStatus
        {
            get { return this.employmentStatus; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "EmploymentStatus");
                this.employmentStatus = value;
            }
        }

        private string employmentStatus;

        /// <summary>
        /// Gets or sets a value indicating whether the person is deceased.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the person is deceased; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool? IsDeceased
        {
            get { return this.isDeceased; }
            set { this.isDeceased = value; }
        }

        private bool? isDeceased;

        /// <summary>
        /// Gets or sets the date of death for the person.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="ApproximateDateTime"/> instance representing the date.
        /// </value>
        ///
        public ApproximateDateTime DateOfDeath
        {
            get { return this.dateOfDeath; }
            set { this.dateOfDeath = value; }
        }

        private ApproximateDateTime dateOfDeath;

        /// <summary>
        /// Gets or sets the religion of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the religion.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the religion should not be stored.
        /// </remarks>
        ///
        public CodableValue Religion
        {
            get { return this.religion; }
            set { this.religion = value; }
        }

        private CodableValue religion;

        /// <summary>
        /// Gets or sets a value indicating whether the person is a veteran.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the person is a veteran; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool? IsVeteran
        {
            get { return this.isVeteran; }
            set { this.isVeteran = value; }
        }

        private bool? isVeteran;

        /// <summary>
        /// Gets or sets the highest education level of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the education level.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the level should not be stored.
        /// </remarks>
        ///
        public CodableValue HighestEducationLevel
        {
            get { return this.highestEducationLevel; }
            set { this.highestEducationLevel = value; }
        }

        private CodableValue highestEducationLevel;

        /// <summary>
        /// Gets or sets a value indicating whether the person has a disability.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the person has a disability; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool? IsDisabled
        {
            get { return this.isDisabled; }
            set { this.isDisabled = value; }
        }

        private bool? isDisabled;

        /// <summary>
        /// Gets or sets the organ donor status of the person.
        /// </summary>
        ///
        /// <value>
        /// A string representing the status.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the status should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string OrganDonor
        {
            get { return this.organDonor; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "OrganDonor");
                this.organDonor = value;
            }
        }

        private string organDonor;

        /// <summary>
        /// Gets a string representation of the personal item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the personal item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Name != null)
            {
                result = this.Name.ToString();
            }
            else if (this.BirthDate != null || this.Ethnicity != null)
            {
                if (this.BirthDate != null && this.Ethnicity != null)
                {
                    result =
                        string.Format(
                            ResourceRetriever.GetResourceString(
                                "PersonalToStringFormatBirthDateAndEthnicity"),
                            this.BirthDate.ToString(),
                            this.Ethnicity.Text);
                }
                else if (this.BirthDate != null)
                {
                    result = this.BirthDate.ToString();
                }
                else
                {
                    result = this.Ethnicity.Text;
                }
            }
            else if (this.CommonData.Note != null)
            {
                result = this.CommonData.Note;
            }
            else
            {
                result =
                    ResourceRetriever.GetResourceString(
                        "PersonalToStringFormatSeeDetails");
            }

            return result;
        }
    }
}
