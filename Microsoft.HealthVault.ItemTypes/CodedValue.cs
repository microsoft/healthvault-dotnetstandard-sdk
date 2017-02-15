// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the code description for a <see cref="CodableValue"/>.
    /// </summary>
    ///
    public class CodedValue : HealthRecordItemData
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
            Validator.ThrowIfArgumentNull(key, "key", "VocabularyKeyMandatory");

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

            _value = navigator.SelectSingleNode("value").Value;

            XPathNavigator famNav = navigator.SelectSingleNode("family");
            if (famNav != null)
            {
                _family = famNav.Value;
            }

            _vocabName = navigator.SelectSingleNode("type").Value;

            XPathNavigator versionNav =
                navigator.SelectSingleNode("version");

            if (versionNav != null)
            {
                _version = versionNav.Value;
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

            Validator.ThrowSerializationIf(_value == null, "ValueNotSet");
            Validator.ThrowSerializationIf(_vocabName == null, "NameNotSet");

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("value", _value);

            if (!string.IsNullOrEmpty(_family))
            {
                writer.WriteElementString("family", _family);
            }

            writer.WriteElementString("type", _vocabName);

            if (!string.IsNullOrEmpty(_version))
            {
                writer.WriteElementString("version", _version);
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
            get { return _value; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Value");
                Validator.ThrowIfStringIsWhitespace(value, "Value");
                _value = value;
            }
        }
        private string _value;

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
            get { return _family; }
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw Validator.ArgumentException("Family", "WhitespaceOnlyValue");
                }
                _family = value;
            }
        }
        private string _family;

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
            get { return _vocabName; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "VocabularyName");
                Validator.ThrowIfStringIsWhitespace(value, "VocabularyName");
                _vocabName = value;
            }
        }
        private string _vocabName;

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
            get { return _version; }
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw Validator.ArgumentException("Version", "WhitespaceOnlyValue");
                }
                _version = value;
            }
        }
        private string _version;

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

            if (Family != null)
            {
                result.Append(Family);
            }

            if (VocabularyName != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.Append(VocabularyName);
            }

            if (Version != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.Append(Version);
            }

            if (Value != null)
            {
                if (result.Length > 0)
                {
                    result.Append(
                        ResourceRetriever.GetResourceString(
                            "ListSeparator"));
                }
                result.Append(Value);
            }

            return result.ToString();
        }
    }
}
