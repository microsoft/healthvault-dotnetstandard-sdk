// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information about a relative of the record owner.
    /// </summary>
    ///
    /// <remarks>
    /// A family history person item stores the information about a relative
    /// of the record owner, for example, mother, father or aunt. Relating
    /// this item to family history condition items will provide a comprehensive
    /// family medical history.
    /// </remarks>
    ///
    public class FamilyHistoryRelative : HealthRecordItemData
    {
        /// <summary>
        /// Populates this <see cref="FamilyHistoryRelative"/> instance from the data in the XML.
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

            this.relativeName =
                XPathHelper.GetOptNavValue<PersonItem>(navigator, "relative-name");

            this.relationship =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "relationship");

            this.dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-birth");

            this.dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-death");
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <family-history-relative>
            writer.WriteStartElement(nodeName);

            // relative-name
            XmlWriterHelper.WriteOpt(
                writer,
                "relative-name",
                this.relativeName);

            // relationship
            XmlWriterHelper.WriteOpt(
                writer,
                "relationship",
                this.relationship);

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

            // </family-history-relative>
            writer.WriteEndElement();
        }

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
        /// Gets or sets the relationship between the relative and the record owner.
        /// </summary>
        ///
        /// <remarks>
        /// The relationship should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Relationship
        {
            get { return this.relationship; }
            set { this.relationship = value; }
        }

        private CodableValue relationship;

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
        /// Gets a string representation of the family history person item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the family history person item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;
            if (this.relativeName != null && this.relationship != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        this.relativeName.ToString(),
                        this.relationship.ToString());
            }
            else if (this.relationship != null)
            {
                result =
                    string.Format(
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
