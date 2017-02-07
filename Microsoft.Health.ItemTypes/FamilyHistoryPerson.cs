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
    public class FamilyHistoryPerson : HealthRecordItem
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryPerson"/> 
        /// class with default values.
        /// </summary>
        /// 
        public FamilyHistoryPerson()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryPerson"/> 
        /// class with mandatory parameters.
        /// </summary>
        /// 
        /// <param name="name">The name of a relative. </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b> null </b>.
        /// </exception>
        /// 
        public FamilyHistoryPerson(PersonItem name) : base(TypeId)
        {
            RelativeName = name;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("cc23422c-4fba-4a23-b52a-c01d6cd53fdf");

        /// <summary>
        /// Populates this <see cref="FamilyHistoryPerson"/> instance from the 
        /// data in the XML. 
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the family history person data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a family history person node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("family-history-person");

            Validator.ThrowInvalidIfNull(itemNav, "FamilyHistoryPersonUnexpectedNode");

            // relative-name
            _relativeName = new PersonItem();
            _relativeName.ParseXml(itemNav.SelectSingleNode("relative-name"));

            // relationship
            _relationship = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav,"relationship");

            // date-of-birth
            _dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(itemNav, "date-of-birth");

            // date-of-death
            _dateOfDeath = 
                XPathHelper.GetOptNavValue<ApproximateDate>(itemNav,"date-of-death");
        }

        /// <summary>
        /// Writes the family history person data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the family history person data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="RelativeName"/> parameter is <b>null</b>.
        /// </exception> 
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_relativeName, "FamilyHistoryPersonRelativeNameNotSet");

            // <family-history-person>
            writer.WriteStartElement("family-history-person");

            // relative-name
            _relativeName.WriteXml("relative-name", writer);

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

            // </family-history-person>
            writer.WriteEndElement();
         
        }

        /// <summary>
        /// Gets or sets the name and other information of a relative.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        /// 
        public PersonItem RelativeName
        {
            get { return _relativeName; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "RelativeName", "FamilyHistoryPersonRelativeNameMandatory");
                _relativeName = value;
            }
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
            string result = _relativeName.ToString();

            if (_relationship != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryToStringFormat"),
                        _relativeName.ToString(),
                        _relationship.ToString());
            }
            return result;
        }
    }
}
