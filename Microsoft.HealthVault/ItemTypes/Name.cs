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
            this.Full = fullName;
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
            this.First = first;
            this.Middle = middle;
            this.Last = last;
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
            this.Suffix = suffix;
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

            this.full = navigator.SelectSingleNode("full").Value;

            XPathNavigator titleNav =
                navigator.SelectSingleNode("title");

            if (titleNav != null)
            {
                this.title = new CodableValue();
                this.title.ParseXml(titleNav);
            }

            XPathNavigator firstNav =
                navigator.SelectSingleNode("first");

            if (firstNav != null)
            {
                this.first = firstNav.Value;
            }

            XPathNavigator middleNav =
                navigator.SelectSingleNode("middle");

            if (middleNav != null)
            {
                this.middle = middleNav.Value;
            }

            XPathNavigator lastNav =
                navigator.SelectSingleNode("last");

            if (lastNav != null)
            {
                this.last = lastNav.Value;
            }

            XPathNavigator suffixNav =
                navigator.SelectSingleNode("suffix");

            if (suffixNav != null)
            {
                this.suffix = new CodableValue();
                this.suffix.ParseXml(suffixNav);
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
            Validator.ThrowSerializationIfNull(this.full, Resources.FullNotSet);

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("full", this.full);

            if (this.title != null)
            {
                this.title.WriteXml("title", writer);
            }

            if (!string.IsNullOrEmpty(this.first))
            {
                writer.WriteElementString("first", this.first);
            }

            if (!string.IsNullOrEmpty(this.middle))
            {
                writer.WriteElementString("middle", this.middle);
            }

            if (!string.IsNullOrEmpty(this.last))
            {
                writer.WriteElementString("last", this.last);
            }

            if (this.suffix != null)
            {
                this.suffix.WriteXml("suffix", writer);
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
            get { return this.full; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Full), Resources.FullMandatory);
                this.full = value;
            }
        }

        private string full;

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
            get { return this.title; }
            set { this.title = value; }
        }

        private CodableValue title;

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
            get { return this.first; }
            set { this.first = value; }
        }

        private string first;

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
            get { return this.middle; }
            set { this.middle = value; }
        }

        private string middle;

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
            get { return this.last; }
            set { this.last = value; }
        }

        private string last;

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
            get { return this.suffix; }
            set { this.suffix = value; }
        }

        private CodableValue suffix;

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
            string result = this.Full;

            if (this.Full == null)
            {
                result =
                    string.Format(
                        Resources.NameToStringFormat,
                        this.Title != null ? this.Title.Text : string.Empty,
                        this.First != null ? this.First : string.Empty,
                        this.Middle != null ? this.Middle : string.Empty,
                        this.Last != null ? this.Last : string.Empty,
                        this.Suffix != null ? this.Suffix.Text : string.Empty);
            }

            return result;
        }
    }
}
