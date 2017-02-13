// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A health record item containing the results of a health assessment.
    /// </summary>
    /// 
    /// <remarks>
    /// Examples of health assessment include high blood pressure assessment and diabetes assessment.
    /// </remarks>
    /// 
    public class HealthAssessment : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthAssessment"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public HealthAssessment()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthAssessment"/> class with the specified 
        /// mandatory parameters.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date and time the assessment was completed.
        /// </param>
        /// 
        /// <param name="name">
        /// The application's name for the assessment. See <see cref="Name"/> for more information.
        /// </param>
        /// 
        /// <param name="category">
        /// The type of the assessment.
        /// </param>
        /// 
        /// <param name="result">
        /// The results of the assessment.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> or <paramref name="category"/>, or <paramref name="result"/> 
        /// is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty or <paramref name="result"/> is empty.
        /// </exception>
        /// 
        public HealthAssessment(
            HealthServiceDateTime when, 
            string name,
            CodableValue category,
            IList<Assessment> result)
            : base(TypeId)
        {
            this.When = when;
            this.Name = name;
            this.Category = category;

            Validator.ThrowIfArgumentNull(result, "result", "HealthAssessmentResultMandatory");

            if (result.Count == 0)
            {
                throw Validator.ArgumentException("result", "HealthAssessmentResultMandatory");
            }

            _result.Clear();

            foreach (Assessment assessment in result)
            {
                _result.Add(assessment);
            }
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("58fd8ac4-6c47-41a3-94b2-478401f0e26c");

        /// <summary>
        /// Populates this health assessment instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the health assessment data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a health-assessment node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("health-assessment");

            Validator.ThrowInvalidIfNull(itemNav, "HealthAssessmentUnexpectedNode");

            // <when>
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <name>
            _name = itemNav.SelectSingleNode("name").Value;

            // <category>
            _category = new CodableValue();
            _category.ParseXml(itemNav.SelectSingleNode("category"));

            // <result>
            _result.Clear();
            XPathNodeIterator resultIterator = itemNav.Select("result");
            foreach (XPathNavigator resultNav in resultIterator)
            {
                Assessment result = new Assessment();
                result.ParseXml(resultNav);
                _result.Add(result);
            }
        }

        /// <summary>
        /// Writes the health assessment data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the health assessment data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/>, <see cref="Name"/>, <see cref="Category"/>, or 
        /// <see cref="Result"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "WhenNullValue");
            Validator.ThrowSerializationIfNull(_name, "HealthAssessmentNameNotSet");
            Validator.ThrowSerializationIfNull(_category, "HealthAssessmentCategoryNotSet");
            Validator.ThrowSerializationIf(_result.Count == 0, "HealthAssessmentResultNotSet");

            // <health-assessment>
            writer.WriteStartElement("health-assessment");

            // <when>
            _when.WriteXml("when", writer);

            // <name>
            writer.WriteElementString("name", _name);

            // <category>
            _category.WriteXml("category", writer);

            // <result>
            foreach (Assessment result in _result)
            {
                result.WriteXml("result", writer);
            }

            // </health-assessment>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the health assessment was completed.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date 
        /// and time.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
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
        private HealthServiceDateTime _when;

        /// <summary>
        /// Gets or sets the application's name for the assessment.
        /// </summary>
        /// 
        /// <remarks>
        /// Example: Fabrikam's Heart Risk Assessment.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        /// 
        public string Name
        {
            get { return _name; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the type of assessment.
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: Heart assessment, diabetes assessment, colon cancer assessment.
        /// The preferred vocabulary for route is "health-assessment-category".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        /// 
        public CodableValue Category
        {
            get { return _category; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Category", "HealthAssessmentCategoryMandatory");
                _category = value;
            }
        }
        private CodableValue _category;


        /// <summary>
        /// Gets a collection of the results of the assessment.
        /// </summary>
        /// 
        /// <remarks>
        /// Example: High blood pressure, low risk.
        /// </remarks>
        /// 
        public Collection<Assessment> Result
        {
            get { return _result; }
        }
        private Collection<Assessment> _result = new Collection<Assessment>();


        /// <summary>
        /// Gets a string representation of the health assessment.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the health assessment.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            for (int index = 0; index < _result.Count; ++index)
            {
                if (index > 0)
                {
                    result.Append(ResourceRetriever.GetResourceString("GroupSeparator"));
                }
                result.Append(_result[index]);
            }

            return result.ToString();
        }
    }
}
