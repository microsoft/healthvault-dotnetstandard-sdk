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
            GroupName = groupName;
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
           _groupName = new CodableValue();
           _groupName.ParseXml(navigator.SelectSingleNode("group-name"));
           
           // laboratory-name
           _laboratoryName = 
               XPathHelper.GetOptNavValue<Organization>(navigator,"laboratory-name");

           // status
           _status = 
               XPathHelper.GetOptNavValue<CodableValue>(navigator,"status");

           // sub-groups
           XPathNodeIterator subGroupsIterator = navigator.Select("sub-groups");

           _subGroups = new Collection<LabTestResultGroup>();
           foreach (XPathNavigator subGroupNav in subGroupsIterator)
           {
               LabTestResultGroup _subGroup = new LabTestResultGroup();
               _subGroup.ParseXml(subGroupNav);
               _subGroups.Add(_subGroup);
           }

           // results
           XPathNodeIterator resultsIterator = navigator.Select("results");

           _results = new Collection<LabTestResultDetails>();
           foreach (XPathNavigator resultNav in resultsIterator)
           {
               LabTestResultDetails result = new LabTestResultDetails();
               result.ParseXml(resultNav);
               _results.Add(result);
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
            Validator.ThrowSerializationIfNull(_groupName, "LabTestResultsGroupTypeGroupNameNotSet");

            // <lab-test-results-group-type>
            writer.WriteStartElement(nodeName);

            // group-name
            _groupName.WriteXml("group-name",writer);

            // laboratory-name
            XmlWriterHelper.WriteOpt<Organization>( 
                writer,
                "laboratory-name",
                _laboratoryName);

            // status
            XmlWriterHelper.WriteOpt<CodableValue>( 
                writer,
                "status",
                _status);

            // sub-groups
            if (_subGroups != null)
            {
                for (int index = 0; index < _subGroups.Count; ++index)
                {
                    _subGroups[index].WriteXml("sub-groups", writer);
                }
            }

            // results
            for (int index = 0; index < _results.Count; ++index)
            {
                _results[index].WriteXml("results", writer);
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
            get { return _groupName;}
            set
            {
                Validator.ThrowIfArgumentNull(value, "GroupName", "LabTestResultsGroupTypeGroupNameMandatory");
                _groupName =  value;
            }
        }
        private CodableValue _groupName;

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
            get { return _laboratoryName; }
            set { _laboratoryName = value; }
        }
        private Organization _laboratoryName;

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
            get { return _status;}
            set { _status = value; }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets lab test result sub groups.  
        /// </summary>
        /// 
        public Collection<LabTestResultGroup> SubGroups
        {
            get { return _subGroups; }
        }
        private Collection<LabTestResultGroup> _subGroups =
            new Collection<LabTestResultGroup>();

        /// <summary>
        /// Gets a set of results for this group.
        /// </summary>
        /// 
        public Collection<LabTestResultDetails> Results
        {
            get { return _results; }
        }
        private Collection<LabTestResultDetails> _results=new Collection<LabTestResultDetails>();

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
            result.Append(_groupName.ToString());
            if (_laboratoryName != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    _laboratoryName.ToString());
            }
            if (_status != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    _status.ToString());
            }
            if (_subGroups != null)
            {
                for (int index = 0; index < _subGroups.Count; ++index)
                {
                    result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    _subGroups[index].ToString());
                }
            }
            if (_results != null)
            {
                for (int index = 0; index < _results.Count; ++index)
                {
                    result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    _results[index].ToString());
                }
            }
            return result.ToString();
        }
    }
}
