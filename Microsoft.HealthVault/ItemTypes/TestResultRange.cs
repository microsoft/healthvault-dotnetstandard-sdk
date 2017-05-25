// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A range related to a specific test result.
    /// </summary>
    ///
    public class TestResultRange : ItemBase
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Type"/> or <see cref="Text"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_rangeType, Resources.TestResultRangeRangeTypeNotSet);
            Validator.ThrowSerializationIfNull(_text, Resources.TestResultRangeTextNotSet);

            // <test-result-range>
            writer.WriteStartElement(nodeName);

            // type
            _rangeType.WriteXml("type", writer);

            // text
            _text.WriteXml("text", writer);

            // value
            XmlWriterHelper.WriteOpt(writer, "value", _value);

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
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue RangeType
        {
            get { return _rangeType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(RangeType), Resources.TestResultRangeRangeTypeNotSet);
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
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Text
        {
            get { return _text; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Text), Resources.TestResultRangeTextNotSet);
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
                string.Format(
                    Resources.TestResultRangeToStringFormat,
                    _rangeType.ToString(),
                    _text.ToString());
        }
    }
}
