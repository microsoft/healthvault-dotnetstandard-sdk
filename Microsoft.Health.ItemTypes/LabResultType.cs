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
    /// Represents a clinical value within lab result.
    /// </summary>
    /// 
    public class LabResultType : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="LabResultType"/> class with 
        /// default values.
        /// </summary>
        /// 
        public LabResultType()
            : base()
        {
        }

        /// <summary> 
        /// Populates the data for the lab result type from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the lab result type.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _value = XPathHelper.GetOptNavValueAsDouble(navigator, "value");
            _unit = XPathHelper.GetOptNavValue<CodableValue>(navigator, "unit");
            _referenceRange = XPathHelper.GetOptNavValue<DoubleRange>(navigator, "reference-range");
            _toxicRange = XPathHelper.GetOptNavValue<DoubleRange>(navigator, "toxic-range");
            _textValue = XPathHelper.GetOptNavValue(navigator, "text-value");

            _flag.Clear();
            XPathNodeIterator flagIterator = navigator.Select("flag");
            foreach (XPathNavigator flagNav in flagIterator)
            {
                CodableValue flagValue = new CodableValue();
                flagValue.ParseXml(flagNav);
                _flag.Add(flagValue);
            }
        }

        /// <summary> 
        /// Writes the lab result type data to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the lab result type.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the lab result type to.
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

            XmlWriterHelper.WriteOptDouble(writer, "value", _value);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "unit", _unit);
            XmlWriterHelper.WriteOpt<DoubleRange>(writer, "reference-range", _referenceRange);
            XmlWriterHelper.WriteOpt<DoubleRange>(writer, "toxic-range", _toxicRange);
            XmlWriterHelper.WriteOptString(writer, "text-value", _textValue);

            foreach (CodableValue flagValue in _flag)
            {
                flagValue.WriteXml("flag", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the value for the lab result.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="Double"/> representing the value of the lab result, or <b>null</b> if
        /// no lab result is available.
        /// </value>
        /// 
        public double? Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private double? _value;

        /// <summary>
        /// Gets or sets the unit of measure for the <see cref="Value"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// The unit of measure may defined by the HealthVault dictionary or the 
        /// <see cref="CodableValue.Text"/> property may be set to any unit of measure.
        /// </remarks>
        /// 
        public CodableValue Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        private CodableValue _unit;

        /// <summary>
        /// Gets or sets the reference range for the lab result type.
        /// </summary>
        /// 
        /// <value>
        /// The "normal" range of values for this lab result type or <b>null</b> if the reference
        /// range is not available.
        /// </value>
        /// 
        public DoubleRange ReferenceRange
        {
            get { return _referenceRange; }
            set { _referenceRange = value; }
        }
        private DoubleRange _referenceRange;

        /// <summary>
        /// Gets or sets the toxic range for the lab result type.
        /// </summary>
        /// 
        /// <value>
        /// The toxic range of values for this lab result type or <b>null</b> if the toxic
        /// range is not available.
        /// </value>
        /// 
        public DoubleRange ToxicRange
        {
            get { return _toxicRange; }
            set { _toxicRange = value; }
        }
        private DoubleRange _toxicRange;

        /// <summary>
        /// Gets or set the text representation of the value.
        /// </summary>
        /// 
        /// <value>
        /// A string representation of the <see cref="Value"/> or <b>null</b> if the text value is
        /// not available.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string TextValue
        {
            get { return _textValue; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "TextValue");
                _textValue = value;
            }
        }
        private string _textValue;

        /// <summary>
        /// Gets a collection of the flags associated with the lab result type.
        /// </summary>
        /// 
        /// <value>
        /// The flags for the lab result type are generally values like "normal", "critical", "high",
        /// "low", etc. which are defined by the "lab-result-flag" HealthVault vocabulary.
        /// </value>
        /// 
        public Collection<CodableValue> Flags
        {
            get { return _flag; }
        }
        private Collection<CodableValue> _flag = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the lab result type.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the lab result type.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (Value != null && Unit != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "LabResultTypeToStringFormatValueAndUnit"),
                    Value,
                    Unit.ToString());
            }
            else if (Value != null)
            {
                result.Append(Value);
            }
            
            if (!String.IsNullOrEmpty(TextValue))
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.Append(TextValue);
            }

            foreach (CodableValue flag in _flag)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.Append(flag.ToString());
            }
            return result.ToString();
        }
    }
}
