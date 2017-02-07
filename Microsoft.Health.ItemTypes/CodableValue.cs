// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents the value and code of an entry in the HealthVault vocabulary
    /// store.
    /// </summary>
    /// 
    public class CodableValue : HealthRecordItemData,
        IList<CodedValue>, ICollection<CodedValue>, IEnumerable<CodedValue>
    {
        /// <summary>
        /// Constructs a CodableValue with empty values.
        /// </summary>
        /// 
        public CodableValue()
        {
        }

        /// <summary>
        /// Constructs a CodableValue with an initial value for the Text.
        /// </summary>
        /// 
        /// <param name="text">
        /// The text value of the codable value.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="text"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public CodableValue(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Constructs a CodableValue with an initial value for the Text
        /// and the specified code.
        /// </summary>
        /// 
        /// <param name="text">
        /// The text value of the codable value.
        /// </param>
        /// 
        /// <param name="code">
        /// The code representation of the text value.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="text"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue(string text, CodedValue code)
        {
            this.Text = text;

            if (code != null)
            {
                _codes.Add(code);
            }
        }

        /// <summary>
        /// Constructs a CodableValue based on display value and
        /// a <see cref="VocabularyItem"/>. 
        /// </summary>
        /// 
        /// <param name="text">
        /// The text value of the codable value.
        /// </param>
        /// 
        /// <param name="item">
        /// The <see cref="VocabularyItem"/>. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="text"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue(string text, VocabularyItem item)
        {
            this.Text = text;

            if (item != null)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CodableValue"/> class 
        /// with the specified text, code value, vocabulary name, family, and version.
        /// </summary>
        /// 
        /// <param name="text">
        /// The text value of the codable value.
        /// </param>
        /// 
        /// <param name="code">
        /// The code representation of the text value.
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
        /// The <paramref name="text"/> or <paramref name="code"/> or 
        /// <paramref name="vocabularyName"/> parameter is <b>null</b> or empty.
        /// </exception>
        public CodableValue(
            string text,
            string code,
            string vocabularyName,
            string family,
            string version)
        {
            CodedValue codedValue = new CodedValue(code, vocabularyName, family, version);
            this.Text = text;
            _codes.Add(codedValue);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CodableValue"/> class 
        /// with the specified text, code value, and vocabulary key.
        /// </summary>
        /// 
        /// <param name="text">
        /// The text value of the codable value.
        /// </param>
        /// 
        /// <param name="code">
        /// The code representation of the text value.
        /// </param>
        /// 
        /// <param name="key">
        /// key for identifying a Vocabulary in the HealthLexicon.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="text"/> or <paramref name="code"/> or <paramref name="key"/>
        /// parameter is <b>null</b>.
        /// </exception>
        public CodableValue(
            string text,
            string code,
            VocabularyKey key)
        {
            Validator.ThrowIfArgumentNull(key, "key", "VocabularyKeyMandatory");

            CodedValue codedValue = new CodedValue(code, key);
            this.Text = text;
            _codes.Add(codedValue);
        }

        #region Interface methods

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the element to get or set
        /// </param>
        /// 
        /// <returns>
        /// The code item at the specified index.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> parameter is out of range.
        /// </exception>
        /// 
        public CodedValue this[int index]
        {
            get { return Codes[index]; }
            set { Codes[index] = value; }
        }

        /// <summary>
        /// Determines the index of a specific item
        /// </summary>
        /// 
        /// <param name="item">
        /// The <see cref="CodedValue"/> to find the index of.
        /// </param>
        /// 
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        /// 
        public int IndexOf(CodedValue item)
        {
            return Codes.IndexOf(item);
        }

        /// <summary>
        /// Inserts an code item at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// 
        /// <param name="item">
        /// The object to insert.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> parameter is out of range.
        /// </exception>
        /// 
        public void Insert(int index, CodedValue item)
        {
            Codes.Insert(index, item);
        }

        /// <summary>
        /// Removes a code item at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> parameter is out of range.
        /// </exception>
        /// 
        public void RemoveAt(int index)
        {
            Codes.RemoveAt(index);
        }

        /// <summary>
        /// Gets the number of code items
        /// </summary>
        /// 
        public int Count
        {
            get { return Codes.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the code item list is read-only.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the list is read-only; otherwise, <b>false</b>.
        /// </value>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an item to the code list. 
        /// </summary>
        /// 
        /// <param name="item">
        /// The code item to add.
        /// </param>
        /// 
        public void Add(CodedValue item)
        {
            Codes.Add(item);
        }

        /// <summary>
        /// Encodes a <see cref="VocabularyItem"/> as a CodedValue and
        /// adds it to the list of coded values.
        /// </summary>
        /// 
        /// <param name="item">
        /// The <see cref="VocabularyItem"/> to use. 
        /// </param>
        /// 
        public void Add(VocabularyItem item)
        {
            CodedValue codedValue = 
                new CodedValue(item.Value,
                               item.VocabularyName,
                               item.Family,
                               item.Version);

            if (_text == null)
            {
                _text = item.DisplayText;
            }
            Codes.Add(codedValue);
        }

        /// <summary>
        /// Removes all items from the code item list
        /// </summary>
        /// 
        public void Clear()
        {
            this._text = null;
            Codes.Clear();
        }

        /// <summary>
        /// Gets a value indicating whether the code item list contains a 
        /// specific code.
        /// </summary>
        /// 
        /// <param name="item">
        /// The code item to search for in the list.
        /// </param>
        /// 
        /// <returns>
        /// <b>true</b> if the item is in the collection; otherwise, <b>false</b>.
        /// </returns>
        /// 
        public bool Contains(CodedValue item)
        {
            return Codes.Contains(item);
        }
        /// <summary>
        /// Copies the elements of the code item list to an array, starting at 
        /// a particular array index.
        /// </summary>
        /// 
        /// <param name="array">
        /// The array to copy the elements to.
        /// </param>
        /// 
        /// <param name="arrayIndex">
        /// The array location at which to begin copying.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="arrayIndex"/> parameter is less than 0.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="array"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="array"/> parameter is multidimensional, 
        /// or the <paramref name="arrayIndex"/> value is equal to or greater 
        /// than the length of the array, or the number of elements in the 
        /// source collection is greater than the available space from <paramref name="arrayIndex"/> 
        /// to the end of the destination array, or a type cannot be cast 
        /// automatically to the type of the destination array.
        /// </exception>
        /// 
        public void CopyTo(CodedValue[] array, int arrayIndex)
        {
            Codes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of this code item from the code item
        /// list.
        /// </summary>
        /// 
        /// <param name="item">
        /// The <see cref="CodedValue"/> object to remove.
        /// </param>
        /// 
        /// <returns>
        /// <b>true</b> if removal succeeded; otherwise, <b>false</b>.
        /// </returns>
        /// 
        public bool Remove(CodedValue item)
        {
            return Codes.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the code list.
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Codes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator.
        /// </returns>
        /// 
        public IEnumerator<CodedValue> GetEnumerator()
        {
            return Codes.GetEnumerator();
        }
        #endregion

        /// <summary> 
        /// Populates the data for the vocabulary entry from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the vocabulary entry.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            Clear();
            _text = navigator.SelectSingleNode("text").Value;

            // optional code
            try
            {
                XPathNodeIterator codeIterator = navigator.Select("code");

                foreach (XPathNavigator code in codeIterator)
                {
                    CodedValue codedValue = new CodedValue();
                    codedValue.ParseXml(code);
                    _codes.Add(codedValue);
                }
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary> 
        /// Writes the vocabulary entry to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the vocabulary entry.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the vocabulary entry to.
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
        /// The <see cref="Text"/>, <see cref="CodedValue.Value"/>, or
        /// <see cref="CodedValue.Family"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("text", _text);

            foreach (CodedValue codedValue in _codes)
            {
                if (codedValue != null)
                {
                    codedValue.WriteXml("code", writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the value of the codable value.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the value.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace when setting.
        /// </exception>
        /// 
        public string Text
        {
            get { return _text; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Text");
                Validator.ThrowIfStringIsWhitespace(value, "Text");
                _text = value;
            }
        }
        private string _text;

        /// <summary>
        /// Gets the string representation of a codable value.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the value.
        /// </returns>
        /// 
        public override string ToString()
        {
            if (_text == null)
            {
                // not initialized
                return String.Empty;
            }
            else
            {
                return this.Text;
            }
        }

        /// <summary>
        /// Gets coded versions of the value found in the HealthVault 
        /// Vocabulary store.
        /// </summary>
        /// 
        /// <value>
        /// An IList instance representing the value versions.
        /// </value>
        /// 
        /// <remarks>
        /// To have a coded representation of the <see cref="Text"/> property,
        /// add <see cref="CodedValue"/> instances to the returned collection.
        /// </remarks>
        /// 
        internal IList<CodedValue> Codes
        {
            get { return _codes; }
        }
        private List<CodedValue> _codes = new List<CodedValue>();
    }
}
