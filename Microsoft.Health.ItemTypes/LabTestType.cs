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
    /// Represents a lab test component, including the lab result value.
    /// </summary>
    /// 
    public class LabTestType : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="LabTestType"/> class with 
        /// default values.
        /// </summary>
        /// 
        public LabTestType()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="LabTestType"/> with the specified test
        /// date.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date and time when the laboratory test was performed.
        /// </param>
        /// 
        public LabTestType(HealthServiceDateTime when)
        {
            When = when;
        }

        /// <summary> 
        /// Populates the data for the lab test type from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the lab test type.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _when = new HealthServiceDateTime();
            _when.ParseXml(navigator.SelectSingleNode("when"));

            _name = XPathHelper.GetOptNavValue(navigator, "name");
            _substance = XPathHelper.GetOptNavValue<CodableValue>(navigator, "substance");

            _collectionMethod = 
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator, 
                    "collection-method");

            _abbreviation = XPathHelper.GetOptNavValue(navigator, "abbreviation");
            _description = XPathHelper.GetOptNavValue(navigator, "description");

            _code.Clear();
            XPathNodeIterator codeIterator = navigator.Select("code");
            foreach (XPathNavigator codeNav in codeIterator)
            {
                CodableValue codeValue = new CodableValue();
                codeValue.ParseXml(codeNav);
                _code.Add(codeValue);
            }

            _result = XPathHelper.GetOptNavValue<LabResultType>(navigator, "result");
            _status = XPathHelper.GetOptNavValue<CodableValue>(navigator, "status");
        }

        /// <summary> 
        /// Writes the lab test type data to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the lab test type.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the lab test type to.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            _when = new HealthServiceDateTime();
            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptString(writer, "name", _name);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "substance", _substance);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "collection-method", _collectionMethod);
            XmlWriterHelper.WriteOptString(writer, "abbreviation", _abbreviation);
            XmlWriterHelper.WriteOptString(writer, "description", _description);

            foreach (CodableValue codeValue in _code)
            {
                codeValue.WriteXml("code", writer);
            }

            XmlWriterHelper.WriteOpt<LabResultType>(writer, "result", _result);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "status", _status);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the lab test was conducted.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing 
        /// the date and time. The default value is the current year, month, and day.
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
        /// Gets or sets the name of the test.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Name
        {
            get { return _name; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the substance that was tested.
        /// </summary>
        /// 
        public CodableValue Substance
        {
            get { return _substance; }
            set { _substance = value; }
        }
        private CodableValue _substance;

        /// <summary>
        /// Gets or sets the method used to collect the substance.
        /// </summary>
        /// 
        public CodableValue CollectionMethod
        {
            get { return _collectionMethod; }
            set { _collectionMethod = value; }
        }
        private CodableValue _collectionMethod;

        /// <summary>
        /// Gets or sets the abbreviation for the test.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string TestAbbreviation
        {
            get { return _abbreviation; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "TestAbbreviation");
                _abbreviation = value;
            }
        }
        private string _abbreviation;

        /// <summary>
        /// Gets or sets the description for the test.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Description
        {
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }
        private string _description;


        /// <summary>
        /// Gets a collection of the clinical code(s) used for the test.
        /// </summary>
        /// 
        public Collection<CodableValue> Code
        {
            get { return _code; }
        }
        private Collection<CodableValue> _code = new Collection<CodableValue>();

        /// <summary>
        /// Gets or sets the result of the lab test.
        /// </summary>
        /// 
        public LabResultType Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private LabResultType _result;


        /// <summary>
        /// Gets or sets the status of the lab results.
        /// </summary>
        /// 
        /// <value>
        /// A CodableValue representing the status of the lab results. For example, "completed" or
        /// "pending". These values can be found in the "lab-results-status" HealthVault vocabulary.
        /// </value>
        /// 
        public CodableValue Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets a string representation of the lab test type.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the lab test type.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(When.ToString());

            if (!String.IsNullOrEmpty(this.Name))
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Name);
            }

            if (Substance != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Substance.ToString());
            }

            if (CollectionMethod != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    CollectionMethod.ToString());
            }

            if (TestAbbreviation != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    TestAbbreviation.ToString());
            }

            if (!String.IsNullOrEmpty(Description))
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Description);
            }

           if (Result != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Result.ToString());
            }

            if (Status != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Status.ToString());
            }
            return result.ToString();
        }
    }
}
