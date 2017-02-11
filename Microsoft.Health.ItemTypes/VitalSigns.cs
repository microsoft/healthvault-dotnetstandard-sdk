// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates vital signs.
    /// </summary>
    /// 
    public class VitalSigns : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VitalSigns"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public VitalSigns()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VitalSigns"/> class with 
        /// the specified date.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the vital signs were taken.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public VitalSigns(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        /// 
        public new static readonly Guid TypeId =
            new Guid("73822612-C15F-4B49-9E65-6AF369E55C65");

        /// <summary>
        /// Populates this vital signs instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the vital signs data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a vital signs node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("vital-signs");

            Validator.ThrowInvalidIfNull(itemNav, "VitalSignsUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));


            XPathNodeIterator vitalSignsResultsIterator =
                itemNav.Select("vital-signs-results");

            foreach (XPathNavigator vitalSignsResultsNav in vitalSignsResultsIterator)
            {
                VitalSignsResultType vitalSignResult = new VitalSignsResultType();
                vitalSignResult.ParseXml(vitalSignsResultsNav);
                _vitalSignsResults.Add(vitalSignResult);
            } 
            
            // <site>
            _site =
                XPathHelper.GetOptNavValue(itemNav, "site");
            
            // <position>
            _position =
                XPathHelper.GetOptNavValue(itemNav, "position");

        }

        /// <summary>
        /// Writes the vital signs data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the vital signs data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "VitalSignsWhenNotSet");

            // <vital-signs>
            writer.WriteStartElement("vital-signs");

            // <when>
            _when.WriteXml("when", writer);

            foreach (VitalSignsResultType vitalSignResult in _vitalSignsResults)
            {
                vitalSignResult.WriteXml("vital-signs-results", writer);
            }

            // <site>
            XmlWriterHelper.WriteOptString(
                writer,
                "site",
                _site);

            // <position>
            XmlWriterHelper.WriteOptString(
                writer,
                "position",
                _position);

            // </vital-signs>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the vital signs were taken.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the 
        /// date. The default value is the current year, month, and day.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public HealthServiceDateTime When
        {
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the vital sign results.
        /// </summary>
        /// 
        /// <value>
        /// A collection of vital sign results.
        /// </value>
        /// 
        public Collection<VitalSignsResultType> VitalSignsResults
        {
            get { return _vitalSignsResults; }
        }
        private Collection<VitalSignsResultType> _vitalSignsResults =
            new Collection<VitalSignsResultType>();

        /// <summary>
        /// Gets or sets the site for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the site.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the site should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Site
        {
            get { return _site; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Site");
                _site = value;
            }
        }
        private string _site;
        
        /// <summary>
        /// Gets or sets the position for the vital signs.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the position.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the position should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Position
        {
            get { return _position; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Position");
                _position = value;
            }
        }
        private string _position;

        /// <summary>
        /// Gets a string representation of the vital signs results.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the vital signs results.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            if (VitalSignsResults.Count > 0)
            {
                for (int index = 0; index < VitalSignsResults.Count; ++index)
                {
                    if (index > 0)
                    {
                        result.Append(
                            ResourceRetriever.GetResourceString(
                                "ListSeparator"));
                    }
                    
                    VitalSignsResultType vitalSign = VitalSignsResults[index];
                    result.Append(vitalSign.ToString());
                }
            }
            else
            {
                result.Append(When.ToString());
            }
            return result.ToString();
        }
    }
}
