// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a baby associated with a pregnancy or delivery.
    /// </summary>
    ///
    public class Baby : ItemBase
    {
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

            XmlWriterHelper.WriteOpt(writer, "name", _name);
            XmlWriterHelper.WriteOpt(writer, "gender", _gender);
            XmlWriterHelper.WriteOpt(writer, "weight", _weight);
            XmlWriterHelper.WriteOpt(writer, "length", _length);
            XmlWriterHelper.WriteOpt(writer, "head-circumference", _head);
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
        public string Note
        {
            get { return _note; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                _note = value;
            }
        }

        private string _note;

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
                result.Append(Name);
            }

            if (Weight != null && Length != null)
            {
                result.AppendFormat(
                    Resources.BabyToStringFormatWeightAndLength,
                    Weight.ToString(),
                    Length.ToString());
            }
            else if (Weight != null)
            {
                result.AppendFormat(
                    Resources.BabyToStringFormatWeight,
                    Weight.ToString());
            }
            else if (Length != null)
            {
                result.AppendFormat(
                    Resources.BabyToStringFormatLength,
                    Length.ToString());
            }

            return result.ToString();
        }
    }
}
