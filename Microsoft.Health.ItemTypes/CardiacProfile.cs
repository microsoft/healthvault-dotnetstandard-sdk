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
    /// Represents a health record item type that encapsulates a person's 
    /// cardiac profile at a single point in time.
    /// </summary>
    /// 
    public class CardiacProfile : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CardiacProfile"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> 
        /// method is called.
        /// </remarks>
        /// 
        public CardiacProfile()
            : base(TypeId)
        {
        }
 
        /// <summary>
        /// Creates a new instance of the <see cref="CardiacProfile"/> class 
        /// with the specified date and time.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the cardiac profile was take.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CardiacProfile(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
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
            new Guid("adaf49ad-8e10-49f8-9783-174819e97051");

        /// <summary>
        /// Populates this <see cref="CardiacProfile"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the cardiac profile data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a cardiac-profile node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "cardiac-profile");

            Validator.ThrowInvalidIfNull(itemNav, "CardiacProfileUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _onHypertensionDiet =
                XPathHelper.GetOptNavValueAsBool(itemNav, "on-hypertension-diet");

            _onHypertensionMedication =
                XPathHelper.GetOptNavValueAsBool(itemNav, "on-hypertension-medication");

            _renalFailureDiagnosed =
                XPathHelper.GetOptNavValueAsBool(itemNav, "renal-failure-diagnosed");

            _diabetesDiagnosed =
                XPathHelper.GetOptNavValueAsBool(itemNav, "diabetes-diagnosed");

            _hasFamilyHeartDiseaseHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-family-heart-disease-history");

            _hasFamilyStrokeHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-family-stroke-history");

            _hasPersonalHeartDiseaseHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-personal-heart-disease-history");

            _hasPersonalStrokeHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-person-stroke-history");

        }

        /// <summary>
        /// Writes the cardiac profile data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the cardiac profile data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <cardiac-profile>
            writer.WriteStartElement("cardiac-profile");

            // <when>
            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "on-hypertension-diet", 
                _onHypertensionDiet);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "on-hypertension-medication", 
                _onHypertensionMedication);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "renal-failure-diagnosed", 
                _renalFailureDiagnosed);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "diabetes-diagnosed", 
                _diabetesDiagnosed);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "has-family-heart-disease-history", 
                _hasFamilyHeartDiseaseHistory);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "has-family-stroke-history", 
                _hasFamilyStrokeHistory);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "has-personal-heart-disease-history", 
                _hasPersonalHeartDiseaseHistory);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "has-person-stroke-history", 
                _hasPersonalStrokeHistory);


            // </cardiac-profile>
            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the date/time when the cardiac profile was taken.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date. 
        /// The default value is the current year, month, and day.
        /// </value>
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
        /// Gets or sets whether the person is on a hypertension specific
        /// diet.
        /// </summary>
        /// 
        /// <value>
        /// True if the person is on a hypertension diet, false if not, or null
        /// if unknown.
        /// </value>
        /// 
        public bool? IsOnHypertensionDiet
        {
            get { return _onHypertensionDiet; }
            set { _onHypertensionDiet = value; }
        }
        private bool? _onHypertensionDiet;

        /// <summary>
        /// Gets or sets whether the person is on a hypertension specific
        /// medication.
        /// </summary>
        /// 
        /// <value>
        /// True if the person is on hypertension medication, false if not, or null
        /// if unknown.
        /// </value>
        /// 
        public bool? IsOnHypertensionMedication
        {
            get { return _onHypertensionMedication; }
            set { _onHypertensionMedication = value; }
        }
        private bool? _onHypertensionMedication;

        /// <summary>
        /// Gets or sets whether renal failure has been diagnosed for the person.
        /// </summary>
        /// 
        /// <value>
        /// True if renal failure has been diagnosed for the person, false if not, 
        /// or null if unknown.
        /// </value>
        /// 
        public bool? HasRenalFailureBeenDiagnosed
        {
            get { return _renalFailureDiagnosed; }
            set { _renalFailureDiagnosed = value; }
        }
        private bool? _renalFailureDiagnosed;

        /// <summary>
        /// Gets or sets whether diabetes has been diagnosed for the person.
        /// </summary>
        /// 
        /// <value>
        /// True if diabetes has been diagnosed for the person, false if not, 
        /// or null if unknown.
        /// </value>
        /// 
        public bool? HasDiabetesBeenDiagnosed
        {
            get { return _diabetesDiagnosed; }
            set { _diabetesDiagnosed = value; }
        }
        private bool? _diabetesDiagnosed;

        /// <summary>
        /// Gets or sets whether heart disease has been diagnosed for anyone
        /// in the person's family.
        /// </summary>
        /// 
        /// <value>
        /// True if heart disease has been diagnosed for anyone in the person's
        /// family, false if not, or null if unknown.
        /// </value>
        /// 
        public bool? HasFamilyHeartDiseaseHistory
        {
            get { return _hasFamilyHeartDiseaseHistory; }
            set { _hasFamilyHeartDiseaseHistory = value; }
        }
        private bool? _hasFamilyHeartDiseaseHistory;

        /// <summary>
        /// Gets or sets whether stroke has been diagnosed for anyone
        /// in the person's family.
        /// </summary>
        /// 
        /// <value>
        /// True if stroke has been diagnosed for anyone in the person's
        /// family, false if not, or null if unknown.
        /// </value>
        /// 
        public bool? HasFamilyStrokeHistory
        {
            get { return _hasFamilyStrokeHistory; }
            set { _hasFamilyStrokeHistory = value; }
        }
        private bool? _hasFamilyStrokeHistory;

        /// <summary>
        /// Gets or sets whether the person has been diagnosed with heart disease.
        /// </summary>
        /// 
        /// <value>
        /// True if the person has been diagnosed with heart disease, 
        /// false if not, or null if unknown.
        /// </value>
        /// 
        public bool? HasPersonalHeartDiseaseHistory
        {
            get { return _hasPersonalHeartDiseaseHistory; }
            set { _hasPersonalHeartDiseaseHistory = value; }
        }
        private bool? _hasPersonalHeartDiseaseHistory;

        /// <summary>
        /// Gets or sets whether the person has been diagnosed with a stroke.
        /// </summary>
        /// 
        /// <value>
        /// True if the person has been diagnosed with a stroke, false if not, 
        /// or null if unknown.
        /// </value>
        /// 
        public bool? HasPersonalStrokeHistory
        {
            get { return _hasPersonalStrokeHistory; }
            set { _hasPersonalStrokeHistory = value; }
        }
        private bool? _hasPersonalStrokeHistory;

        /// <summary>
        /// Gets a string representation of the cardiac profile item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the cardiac profile item.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(250);

            string trueString = ResourceRetriever.GetResourceString("True");
            string falseString = ResourceRetriever.GetResourceString("False");

            if (IsOnHypertensionDiet != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatOnHypertensionDiet"),
                        IsOnHypertensionDiet.Value ? trueString : falseString);
            }

            if (IsOnHypertensionMedication != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatOnHypertensionMeds"),
                        IsOnHypertensionMedication.Value ? trueString : falseString);
            }

            if (HasRenalFailureBeenDiagnosed != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatRenalFailureDiagnosed"),
                        HasRenalFailureBeenDiagnosed.Value ? trueString : falseString);
            }

            if (HasDiabetesBeenDiagnosed != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatDiabetesDiagnosed"),
                        HasDiabetesBeenDiagnosed.Value ? trueString : falseString);
            }

            if (HasFamilyHeartDiseaseHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatHasFamlyHeartDiseaseHistory"),
                        HasFamilyHeartDiseaseHistory.Value ? trueString : falseString);
            }

            if (HasFamilyStrokeHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatHasFamlyStrokeHistory"),
                        HasFamilyStrokeHistory.Value ? trueString : falseString);
            }

            if (HasPersonalHeartDiseaseHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatHasPersonalHeartDiseaseHistory"),
                        HasPersonalHeartDiseaseHistory.Value ? trueString : falseString);
            }

            if (HasPersonalStrokeHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "CardiacProfileToStringFormatHasPersonalStrokeHistory"), 
                        HasPersonalStrokeHistory.Value ? trueString : falseString);
            }

            return result.ToString();
        }
    }

}
