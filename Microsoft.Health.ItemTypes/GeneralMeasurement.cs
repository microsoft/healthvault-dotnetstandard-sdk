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
    /// A coded measurement and a display representation.
    /// </summary>
    /// 
    /// <remarks>
    /// Examples include 30 cc, 500 mg, 15 liters, 30 inches, etc.
    /// </remarks>
    /// 
    public class GeneralMeasurement : HealthRecordItemData
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="GeneralMeasurement"/> 
        /// class with default values.
        /// </summary>
        /// 
        public GeneralMeasurement()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="GeneralMeasurement"/> 
        /// class with display parameter.
        /// </summary>
        /// 
        /// <param name="display"> It is a sentence to display for the measurement
        /// by the application. </param>
        /// 
        /// <exception cref="ArgumentException">
        /// 
        /// If <paramref name="display"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public GeneralMeasurement(string display) 
        {
            Display = display;
        }

        /// <summary>
        /// Populates this <see cref="GeneralMeasurement"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML to get the general measurement data from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // display
            _display = navigator.SelectSingleNode("display").Value;

            // structured
            XPathNodeIterator structuredIterator = navigator.Select("structured");

            _structured = new Collection<StructuredMeasurement>();
            foreach (XPathNavigator structuredNav in structuredIterator)
            {
                StructuredMeasurement _structuredMeasurement = new StructuredMeasurement();
                _structuredMeasurement.ParseXml(structuredNav);
                _structured.Add(_structuredMeasurement);
            }
        }

        /// <summary>
        /// Writes the general measurement data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="nodeName">
        /// The name of the node to write XML.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the concern data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Display"/> is <b>null</b>.
        /// </exception> 
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_display, "GeneralMeasurementDisplayNotSet");

            // <general-measurement>
            writer.WriteStartElement(nodeName);

            // display
            writer.WriteElementString("display", _display);

            // structured
            for (int index = 0; index < _structured.Count; ++index)
            {
                _structured[index].WriteXml("structured", writer);
            }

            // </general-measurement>
            writer.WriteEndElement();
         
        }

        /// <summary>
        /// Gets or sets a user readable string to display for the measurement 
        /// by the applications.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> parameter is <b>null</b>, empty, or contains only 
        /// whitespace on set.
        /// </exception>
        /// 
        public string Display
        {
            get { return _display;}
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Display");
                Validator.ThrowIfStringIsWhitespace(value, "Display");
                _display = value;
            }
        }
        private string _display;

        /// <summary>
        /// Gets the coded values of the measurements.  
        /// </summary>
        /// 
        /// <remarks>
        /// Applications typically use this for calculations, charting, or graphing. 
        /// </remarks>
        /// 
        public Collection<StructuredMeasurement> Structured
        {
            get { return _structured;}
        }
        private Collection<StructuredMeasurement> _structured =
            new Collection<StructuredMeasurement>();

        /// <summary>
        /// Gets a string representation of the general measurement item.
        /// </summary> 
        /// 
        /// <returns>
        /// A string representation of the general measurement item.
        /// </returns>
        ///
        public override string ToString()
        {
            return _display;
        }
    }
}
