// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the code description for a <see cref="CodableValue"/>.
    /// </summary>
    ///
    public class CodedValue : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CodedValue"/> class with default values.
        /// </summary>
        ///
        public CodedValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CodedValue"/> class with
        /// the specified code value and vocabulary.
        /// </summary>
        ///
        /// <param name="value">
        /// The identifying value for the code.
        /// </param>
        ///
        /// <param name="vocabularyName">
        /// The name of the vocabulary the code belongs to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter or <paramref name="vocabularyName"/>
        /// is <b>null</b> or empty.
        /// </exception>
        ///
        public CodedValue(string value, string vocabularyName)
        {
            this.Value = value;
            this.VocabularyName = vocabularyName;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CodedValue"/> class
        /// with the specified code value, vocabulary, family, and version.
        /// </summary>
        ///
        /// <param name="value">
        /// The identifying value for the code.
        /// </param>
        ///
        /// <param name="vocabularyName">
        /// The name of the vocabulary the code belongs to.
        /// </param>
        ///
        /// <param name="family">
        /// The family of vocabulary terms that the code belongs to.
        /// </param>
        ///
        /// <param name="version">
        /// The version of the vocabulary the code belongs to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> or <paramref name="vocabularyName"/> parameter
        /// is <b>null</b> or empty.
        /// </exception>
        ///
        public CodedValue(
            string value,
            string vocabularyName,
            string family,
            string version)
        {
            this.Value = value;
            this.VocabularyName = vocabularyName;
            this.Family = family;
            this.Version = version;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CodedValue"/> class
        /// with the specified code value, vocabulary, family, and version.
        /// </summary>
        /// <param name="value">
        /// The identifying value for the code.
        /// </param>
        /// <param name="key">
        /// key for identifying a Vocabulary in the HealthLexicon.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> or <paramref name="key"/> parameter
        /// is <b>null</b> or empty.
        /// </exception>
        public CodedValue(string value, VocabularyKey key)
        {
            Validator.ThrowIfArgumentNull(key, nameof(key), Resources.VocabularyKeyMandatory);

            this.Value = value;
            this.VocabularyName = key.Name;
            this.Family = key.Family;
            this.Version = key.Version;
        }

        /// <summary>
        /// Populates the description of the code from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the coded value.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.value = navigator.SelectSingleNode("value").Value;

            XPathNavigator famNav = navigator.SelectSingleNode("family");
            if (famNav != null)
            {
                this.family = famNav.Value;
            }

            this.vocabName = navigator.SelectSingleNode("type").Value;

            XPathNavigator versionNav =
                navigator.SelectSingleNode("version");

            if (versionNav != null)
            {
                this.version = versionNav.Value;
            }
        }

        /// <summary>
        /// Writes the code description to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the code value.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the code description to.
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Value"/> or <see cref="VocabularyName"/> property
        /// is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            if (this.value == null)
            {
                throw new ThingSerializationException(Resources.ValueNotSet);
            }

            if (this.vocabName == null)
            {
                throw new ThingSerializationException(Resources.NameNotSet);
            }

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("value", this.value);

            if (!string.IsNullOrEmpty(this.family))
            {
                writer.WriteElementString("family", this.family);
            }

            writer.WriteElementString("type", this.vocabName);

            if (!string.IsNullOrEmpty(this.version))
            {
                writer.WriteElementString("version", this.version);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the code value.
        /// </summary>
        ///
        /// <value>
        /// A string representing the code value.
        /// </value>
        ///
        /// <remarks>
        /// The code value is the identifier for the code in the specified
        /// vocabulary and family.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        ///
        public string Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Value");
                Validator.ThrowIfStringIsWhitespace(value, "Value");
                this.value = value;
            }
        }

        private string value;

        /// <summary>
        /// Gets or sets the code family.
        /// </summary>
        ///
        /// <value>
        /// A string representing the code family.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the property should not be stored.
        /// <br/><br/>
        /// The family represents the NCPDP value for a code.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Family
        {
            get { return this.family; }

            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(Resources.WhitespaceOnlyValue, nameof(this.Family));
                }

                this.family = value;
            }
        }

        private string family;

        /// <summary>
        /// Gets or sets the vocabulary name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the vocabulary name.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> set is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        ///
        public string VocabularyName
        {
            get { return this.vocabName; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "VocabularyName");
                Validator.ThrowIfStringIsWhitespace(value, "VocabularyName");
                this.vocabName = value;
            }
        }

        private string vocabName;

        /// <summary>
        /// Gets or sets the code version.
        /// </summary>
        ///
        /// <value>
        /// A string representing the code version.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the property should not be stored.
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
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(Resources.WhitespaceOnlyValue, nameof(this.Version));
                }

                this.version = value;
            }
        }

        private string version;

        /// <summary>
        /// Gets a string representation of the vocabulary item reference.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the vocabulary item reference.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(30);

            if (this.Family != null)
            {
                result.Append(this.Family);
            }

            if (this.VocabularyName != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.Append(this.VocabularyName);
            }

            if (this.Version != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.Append(this.Version);
            }

            if (this.Value != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        Resources.ListSeparator);
                }

                result.Append(this.Value);
            }

            return result.ToString();
        }
    }
}
