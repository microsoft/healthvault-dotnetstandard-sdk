// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// Represents an item in the HealthVault <see cref="Vocabulary"/>.
    /// </summary>
    ///
    public class VocabularyItem : ItemBase, IXmlSerializable
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
            DisplayText = displayText;
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
            DisplayText = displayText;
            AbbreviationText = abbreviationText;
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

            Value = navigator.SelectSingleNode("code-value").Value;

            DisplayText = XPathHelper.GetOptNavValue(navigator, "display-text");

            // preserve previous behavior where items without abbreviations have an empty string...
            string abbreviationText = XPathHelper.GetOptNavValue(navigator, "abbreviation-text");
            if (abbreviationText != null)
            {
                AbbreviationText = abbreviationText;
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
                    // don't want to expose info about XML parse errors here
                    // to the outside world as recommended in .NET
                    // documentation
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
        /// <exception cref="ThingSerializationException">
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

            XmlWriterHelper.WriteOptString(writer, "display-text", DisplayText);
            XmlWriterHelper.WriteOptString(writer, "abbreviation-text", AbbreviationText);

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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Value"/> or <see cref="VocabularyName"/> property
        /// is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            if (string.IsNullOrEmpty(_value))
            {
                throw new ThingSerializationException(Resources.ValueNotSet);
            }

            if (string.IsNullOrEmpty(_vocabName))
            {
                throw new ThingSerializationException(Resources.NameNotSet);
            }

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
        /// Gets a localized display text.
        /// </summary>
        ///
        /// <value>
        /// A string representing the display text.
        /// </value>
        ///
        public string DisplayText { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a localized and abbreviated representation
        /// of the display text.
        /// </summary>
        ///
        /// <value>
        /// A string representing the abbreviation text.
        /// </value>
        ///
        public string AbbreviationText { get; private set; } = string.Empty;

        /// <summary>
        /// Gets any extra information associated with the code item.
        /// </summary>
        ///
        /// <value>
        /// An IXPathNavigable object representing the extra information.
        /// </value>
        ///
        public IXPathNavigable InfoXml => _infoXml;

        private XPathDocument _infoXml;

        /// <summary>
        /// Set the InfoXml for this instance.
        /// </summary>
        /// <remarks>
        /// The info xml is a place where additional xml information can
        /// be stored in an XML item.
        /// </remarks>
        /// <param name="infoXml"> the info XML for this item</param>
        public void SetInfoXml(string infoXml)
        {
            Validator.ThrowIfArgumentNull(infoXml, nameof(infoXml), Resources.VocabularyItemInfoXmlNull);

            try
            {
                using (StringReader stringReader = new StringReader(infoXml))
                {
                    _infoXml = new XPathDocument(XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings));
                }
            }
            catch (XmlException)
            {
                // don't want to expose info about XML parse errors here
                // to the outside world as recommended in .NET
                // documentation
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
            return DisplayText;
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
            if (Vocabulary != null)
            {
                Family = Vocabulary.Family;
                VocabularyName = Vocabulary.Name;
                Version = Vocabulary.Version;
            }
            else
            {
                Family = null;
                VocabularyName = null;
                Version = null;
            }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema</returns>
        public XmlSchema GetSchema()
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
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(Resources.WhitespaceOnlyValue, nameof(Family));
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
                    throw new ArgumentException(Resources.WhitespaceOnlyValue, nameof(Version));
                }

                _version = value;
            }
        }

        private string _version;
    }
}
