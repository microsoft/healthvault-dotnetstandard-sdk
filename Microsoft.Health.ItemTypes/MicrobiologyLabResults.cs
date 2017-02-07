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
    /// Represents a health record item type that encapsulates microbiology laboratory results.
    /// </summary>
    /// 
    public class MicrobiologyLabResults : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MicrobiologyLabResults"/> class 
        /// with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public MicrobiologyLabResults()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MicrobiologyLabResults"/> class 
        /// with the specified date.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time for the microbiology laboratory results.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> is <b>null</b>.
        /// </exception>
        /// 
        public MicrobiologyLabResults(HealthServiceDateTime when)
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
            new Guid("B8FCB138-F8E6-436A-A15D-E3A2D6916094");

        /// <summary>
        /// Populates this microbiology laboratory results instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the microbiology laboratory results data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a laboratory results node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("microbiology-lab-results");

            Validator.ThrowInvalidIfNull(itemNav, "MicrobiologyLabResultsUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _labTests.Clear();
            XPathNodeIterator labTestIterator = itemNav.Select("lab-tests");
            foreach (XPathNavigator labTestNav in labTestIterator)
            {
                LabTestType labTest = new LabTestType();
                labTest.ParseXml(labTestNav);
                _labTests.Add(labTest);
            }

            _sensitivityAgent = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "sensitivity-agent");

            _sensitivityValue = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "sensitivity-value");

            _sensitivityInterpretation = 
                XPathHelper.GetOptNavValue(itemNav, "sensitivity-interpretation");

            _specimenType = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "specimen-type");

            _organismName = 
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "organism-name");

            _organismComment = 
                XPathHelper.GetOptNavValue(itemNav, "organism-comment");
        }

        /// <summary>
        /// Writes the microbiology laboratory results data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the microbiology laboratory results data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "LabResultsWhenNotSet");

            // <microbiology-lab-results>
            writer.WriteStartElement("microbiology-lab-results");

            // <when>
            _when.WriteXml("when", writer);

            for (int index = 0; index < _labTests.Count; ++index)
            {
                _labTests[index].WriteXml("lab-tests", writer);
            }

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "sensitivity-agent", 
                _sensitivityAgent);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "sensitivity-value", 
                _sensitivityValue);

            XmlWriterHelper.WriteOptString(
                writer, 
                "sensitivity-interpretation", 
                _sensitivityInterpretation);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "specimen-type",
                _specimenType);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "organism-name", 
                _organismName);

            XmlWriterHelper.WriteOptString(
                writer, 
                "organism-comment", 
                _organismComment);

            // </microbiology-lab-result>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the microbiology laboratory results occurred.
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
        /// Gets a collection of laboratory tests.
        /// </summary>
        /// 
        public Collection<LabTestType> LabTests
        {
            get { return _labTests; }
        }
        private Collection<LabTestType> _labTests = new Collection<LabTestType>();

        /// <summary>
        /// Gets or sets the sensitivity agent.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity agent
        /// is not available.
        /// </remarks>
        /// 
        public CodableValue SensitivityAgent
        {
            get { return _sensitivityAgent; }
            set { _sensitivityAgent = value; }
        }
        private CodableValue _sensitivityAgent;

        /// <summary>
        /// Gets or sets the sensitivity value.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity value
        /// is not available.
        /// </remarks>
        /// 
        public CodableValue SensitivityValue
        {
            get { return _sensitivityValue; }
            set { _sensitivityValue = value; }
        }
        private CodableValue _sensitivityValue;

        /// <summary>
        /// Gets or sets the sensitivity interpretation.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity 
        /// interpretation is not available.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string SensitivityInterpretation
        {
            get { return _sensitivityInterpretation; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "SensitivityInterpretation");
                _sensitivityInterpretation = value;
            }
        }
        private string _sensitivityInterpretation;

        /// <summary>
        /// Gets or sets the specimen type.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the specimen type
        /// is not available.
        /// </remarks>
        /// 
        public CodableValue SpecimenType
        {
            get { return _specimenType; }
            set { _specimenType = value; }
        }
        private CodableValue _specimenType;

        /// <summary>
        /// Gets or sets the organism name.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the organism name
        /// is not available.
        /// </remarks>
        /// 
        public CodableValue OrganismName
        {
            get { return _organismName; }
            set { _organismName = value; }
        }
        private CodableValue _organismName;

        /// <summary>
        /// Gets or sets the organism comment.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the organism comment
        /// is not available.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string OrganismComment
        {
            get { return _organismComment; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "OrganismComment");
                _organismComment = value;
            }
        }
        private string _organismComment;

        /// <summary>
        /// Gets a string representation of the microbiology lab results.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the microbiology lab results.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);
            
            result.Append(When.ToString());

            for (int index = 0; index < LabTests.Count; ++index)
            {
                if (!String.IsNullOrEmpty(LabTests[index].Name))
                {
                    result.AppendFormat(
                        ResourceRetriever.GetResourceString(
                            "ListFormat"),
                        LabTests[index].Name);
                }
            }
            return result.ToString();
        }
    }
}
