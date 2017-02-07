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
    /// Defines a result from a health assessment containing the name and value of the assessed area.
    /// </summary>
    /// 
    /// <remarks>
    /// See <see cref="HealthAssessment"/> for more information.
    /// </remarks>
    /// 
    public class Assessment : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Assessment"/> class 
        /// with default values.
        /// </summary>
        /// 
        public Assessment()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Assessment"/> class 
        /// with name and value.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the assessed area. See <see cref="Name"/> for more information.
        /// </param>
        /// 
        /// <param name="value">
        /// The calculated value of the assessed area. See <see cref="Value"/> for more information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="value"/> is <b> null </b>.
        /// </exception>
        /// 
        public Assessment(CodableValue name, CodableValue value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Populates this <see cref="Assessment"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML to get the assessment data from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // <name>
            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));

            // <value>
            _value = new CodableValue();
            _value.ParseXml(navigator.SelectSingleNode("value"));

            // <group>
            _group = XPathHelper.GetOptNavValue<CodableValue>(navigator, "group");

       }

        /// <summary>
        /// Writes the assessment data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the assessment item.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the assessment data to.
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
        /// If <see cref="Name"/> or <see cref="Value"/> is <b>null</b>.
        /// </exception> 
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, "AssessmentNameNotSet");
            Validator.ThrowSerializationIfNull(_value, "AssessmentValueNotSet");

            writer.WriteStartElement(nodeName);

            // <name>
            _name.WriteXml("name", writer);

            // <value>
            _value.WriteXml("value", writer);

            // <group>
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "group", _group);
 
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the assessed area.  
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: Heart attack risk, high blood pressure.
        /// The preferred vocabulary for route is "health-assessment-name".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Name"/> is <b>null</b>. 
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "AssessmentNameMandatory");
                _name = value;
            }
        }
        private CodableValue _name;


        /// <summary>
        /// Gets or sets the calculated value of the assessed area.
        /// </summary>
        /// 
        /// <remarks>
        /// The value may be coded using a specific set of values. 
        /// Example: Low/Medium/High risk.
        /// A list of vocabularies may be found in the preferred vocabulary 
        /// "health-assessment-value-sets". 
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Value
        {
            get { return _value; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "AssessmentValueMandatory");
                _value = value;
            }
        }
        private CodableValue _value;

        /// <summary>
        /// Gets or sets the additional information that can be used to help organize the
        /// results.
        /// </summary>
        /// 
        /// <remarks>
        /// The group is used to specify which group a specific result is in. 
        /// For example, the supporting assessments that follow a main assessment are coded
        /// to indicate that they are supporting by specifying the "supporting" code.
        /// The preferred vocabulary for route is "health-assessment-groups".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        /// 
        public CodableValue Group
        {
            get { return _group; }
            set { _group = value; }
        }
        private CodableValue _group;

        /// <summary>
        /// Gets a string of the name or description of the assessment.
        /// </summary> 
        /// 
        public override string ToString()
        {
            string result = String.Empty;

            if (_name != null && _value != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "AssessmentToStringFormat"),
                        _name.ToString(),
                        _value.ToString());
            }
            else if (_name != null)
            {
                result = _name.ToString();
            }
            else if (_value != null)
            {
                result = _value.ToString();
            }
            return result;
        }
    }
}
