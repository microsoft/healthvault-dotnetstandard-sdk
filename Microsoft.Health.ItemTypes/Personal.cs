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
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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

            _name =
                XPathHelper.GetOptNavValue<Name>(itemNav, "name");

            _birthDate =
                XPathHelper.GetOptNavValue<HealthServiceDateTime>(
                    itemNav,
                    "birthdate");

            _bloodtype =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "blood-type");

            _ethnicity =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "ethnicity");

            _ssn =
                XPathHelper.GetOptNavValue(itemNav, "ssn");

            _maritalStatus =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "marital-status");

            _employmentStatus =
                XPathHelper.GetOptNavValue(itemNav, "employment-status");

            // <is-deceased>
            _isDeceased =
                XPathHelper.GetOptNavValueAsBool(
                    itemNav,
                    "is-deceased");
            
            // <date-of-death>
            _dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "date-of-death");

            // <religion>
            _religion =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "religion");

            // <is-veteran>
            _isVeteran =
                XPathHelper.GetOptNavValueAsBool(itemNav, "is-veteran");

            // <highest-education-level>
            _highestEducationLevel =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "highest-education-level");
            
            // <is-disabled>
            _isDisabled =
                XPathHelper.GetOptNavValueAsBool(itemNav, "is-disabled");

            // <organ-donor>
            _organDonor =
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

            XmlWriterHelper.WriteOpt<Name>(
                writer,
                "name",
                _name);

            XmlWriterHelper.WriteOpt<HealthServiceDateTime>(
                writer,
                "birthdate",
                _birthDate);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "blood-type",
                _bloodtype);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "ethnicity",
                _ethnicity);

            XmlWriterHelper.WriteOptString(
                writer,
                "ssn",
                _ssn);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "marital-status",
                _maritalStatus);

            XmlWriterHelper.WriteOptString(
                writer,
                "employment-status",
                _employmentStatus);

            // <is-deceased>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-deceased",
                _isDeceased);

            // <date-of-death>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "date-of-death",
                _dateOfDeath);

            // <religion>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "religion",
                _religion);
            
            // <is-veteran>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-veteran",
                _isVeteran);

            // <highest-education-level>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "highest-education-level",
                _highestEducationLevel);

            // <is-disabled>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-disabled",
                _isDisabled);

            // <organ-donor>
            XmlWriterHelper.WriteOptString(
                writer,
                "organ-donor",
                _organDonor);

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
            get { return _name; }
            set { _name = value; }
        }
        private Name _name;

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
            get { return _birthDate; }
            set { _birthDate = value; }
        }
        private HealthServiceDateTime _birthDate;

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
            get { return _bloodtype; }
            set { _bloodtype = value; }
        }
        private CodableValue _bloodtype;

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
            get { return _ethnicity; }
            set { _ethnicity = value; }
        }
        private CodableValue _ethnicity;

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
            get { return _ssn; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "SocialSecurityNumber");
                _ssn = value;
            }
        }
        private string _ssn;

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
            get { return _maritalStatus; }
            set { _maritalStatus = value; }
        }
        private CodableValue _maritalStatus;

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
            get { return _employmentStatus; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "EmploymentStatus");
                _employmentStatus = value;
            }
        }
        private string _employmentStatus;

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
            get { return _isDeceased; }
            set { _isDeceased = value; }
        }
        private bool? _isDeceased;

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
            get { return _dateOfDeath; }
            set { _dateOfDeath = value; }
        }
        private ApproximateDateTime _dateOfDeath;

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
            get { return _religion; }
            set { _religion = value; }
        }
        private CodableValue _religion;

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
            get { return _isVeteran; }
            set { _isVeteran = value; }
        }
        private bool? _isVeteran;
       
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
            get { return _highestEducationLevel; }
            set { _highestEducationLevel = value; }
        }
        private CodableValue _highestEducationLevel;

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
            get { return _isDisabled; }
            set { _isDisabled = value; }
        }
        private bool? _isDisabled;

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
            get { return _organDonor; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "OrganDonor");
                _organDonor = value;
            }
        }
        private string _organDonor;

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
            string result = String.Empty;

            if (Name != null)
            {
                result = Name.ToString();
            }
            else if (BirthDate != null || Ethnicity != null)
            {
                if (BirthDate != null && Ethnicity != null)
                {
                    result =
                        String.Format(
                            ResourceRetriever.GetResourceString(
                                "PersonalToStringFormatBirthDateAndEthnicity"),
                            BirthDate.ToString(),
                            Ethnicity.Text);
                }
                else if (BirthDate != null)
                {
                    result = BirthDate.ToString();
                }
                else
                {
                    result = Ethnicity.Text;
                }

            }
            else if (CommonData.Note != null)
            {
                result = CommonData.Note;
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
