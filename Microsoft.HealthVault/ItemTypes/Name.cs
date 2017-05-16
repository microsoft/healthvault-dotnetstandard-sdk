// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a person's name.
    /// </summary>
    ///
    public class Name : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the  <see cref="Name"/> class with default values.
        /// </summary>
        ///
        public Name()
        {
        }

        /// <summary>
        /// Creates a new instance of the  <see cref="Name"/> class with the specified full name.
        /// </summary>
        ///
        /// <param name="fullName">
        /// The full name.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="fullName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Name(string fullName)
        {
            Full = fullName;
        }

        /// <summary>
        /// Creates a new instance of the  <see cref="Name"/> class with the
        /// specified first, middle, and last name.
        /// </summary>
        ///
        /// <param name="fullName">
        /// The full name.
        /// </param>
        ///
        /// <param name="first">
        /// The first name (given name).
        /// </param>
        ///
        /// <param name="middle">
        /// The middle name.
        /// </param>
        ///
        /// <param name="last">
        /// The last name (surname).
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="fullName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Name(string fullName, string first, string middle, string last)
            : this(fullName)
        {
            First = first;
            Middle = middle;
            Last = last;
        }

        /// <summary>
        /// Creates a new instance of the  <see cref="Name"/> class with the
        /// specified first, middle, and last name and suffix.
        /// </summary>
        ///
        /// <param name="fullName">
        /// The full name.
        /// </param>
        ///
        /// <param name="first">
        /// The first name (given name).
        /// </param>
        ///
        /// <param name="middle">
        /// The middle name.
        /// </param>
        ///
        /// <param name="last">
        /// The last name (surname).
        /// </param>
        ///
        /// <param name="suffix">
        /// The name suffix.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="fullName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Name(
            string fullName,
            string first,
            string middle,
            string last,
            CodableValue suffix)
            : this(fullName, first, middle, last)
        {
            Suffix = suffix;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the name information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _full = navigator.SelectSingleNode("full").Value;

            XPathNavigator titleNav =
                navigator.SelectSingleNode("title");

            if (titleNav != null)
            {
                _title = new CodableValue();
                _title.ParseXml(titleNav);
            }

            XPathNavigator firstNav =
                navigator.SelectSingleNode("first");

            if (firstNav != null)
            {
                _first = firstNav.Value;
            }

            XPathNavigator middleNav =
                navigator.SelectSingleNode("middle");

            if (middleNav != null)
            {
                _middle = middleNav.Value;
            }

            XPathNavigator lastNav =
                navigator.SelectSingleNode("last");

            if (lastNav != null)
            {
                _last = lastNav.Value;
            }

            XPathNavigator suffixNav =
                navigator.SelectSingleNode("suffix");

            if (suffixNav != null)
            {
                _suffix = new CodableValue();
                _suffix.ParseXml(suffixNav);
            }
        }

        /// <summary>
        /// Writes the XML representation of the name into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the name.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the name should be
        /// written.
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
        /// The <see cref="Full"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_full, Resources.FullNotSet);

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("full", _full);

            if (_title != null)
            {
                _title.WriteXml("title", writer);
            }

            if (!string.IsNullOrEmpty(_first))
            {
                writer.WriteElementString("first", _first);
            }

            if (!string.IsNullOrEmpty(_middle))
            {
                writer.WriteElementString("middle", _middle);
            }

            if (!string.IsNullOrEmpty(_last))
            {
                writer.WriteElementString("last", _last);
            }

            if (_suffix != null)
            {
                _suffix.WriteXml("suffix", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b> or empty when setting.
        /// </exception>
        ///
        public string Full
        {
            get { return _full; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Full), Resources.FullMandatory);
                _full = value;
            }
        }

        private string _full;

        /// <summary>
        /// Gets or sets the person's title.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the title.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be stored.
        /// </remarks>
        ///
        public CodableValue Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private CodableValue _title;

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the first name.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the first name should not be stored.
        /// </remarks>
        ///
        public string First
        {
            get { return _first; }
            set { _first = value; }
        }

        private string _first;

        /// <summary>
        /// Gets or sets the person's middle name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the middle name.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the middle name should not be stored.
        /// </remarks>
        ///
        public string Middle
        {
            get { return _middle; }
            set { _middle = value; }
        }

        private string _middle;

        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the last name.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the last name should not be stored.
        /// </remarks>
        ///
        public string Last
        {
            get { return _last; }
            set { _last = value; }
        }

        private string _last;

        /// <summary>
        /// Gets or sets the person's suffix.
        /// </summary>
        ///
        /// <value>
        /// A string representing the suffix.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the suffix should not be stored.
        /// </remarks>
        ///
        public CodableValue Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        private CodableValue _suffix;

        /// <summary>
        /// Gets a string representation of the name.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the name.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = Full;

            if (Full == null)
            {
                result =
                    string.Format(
                        Resources.NameToStringFormat,
                        Title != null ? Title.Text : string.Empty,
                        First != null ? First : string.Empty,
                        Middle != null ? Middle : string.Empty,
                        Last != null ? Last : string.Empty,
                        Suffix != null ? Suffix.Text : string.Empty);
            }

            return result;
        }
    }
}
