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
    /// A single laboratory test.
    /// </summary>
    /// 
    public class LabTestResultDetails : HealthRecordItemData
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResultDetails"/> 
        /// class with default values.
        /// </summary>
        /// 
        public LabTestResultDetails()
        {
        }

        /// <summary>
        /// Populates this <see cref="LabTestResultDetails"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML to get the lab test result type data from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If the first node in <paramref name="navigator"/> is not
        /// a lab test result type node.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // when 
            _when = 
                XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator,"when");

            // name 
            _name =
                XPathHelper.GetOptNavValue(navigator, "name");

            // substance
            _substance = 
                XPathHelper.GetOptNavValue<CodableValue>(navigator,"substance");

            // collection-method
            _collectionMethod = 
                XPathHelper.GetOptNavValue<CodableValue>(navigator,"collection-method");

            // clinical-code
            _clinicalCode = 
                XPathHelper.GetOptNavValue<CodableValue>(navigator,"clinical-code");

            // value
            _value = 
                XPathHelper.GetOptNavValue<LabTestResultValue>(navigator,"value");

            // status
            _status = 
                XPathHelper.GetOptNavValue<CodableValue>(navigator,"status");
            
            // note
            _note =
                XPathHelper.GetOptNavValue(navigator, "note");
        }

        /// <summary>
        /// Writes the lab test result type data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="nodeName">
        /// The name of the node to write XML output.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the lab test result type data to.
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <lab-test-result-type>
            writer.WriteStartElement(nodeName);

            // when
            XmlWriterHelper.WriteOpt<ApproximateDateTime>( 
                writer,
                "when",
                _when);

            // name
            XmlWriterHelper.WriteOptString( 
                writer,
                "name",
                _name);

            // substance
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "substance",
                _substance);

            // collection-method
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "collection-method",
                _collectionMethod);

            // clinical-code
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "clinical-code",
                _clinicalCode);

            // value
            XmlWriterHelper.WriteOpt<LabTestResultValue>( 
                writer,
                "value",
                _value);

            // status
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "status",
                _status);

            // note 
            XmlWriterHelper.WriteOptString(
                writer,
                "note",
                _note);

            // </lab-test-result-type>
            writer.WriteEndElement();
        }

        
        /// <summary>
        /// Gets or sets date and the time of the laboratory test.  
        /// </summary>
        /// 
        /// <remarks>
        /// They should be set to <b>null</b> if they are unknown. 
        /// </remarks>
        /// 
        public ApproximateDateTime When
        {
            get { return _when;}
            set { _when = value; }
        }
        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets name of the laboratory test.  
        /// </summary>
        /// 
        /// <remarks>
        /// It should be set to <b>null</b> or empty if it is unknown.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Name
        {
            get { return _name;}
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets substance that is tested.  
        /// </summary>
        /// 
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Substance
        {
            get { return _substance;}
            set { _substance = value; }
        }
        private CodableValue _substance;

        /// <summary>
        /// Gets or sets the collection method for the laboratory test.  
        /// </summary>
        /// 
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        /// 
        public CodableValue CollectionMethod
        {
            get { return _collectionMethod;}
            set { _collectionMethod = value; }
        }
        private CodableValue _collectionMethod;

        /// <summary>
        /// Gets or sets the clinical code for the lab tests.  
        /// </summary>
        /// 
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        /// 
        public CodableValue ClinicalCode
        {
            get { return _clinicalCode;}
            set { _clinicalCode = value; }
        }
        private CodableValue _clinicalCode;

        /// <summary>
        /// Gets or sets the clinical value within a laboratory result. 
        /// </summary>
        /// 
        /// <remarks>
        /// The type of value is defined within a laboratory result, which includes 
        /// value, unit, reference and toxic range. It should be set to <b>null</b> 
        /// if it is unknown.
        /// </remarks>
        /// 
        public LabTestResultValue Value
        {
            get { return _value;}
            set { _value = value; }
        }
        private LabTestResultValue _value;

        /// <summary>
        /// Gets or sets the status of the laboratory results.  
        /// </summary>
        /// 
        /// <remarks> 
        /// Examples of status include complete and pending. It should be 
        /// set to <b>null</b> if it is unknown. 
        /// </remarks>
        /// 
        public CodableValue Status
        {
            get { return _status;}
            set { _status = value; }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets or sets a note that augments the laboratory result.  
        /// </summary>
        /// 
        /// <remarks> 
        /// There are two scenarios in which a note is typically added:
        /// A note may provide additional information about interpreting 
        /// the result beyond what is stored in the ranges associated with 
        /// the value. 
        /// Or, a note may be use to provide the textual result of a lab test
        /// that does not have a measured value.
        /// 
        /// Formatting:
        /// Notes may come from systems that require a line-based approach to textual display. To support
        /// this, applications should render the spaces and newlines in the note, and use a fixed-width font. 
        /// The form transform for the lab test results type shows one way to do this.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Note 
        {
            get { return _note;}
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                _note = value;
            }
        }
        private string _note;

        /// <summary>
        /// Gets a string representation of the lab test result type item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the lab test result type item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            string space = ResourceRetriever.GetSpace("errors");

            Boolean first = true;
            if (_when != null)
            {
                result.Append(_when.ToString());
                first = false;
            }
            if (!String.IsNullOrEmpty(_name))
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_name.ToString());
                first = false;
            }
            if (_substance != null)
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_substance.ToString());
                first = false;
            }
            if (_collectionMethod != null)
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_collectionMethod.ToString());
                first = false;
            }
            if (_clinicalCode != null)
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_clinicalCode.ToString());
                first = false;
            }
            if (_value != null)
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_value.ToString());
                first = false;
            }
            if (_status != null)
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_status.ToString());
                first = false;
            }
            if (!String.IsNullOrEmpty(_note))
            {
                if (!first)
                {
                    result.Append(space);
                }
                result.Append(_note);
                first = false;
            }
            return result.ToString();
        }
    }
}
