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

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a lab test component, including the lab result value.
    /// </summary>
    ///
    public class LabTestType : ItemBase
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="LabTestType"/> class with
        /// default values.
        /// </summary>
        ///
        public LabTestType()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="LabTestType"/> with the specified test
        /// date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time when the laboratory test was performed.
        /// </param>
        ///
        public LabTestType(HealthServiceDateTime when)
        {
            this.When = when;
        }

        /// <summary>
        /// Populates the data for the lab test type from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the lab test type.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(navigator.SelectSingleNode("when"));

            this.name = XPathHelper.GetOptNavValue(navigator, "name");
            this.substance = XPathHelper.GetOptNavValue<CodableValue>(navigator, "substance");

            this.collectionMethod =
                XPathHelper.GetOptNavValue<CodableValue>(
                    navigator,
                    "collection-method");

            this.abbreviation = XPathHelper.GetOptNavValue(navigator, "abbreviation");
            this.description = XPathHelper.GetOptNavValue(navigator, "description");

            this.code.Clear();
            XPathNodeIterator codeIterator = navigator.Select("code");
            foreach (XPathNavigator codeNav in codeIterator)
            {
                CodableValue codeValue = new CodableValue();
                codeValue.ParseXml(codeNav);
                this.code.Add(codeValue);
            }

            this.result = XPathHelper.GetOptNavValue<LabResultType>(navigator, "result");
            this.status = XPathHelper.GetOptNavValue<CodableValue>(navigator, "status");
        }

        /// <summary>
        /// Writes the lab test type data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the lab test type.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lab test type to.
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

            this.when = new HealthServiceDateTime();
            this.when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptString(writer, "name", this.name);
            XmlWriterHelper.WriteOpt(writer, "substance", this.substance);
            XmlWriterHelper.WriteOpt(writer, "collection-method", this.collectionMethod);
            XmlWriterHelper.WriteOptString(writer, "abbreviation", this.abbreviation);
            XmlWriterHelper.WriteOptString(writer, "description", this.description);

            foreach (CodableValue codeValue in this.code)
            {
                codeValue.WriteXml("code", writer);
            }

            XmlWriterHelper.WriteOpt(writer, "result", this.result);
            XmlWriterHelper.WriteOpt(writer, "status", this.status);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the lab test was conducted.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date and time. The default value is the current year, month, and day.
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
        /// Gets or sets the name of the test.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the substance that was tested.
        /// </summary>
        ///
        public CodableValue Substance
        {
            get { return this.substance; }
            set { this.substance = value; }
        }

        private CodableValue substance;

        /// <summary>
        /// Gets or sets the method used to collect the substance.
        /// </summary>
        ///
        public CodableValue CollectionMethod
        {
            get { return this.collectionMethod; }
            set { this.collectionMethod = value; }
        }

        private CodableValue collectionMethod;

        /// <summary>
        /// Gets or sets the abbreviation for the test.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string TestAbbreviation
        {
            get { return this.abbreviation; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "TestAbbreviation");
                this.abbreviation = value;
            }
        }

        private string abbreviation;

        /// <summary>
        /// Gets or sets the description for the test.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets a collection of the clinical code(s) used for the test.
        /// </summary>
        ///
        public Collection<CodableValue> Code => this.code;

        private readonly Collection<CodableValue> code = new Collection<CodableValue>();

        /// <summary>
        /// Gets or sets the result of the lab test.
        /// </summary>
        ///
        public LabResultType Result
        {
            get { return this.result; }
            set { this.result = value; }
        }

        private LabResultType result;

        /// <summary>
        /// Gets or sets the status of the lab results.
        /// </summary>
        ///
        /// <value>
        /// A CodableValue representing the status of the lab results. For example, "completed" or
        /// "pending". These values can be found in the "lab-results-status" HealthVault vocabulary.
        /// </value>
        ///
        public CodableValue Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        private CodableValue status;

        /// <summary>
        /// Gets a string representation of the lab test type.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the lab test type.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(this.When);

            if (!string.IsNullOrEmpty(this.Name))
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Name);
            }

            if (this.Substance != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Substance.ToString());
            }

            if (this.CollectionMethod != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.CollectionMethod.ToString());
            }

            if (this.TestAbbreviation != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.TestAbbreviation);
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Description);
            }

            if (this.Result != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Result.ToString());
            }

            if (this.Status != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.Status.ToString());
            }

            return result.ToString();
        }
    }
}
