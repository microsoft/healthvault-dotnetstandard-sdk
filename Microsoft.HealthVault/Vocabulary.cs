// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Vocabulary list
    /// </summary>
    /// 
    public class Vocabulary : Dictionary<string, VocabularyItem>, IXmlSerializable
    {
        internal Vocabulary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="Vocabulary"/> class
        /// with the specified name
        /// </summary>
        /// <param name="name">The name of the vocabulary</param>
        public Vocabulary(string name)
            : this()
        {
            _name = name;
        }

        /// <summary>
        /// Create a new instance of the <see cref="Vocabulary"/> class
        /// with the specified name, family, and version
        /// </summary>
        /// <param name="name">The name of the vocabulary</param>
        /// <param name="family">The family of the vocabulary</param>
        /// <param name="version">The version of the vocabulary</param>
        public Vocabulary(string name, string family, string version)
            : this()
        {
            _name = name;
            _family = family;
            _version = version;
        }

        /// <summary>
        /// Gets or sets culture information containing the language in which the
        /// vocabulary items are represented.
        /// </summary>
        /// 
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        CultureInfo _culture;

        /// <summary>
        /// Gets the name of the <see cref="Vocabulary"/>.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> name.
        /// </value>
        /// 
        public string Name
        {
            get { return _name; }
        }
        string _name;

        /// <summary>
        /// Gets the family of the vocabulary.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> family.
        /// </value>
        /// 
        /// <remarks>
        /// The family indicates the source of the 
        /// information in the vocabulary, including 
        /// external standards such as ISO or 
        /// internal standards such as HealthVault Vocabulary. 
        /// </remarks>
        /// 
        public string Family
        {
            get { return _family; }
        }
        string _family;

        /// <summary>
        /// Gets the version of the <see cref="Vocabulary"/>.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the <see cref="Vocabulary"/> version.
        /// </value>
        /// 
        public string Version
        {
            get { return _version; }
        }
        string _version;

        /// <summary>
        /// Gets if the vocabulary contains none empty member.  
        /// </summary>
        public bool IsNotEmpty
        {
            get
            {
                foreach (KeyValuePair<string, VocabularyItem> kvp in this)
                {
                    if ((!String.IsNullOrEmpty(kvp.Value.AbbreviationText))
                        || (!String.IsNullOrEmpty(kvp.Value.DisplayText)))
                    {
                        _isNotEmpty = true;
                        break;
                    }
                }
                return _isNotEmpty;
            }
        }
        private bool _isNotEmpty;

        /// <summary>
        /// Gets if the set vocabulary items in the <see cref="Vocabulary"/> has been truncated i.e. 
        /// there could be more <see cref="VocabularyItem"/>s in the <see cref="Vocabulary"/>. 
        /// </summary>
        /// 
        [Obsolete("Use Vocabulary.IsTruncated instead.")]
        public bool IsTruncted
        {
            get { return _isTruncated; }
        }

        /// <summary>
        /// Gets if the set vocabulary items in the <see cref="Vocabulary"/> has been truncated i.e. 
        /// there could be more <see cref="VocabularyItem"/>s in the <see cref="Vocabulary"/>. 
        /// </summary>
        /// 
        public bool IsTruncated
        {
            get { return _isTruncated; }
            set { _isTruncated = value; }
        }
        private bool _isTruncated;

        /// <summary>
        /// Adds a vocabulary item to the vocabulary.
        /// </summary>
        /// <param name="item">The <see cref="VocabularyItem"/> instance to add.</param>
        public void Add(VocabularyItem item)
        {
            item.Vocabulary = this;
            base.Add(item.Value, item);
        }

        internal virtual void AddVocabularyItem(string key, VocabularyItem item)
        {
            item.Vocabulary = this;
            base.Add(key, item);
        }

        internal void PopulateFromXml(
            XPathNavigator vocabularyNav)
        {
            _name = vocabularyNav.SelectSingleNode("name").Value;
            _family = vocabularyNav.SelectSingleNode("family").Value;
            _version = vocabularyNav.SelectSingleNode("version").Value;
            _culture = new CultureInfo(vocabularyNav.XmlLang);

            XPathNodeIterator vocabularyItemsNav = vocabularyNav.Select("code-item");

            foreach (XPathNavigator vocabularyItemNav in vocabularyItemsNav)
            {
                // Check in case HealthVault returned invalid data.
                if (vocabularyItemNav == null)
                {
                    continue;
                }

                VocabularyItem code = new VocabularyItem();
                code.Vocabulary = this;
                code.ParseXml(vocabularyItemNav);
                AddVocabularyItem(code.Value, code);
            }

            XPathNavigator isTruncatedNav = vocabularyNav.SelectSingleNode("is-vocab-truncated");
            _isTruncated = isTruncatedNav != null ? isTruncatedNav.ValueAsBoolean : false;
        }

        private static XPathExpression _infoPath = XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetInfoXPathExpression(
            string methodNSSuffix, XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + methodNSSuffix);

            XPathExpression infoPathClone = null;
            lock (_infoPath)
            {
                infoPathClone = _infoPath.Clone();
            }

            infoPathClone.SetContext(infoXmlNamespaceManager);

            return infoPathClone;
        }

        #region IXmlSerializable Members

        /// <summary>
        /// GetSchema method for serialization
        /// </summary>
        /// <returns>Schema</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Serialization method to read the Xml from the reader and deserialize the instance.
        /// </summary>
        /// <param name="reader">The reader</param>
        public void ReadXml(XmlReader reader)
        {
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();

            XPathNavigator vocabularyNode = navigator.SelectSingleNode("Vocabulary");

            _name = vocabularyNode.SelectSingleNode("name").Value;

            _family = XPathHelper.GetOptNavValue(vocabularyNode, "family");
            _version = XPathHelper.GetOptNavValue(vocabularyNode, "version");

            XPathNavigator itemsNode = vocabularyNode.SelectSingleNode("items");
            if (itemsNode != null)
            {
                foreach (XPathNavigator itemNode in itemsNode.SelectChildren(XPathNodeType.Element))
                {
                    VocabularyItem vocabItem = new VocabularyItem();
                    vocabItem.ParseXml(itemNode);
                    Add(vocabItem);
                }
            }
        }

        /// <summary>
        /// Write the serialized version of the data to the specified writer.
        /// </summary>
        /// <param name="writer">The writer</param>
        public void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowInvalidIf(
                String.IsNullOrEmpty(_name),
                "VocabularyNameNullOrEmpty");

            writer.WriteElementString("vocabulary-format-version", "1");

            writer.WriteElementString("name", _name);

            XmlWriterHelper.WriteOptString(writer, "family", _family);
            XmlWriterHelper.WriteOptString(writer, "version", _version);

            if (Count == 0)
            {
                return;
            }

            writer.WriteStartElement("items");
            {
                foreach (KeyValuePair<string, VocabularyItem> item in this)
                {
                    item.Value.WriteXmlInternal("item", writer);
                }
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
