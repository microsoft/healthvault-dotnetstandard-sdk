// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.ItemTypes
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
        /// Initialize a new instance of the <see cref="FamilyHistoryRelative"/> 
        /// class with default values.
        /// </summary>
        /// 
        public FamilyHistoryRelative()
        {
        }

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

            _relativeName =
                XPathHelper.GetOptNavValue<PersonItem>(navigator, "relative-name");

            _relationship =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "relationship");

            _dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-birth");

            _dateOfDeath =
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
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "relative-name",
                _relativeName);

            // relationship
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "relationship",
                _relationship);

            // date-of-birth
            XmlWriterHelper.WriteOpt<ApproximateDate>(
                writer,
                "date-of-birth",
                _dateOfBirth);

           // date-of-death
            XmlWriterHelper.WriteOpt<ApproximateDate>( 
                writer,
                "date-of-death",
                _dateOfDeath);

            // </family-history-relative>
            writer.WriteEndElement();
         
        }

        /// <summary>
        /// Gets or sets the name and other information of a relative.
        /// </summary>
        ///
        public PersonItem RelativeName
        {
            get { return _relativeName; }
            set { _relativeName = value; }
        }
        private PersonItem _relativeName;

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
            get { return _relationship;}
            set { _relationship = value;}
        }
        private CodableValue _relationship;

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
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }
        private ApproximateDate _dateOfBirth;

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
            get { return _dateOfDeath;}
            set { _dateOfDeath = value; }
        }
        private ApproximateDate _dateOfDeath;

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
            string result = String.Empty;
            if (_relativeName != null && _relationship != null) 
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        _relativeName.ToString(),
                        _relationship.ToString());
            }
            else if (_relationship != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        String.Empty,
                        _relationship.ToString()); 
            }
            else if (_relativeName != null)
            {
                result = _relativeName.ToString();
            }
            return result;
        }
    }
}
