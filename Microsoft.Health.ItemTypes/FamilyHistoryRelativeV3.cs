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
    /// A family history relative item stores the information about a relative
    /// of the record owner, for example, mother, father or aunt. Relating 
    /// this item to family history condition items will provide a comprehensive 
    /// family medical history. 
    /// </remarks>
    /// 
    public class FamilyHistoryRelativeV3 : HealthRecordItemData
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
            Relationship = relationship;
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
            _relationship = new CodableValue();
            _relationship.ParseXml(navigator.SelectSingleNode("relationship"));

            // relative-name
            _relativeName =
                XPathHelper.GetOptNavValue<PersonItem>(navigator, "relative-name");

            // date-of-birth
            _dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-birth");

            // date-of-death
            _dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-death");

            //region-of-origin
            _regionOfOrigin =
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Relationship"/> is <b> null </b>.
        /// </exception> 
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_relationship, "RelationshipNullValue");

            // <family-history-relative>
            writer.WriteStartElement(nodeName);

            // relationship
            _relationship.WriteXml("relationship", writer);

            // relative-name
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "relative-name",
                _relativeName);

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

            // region-of-origin
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "region-of-origin",
                _regionOfOrigin);

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
            get { return _relationship; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Relationship", "RelationshipNullValue");
                _relationship = value;
            }
        }
        private CodableValue _relationship;

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
            get { return _dateOfDeath; }
            set { _dateOfDeath = value; }
        }
        private ApproximateDate _dateOfDeath;

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
            get { return _regionOfOrigin; }
            set { _regionOfOrigin = value; }
        }
        private CodableValue _regionOfOrigin;
        
        
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
            string result = String.Empty;
            if (_relativeName != null && _relationship != null)
            {
                result =
                    String.Format(
                        CultureInfo.InvariantCulture,
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryRelativeToStringFormatNameAndRelationship"),
                        _relativeName.ToString(),
                        _relationship.ToString());
            }
            else if (_relationship != null)
            {
                result =
                    String.Format(
                        CultureInfo.InvariantCulture,
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
