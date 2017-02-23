// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Defines a result from a health assessment containing the name and value of the assessed area.
    /// </summary>
    ///
    /// <remarks>
    /// See <see cref="HealthAssessment"/> for more information.
    /// </remarks>
    ///
    public class Assessment : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Assessment"/> class
        /// with default values.
        /// </summary>
        ///
        public Assessment()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Assessment"/> class
        /// with name and value.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the assessed area. See <see cref="Name"/> for more information.
        /// </param>
        ///
        /// <param name="value">
        /// The calculated value of the assessed area. See <see cref="Value"/> for more information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="value"/> is <b> null </b>.
        /// </exception>
        ///
        public Assessment(CodableValue name, CodableValue value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Populates this <see cref="Assessment"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the assessment data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // <name>
            this.name = new CodableValue();
            this.name.ParseXml(navigator.SelectSingleNode("name"));

            // <value>
            this.value = new CodableValue();
            this.value.ParseXml(navigator.SelectSingleNode("value"));

            // <group>
            this.group = XPathHelper.GetOptNavValue<CodableValue>(navigator, "group");
        }

        /// <summary>
        /// Writes the assessment data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the assessment item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the assessment data to.
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> or <see cref="Value"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, "AssessmentNameNotSet");
            Validator.ThrowSerializationIfNull(this.value, "AssessmentValueNotSet");

            writer.WriteStartElement(nodeName);

            // <name>
            this.name.WriteXml("name", writer);

            // <value>
            this.value.WriteXml("value", writer);

            // <group>
            XmlWriterHelper.WriteOpt(writer, "group", this.group);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the assessed area.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Heart attack risk, high blood pressure.
        /// The preferred vocabulary for route is "health-assessment-name".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If setter value is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "AssessmentNameMandatory");
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets the calculated value of the assessed area.
        /// </summary>
        ///
        /// <remarks>
        /// The value may be coded using a specific set of values.
        /// Example: Low/Medium/High risk.
        /// A list of vocabularies may be found in the preferred vocabulary
        /// "health-assessment-value-sets".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "AssessmentValueMandatory");
                this.value = value;
            }
        }

        private CodableValue value;

        /// <summary>
        /// Gets or sets the additional information that can be used to help organize the
        /// results.
        /// </summary>
        ///
        /// <remarks>
        /// The group is used to specify which group a specific result is in.
        /// For example, the supporting assessments that follow a main assessment are coded
        /// to indicate that they are supporting by specifying the "supporting" code.
        /// The preferred vocabulary for route is "health-assessment-groups".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        ///
        public CodableValue Group
        {
            get { return this.group; }
            set { this.group = value; }
        }

        private CodableValue group;

        /// <summary>
        /// Gets a string of the name or description of the assessment.
        /// </summary>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.name != null && this.value != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "AssessmentToStringFormat"),
                        this.name.ToString(),
                        this.value.ToString());
            }
            else if (this.name != null)
            {
                result = this.name.ToString();
            }
            else if (this.value != null)
            {
                result = this.value.ToString();
            }

            return result;
        }
    }
}
