// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a medical annotation.
    /// </summary>
    ///
    public class Annotation : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Annotation"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItemAsync(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Annotation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Annotation"/> class with
        /// the specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the annotation was taken.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Annotation(HealthServiceDateTime when)
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
            new Guid("7AB3E662-CC5B-4BE2-BF38-78F8AAD5B161");

        /// <summary>
        /// Populates this <see cref="Annotation"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the annotation data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an annotation node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("annotation");

            Validator.ThrowInvalidIfNull(itemNav, "AnnotationUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.content =
                XPathHelper.GetOptNavValue(itemNav, "content");

            this.author =
                XPathHelper.GetOptNavValue<PersonItem>(itemNav, "author");

            this.classification =
                XPathHelper.GetOptNavValue(itemNav, "classification");

            this.index =
                XPathHelper.GetOptNavValue(itemNav, "index");

            this.version =
                XPathHelper.GetOptNavValue(itemNav, "version");
        }

        /// <summary>
        /// Writes the annotation data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the annotation data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// A mandatory property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            Validator.ThrowSerializationIfNull(this.when, "AnnotationWhenNotSet");

            // <annotation>
            writer.WriteStartElement("annotation");

            // <when>
            this.when.WriteXml("when", writer);

            // <content>
            XmlWriterHelper.WriteOptString(
                writer,
                "content",
                this.content);

            // <author>
            XmlWriterHelper.WriteOpt(
                writer,
                "author",
                this.Author);

            // <classification>
            XmlWriterHelper.WriteOptString(
                writer,
                "classification",
                this.classification);

            // <index>
            XmlWriterHelper.WriteOptString(
                writer,
                "index",
                this.index);

            // <version>
            XmlWriterHelper.WriteOptString(
                writer,
                "version",
                this.version);

            // </annotation>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the annotation was created.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/>. The default value is
        /// the current year, month, and day.
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
        /// Gets or sets the content for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the content.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the content should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Content
        {
            get { return this.content; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Content");
                this.content = value;
            }
        }

        private string content;

        /// <summary>
        /// Gets or sets the author contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the author contact information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the author contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Author
        {
            get { return this.author; }
            set { this.author = value; }
        }

        private PersonItem author;

        /// <summary>
        /// Gets or sets the classification for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the classification.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the classification should not be fstored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Classification
        {
            get { return this.classification; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Classification");
                this.classification = value;
            }
        }

        private string classification;

        /// <summary>
        /// Gets or sets the index for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the index.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the index should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Index
        {
            get { return this.index; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Index");
                this.index = value;
            }
        }

        private string index;

        /// <summary>
        /// Gets or sets the version for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the version.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the version should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Version
        {
            get { return this.version; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Version");
                this.version = value;
            }
        }

        private string version;

        /// <summary>
        /// Gets a string representation of the annotation item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the annotation item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result;

            if (this.Content != null)
            {
                result = this.Content;

                if (this.Content.Length > 50)
                {
                    result = this.Content.Substring(0, 50) +
                             ResourceRetriever.GetResourceString("Ellipsis");
                }
            }
            else if (this.Author != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "AnnotationAuthorFormat"),
                        this.Author.ToString());
            }
            else
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "AnnotationDateFormat"),
                        this.When.ToString());
            }

            return result;
        }
    }
}
