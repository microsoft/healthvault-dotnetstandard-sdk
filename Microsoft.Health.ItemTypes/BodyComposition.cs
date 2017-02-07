// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Information about the body composition of the record owner. 
    /// </summary>
    /// 
    public class BodyComposition : HealthRecordItem
    {
        /// <summary>
        /// Creates an instance of information about the body composition of the record owner
        /// with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: % body fat, lean muscle mass.
        /// </remarks>
        /// 
        public BodyComposition()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates an instance of information about the body composition of the record owner
        /// with specified time, measurement name, and value.
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: % body fat, lean muscle mass.
        /// </remarks>
        /// 
        /// <param name="when"> 
        /// The date and time of the measurement. 
        /// </param>
        /// 
        /// <param name="measurementName">
        /// The name of the measurement.
        /// </param>
        /// 
        /// <param name="compositionValue"> 
        /// The value of the measurement. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/>, <paramref name="measurementName"/> or 
        /// <paramref name="compositionValue"/> is <b>null</b>.
        /// </exception>
        /// 
        public BodyComposition(
            ApproximateDateTime when,
            CodableValue measurementName,
            BodyCompositionValue compositionValue)
            : base(TypeId)
        {
            When = when;
            MeasurementName = measurementName;
            Value = compositionValue;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("18adc276-5144-4e7e-bf6c-e56d8250adf8");

        /// <summary>
        /// Populates this <see cref="BodyComposition"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the body composition data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "body-composition" node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("body-composition");

            Validator.ThrowInvalidIfNull(itemNav, "BodyCompositionUnexpectedNode");

            // when (approxi-date-time, mandatory) 
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            _measurementName = new CodableValue();
            _measurementName.ParseXml(itemNav.SelectSingleNode("measurement-name"));

            // value (BodyCompositionValue, mandatory)
            _value = new BodyCompositionValue();
            _value.ParseXml(itemNav.SelectSingleNode("value"));

            // measurement-method (codable value )
            _measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // site 
            _site =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "site");

        }

        /// <summary>
        /// Writes the body composition data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the body composition data to. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/>, <see cref="MeasurementName"/> or <see cref="Value"/>
        /// is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "WhenNullValue");
            Validator.ThrowSerializationIfNull(_measurementName, "BodyCompositionMeasurementNameNotSet");
            Validator.ThrowSerializationIfNull(_value, "BodyCompositionValueNotSet");

            // <body-composition>
            writer.WriteStartElement("body-composition");

            // <when>
            _when.WriteXml("when", writer);

            // <measurement-name>
            _measurementName.WriteXml("measurement-name", writer);

            // <value>
            _value.WriteXml("value", writer);

            // <measurement-method>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "measurement-method",
                _measurementMethod);

            // <site>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "site",
                _site);

            // </body-composition>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of this measurement.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date 
        /// and time.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public ApproximateDateTime When
        {
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets the name of this measurement.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the name
        /// of this measurement.
        /// </value>
        /// 
        /// <remarks>
        /// Examples: Body fat, lean muscle. The preferred vocabulary is 
        /// "body-composition-measurement-names".
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue MeasurementName
        {
            get { return _measurementName; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "MeasurementName", "BodyCompositionMeasurementNameNullValue");
                _measurementName = value;
            }
        }
        private CodableValue _measurementName;

        /// <summary>
        /// Gets or sets the value of this measurement.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="BodyCompositionValue"/> instance representing the value of 
        /// this measurement.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public BodyCompositionValue Value
        {
            get { return _value; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "BodyCompositionMeasurementNameNullValue");
                _value = value;
            }
        }
        private BodyCompositionValue _value;

        /// <summary>
        /// Gets or sets the technique used to obtain the measurement.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the technique used 
        /// to obtain the measurement. 
        /// </value>
        /// 
        /// <remarks>
        /// Examples: Bioelectrical impedance, DXA, Skinfold (calipers). If the measurement method 
        /// is missing, the value should be set to <b>null</b>. The preferred vocabulary is
        /// "body-composition-measurement-methods".
        /// </remarks>
        /// 
        public CodableValue MeasurementMethod
        {
            get { return _measurementMethod; }
            set { _measurementMethod = value; }
        }
        private CodableValue _measurementMethod;

        /// <summary>
        /// Gets or sets the body part that is the subject of the measurement.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the site 
        /// </value>
        /// 
        /// <remarks>
        /// Examples: Left arm, head, torso. If the site is absent, the measurement is for 
        /// the whole body. The preferred vocabulary is "body-composition-sites".
        /// </remarks>
        /// 
        public CodableValue Site
        {
            get { return _site; }
            set { _site = value; }
        }
        private CodableValue _site;

        /// <summary>
        /// Gets the representation of a body composition instance.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the body composition item.
        /// </returns>
        /// 
        public override string ToString()
        {
            List<string> elements = new List<string>();
            
            if (_measurementName != null)
            {
                elements.Add(_measurementName.ToString());
            }

            if (_value != null)
            {
                string valueString = _value.ToString();
                if (!String.IsNullOrEmpty(valueString))
                {
                    elements.Add(valueString);
                }
            }

            if (_measurementMethod != null)
            {
                elements.Add(_measurementMethod.ToString());
            }

            string separator =
                ResourceRetriever.GetResourceString("ListSeparator");

            return String.Join(separator, elements.ToArray());
        }
    }
}
