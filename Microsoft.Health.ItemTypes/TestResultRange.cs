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
    /// A range related to a specific test result.
    /// </summary>
    ///
    public class TestResultRange : HealthRecordItemData
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="TestResultRange"/> 
        /// class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// Each test result can contain multiple ranges that are useful 
        /// to interpret the result value. Examples include reference range 
        /// and therapeutic range.
        /// </remarks>
        /// 
        public TestResultRange()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="TestResultRange"/> 
        /// class with mandatory parameters.
        /// </summary>
        /// 
        /// <param name="type"> 
        /// Type is the type of a test result.
        /// </param>
        /// 
        /// <param name="text"> 
        /// Text is the range expressed as text. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="text"/> or <paramref name="type"/> is <b> null </b>.
        /// </exception>
        /// 
        public TestResultRange(CodableValue type, CodableValue text)
        {
            RangeType = type;
            Text = text;
        }

        /// <summary>
        /// Populates this <see cref="TestResultRange"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML to get the test result range data from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The first node in <paramref name="navigator"/> is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // type
            _rangeType = new CodableValue();
            _rangeType.ParseXml(navigator.SelectSingleNode("type"));

            // text
            _text = new CodableValue();
            _text.ParseXml(navigator.SelectSingleNode("text"));

            // value
            _value = XPathHelper.GetOptNavValue<TestResultRangeValue>(navigator, "value");
        }

        /// <summary>
        /// Writes the test result range data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="nodeName">
        /// The name of the node to write XML.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the test result range data to.
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
        /// If <see cref="Type"/> or <see cref="Text"/> is <b> null </b>.
        /// </exception> 
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_rangeType, "TestResultRangeRangeTypeNotSet");
            Validator.ThrowSerializationIfNull(_text, "TestResultRangeTextNotSet");

            // <test-result-range>
            writer.WriteStartElement(nodeName);

            // type
            _rangeType.WriteXml("type", writer);

            // text
            _text.WriteXml("text", writer);

            // value
            XmlWriterHelper.WriteOpt<TestResultRangeValue>(writer, "value", _value);

            // </test-result-range>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the minimum and maximum of the range.
        /// </summary>
        /// 
        /// <remarks>
        /// The minimum or maximum value may be omitted to specify open-ended ranges. 
        /// Example:
        /// The range "greater than 3.5" would be coded by setting the minimum to 3.5 and omitting the maximum.
        /// </remarks>
        /// 
        public TestResultRangeValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private TestResultRangeValue _value;

        /// <summary>
        /// Gets or sets the type of the range.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="RangeType"/> is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue RangeType
        {
            get { return _rangeType;}
            set
            {
                Validator.ThrowIfArgumentNull(value, "RangeType", "TestResultRangeRangeTypeNotSet");
                _rangeType = value;
            }
        }
        private CodableValue _rangeType;


        /// <summary>
        /// Gets or sets the range expressed as text.
        /// </summary>
        /// <remarks>
        /// The text element is used in two different ways:
        /// When a numeric range is used, the text element should contain a textual version of the
        /// numeric range. 
        /// When the range is non-numeric, the text element contains the range and the range value is omitted. The range may
        /// also be coded to a vocabulary.
        /// 
        /// Examples:
        /// A color range (such as clear to yellow) would be coded using by setting the text element to "clear to yellow",
        /// and by assigning a code from an appropriate vocabulary.
        /// 
        /// A numeric range (such as 0.5 - 1.6) would be stored in the minimum and maximum elements of the value, and \
        /// additionally would be coded by setting the text element to "0.5 - 1.6".
        /// 
        /// Contact the HealthVault team to help define this vocabulary.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Text"/> is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Text
        {
            get { return _text; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Text", "TestResultRangeTextNotSet");
                _text = value;
            }
        }
        private CodableValue _text;


        /// <summary>
        /// Gets a string representation of the test result range item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the test result range item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "TestResultRangeToStringFormat"),
                    _rangeType.ToString(),
                    _text.ToString());
        }
    }
}
