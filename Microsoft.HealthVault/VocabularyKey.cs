// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents a key for identifying a Vocabulary in the HealthLexicon.
    /// </summary>
    /// 
    public class VocabularyKey
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VocabularyKey"/> class 
        /// consisting only of a vocabulary name.
        /// </summary>
        /// 
        /// <remarks>
        /// Since the family is unspecified, it is assumed to be the
        /// HealthService family of vocabularies. Since the version is unspecified, 
        /// the most current version of the vocabulary is referenced for retrieval
        /// or searching.
        /// </remarks>
        /// 
        /// <param name="name">
        /// The name of the vocabulary.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public VocabularyKey(string name)
        {
            Validator.ThrowIfStringNullOrEmpty(name, "name");
            _name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VocabularyKey"/> class 
        /// consisting of a vocabulary name and the vocabulary family name. 
        /// </summary>
        /// 
        /// <remarks>
        /// Since the version is unspecified, the most current version of the 
        /// vocabulary will be returned.
        /// </remarks>
        /// 
        /// <param name="name">
        /// The name of the vocabulary.
        /// </param>
        /// <param name="family">
        /// The name of the family the vocabulary belongs to. 
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public VocabularyKey(string name, string family)
            : this(name)
        {
            if (!String.IsNullOrEmpty(family))
            {
                _family = family;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VocabularyKey"/> class 
        /// consisting of a vocabulary name, vocabulary version, and the 
        /// vocabulary family name.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the vocabulary.
        /// </param>
        /// <param name="family">
        /// The name of the family the vocabulary belongs to.
        /// </param>
        /// <param name="version">
        /// The version of the vocabulary. 
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/>  is <b>null</b> or empty.
        /// </exception>
        /// 
        public VocabularyKey(string name, string family, string version)
            : this(name, family)
        {
            if (!String.IsNullOrEmpty(version))
            {
                _version = version;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VocabularyKey"/> class 
        /// consisting of a vocabulary name, vocabulary version, the 
        /// vocabulary family name and the code value of the vocabulary item which should 
        /// be the first item in the result set of vocabulary items returned.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the vocabulary.
        /// </param>
        /// 
        /// <param name="family">
        /// The name of the family the vocabulary belongs to.
        /// </param>
        /// 
        /// <param name="version">
        /// The version of the vocabulary. 
        /// </param>
        /// 
        /// <param name="codeValue">
        /// The code value representing the vocabulary item which is to be the first item in the 
        /// result set of vocabulary items returned. 
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public VocabularyKey(string name, string family, string version, string codeValue)
            : this(name, family, version)
        {
            if (!String.IsNullOrEmpty(codeValue))
            {
                _codeValue = codeValue;
            }
        }

        internal VocabularyKey() { }

        /// <summary>
        /// Gets or sets the name of the Vocabulary.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If the property value is <b>null</b> or empty.
        /// </exception>
        /// 
        public string Name
        {
            get { return _name; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty("value", "Name");
                _name = value;
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the name of the family the <see cref="Vocabulary"/> belongs to. 
        /// </summary>
        /// 
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> family name.
        /// </value>
        /// 
        /// <remarks>
        /// If the family is unspecified, the system assumes that the requested
        /// vocabulary belongs to the HealthService family of vocabularies.
        /// </remarks>
        /// 
        public string Family
        {
            get { return _family; }
            set { _family = value; }
        }
        private string _family;

        /// <summary>
        /// Gets or sets the version of the <see cref="Vocabulary"/>. 
        /// </summary>
        /// 
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> version.
        /// </value>
        /// 
        /// <remarks>
        /// If the version is unspecified, the most current version of the 
        /// vocabulary is returned.
        /// </remarks>
        /// 
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }
        private string _version;

        /// <summary>
        /// Gets or sets the code value of the vocabulary key.
        /// </summary>
        /// 
        /// <value>
        /// The code value of the vocabulary key.
        /// </value>
        /// 
        /// <remarks>
        /// Use the <see cref="HealthVaultPlatform.GetVocabulary(Microsoft.Health.HealthServiceConnection, Microsoft.Health.VocabularyKey, bool)"/> 
        /// method and a 
        /// <see cref="VocabularyKey"/> to retrieve <see cref="VocabularyItem"/>
        /// objects. The GetVocabulary method returns a 
        /// <see cref="Vocabulary"/> object that contains 
        /// <see cref="VocabularyItem"/> objects in the 
        /// indicated vocabulary, sorted by code value, starting from the first 
        /// vocabulary item that has a code value greater than the code value
        /// of the vocabulary key. If the code value of the vocabulary key is 
        /// null or the empty string, the collection begins with the first 
        /// vocabulary item in the
        /// vocabulary. The maxVocabularyItems platform configuration item 
        /// limits maximum number of items the returned <see cref="Vocabulary"/> 
        /// object can contain. To retrieve the next set of vocabulary items,
        /// use a vocabulary key that has a code value equal to the code value of
        /// the last vocabulary item returned from the previous call.
        /// </remarks>
        /// <seealso cref="HealthVaultPlatform.GetVocabulary(Microsoft.Health.HealthServiceConnection, Microsoft.Health.VocabularyKey, bool)">
        /// HealthVaultPlatform.GetVocabulary Method</seealso>
        /// <seealso cref="Vocabulary">Vocabulary Class</seealso>
        /// <seealso cref="VocabularyItem">VocabularyItem Class</seealso>
        public string CodeValue
        {
            get { return _codeValue; }
            set { _codeValue = value; }
        }
        private string _codeValue;

        /// <summary>
        /// Gets or sets a text description of the <see cref="Vocabulary"/>.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the description.
        /// </value> 
        /// 
        /// <remarks>
        /// The description is not an integral part of the vocabulary itself, 
        /// but simply provides additional information about the vocabulary as 
        /// a whole.
        /// </remarks>
        /// 
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description = String.Empty;

        /// <summary>
        /// Returns a String that represents the current <see cref="Vocabulary"/>.
        /// </summary>
        /// 
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(56);
            sb.Append(_name);
            if (!String.IsNullOrEmpty(_family))
            {
                sb.Append("_");
                sb.Append(_family);
            }
            if (!String.IsNullOrEmpty(_version))
            {
                sb.Append("_");
                sb.Append(_version);
            }
            if (!String.IsNullOrEmpty(_codeValue))
            {
                sb.Append("_");
                sb.Append(_codeValue);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Writes the vocabulary key to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the vocabulary key to.
        /// </param> 
        ///
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("vocabulary-key");
            writer.WriteElementString("name", _name);
            if (!String.IsNullOrEmpty(_family))
            {
                writer.WriteElementString("family", _family);
            }
            if (!String.IsNullOrEmpty(_version))
            {
                writer.WriteElementString("version", _version);
            }
            if (!String.IsNullOrEmpty(_codeValue))
            {
                writer.WriteElementString("code-value", _codeValue);
            }
            writer.WriteEndElement(); //</vocabulary-key>
        }

        internal void ParseXml(XPathNavigator vocabularyKeyNav)
        {
            _name = vocabularyKeyNav.SelectSingleNode("name").Value;
            _family = vocabularyKeyNav.SelectSingleNode("family").Value;
            _version = vocabularyKeyNav.SelectSingleNode("version").Value;

            XPathNavigator codeValueNav
                = vocabularyKeyNav.SelectSingleNode("code-value");
            _codeValue = (codeValueNav != null) ? codeValueNav.Value : null;

            XPathNavigator descNav
                = vocabularyKeyNav.SelectSingleNode("description");
            _description = (descNav != null) ? descNav.Value : String.Empty;
        }
    }
}
