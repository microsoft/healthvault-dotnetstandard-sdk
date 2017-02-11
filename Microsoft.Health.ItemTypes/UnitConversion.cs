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
    /// Unit conversion representation.
    /// </summary>
    ///
    public class UnitConversion : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UnitConversion"/> class with default values.
        /// </summary>
        ///
        public UnitConversion()
        {
        }
        
        /// <summary>
        /// Populates this <see cref="UnitConversion"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the UnitConversion data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);
            
            _multiplier = XPathHelper.GetOptNavValueAsDouble(navigator, "multiplier");
            _offset = XPathHelper.GetOptNavValueAsDouble(navigator, "offset");
        }
        
        /// <summary>
        /// Writes the XML representation of the UnitConversion into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the unit conversion.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the UnitConversion should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");           
            
            writer.WriteStartElement("unit-conversion");
            
            XmlWriterHelper.WriteOptDouble(writer, "multiplier", _multiplier);
            XmlWriterHelper.WriteOptDouble(writer, "offset", _offset);
            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets the multiplier.
        /// </summary>
        /// 
        public double? Multiplier
        {
            get
            {
                return _multiplier;
            }
            
            set
            {
                _multiplier = value;
            }
        }
        
        private double? _multiplier;
        
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// 
        public double? Offset
        {
            get
            {
                return _offset;
            }
            
            set
            {
                _offset = value;
            }
        }
        
        private double? _offset;
        
        /// <summary>
        /// Gets a string representation of the UnitConversion.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the UnitConversion.
        /// </returns>
        ///
        public override string ToString()
        {
            return String.Format(
                CultureInfo.CurrentUICulture,
                ResourceRetriever.GetResourceString("UnitConversionFormat"),
                _multiplier,
                _offset);
        }

        /// <summary>
        /// Convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns></returns>
        public double Convert(double value)
        {
            if (_multiplier.HasValue)
            {
                value = value * _multiplier.Value;
            }

            if (_offset.HasValue)
            {
                value = value + _offset.Value;
            }

            return value;
        }

        /// <summary>
        /// Reverse convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns></returns>
        public double ReverseConvert(double value)
        {
            if (_offset.HasValue)
            {
                value = value - _offset.Value;
            }

            if (_multiplier.HasValue)
            {
                value = value / _multiplier.Value;
            }

            return value;
        }
    }
}
