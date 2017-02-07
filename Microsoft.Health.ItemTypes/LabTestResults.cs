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
    /// A series of lab test results.
    /// </summary>
    /// 
    public class LabTestResults : HealthRecordItem
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResults"/> 
        /// class with default values.
        /// </summary>
        /// 
        public LabTestResults()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResults"/> 
        /// class with mandatory parameters.
        /// </summary>
        /// 
        /// <param name="labGroups">Lab groups is a set of lab results.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="labGroups"/> parameter is <b> null </b>.
        /// </exception>
        /// 
        public LabTestResults(IEnumerable<LabTestResultGroup> labGroups)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(labGroups, "labGroups", "LabTestResultsLabGroupMandatory");

            foreach (LabTestResultGroup labGroup in labGroups)
            {
                _labGroup.Add(labGroup);
            }
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("5800eab5-a8c2-482a-a4d6-f1db25ae08c3");

        /// <summary>
        /// Populates this <see cref="LabTestResults"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the lab test results data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a lab test results node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("lab-test-results");

            Validator.ThrowInvalidIfNull(itemNav, "LabTestResultsUnexpectedNode");

            // when 
            _when =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "when");

            // lab-group
            XPathNodeIterator labGroupIterator =
                itemNav.Select("lab-group");
            _labGroup = new Collection<LabTestResultGroup>();
            foreach (XPathNavigator labGroupNav in labGroupIterator)
            {
                LabTestResultGroup labTestResultGroup = new LabTestResultGroup();
                labTestResultGroup.ParseXml(labGroupNav);
                _labGroup.Add(labTestResultGroup);
            }

            // ordered-by
            _orderedBy =
                XPathHelper.GetOptNavValue<Organization>(itemNav, "ordered-by");

        }

        /// <summary>
        /// Writes the lab test results data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the lab test results data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Groups"/> is <b>null</b> or empty.
        /// </exception> 
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIf(
                _labGroup == null || _labGroup.Count == 0,
                "LabTestResultsLabGroupNotSet");

            // <lab-test-results>
            writer.WriteStartElement("lab-test-results");

            // when
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "when",
                _when);

            // lab-group
            for (int index = 0; index < _labGroup.Count; ++index)
            {
                _labGroup[index].WriteXml("lab-group", writer);
            }

            // ordered-by
            XmlWriterHelper.WriteOpt<Organization>(
                writer,
                "ordered-by",
                _orderedBy);

            // </lab-test-results>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the lab tests results.  
        /// </summary>
        /// 
        /// <remarks>
        /// The date and time should be set to <b> null </b> if they are 
        /// unknown. 
        /// </remarks>
        /// 
        public ApproximateDateTime When
        {
            get { return _when; }
            set { _when = value; }
        }
        private ApproximateDateTime _when;

        /// <summary>
        /// Gets a set of lab results.
        /// </summary>
        /// 
        public Collection<LabTestResultGroup> Groups
        {
            get { return _labGroup; }
        }
        private Collection<LabTestResultGroup> _labGroup =
            new Collection<LabTestResultGroup>();

        /// <summary>
        /// Gets or sets the information about the organization which
        /// ordered the lab tests.
        /// </summary>
        /// 
        /// <remarks>
        /// It should be set to <b> null</b> if it is unknown. 
        /// </remarks>
        /// 
        public Organization OrderedBy
        {
            get { return _orderedBy; }
            set { _orderedBy = value; }
        }
        private Organization _orderedBy;

        /// <summary>
        /// Gets a string representation of the lab test results item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the lab test results item.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            for (int index = 0; index < _labGroup.Count; ++index)
            {
                if (_labGroup[index].GroupName != null)
                {
                    if (!String.IsNullOrEmpty(_labGroup[index].GroupName.Text))
                    {
                        if (index > 0)
                        {
                            result.AppendFormat(
                                ResourceRetriever.GetResourceString(
                                    "ListFormat"),
                                _labGroup[index].GroupName.Text.ToString());
                        }
                        else
                        {
                            result.Append(_labGroup[index].GroupName.Text.ToString());
                        }
                    }
                }
            }
            return result.ToString();
        }
    }
}
