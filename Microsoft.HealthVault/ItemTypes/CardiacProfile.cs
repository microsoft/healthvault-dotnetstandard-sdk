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
    /// Represents a thing type that encapsulates a person's
    /// cardiac profile at a single point in time.
    /// </summary>
    ///
    public class CardiacProfile : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CardiacProfile"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.CardiacProfileUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.onHypertensionDiet =
                XPathHelper.GetOptNavValueAsBool(itemNav, "on-hypertension-diet");

            this.onHypertensionMedication =
                XPathHelper.GetOptNavValueAsBool(itemNav, "on-hypertension-medication");

            this.renalFailureDiagnosed =
                XPathHelper.GetOptNavValueAsBool(itemNav, "renal-failure-diagnosed");

            this.diabetesDiagnosed =
                XPathHelper.GetOptNavValueAsBool(itemNav, "diabetes-diagnosed");

            this.hasFamilyHeartDiseaseHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-family-heart-disease-history");

            this.hasFamilyStrokeHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-family-stroke-history");

            this.hasPersonalHeartDiseaseHistory =
                XPathHelper.GetOptNavValueAsBool(itemNav, "has-personal-heart-disease-history");

            this.hasPersonalStrokeHistory =
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
            this.when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptBool(
                writer,
                "on-hypertension-diet",
                this.onHypertensionDiet);

            XmlWriterHelper.WriteOptBool(
                writer,
                "on-hypertension-medication",
                this.onHypertensionMedication);

            XmlWriterHelper.WriteOptBool(
                writer,
                "renal-failure-diagnosed",
                this.renalFailureDiagnosed);

            XmlWriterHelper.WriteOptBool(
                writer,
                "diabetes-diagnosed",
                this.diabetesDiagnosed);

            XmlWriterHelper.WriteOptBool(
                writer,
                "has-family-heart-disease-history",
                this.hasFamilyHeartDiseaseHistory);

            XmlWriterHelper.WriteOptBool(
                writer,
                "has-family-stroke-history",
                this.hasFamilyStrokeHistory);

            XmlWriterHelper.WriteOptBool(
                writer,
                "has-personal-heart-disease-history",
                this.hasPersonalHeartDiseaseHistory);

            XmlWriterHelper.WriteOptBool(
                writer,
                "has-person-stroke-history",
                this.hasPersonalStrokeHistory);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

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
            get { return this.onHypertensionDiet; }
            set { this.onHypertensionDiet = value; }
        }

        private bool? onHypertensionDiet;

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
            get { return this.onHypertensionMedication; }
            set { this.onHypertensionMedication = value; }
        }

        private bool? onHypertensionMedication;

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
            get { return this.renalFailureDiagnosed; }
            set { this.renalFailureDiagnosed = value; }
        }

        private bool? renalFailureDiagnosed;

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
            get { return this.diabetesDiagnosed; }
            set { this.diabetesDiagnosed = value; }
        }

        private bool? diabetesDiagnosed;

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
            get { return this.hasFamilyHeartDiseaseHistory; }
            set { this.hasFamilyHeartDiseaseHistory = value; }
        }

        private bool? hasFamilyHeartDiseaseHistory;

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
            get { return this.hasFamilyStrokeHistory; }
            set { this.hasFamilyStrokeHistory = value; }
        }

        private bool? hasFamilyStrokeHistory;

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
            get { return this.hasPersonalHeartDiseaseHistory; }
            set { this.hasPersonalHeartDiseaseHistory = value; }
        }

        private bool? hasPersonalHeartDiseaseHistory;

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
            get { return this.hasPersonalStrokeHistory; }
            set { this.hasPersonalStrokeHistory = value; }
        }

        private bool? hasPersonalStrokeHistory;

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

            string trueString = Resources.True;
            string falseString = Resources.False;

            if (this.IsOnHypertensionDiet != null)
            {
                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatOnHypertensionDiet,
                        this.IsOnHypertensionDiet.Value ? trueString : falseString);
            }

            if (this.IsOnHypertensionMedication != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatOnHypertensionMeds,
                        this.IsOnHypertensionMedication.Value ? trueString : falseString);
            }

            if (this.HasRenalFailureBeenDiagnosed != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatRenalFailureDiagnosed,
                        this.HasRenalFailureBeenDiagnosed.Value ? trueString : falseString);
            }

            if (this.HasDiabetesBeenDiagnosed != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatDiabetesDiagnosed,
                        this.HasDiabetesBeenDiagnosed.Value ? trueString : falseString);
            }

            if (this.HasFamilyHeartDiseaseHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatHasFamlyHeartDiseaseHistory,
                        this.HasFamilyHeartDiseaseHistory.Value ? trueString : falseString);
            }

            if (this.HasFamilyStrokeHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatHasFamlyStrokeHistory,
                        this.HasFamilyStrokeHistory.Value ? trueString : falseString);
            }

            if (this.HasPersonalHeartDiseaseHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatHasPersonalHeartDiseaseHistory,
                        this.HasPersonalHeartDiseaseHistory.Value ? trueString : falseString);
            }

            if (this.HasPersonalStrokeHistory != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.AppendFormat(
                    Resources.CardiacProfileToStringFormatHasPersonalStrokeHistory,
                        this.HasPersonalStrokeHistory.Value ? trueString : falseString);
            }

            return result.ToString();
        }
    }
}
