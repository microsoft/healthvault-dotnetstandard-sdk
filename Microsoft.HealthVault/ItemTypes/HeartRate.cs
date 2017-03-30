// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A measurement of the record owner's heart rate.
    /// </summary>
    ///
    public class HeartRate : ThingBase
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
            this.When = when;
            this.Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the heart rate item type.
        /// </summary>
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.HeartRateUnexpectedNode);

            // when
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // value
            this.value = itemNav.SelectSingleNode("value").ValueAsInt;

            // measurement-method
            this.measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // measurement-condition
            this.measurementConditions =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-conditions");

            // measurement-flags
            this.measurementFlags =
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);

            // <heart-rate>
            writer.WriteStartElement("heart-rate");

            // <when>
            this.when.WriteXml("when", writer);

            // <value>
            writer.WriteElementString(
                "value",
                this.value.ToString(CultureInfo.InvariantCulture));

            // <measurement-method>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-method",
                this.measurementMethod);

            // <measurement-conditions>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-conditions",
                this.measurementConditions);

            // <measurement-flags>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-flags",
                this.measurementFlags);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

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
            get { return this.value; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Value), Resources.HeartRateValueNegative);
                }

                this.value = value;
            }
        }

        private int value;

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
            get { return this.measurementMethod; }
            set { this.measurementMethod = value; }
        }

        private CodableValue measurementMethod;

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
            get { return this.measurementConditions; }
            set { this.measurementConditions = value; }
        }

        private CodableValue measurementConditions;

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
            get { return this.measurementFlags; }
            set { this.measurementFlags = value; }
        }

        private CodableValue measurementFlags;

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
                string.Format(
                    Resources.HeartRateToStringFormat,
                    this.value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
