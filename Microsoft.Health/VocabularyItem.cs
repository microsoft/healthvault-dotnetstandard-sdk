// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.Health.ItemTypes;

namespace Microsoft.Health
{
    /// <summary>
    /// Represents an item in the HealthVault <see cref="Vocabulary"/>.
    /// </summary>
    /// 
    [XmlRoot("item")]
    public class VocabularyItem : HealthRecordItemData, IXmlSerializable
    {
        /// <summary>
        /// Create an instance of the <see cref="VocabularyItem"/> class.
        /// </summary>
        public VocabularyItem()
        {
        }

        /// <summary>
        /// Create an instance of the <see cref="VocabularyItem"/> class
        /// with the specified code value.
        /// </summary>
        /// <param name="value">The code value for the item</param>
        public VocabularyItem(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Create an instance of the <see cref="VocabularyItem"/> class
        /// with the specified code value and display value
        /// </summary>
        /// <param name="value">The code value for the item</param>
        /// <param name="displayText">The display text for the item</param>
        public VocabularyItem(string value, string displayText)
        {
            Value = value;
            _displayText = displayText;
        }

        /// <summary>
        /// Create an instance of the <see cref="VocabularyItem"/> class
        /// with the specified code value and display value
        /// </summary>
        /// <param name="value">The code value for the item</param>
        /// <param name="displayText">The display text for the item</param>
        /// <param name="abbreviationText">The abbreviation text for the item</param>
        public VocabularyItem(string value, string displayText, string abbreviationText)
        {
            Value = value;
            _displayText = displayText;
            _abbreviationText = abbreviationText;
        }

        /// <summary> 
        /// Populates the description of the code from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the coded item.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.Value = navigator.SelectSingleNode("code-value").Value;

            _displayText = XPathHelper.GetOptNavValue(navigator, "display-text");

            // preserve previous behavior where items without abbreviations have an empty string...
            string abbreviationText = XPathHelper.GetOptNavValue(navigator, "abbreviation-text");
            if (abbreviationText != null)
            {
                _abbreviationText = abbreviationText;
            }

            XPathNavigator infoNav
                = navigator.SelectSingleNode("info-xml");
            if (infoNav != null)
            {
                try
                {
                    using (XmlReader reader = SDKHelper.GetXmlReaderForXml(infoNav.InnerXml, SDKHelper.XmlReaderSettings))
                    {
                        _infoXml = new XPathDocument(reader);
                    }
                }
                catch (XmlException)
                {
                    //don't want to expose info about XML parse errors here
                    //to the outside world as recommended in .NET 
                    //documentation
                    throw new HealthServiceException(
                        HealthServiceStatusCode.VocabularyLoadError);
                }
            }
        }

        /// <summary> 
        /// Writes the vocabulary item to the specified XML writer.
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
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The Value or VocabularyName property 
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        internal void WriteXmlInternal(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowIfStringNullOrEmpty(Value, "Value");

            if (nodeName != null)
            {
                writer.WriteStartElement(nodeName);
            }

            writer.WriteElementString("code-value", Value);

            XmlWriterHelper.WriteOptString(writer, "display-text", _displayText);
            XmlWriterHelper.WriteOptString(writer, "abbreviation-text", _abbreviationText);

            if (_infoXml != null)
            {
                writer.WriteStartElement("info-xml");
                {
                    writer.WriteRaw(_infoXml.CreateNavigator().InnerXml);
                }
                writer.WriteEndElement();
            }

            if (nodeName != null)
            {
                writer.WriteEndElement();
            }
        }

        /// <summary> 
        /// Writes the vocabulary item to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the code value.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the vocabulary item to.
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
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIf(String.IsNullOrEmpty(_value), "ValueNotSet");
            Validator.ThrowSerializationIf(String.IsNullOrEmpty(_vocabName), "NameNotSet");

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("value", _value);

            if (!String.IsNullOrEmpty(_family))
            {
                writer.WriteElementString("family", _family);
            }

            writer.WriteElementString("type", _vocabName);

            if (!String.IsNullOrEmpty(_version))
            {
                writer.WriteElementString("version", _version);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a localized display text. 
        /// </summary>
        /// 
        /// <value>
        /// A string representing the display text.
        /// </value>
        /// 
        public string DisplayText
        {
            get { return _displayText; }
        }
        private string _displayText = String.Empty;

        /// <summary>
        /// Gets a localized and abbreviated representation 
        /// of the display text.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the abbreviation text.
        /// </value>
        /// 
        public string AbbreviationText
        {
            get { return _abbreviationText; }
        }
        private string _abbreviationText = String.Empty;

        /// <summary>
        /// Gets any extra information associated with the code item.
        /// </summary>
        /// 
        /// <value>
        /// An IXPathNavigable object representing the extra information.
        /// </value>
        /// 
        public IXPathNavigable InfoXml
        {
            get { return _infoXml; }
        }
        private XPathDocument _infoXml;

        /// <summary>
        /// Set the InfoXml for this instance.
        /// </summary>
        /// <remarks>
        /// The info xml is a place where additional xml information can
        /// be stored in an XML item. 
        /// </remarks>
        /// <param name="infoXml"></param>
        public void SetInfoXml(string infoXml)
        {
            Validator.ThrowIfArgumentNull(infoXml, "infoXml", "VocabularyItemInfoXmlNull");

            try
            {
                using (StringReader stringReader = new StringReader(infoXml))
                {
                    _infoXml = new XPathDocument(XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings));
                }
            }
            catch (XmlException)
            {
                //don't want to expose info about XML parse errors here
                //to the outside world as recommended in .NET 
                //documentation
                throw new HealthServiceException(
                    HealthServiceStatusCode.VocabularyLoadError);
            }
        }

        /// <summary>
        /// Retrieves the code item as a string.
        /// </summary>
        /// 
        /// <returns>
        /// The display text associated with this code item. 
        /// </returns>
        /// 
        public override string ToString()
        {
            return _displayText;
        }

        /// <summary>
        /// The <see cref="Vocabulary"/> to which this item belongs.
        /// </summary>
        /// 
        public Vocabulary Vocabulary
        {
            get { return _vocabulary; }
            set
            {
                _vocabulary = value;
                CopyNameFamilyVersionFromVocabulary();
            }
        }
        private Vocabulary _vocabulary;

        private void CopyNameFamilyVersionFromVocabulary()
        {
            if (this.Vocabulary != null)
            {
                this.Family = this.Vocabulary.Family;
                this.VocabularyName = this.Vocabulary.Name;
                this.Version = this.Vocabulary.Version;
            }
            else
            {
                this.Family = null;
                this.VocabularyName = null;
                this.Version = null;
            }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Set the instance values from the specified reader.
        /// </summary>
        /// <param name="reader">The reader</param>
        public void ReadXml(XmlReader reader)
        {
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();

            ParseXml(navigator.SelectSingleNode("item"));
        }

        /// <summary>
        /// Write the instance values to the specified writer.
        /// </summary>
        /// <param name="writer">The writer</param>
        public void WriteXml(XmlWriter writer)
        {
            WriteXmlInternal(null, writer);
        }

        #endregion

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
                if (!String.IsNullOrEmpty(value) && String.IsNullOrEmpty(value.Trim()))
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
                if (!String.IsNullOrEmpty(value) && String.IsNullOrEmpty(value.Trim()))
                {
                    throw Validator.ArgumentException("Version", "WhitespaceOnlyValue");
                }
                _version = value;
            }
        }
        private string _version;
    }
}
