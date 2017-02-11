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
    /// Represents a vital sign result type.
    /// </summary>
    /// 
    public class VitalSignsResultType : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VitalSignsResultType"/> class with 
        /// default values.
        /// </summary>
        /// 
        public VitalSignsResultType()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VitalSignsResultType"/> class
        /// with the specified name.
        /// </summary>
        /// 
        /// <param name="title">
        /// The name of the vital sign result.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="title"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public VitalSignsResultType(CodableValue title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the vital sign result information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _title = new CodableValue();
            _title.ParseXml(navigator.SelectSingleNode("title"));

            _value =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "value");


            // <unit>
            _unit =
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator,
                    "unit");

            // <reference-minimum>
            _referenceMinimum =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "reference-minimum");

            // <reference-maximum>
            _referenceMaximum =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "reference-maximum");
            
            // <text-value>
            _textValue =
                XPathHelper.GetOptNavValue(
                    navigator,
                    "text-value");

            // <flag>
            _flag =
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator,
                    "flag");
        }

        /// <summary>
        /// Writes the XML representation of the vital sign result into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the vital sign result.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the vital sign result should be 
        /// written.
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Title"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_title, "VitalSignResultTitleNotSet");
            Validator.ThrowSerializationIfNull(_title.Text, "CodableValueNullText");

            writer.WriteStartElement(nodeName);

            // <title>
            _title.WriteXml("title", writer);

            // <value>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "value",
                _value);

            // <unit>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "unit",
                _unit);

            // <reference-minimum>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "reference-minimum",
                _referenceMinimum);

            // <reference-maximum>
            XmlWriterHelper.WriteOptDouble(
                writer,
                "reference-maximum",
                _referenceMaximum);
            
            // <text-value>
            XmlWriterHelper.WriteOptString(
                writer,
                "text-value",
                _textValue);

            // <flag>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "flag",
                _flag);


            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the title for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing 
        /// the title.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Title
        {
            get { return _title; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Title", "VitalSignResultTitleMandatory");
                _title = value;
            }
        }
        private CodableValue _title = new CodableValue();

        /// <summary>
        /// Gets or sets the vital sign value.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the vital sign value.
        /// </value> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the value should not
        /// be stored.
        /// </remarks>
        /// 
        public double? Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private double? _value;
        
        /// <summary>
        /// Gets or sets the unit for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing 
        /// the unit.
        /// </value> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the unit should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        private CodableValue _unit;

        /// <summary>
        /// Gets or sets the reference minimum for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the reference minimum.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the reference minimum should not be 
        /// stored.
        /// </remarks>
        /// 
        public double? ReferenceMinimum
        {
            get { return _referenceMinimum; }
            set { _referenceMinimum = value; }
        }
        private double? _referenceMinimum;

        /// <summary>
        /// Gets or sets the reference maximum for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the reference maximum.
        /// </value> 
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the reference maximum should not be 
        /// stored.
        /// </remarks>
        /// 
        public double? ReferenceMaximum
        {
            get { return _referenceMaximum; }
            set { _referenceMaximum = value; }
        }
        private double? _referenceMaximum;
                
        /// <summary>
        /// Gets or sets the text value for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the text value.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the text value should not be 
        /// stored.
        /// </remarks>
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
        /// Gets or sets the flag for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing 
        /// the flag.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the flag should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
        private CodableValue _flag;

        /// <summary>
        /// Gets a string representation of the vital signs result type.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the vital signs result type.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            if (_title != null)
            {
                result.Append(_title.ToString());
            }

            if (_value != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "VitalSignResultToStringFormatValue"),
                    _value.Value);
            }

            if (_unit != null)
            {
                result.Append(ResourceRetriever.GetSpace("sdkerrors"));

                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "VitalSignResultToStringFormatUnit"),
                    _unit.ToString());
            }
            return result.ToString();
        }
    }
}
