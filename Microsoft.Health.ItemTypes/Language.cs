// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents the language that a person speaks.
    /// </summary>
    /// 
    public class Language : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Language"/> class with empty values.
        /// </summary>
        /// 
        public Language()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Language"/> class with 
        /// the specified spoken language.
        /// </summary>
        /// 
        /// <param name="spokenLanguage">
        /// The spoken language.
        /// </param>
        /// 
        public Language(CodableValue spokenLanguage)
        {
            this.SpokenLanguage = spokenLanguage;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="Language"/> class with 
        /// the specified spoken language and primary designator.
        /// </summary>
        /// 
        /// <param name="spokenLanguage">
        /// The spoken language.
        /// </param>
        /// 
        /// <param name="isPrimary">
        /// <b>true</b> if <paramref name="spokenLanguage"/> is the primary 
        /// language for the person; otherwise, <b>false</b>.
        /// </param>
        /// 
        public Language(CodableValue spokenLanguage, bool isPrimary)
        {
            this.SpokenLanguage = spokenLanguage;
            this.IsPrimary = isPrimary;
        }
        
        /// <summary> 
        /// Populates the data for the language from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the language.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            _language.Clear();

            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator languageNav = navigator.SelectSingleNode("language");
            if (languageNav != null)
            {
                _language.ParseXml(languageNav);
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");
            if (isPrimaryNav != null)
            {
                _isPrimary = isPrimaryNav.ValueAsBoolean;
            }
        }

        /// <summary> 
        /// Writes the language to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the language.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the language to.
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // null indicates uninitialized
            if (_language.Text != null)
            {
                writer.WriteStartElement(nodeName);

                _language.WriteXml("language", writer);

                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool(_isPrimary));

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Gets or sets the spoken language.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> representing the language.
        /// </value>
        /// 
        public CodableValue SpokenLanguage
        {
            get { return _language; }
            set { _language = value; }
        }
        private CodableValue _language = new CodableValue();

        /// <summary>
        /// Gets or sets a value indicating whether the language is the 
        /// person's primary language.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the language is the person's primary language; 
        /// otherwise, <b>false</b>. The default is <b>true</b>.
        /// </value>
        /// 
        public bool IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }
        private bool _isPrimary = true;
    }

}
