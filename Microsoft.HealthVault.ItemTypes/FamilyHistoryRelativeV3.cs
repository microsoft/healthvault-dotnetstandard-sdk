// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information about a relative of the record owner.
    /// </summary>
    ///
    /// <remarks>
    /// A family history relative item stores the information about a relative
    /// of the record owner, for example, mother, father or aunt. Relating
    /// this item to family history condition items will provide a comprehensive
    /// family medical history.
    /// </remarks>
    ///
    public class FamilyHistoryRelativeV3 : ItemBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryRelativeV3"/>
        /// class with default values.
        /// </summary>
        ///
        public FamilyHistoryRelativeV3()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryRelativeV3"/>
        /// class with the specified relatinship.
        /// </summary>
        ///
        /// <param name="relationship">
        /// The relationship of this person to the record owner.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="relationship"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public FamilyHistoryRelativeV3(CodableValue relationship)
        {
            this.Relationship = relationship;
        }

        /// <summary>
        /// Populates this <see cref="FamilyHistoryRelativeV3"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the relative's data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // relationship
            this.relationship = new CodableValue();
            this.relationship.ParseXml(navigator.SelectSingleNode("relationship"));

            // relative-name
            this.relativeName =
                XPathHelper.GetOptNavValue<PersonItem>(navigator, "relative-name");

            // date-of-birth
            this.dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-birth");

            // date-of-death
            this.dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-death");

            // region-of-origin
            this.regionOfOrigin =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "region-of-origin");
        }

        /// <summary>
        /// Writes the family history relative data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the family history relative item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the family history relative data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Relationship"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.relationship, "RelationshipNullValue");

            // <family-history-relative>
            writer.WriteStartElement(nodeName);

            // relationship
            this.relationship.WriteXml("relationship", writer);

            // relative-name
            XmlWriterHelper.WriteOpt(
                writer,
                "relative-name",
                this.relativeName);

            // date-of-birth
            XmlWriterHelper.WriteOpt(
                writer,
                "date-of-birth",
                this.dateOfBirth);

            // date-of-death
            XmlWriterHelper.WriteOpt(
                writer,
                "date-of-death",
                this.dateOfDeath);

            // region-of-origin
            XmlWriterHelper.WriteOpt(
                writer,
                "region-of-origin",
                this.regionOfOrigin);

            // </family-history-relative>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the relationship between the relative and the record owner.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Relationship
        {
            get { return this.relationship; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Relationship", "RelationshipNullValue");
                this.relationship = value;
            }
        }

        private CodableValue relationship;

        /// <summary>
        /// Gets or sets the name and other information of a relative.
        /// </summary>
        ///
        public PersonItem RelativeName
        {
            get { return this.relativeName; }
            set { this.relativeName = value; }
        }

        private PersonItem relativeName;

        /// <summary>
        /// Gets or sets the date of birth of the relative.
        /// </summary>
        ///
        /// <remarks>
        /// The date of death should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate DateOfBirth
        {
            get { return this.dateOfBirth; }
            set { this.dateOfBirth = value; }
        }

        private ApproximateDate dateOfBirth;

        /// <summary>
        /// Gets or sets the date of death of the relative.
        /// </summary>
        ///
        /// <remarks>
        /// The date of death should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate DateOfDeath
        {
            get { return this.dateOfDeath; }
            set { this.dateOfDeath = value; }
        }

        private ApproximateDate dateOfDeath;

        /// <summary>
        /// Gets or sets the region of origin the relative.
        /// </summary>
        ///
        /// <remarks>
        /// The region of origin should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue RegionOfOrigin
        {
            get { return this.regionOfOrigin; }
            set { this.regionOfOrigin = value; }
        }

        private CodableValue regionOfOrigin;

        /// <summary>
        /// Gets a string representation of the family history relative item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the family history relative item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;
            if (this.relativeName != null && this.relationship != null)
            {
                result =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        this.relativeName.ToString(),
                        this.relationship.ToString());
            }
            else if (this.relationship != null)
            {
                result =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        string.Empty,
                        this.relationship.ToString());
            }
            else if (this.relativeName != null)
            {
                result = this.relativeName.ToString();
            }

            return result;
        }
    }
}
