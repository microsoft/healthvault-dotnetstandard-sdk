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
    /// Information about a relative of the record owner.
    /// </summary>
    ///
    /// <remarks>
    /// A family history person item stores the information about a relative
    /// of the record owner, for example, mother, father or aunt. Relating
    /// this item to family history condition items will provide a comprehensive
    /// family medical history.
    /// </remarks>
    ///
    public class FamilyHistoryRelative : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="FamilyHistoryRelative"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the relative's data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _relativeName =
                XPathHelper.GetOptNavValue<PersonItem>(navigator, "relative-name");

            _relationship =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "relationship");

            _dateOfBirth =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-birth");

            _dateOfDeath =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "date-of-death");
        }

        /// <summary>
        /// Writes the family history relative data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the family history relative item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the family history relative data to.
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
        /// If <see cref="Name"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <family-history-relative>
            writer.WriteStartElement(nodeName);

            // relative-name
            XmlWriterHelper.WriteOpt(
                writer,
                "relative-name",
                _relativeName);

            // relationship
            XmlWriterHelper.WriteOpt(
                writer,
                "relationship",
                _relationship);

            // date-of-birth
            XmlWriterHelper.WriteOpt(
                writer,
                "date-of-birth",
                _dateOfBirth);

            // date-of-death
            XmlWriterHelper.WriteOpt(
                writer,
                "date-of-death",
                _dateOfDeath);

            // </family-history-relative>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name and other information of a relative.
        /// </summary>
        ///
        public PersonItem RelativeName
        {
            get { return _relativeName; }
            set { _relativeName = value; }
        }

        private PersonItem _relativeName;

        /// <summary>
        /// Gets or sets the relationship between the relative and the record owner.
        /// </summary>
        ///
        /// <remarks>
        /// The relationship should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Relationship
        {
            get { return _relationship; }
            set { _relationship = value; }
        }

        private CodableValue _relationship;

        /// <summary>
        /// Gets or sets the date of birth of the relative.
        /// </summary>
        ///
        /// <remarks>
        /// The date of death should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }

        private ApproximateDate _dateOfBirth;

        /// <summary>
        /// Gets or sets the date of death of the relative.
        /// </summary>
        ///
        /// <remarks>
        /// The date of death should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate DateOfDeath
        {
            get { return _dateOfDeath; }
            set { _dateOfDeath = value; }
        }

        private ApproximateDate _dateOfDeath;

        /// <summary>
        /// Gets a string representation of the family history person item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the family history person item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;
            if (_relativeName != null && _relationship != null)
            {
                result =
                    string.Format(
                        Resources.FamilyHistoryRelativeToStringFormatNameAndRelationship,
                        _relativeName.ToString(),
                        _relationship.ToString());
            }
            else if (_relationship != null)
            {
                result =
                    string.Format(
                        Resources.FamilyHistoryRelativeToStringFormatNameAndRelationship,
                        string.Empty,
                        _relationship.ToString());
            }
            else if (_relativeName != null)
            {
                result = _relativeName.ToString();
            }

            return result;
        }
    }
}
