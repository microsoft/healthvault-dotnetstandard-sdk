// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A single laboratory test.
    /// </summary>
    ///
    public class LabTestResultDetails : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="LabTestResultDetails"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the lab test result type data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the first node in <paramref name="navigator"/> is not
        /// a lab test result type node.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // when
            this.when =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "when");

            // name
            this.name =
                XPathHelper.GetOptNavValue(navigator, "name");

            // substance
            this.substance =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "substance");

            // collection-method
            this.collectionMethod =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "collection-method");

            // clinical-code
            this.clinicalCode =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "clinical-code");

            // value
            this.value =
                XPathHelper.GetOptNavValue<LabTestResultValue>(navigator, "value");

            // status
            this.status =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "status");

            // note
            this.note =
                XPathHelper.GetOptNavValue(navigator, "note");
        }

        /// <summary>
        /// Writes the lab test result type data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the node to write XML output.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lab test result type data to.
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <lab-test-result-type>
            writer.WriteStartElement(nodeName);

            // when
            XmlWriterHelper.WriteOpt(
                writer,
                "when",
                this.when);

            // name
            XmlWriterHelper.WriteOptString(
                writer,
                "name",
                this.name);

            // substance
            XmlWriterHelper.WriteOpt(
                writer,
                "substance",
                this.substance);

            // collection-method
            XmlWriterHelper.WriteOpt(
                writer,
                "collection-method",
                this.collectionMethod);

            // clinical-code
            XmlWriterHelper.WriteOpt(
                writer,
                "clinical-code",
                this.clinicalCode);

            // value
            XmlWriterHelper.WriteOpt(
                writer,
                "value",
                this.value);

            // status
            XmlWriterHelper.WriteOpt(
                writer,
                "status",
                this.status);

            // note
            XmlWriterHelper.WriteOptString(
                writer,
                "note",
                this.note);

            // </lab-test-result-type>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets date and the time of the laboratory test.
        /// </summary>
        ///
        /// <remarks>
        /// They should be set to <b>null</b> if they are unknown.
        /// </remarks>
        ///
        public ApproximateDateTime When
        {
            get { return this.when; }
            set { this.when = value; }
        }

        private ApproximateDateTime when;

        /// <summary>
        /// Gets or sets name of the laboratory test.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> or empty if it is unknown.
        /// </remarks>
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
        /// Gets or sets substance that is tested.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Substance
        {
            get { return this.substance; }
            set { this.substance = value; }
        }

        private CodableValue substance;

        /// <summary>
        /// Gets or sets the collection method for the laboratory test.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue CollectionMethod
        {
            get { return this.collectionMethod; }
            set { this.collectionMethod = value; }
        }

        private CodableValue collectionMethod;

        /// <summary>
        /// Gets or sets the clinical code for the lab tests.
        /// </summary>
        ///
        /// <remarks>
        /// It should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue ClinicalCode
        {
            get { return this.clinicalCode; }
            set { this.clinicalCode = value; }
        }

        private CodableValue clinicalCode;

        /// <summary>
        /// Gets or sets the clinical value within a laboratory result.
        /// </summary>
        ///
        /// <remarks>
        /// The type of value is defined within a laboratory result, which includes
        /// value, unit, reference and toxic range. It should be set to <b>null</b>
        /// if it is unknown.
        /// </remarks>
        ///
        public LabTestResultValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private LabTestResultValue value;

        /// <summary>
        /// Gets or sets the status of the laboratory results.
        /// </summary>
        ///
        /// <remarks>
        /// Examples of status include complete and pending. It should be
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
        /// Gets or sets a note that augments the laboratory result.
        /// </summary>
        ///
        /// <remarks>
        /// There are two scenarios in which a note is typically added:
        /// A note may provide additional information about interpreting
        /// the result beyond what is stored in the ranges associated with
        /// the value.
        /// Or, a note may be use to provide the textual result of a lab test
        /// that does not have a measured value.
        ///
        /// Formatting:
        /// Notes may come from systems that require a line-based approach to textual display. To support
        /// this, applications should render the spaces and newlines in the note, and use a fixed-width font.
        /// The form transform for the lab test results type shows one way to do this.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Note
        {
            get { return this.note; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                this.note = value;
            }
        }

        private string note;

        /// <summary>
        /// Gets a string representation of the lab test result type item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the lab test result type item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            string space = ResourceRetriever.GetSpace("errors");

            bool first = true;
            if (this.when != null)
            {
                result.Append(this.when);
                first = false;
            }

            if (!string.IsNullOrEmpty(this.name))
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.name);
                first = false;
            }

            if (this.substance != null)
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.substance);
                first = false;
            }

            if (this.collectionMethod != null)
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.collectionMethod);
                first = false;
            }

            if (this.clinicalCode != null)
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.clinicalCode);
                first = false;
            }

            if (this.value != null)
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.value);
                first = false;
            }

            if (this.status != null)
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.status);
                first = false;
            }

            if (!string.IsNullOrEmpty(this.note))
            {
                if (!first)
                {
                    result.Append(space);
                }

                result.Append(this.note);
                first = false;
            }

            return result.ToString();
        }
    }
}
