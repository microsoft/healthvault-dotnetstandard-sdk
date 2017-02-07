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
    /// Health record item type that represents a blood glucose measurement.
    /// </summary>
    /// 
    public class BloodGlucose : HealthRecordItem
    {
        /// <summary>
        /// Constructs a new BloodGlucose health record item instance with 
        /// default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item isn't added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public BloodGlucose()
            : base(TypeId)
        {
        }
 
        /// <summary>
        /// Constructs the new blood glucose health record item instance
        /// specifying the mandatory values.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the blood glucose reading was take.
        /// </param>
        /// 
        /// <param name="value">
        /// The blood glucose value of the reading.
        /// </param>
        /// 
        /// <param name="glucoseMeasurementType">
        /// How the glucose was measured; whole blood, plasma, etc. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public BloodGlucose(
            HealthServiceDateTime when,
            BloodGlucoseMeasurement value,
            CodableValue glucoseMeasurementType)
            : base(TypeId)
        {
            this.When = when;
            this.Value = value;
            this.GlucoseMeasurementType = glucoseMeasurementType;
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("879e7c04-4e8a-4707-9ad3-b054df467ce4");

        /// <summary>
        /// Populates this BloodGlucose instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the blood glucose data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a blood-glucose node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator bgNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "blood-glucose");

            Validator.ThrowInvalidIfNull(bgNav, "BGUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(bgNav.SelectSingleNode("when"));

            _value = new BloodGlucoseMeasurement();
            _value.ParseXml(bgNav.SelectSingleNode("value"));

            _glucoseMeasurementType = new CodableValue();
            _glucoseMeasurementType.ParseXml(
                bgNav.SelectSingleNode("glucose-measurement-type"));

            _outsideOperatingTemp =
                XPathHelper.GetOptNavValueAsBool(
                    bgNav, 
                    "outside-operating-temp");

            _isControlTest =
                XPathHelper.GetOptNavValueAsBool(
                    bgNav,
                    "is-control-test");

            XPathNavigator normalcyNav =
                bgNav.SelectSingleNode("normalcy");

            if (normalcyNav != null)
            {
                _normalcyValue = normalcyNav.ValueAsInt;
                if (_normalcyValue < (int)Normalcy.WellBelowNormal ||
                    _normalcyValue > (int)Normalcy.WellAboveNormal)
                {
                    _normalcy = Normalcy.Unknown;
                }
                else
                {
                    _normalcy = (Normalcy)_normalcyValue;
                }
            }


            _measurementContext =
                XPathHelper.GetOptNavValue<CodableValue>(
                    bgNav,
                    "measurement-context");

        }

        /// <summary>
        /// Writes the blood glucose data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the blood glucose data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Value"/> or <see cref="GlucoseMeasurementType"/> 
        /// parameter has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_value, "BGValueNotSet");
            Validator.ThrowSerializationIfNull(_glucoseMeasurementType, "BGMeasurementTypeNotSet");

            // <blood-glucose>
            writer.WriteStartElement("blood-glucose");

            // <when>
            _when.WriteXml("when", writer);

            _value.WriteXml("value", writer);
            _glucoseMeasurementType.WriteXml(
                "glucose-measurement-type", 
                writer);

            XmlWriterHelper.WriteOptBool(
                writer, 
                "outside-operating-temp", 
                _outsideOperatingTemp);


            XmlWriterHelper.WriteOptBool(
                writer,
                "is-control-test",
                _isControlTest);

            if (_normalcy != null && _normalcy != Normalcy.Unknown)
            {
                writer.WriteElementString(
                    "normalcy",
                    ((int)_normalcy).ToString(CultureInfo.InvariantCulture));
            }

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "measurement-context",
                _measurementContext);
                    
            // </blood-glucose>
            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the date/time when the blood glucose measurement
        /// was taken.
        /// </summary>
        /// 
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
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
        /// Gets or set the blood glucose value.
        /// </summary>
        /// 
        /// <remarks>
        /// The blood glucose value is normally measured in mmol/L.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public BloodGlucoseMeasurement Value
        {
            get
            {
                return _value;
            }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "BGValueMandatory");
                _value = value;
            }
        }
        private BloodGlucoseMeasurement _value;

        /// <summary>
        /// Gets or set the glucose measurement type.
        /// </summary>
        /// 
        /// <remarks>
        /// The measurement type is how the reading was taken; whole blood,
        /// plasma, etc. The common value for the measurement types are defined
        /// in the glucose-measurement-type vocabulary.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue GlucoseMeasurementType
        {
            get
            {
                return _glucoseMeasurementType;
            }
            set
            {
                Validator.ThrowIfArgumentNull(value, "GlucoseMeasurementType", "BGMeasurementTypeMandatory");
                _glucoseMeasurementType = value;
            }
        }
        private CodableValue _glucoseMeasurementType;

        /// <summary>
        /// Gets or sets whether the reading was taken outside the operating
        /// temperature for the device.
        /// </summary>
        /// 
        /// <remarks>
        /// If the device does not support this function, the value is 
        /// <b>null</b>.
        /// </remarks>
        /// 
        public bool? OutsideOperatingTemperature
        {
            get { return _outsideOperatingTemp; }
            set { _outsideOperatingTemp = value; }
        }
        private bool? _outsideOperatingTemp;

        /// <summary>
        /// Gets or sets a value indicating whether the reading was taken as a 
        /// control test.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the reading was taken as a control test; otherwise, 
        /// <b>false</b>.
        /// </value>
        /// 
        /// <remarks>
        /// If the device does not support this function, the value is 
        /// <b>null</b>.
        /// </remarks>
        /// 
        public bool? IsControlTest
        {
            get { return _isControlTest; }
            set { _isControlTest = value; }
        }
        private bool? _isControlTest;

        /// <summary>
        /// Gets or sets a value indicating whether the reading was within the 
        /// normal range of values.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the reading was within normal range; otherwise, 
        /// <b>false</b>. 
        /// </value>
        /// 
        /// <remarks>
        /// If the device does not support this function, the value is 
        /// <b>null</b>.
        /// </remarks>
        /// 
        public Normalcy? ReadingNormalcy
        {
            get { return _normalcy; }
            set { _normalcy = value; }
        }
        private Normalcy? _normalcy;
        private int _normalcyValue;

        /// <summary>
        /// Gets or sets the contextual information about the reading.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the information.
        /// </value>
        /// 
        /// <remarks>
        /// Common values for the measurement context can be found in the
        /// "glucose-measurement-context" vocabulary.
        /// </remarks>
        /// 
        public CodableValue MeasurementContext
        {
            get { return _measurementContext; }
            set { _measurementContext = value; }
        }
        private CodableValue _measurementContext;

        /// <summary>
        /// Gets a string representation of the blood glucose item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the blood glucose item.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
