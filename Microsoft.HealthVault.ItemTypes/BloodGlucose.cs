// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// thing type that represents a blood glucose measurement.
    /// </summary>
    ///
    public class BloodGlucose : ThingBase
    {
        /// <summary>
        /// Constructs a new BloodGlucose thing instance with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item isn't added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public BloodGlucose()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Constructs the new blood glucose thing instance
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
        public static new readonly Guid TypeId =
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

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(bgNav.SelectSingleNode("when"));

            this.value = new BloodGlucoseMeasurement();
            this.value.ParseXml(bgNav.SelectSingleNode("value"));

            this.glucoseMeasurementType = new CodableValue();
            this.glucoseMeasurementType.ParseXml(
                bgNav.SelectSingleNode("glucose-measurement-type"));

            this.outsideOperatingTemp =
                XPathHelper.GetOptNavValueAsBool(
                    bgNav,
                    "outside-operating-temp");

            this.isControlTest =
                XPathHelper.GetOptNavValueAsBool(
                    bgNav,
                    "is-control-test");

            XPathNavigator normalcyNav =
                bgNav.SelectSingleNode("normalcy");

            if (normalcyNav != null)
            {
                this.normalcyValue = normalcyNav.ValueAsInt;
                if (this.normalcyValue < (int)Normalcy.WellBelowNormal ||
                    this.normalcyValue > (int)Normalcy.WellAboveNormal)
                {
                    this.normalcy = Normalcy.Unknown;
                }
                else
                {
                    this.normalcy = (Normalcy)this.normalcyValue;
                }
            }

            this.measurementContext =
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
            Validator.ThrowSerializationIfNull(this.value, "BGValueNotSet");
            Validator.ThrowSerializationIfNull(this.glucoseMeasurementType, "BGMeasurementTypeNotSet");

            // <blood-glucose>
            writer.WriteStartElement("blood-glucose");

            // <when>
            this.when.WriteXml("when", writer);

            this.value.WriteXml("value", writer);
            this.glucoseMeasurementType.WriteXml(
                "glucose-measurement-type",
                writer);

            XmlWriterHelper.WriteOptBool(
                writer,
                "outside-operating-temp",
                this.outsideOperatingTemp);

            XmlWriterHelper.WriteOptBool(
                writer,
                "is-control-test",
                this.isControlTest);

            if (this.normalcy != null && this.normalcy != Normalcy.Unknown)
            {
                writer.WriteElementString(
                    "normalcy",
                    ((int)this.normalcy).ToString(CultureInfo.InvariantCulture));
            }

            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-context",
                this.measurementContext);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

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
                return this.value;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "BGValueMandatory");
                this.value = value;
            }
        }

        private BloodGlucoseMeasurement value;

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
                return this.glucoseMeasurementType;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "GlucoseMeasurementType", "BGMeasurementTypeMandatory");
                this.glucoseMeasurementType = value;
            }
        }

        private CodableValue glucoseMeasurementType;

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
            get { return this.outsideOperatingTemp; }
            set { this.outsideOperatingTemp = value; }
        }

        private bool? outsideOperatingTemp;

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
            get { return this.isControlTest; }
            set { this.isControlTest = value; }
        }

        private bool? isControlTest;

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
            get { return this.normalcy; }
            set { this.normalcy = value; }
        }

        private Normalcy? normalcy;
        private int normalcyValue;

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
            get { return this.measurementContext; }
            set { this.measurementContext = value; }
        }

        private CodableValue measurementContext;

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
            return this.Value.ToString();
        }
    }
}
