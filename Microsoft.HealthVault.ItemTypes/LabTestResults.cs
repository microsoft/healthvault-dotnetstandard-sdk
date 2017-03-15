// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A series of lab test results.
    /// </summary>
    ///
    public class LabTestResults : ThingBase
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
                this.labGroup.Add(labGroup);
            }
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
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
            this.when =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "when");

            // lab-group
            XPathNodeIterator labGroupIterator =
                itemNav.Select("lab-group");
            this.labGroup = new Collection<LabTestResultGroup>();
            foreach (XPathNavigator labGroupNav in labGroupIterator)
            {
                LabTestResultGroup labTestResultGroup = new LabTestResultGroup();
                labTestResultGroup.ParseXml(labGroupNav);
                this.labGroup.Add(labTestResultGroup);
            }

            // ordered-by
            this.orderedBy =
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
                this.labGroup == null || this.labGroup.Count == 0,
                "LabTestResultsLabGroupNotSet");

            // <lab-test-results>
            writer.WriteStartElement("lab-test-results");

            // when
            XmlWriterHelper.WriteOpt(
                writer,
                "when",
                this.when);

            // lab-group
            for (int index = 0; index < this.labGroup.Count; ++index)
            {
                this.labGroup[index].WriteXml("lab-group", writer);
            }

            // ordered-by
            XmlWriterHelper.WriteOpt(
                writer,
                "ordered-by",
                this.orderedBy);

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
            get { return this.when; }
            set { this.when = value; }
        }

        private ApproximateDateTime when;

        /// <summary>
        /// Gets a set of lab results.
        /// </summary>
        ///
        public Collection<LabTestResultGroup> Groups => this.labGroup;

        private Collection<LabTestResultGroup> labGroup =
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
            get { return this.orderedBy; }
            set { this.orderedBy = value; }
        }

        private Organization orderedBy;

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

            for (int index = 0; index < this.labGroup.Count; ++index)
            {
                if (this.labGroup[index].GroupName != null)
                {
                    if (!string.IsNullOrEmpty(this.labGroup[index].GroupName.Text))
                    {
                        if (index > 0)
                        {
                            result.AppendFormat(
                                ResourceRetriever.GetResourceString(
                                    "ListFormat"),
                                this.labGroup[index].GroupName.Text);
                        }
                        else
                        {
                            result.Append(this.labGroup[index].GroupName.Text);
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
