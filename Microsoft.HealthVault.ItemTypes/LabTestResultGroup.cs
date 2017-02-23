// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    ///  A set of lab test results.
    /// </summary>
    ///
    public class LabTestResultGroup : HealthRecordItemData
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResultGroup"/>
        /// class with default values.
        /// </summary>
        ///
        public LabTestResultGroup()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LabTestResultGroup"/>
        /// class with group name.
        /// </summary>
        ///
        /// <param name="groupName">
        /// The name of this set of tests.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="groupName"/> is <b>null</b>.
        /// </exception>
        ///
        public LabTestResultGroup(CodableValue groupName)
        {
            this.GroupName = groupName;
        }

        /// <summary>
        /// Populates this <see cref="LabTestResultGroup"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the lab test results group type data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // group-name
            this.groupName = new CodableValue();
            this.groupName.ParseXml(navigator.SelectSingleNode("group-name"));

            // laboratory-name
            this.laboratoryName =
                XPathHelper.GetOptNavValue<Organization>(navigator, "laboratory-name");

            // status
            this.status =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "status");

            // sub-groups
            XPathNodeIterator subGroupsIterator = navigator.Select("sub-groups");

            this.subGroups = new Collection<LabTestResultGroup>();
            foreach (XPathNavigator subGroupNav in subGroupsIterator)
            {
                LabTestResultGroup subGroup = new LabTestResultGroup();
                subGroup.ParseXml(subGroupNav);
                this.subGroups.Add(subGroup);
            }

            // results
            XPathNodeIterator resultsIterator = navigator.Select("results");

            this.results = new Collection<LabTestResultDetails>();
            foreach (XPathNavigator resultNav in resultsIterator)
            {
                LabTestResultDetails result = new LabTestResultDetails();
                result.ParseXml(resultNav);
                this.results.Add(result);
            }
        }

        /// <summary>
        /// Writes the lab test results group type data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the node to write XML output.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lab test results group type data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="GroupName"/> parameter is <b> null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.groupName, "LabTestResultsGroupTypeGroupNameNotSet");

            // <lab-test-results-group-type>
            writer.WriteStartElement(nodeName);

            // group-name
            this.groupName.WriteXml("group-name", writer);

            // laboratory-name
            XmlWriterHelper.WriteOpt(
                writer,
                "laboratory-name",
                this.laboratoryName);

            // status
            XmlWriterHelper.WriteOpt(
                writer,
                "status",
                this.status);

            // sub-groups
            if (this.subGroups != null)
            {
                for (int index = 0; index < this.subGroups.Count; ++index)
                {
                    this.subGroups[index].WriteXml("sub-groups", writer);
                }
            }

            // results
            for (int index = 0; index < this.results.Count; ++index)
            {
                this.results[index].WriteXml("results", writer);
            }

            // </lab-test-results-group-type>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of this set of tests.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue GroupName
        {
            get { return this.groupName; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "GroupName", "LabTestResultsGroupTypeGroupNameMandatory");
                this.groupName = value;
            }
        }

        private CodableValue groupName;

        /// <summary>
        /// Gets or sets the information about the laboratory which performed
        /// this set of tests.
        /// </summary>
        ///
        /// <remarks>
        /// Laboratory name should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public Organization LaboratoryName
        {
            get { return this.laboratoryName; }
            set { this.laboratoryName = value; }
        }

        private Organization laboratoryName;

        /// <summary>
        /// Gets or sets the overall status of this group and the sub group tests.
        /// </summary>
        ///
        /// <remarks>
        /// The status can be incomplete, pending, processing, etc. It should be
        /// set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        private CodableValue status;

        /// <summary>
        /// Gets lab test result sub groups.
        /// </summary>
        ///
        public Collection<LabTestResultGroup> SubGroups => this.subGroups;

        private Collection<LabTestResultGroup> subGroups =
            new Collection<LabTestResultGroup>();

        /// <summary>
        /// Gets a set of results for this group.
        /// </summary>
        ///
        public Collection<LabTestResultDetails> Results => this.results;

        private Collection<LabTestResultDetails> results = new Collection<LabTestResultDetails>();

        /// <summary>
        /// Gets a string representation of the lab test results group type item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the lab test results group type item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            result.Append(this.groupName);
            if (this.laboratoryName != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.laboratoryName.ToString());
            }

            if (this.status != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.status.ToString());
            }

            if (this.subGroups != null)
            {
                for (int index = 0; index < this.subGroups.Count; ++index)
                {
                    result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.subGroups[index].ToString());
                }
            }

            if (this.results != null)
            {
                for (int index = 0; index < this.results.Count; ++index)
                {
                    result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.results[index].ToString());
                }
            }

            return result.ToString();
        }
    }
}
