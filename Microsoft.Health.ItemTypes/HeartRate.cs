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
    /// A measurement of the record owner's heart rate.
    /// </summary>
    /// 
    public class HeartRate : HealthRecordItem
    {
        /// <summary>
        /// Creates an instance of <see cref="HeartRate"/> with default values.
        /// </summary>
        /// 
        public HeartRate()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="HeartRate"/> with specified parameters. 
        /// </summary>
        /// 
        /// <param name="when">The date and time the measurement was taken.</param>
        /// 
        /// <param name="value">The heart rate in beats per minute (BPM).</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>. 
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> is less than zero.
        /// </exception>
        /// 
        public HeartRate(HealthServiceDateTime when, int value)
            : base(TypeId)
        {
            When = when;
            Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the heart rate item type.
        /// </summary>
        public new static readonly Guid TypeId =
            new Guid("b81eb4a6-6eac-4292-ae93-3872d6870994");

        /// <summary>
        /// Populates this <see cref="HeartRate"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the heart rate data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "heart-rate" node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("heart-rate");

            Validator.ThrowInvalidIfNull(itemNav, "HeartRateUnexpectedNode");

            // when
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // value
            _value = itemNav.SelectSingleNode("value").ValueAsInt;

            // measurement-method
            _measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // measurement-condition
            _measurementConditions =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-conditions");

            // measurement-flags
            _measurementFlags =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-flags");

        }

        /// <summary>
        /// Writes the heart rate data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the heart rate data to. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "WhenNullValue");

            // <heart-rate>
            writer.WriteStartElement("heart-rate");

            // <when>
            _when.WriteXml("when", writer);

            // <value>
            writer.WriteElementString(
                "value",
                _value.ToString(CultureInfo.InvariantCulture));

            // <measurement-method>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "measurement-method",
                _measurementMethod);

            // <measurement-conditions>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "measurement-conditions",
                _measurementConditions);

            // <measurement-flags>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "measurement-flags",
                _measurementFlags);

            // </heart-rate>
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
        public HealthServiceDateTime When
        {
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when;

        /// <summary>
        /// Gets or sets the heart rate in beats per minutes (BPM). 
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the heart rate in beats per minute(BPM).
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="value"/> parameter is less than zero.
        /// </exception>
        /// 
        public int Value
        {
            get { return _value; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "Value",
                    "HeartRateValueNegative");
                _value = value;
            }
        }
        private int _value;


        /// <summary>
        /// Gets or sets the technique used to obtain the measurement. 
        /// </summary>
        /// 
        /// <value>
        /// The technique used to obtain the measurement.
        /// </value>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>. The preferred vocabulary 
        /// for this value is "heart-rate-measurement-method".
        /// </remarks>
        /// 
        public CodableValue MeasurementMethod
        {
            get { return _measurementMethod; }
            set { _measurementMethod = value; }
        }
        private CodableValue _measurementMethod;

        /// <summary>
        /// Gets or sets the conditions under which the heart rate was measured.
        /// </summary>
        /// 
        /// <value>
        /// The conditions under which the heart rate was measured. 
        /// </value>
        /// 
        /// <remarks>
        /// Examples: Resting, Active, Morning, Evening. If the value is not known, it will be set to <b>null</b>. The preferred vocabulary 
        /// for this value is "heart-rate-measurement-conditions".
        /// </remarks>
        /// 
        public CodableValue MeasurementConditions
        {
            get { return _measurementConditions; }
            set { _measurementConditions = value; }
        }
        private CodableValue _measurementConditions;

        /// <summary>
        /// Gets or sets the additional information about the measurement. 
        /// </summary>
        /// 
        /// <value>
        /// The additional information about the measurement
        /// </value>
        /// 
        /// <remarks>
        /// Examples: Incomplete measurement, irregular heartbeat, triple scan. The preferred
        /// vocabulary for this value is "heart-rate-measurement-flags". If the value is not 
        /// known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue MeasurementFlags
        {
            get { return _measurementFlags; }
            set { _measurementFlags = value; }
        }
        private CodableValue _measurementFlags;

        /// <summary>
        /// Gets the description of a heart rate instance. 
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the heart rate item. 
        /// </returns>
        /// 
        public override string ToString()
        {
            return 
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "HeartRateToStringFormat"),
                    _value.ToString(CultureInfo.InvariantCulture));
        }

    }
}
