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
    /// Defines a result from a health assessment containing the name and value of the assessed area.
    /// </summary>
    ///
    /// <remarks>
    /// See <see cref="HealthAssessment"/> for more information.
    /// </remarks>
    ///
    public class Assessment : ItemBase
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> or <see cref="Value"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.AssessmentNameNotSet);
            Validator.ThrowSerializationIfNull(_value, Resources.AssessmentValueNotSet);

            writer.WriteStartElement(nodeName);

            // <name>
            _name.WriteXml("name", writer);

            // <value>
            _value.WriteXml("value", writer);

            // <group>
            XmlWriterHelper.WriteOpt(writer, "group", _group);

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
        /// If setter value is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.AssessmentNameMandatory);
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
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.AssessmentValueMandatory);
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
            string result = string.Empty;

            if (_name != null && _value != null)
            {
                result =
                    string.Format(
                        Resources.AssessmentToStringFormat,
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
