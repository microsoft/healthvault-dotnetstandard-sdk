// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Defines a single health or medical issue/problem.
    /// </summary>
    public class ConditionEntry : ItemBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ConditionEntry"/> class
        /// with default values.
        /// </summary>
        ///
        public ConditionEntry()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ConditionEntry"/> class
        /// with name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name or description of a condition.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b> null </b>.
        /// </exception>
        ///
        public ConditionEntry(CodableValue name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Populates this <see cref="Condition"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the condition data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // name
            this.name = new CodableValue();
            this.name.ParseXml(navigator.SelectSingleNode("name"));

            // onset-date
            this.onsetDate =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "onset-date");

            // resolution-date
            this.resolutionDate =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "resolution-date");

            // resolution
            if (navigator.SelectSingleNode("resolution") != null)
            {
                this.resolution = navigator.SelectSingleNode("resolution").Value;
            }

            // occurrence
            this.occurrence =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "occurrence");

            // severity
            this.severity =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "severity");
        }

        /// <summary>
        /// Writes the condition data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the condition item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the condition data to.
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
            Validator.ThrowSerializationIfNull(this.name, Resources.ConditionEntryNameNotSet);

            // <condition>
            writer.WriteStartElement(nodeName);

            // name
            this.name.WriteXml("name", writer);

            // onset-date
            XmlWriterHelper.WriteOpt(
                writer,
                "onset-date",
                this.onsetDate);

            // resolution-date
            XmlWriterHelper.WriteOpt(
                writer,
                "resolution-date",
                this.resolutionDate);

            // resolution
            XmlWriterHelper.WriteOptString(
                writer,
                "resolution",
                this.resolution);

            // occurrence
            XmlWriterHelper.WriteOpt(
                writer,
                "occurrence",
                this.occurrence);

            // severity
            XmlWriterHelper.WriteOpt(
                writer,
                "severity",
                this.severity);

            // </conditon>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name or description of a medical condition entry.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.ConditionEntryNameMandatory);
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets the date of onset or the first diagnosis.
        /// </summary>
        ///
        /// <remarks>
        /// The onset date should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate OnsetDate
        {
            get { return this.onsetDate; }
            set { this.onsetDate = value; }
        }

        private ApproximateDate onsetDate;

        /// <summary>
        /// Gets or sets the date the condition resolved (or for multiple acute
        /// episodes, the last date the condition resolved).
        /// </summary>
        ///
        /// <remarks>
        /// The resolution date should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate ResolutionDate
        {
            get { return this.resolutionDate; }
            set { this.resolutionDate = value; }
        }

        private ApproximateDate resolutionDate;

        /// <summary>
        /// Gets or sets the resolution which is a statement of how the condition
        /// was resolved.
        /// </summary>
        ///
        /// <remarks>
        /// The resolution of a condition should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Resolution
        {
            get { return this.resolution; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Resolution");
                this.resolution = value;
            }
        }

        private string resolution;

        /// <summary>
        /// Gets or sets the description of how often the condition occurs.
        /// </summary>
        ///
        /// <remarks>
        /// Examples of occurrence include acute, chronic. The occurrence of condition
        /// should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Occurrence
        {
            get { return this.occurrence; }
            set { this.occurrence = value; }
        }

        private CodableValue occurrence;

        /// <summary>
        /// Gets or sets the severity of a condition.
        /// </summary>
        ///
        /// <remarks>
        /// The severity should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Severity
        {
            get { return this.severity; }
            set { this.severity = value; }
        }

        private CodableValue severity;

        /// <summary>
        /// Gets a string of the name or description of the condition item.
        /// </summary>
        ///
        public override string ToString()
        {
            return this.name.ToString();
        }
    }
}
