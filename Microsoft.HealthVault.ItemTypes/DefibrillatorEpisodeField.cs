// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information containing name value pair of defibrillator episode property.
    /// </summary>
    public class DefibrillatorEpisodeField : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DefibrillatorEpisodeField"/> class with default values.
        /// </summary>
        ///
        public DefibrillatorEpisodeField()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DefibrillatorEpisodeField"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// Represents the name of the name/value pair.
        /// </param>
        ///
        /// <param name="value">
        /// Represents the value of the name/value pair.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        /// 
        public DefibrillatorEpisodeField(CodableValue name, CodableValue value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Populates this <see cref="DefibrillatorEpisodeField"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the DefibrillatorEpisodeField data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("field-name"));

            _value = new CodableValue();
            _value.ParseXml(navigator.SelectSingleNode("field-value"));
        }

        /// <summary>
        /// Writes the XML representation of the  <see cref="DefibrillatorEpisodeField"/> into 
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the defibrillator episode field item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the DefibrillatorEpisodeField should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If name or value is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "WriteXmlEmptyNodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, "DefibrillatorEpisodeFieldNameNullValue");
            Validator.ThrowSerializationIfNull(_value, "DefibrillatorEpisodeFieldValueNullValue");

            writer.WriteStartElement(nodeName);
            _name.WriteXml("field-name", writer);
            _value.WriteXml("field-value", writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Name of the defibrillator episode property.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Name
        {
            get
            {
                return _name;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "DefibrillatorEpisodeFieldNameNullValue");
                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Value of the defibrillator episode property.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Value
        {
            get
            {
                return _value;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "DefibrillatorEpisodeFieldValueNullValue");
                _value = value;
            }
        }

        private CodableValue _value;
    }
}