// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates radiology
    /// laboratory results.
    /// </summary>
    ///
    public class RadiologyLabResults : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RadiologyLabResults"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public RadiologyLabResults()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RadiologyLabResults"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the radiology laboratory results.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public RadiologyLabResults(HealthServiceDateTime when)
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
            new Guid("E4911BD3-61BF-4E10-AE78-9C574B888B8F");

        /// <summary>
        /// Populates this radiology laboratory results instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the radiology laboratory results data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a radiology laboratory results node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("radiology-lab-results");

            Validator.ThrowInvalidIfNull(itemNav, "RadiologyLabResultsUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // <title>
            this.title =
                XPathHelper.GetOptNavValue(itemNav, "title");

            // <anatomic-site>
            this.anatomicSite =
                XPathHelper.GetOptNavValue(itemNav, "anatomic-site");

            // <result-text>
            this.resultText =
                XPathHelper.GetOptNavValue(itemNav, "result-text");
        }

        /// <summary>
        /// Writes the radiology laboratory results data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the radiology laboratory results data to.
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
            Validator.ThrowSerializationIfNull(this.when, "RadiologyLabResultsWhenNotSet");

            // <radiology-lab-results>
            writer.WriteStartElement("radiology-lab-results");

            // <when>
            this.when.WriteXml("when", writer);

            // <title>
            XmlWriterHelper.WriteOptString(
                writer,
                "title",
                this.title);

            // <anatomic-site>
            XmlWriterHelper.WriteOptString(
                writer,
                "anatomic-site",
                this.anatomicSite);

            // <result-text>
            XmlWriterHelper.WriteOptString(
                writer,
                "result-text",
                this.resultText);

            // </radiology-lab-result>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the radiology laboratory results occurred.
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
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the title for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the title.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Title
        {
            get { return this.title; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Title");
                this.title = value;
            }
        }

        private string title;

        /// <summary>
        /// Gets or sets the anatomic site for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the site.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the site should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AnatomicSite
        {
            get { return this.anatomicSite; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AnatomicSite");
                this.anatomicSite = value;
            }
        }

        private string anatomicSite;

        /// <summary>
        /// Gets or sets the result text for the radiology laboratory results.
        /// </summary>
        ///
        /// <value>
        /// A string representing the text.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the result text should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string ResultText
        {
            get { return this.resultText; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "ResultText");
                this.resultText = value;
            }
        }

        private string resultText;

        /// <summary>
        /// Gets a string representation of the radiology lab results.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the radiology lab results.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Title != null)
            {
                result = this.Title;
            }

            return result;
        }
    }
}
