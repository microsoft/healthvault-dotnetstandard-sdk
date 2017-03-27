// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates microbiology laboratory results.
    /// </summary>
    ///
    public class MicrobiologyLabResults : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MicrobiologyLabResults"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public MicrobiologyLabResults()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MicrobiologyLabResults"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the microbiology laboratory results.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> is <b>null</b>.
        /// </exception>
        ///
        public MicrobiologyLabResults(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("B8FCB138-F8E6-436A-A15D-E3A2D6916094");

        /// <summary>
        /// Populates this microbiology laboratory results instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the microbiology laboratory results data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a laboratory results node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("microbiology-lab-results");

            Validator.ThrowInvalidIfNull(itemNav, Resources.MicrobiologyLabResultsUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.labTests.Clear();
            XPathNodeIterator labTestIterator = itemNav.Select("lab-tests");
            foreach (XPathNavigator labTestNav in labTestIterator)
            {
                LabTestType labTest = new LabTestType();
                labTest.ParseXml(labTestNav);
                this.labTests.Add(labTest);
            }

            this.sensitivityAgent =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "sensitivity-agent");

            this.sensitivityValue =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "sensitivity-value");

            this.sensitivityInterpretation =
                XPathHelper.GetOptNavValue(itemNav, "sensitivity-interpretation");

            this.specimenType =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "specimen-type");

            this.organismName =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "organism-name");

            this.organismComment =
                XPathHelper.GetOptNavValue(itemNav, "organism-comment");
        }

        /// <summary>
        /// Writes the microbiology laboratory results data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the microbiology laboratory results data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.LabResultsWhenNotSet);

            // <microbiology-lab-results>
            writer.WriteStartElement("microbiology-lab-results");

            // <when>
            this.when.WriteXml("when", writer);

            for (int index = 0; index < this.labTests.Count; ++index)
            {
                this.labTests[index].WriteXml("lab-tests", writer);
            }

            XmlWriterHelper.WriteOpt(
                writer,
                "sensitivity-agent",
                this.sensitivityAgent);

            XmlWriterHelper.WriteOpt(
                writer,
                "sensitivity-value",
                this.sensitivityValue);

            XmlWriterHelper.WriteOptString(
                writer,
                "sensitivity-interpretation",
                this.sensitivityInterpretation);

            XmlWriterHelper.WriteOpt(
                writer,
                "specimen-type",
                this.specimenType);

            XmlWriterHelper.WriteOpt(
                writer,
                "organism-name",
                this.organismName);

            XmlWriterHelper.WriteOptString(
                writer,
                "organism-comment",
                this.organismComment);

            // </microbiology-lab-result>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the microbiology laboratory results occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets a collection of laboratory tests.
        /// </summary>
        ///
        public Collection<LabTestType> LabTests => this.labTests;

        private readonly Collection<LabTestType> labTests = new Collection<LabTestType>();

        /// <summary>
        /// Gets or sets the sensitivity agent.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity agent
        /// is not available.
        /// </remarks>
        ///
        public CodableValue SensitivityAgent
        {
            get { return this.sensitivityAgent; }
            set { this.sensitivityAgent = value; }
        }

        private CodableValue sensitivityAgent;

        /// <summary>
        /// Gets or sets the sensitivity value.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity value
        /// is not available.
        /// </remarks>
        ///
        public CodableValue SensitivityValue
        {
            get { return this.sensitivityValue; }
            set { this.sensitivityValue = value; }
        }

        private CodableValue sensitivityValue;

        /// <summary>
        /// Gets or sets the sensitivity interpretation.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the sensitivity
        /// interpretation is not available.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SensitivityInterpretation
        {
            get { return this.sensitivityInterpretation; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SensitivityInterpretation");
                this.sensitivityInterpretation = value;
            }
        }

        private string sensitivityInterpretation;

        /// <summary>
        /// Gets or sets the specimen type.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the specimen type
        /// is not available.
        /// </remarks>
        ///
        public CodableValue SpecimenType
        {
            get { return this.specimenType; }
            set { this.specimenType = value; }
        }

        private CodableValue specimenType;

        /// <summary>
        /// Gets or sets the organism name.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the organism name
        /// is not available.
        /// </remarks>
        ///
        public CodableValue OrganismName
        {
            get { return this.organismName; }
            set { this.organismName = value; }
        }

        private CodableValue organismName;

        /// <summary>
        /// Gets or sets the organism comment.
        /// </summary>
        ///
        /// <remarks>
        /// This property is optional and may be set to <b>null</b> if the organism comment
        /// is not available.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string OrganismComment
        {
            get { return this.organismComment; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "OrganismComment");
                this.organismComment = value;
            }
        }

        private string organismComment;

        /// <summary>
        /// Gets a string representation of the microbiology lab results.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the microbiology lab results.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            result.Append(this.When);

            for (int index = 0; index < this.LabTests.Count; ++index)
            {
                if (!string.IsNullOrEmpty(this.LabTests[index].Name))
                {
                    result.AppendFormat(
                        Resources.ListFormat,
                        this.LabTests[index].Name);
                }
            }

            return result.ToString();
        }
    }
}
