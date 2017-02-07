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
    /// Represents a baby associated with a pregnancy or delivery.
    /// </summary>
    /// 
    public class Baby : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Baby"/> class with default values.
        /// </summary>
        /// 
        public Baby()
        {
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the baby information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _name = XPathHelper.GetOptNavValue<Name>(navigator, "name");
            _gender = XPathHelper.GetOptNavValue<CodableValue>(navigator, "gender");
            _weight = XPathHelper.GetOptNavValue<WeightValue>(navigator, "weight");
            _length = XPathHelper.GetOptNavValue<Length>(navigator, "length");
            _head = XPathHelper.GetOptNavValue<Length>(navigator, "head-circumference");
            _note = XPathHelper.GetOptNavValue(navigator, "note");
        }

        /// <summary>
        /// Writes the XML representation of the baby information into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the baby information.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the baby information should be 
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            
            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt<Name>(writer, "name", _name);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "gender", _gender);
            XmlWriterHelper.WriteOpt<WeightValue>(writer, "weight", _weight);
            XmlWriterHelper.WriteOpt<Length>(writer, "length", _length);
            XmlWriterHelper.WriteOpt<Length>(writer, "head-circumference", _head);
            XmlWriterHelper.WriteOptString(writer, "note", _note);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the baby.
        /// </summary>
        /// 
        /// <remarks>
        /// If the name is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public Name Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private Name _name;

        /// <summary>
        /// Gets or sets the gender of the baby.
        /// </summary>
        /// 
        /// <remarks>
        /// If the gender is not known the value should be set to <b>null</b>.
        /// The preferred vocabulary for this value is "gender-types". 
        /// </remarks>
        /// 
        public CodableValue Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        private CodableValue _gender;

        /// <summary>
        /// Gets or sets the birth weight of the baby.
        /// </summary>
        /// 
        /// <remarks>
        /// If the weight is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        private WeightValue _weight;

        /// <summary>
        /// Gets or sets the birth length of the baby.
        /// </summary>
        /// 
        /// <remarks>
        /// If the length is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public Length Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private Length _length;

        /// <summary>
        /// Gets or sets the circumference of the baby's head.
        /// </summary>
        /// 
        /// <remarks>
        /// If the head circumference is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public Length HeadCircumference
        {
            get { return _head; }
            set { _head = value; }
        }
        private Length _head;

        /// <summary>
        /// Gets or sets additional information about the baby.
        /// </summary>
        /// 
        /// <remarks>
        /// If there are no additional notes the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public String Note
        {
            get { return _note; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                _note = value;
            }
        }
        private String _note;


        /// <summary>
        /// Gets a string representation of the baby information.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the baby information.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (Name != null)
            {
                result.Append(Name.ToString());
            }

            if (Weight != null && Length != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatWeightAndLength"),
                    Weight.ToString(),
                    Length.ToString());
            }
            else if (Weight != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatWeight"),
                    Weight.ToString());
            }
            else if (Length != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "BabyToStringFormatLength"),
                    Length.ToString());
            }

            return result.ToString();
        }
    }

}
