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
    /// Percentage of oxygen saturation in the blood.
    /// </summary>
    ///
    public class BloodOxygenSaturation : ThingBase
    {
        /// <summary>
        /// Creates an instance of <see cref="BloodOxygenSaturation"/> with default values.
        /// </summary>
        ///
        public BloodOxygenSaturation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="BloodOxygenSaturation"/> with specified parameters.
        /// </summary>
        ///
        /// <param name="when">The date and time the measurement was taken.</param>
        ///
        /// <param name="value">The percentage of oxygen saturation in the blood.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public BloodOxygenSaturation(HealthServiceDateTime when, double value)
            : base(TypeId)
        {
            this.When = when;
            this.Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the blood oxygen saturation item type.
        /// </summary>
        public static new readonly Guid TypeId =
            new Guid("3a54f95f-03d8-4f62-815f-f691fc94a500");

        /// <summary>
        /// Populates this <see cref="BloodOxygenSaturation"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the blood oxygen saturation data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "blood-oxygen-saturation" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("blood-oxygen-saturation");

            Validator.ThrowInvalidIfNull(itemNav, Resources.BloodOxygenSaturationUnexpectedNode);

            // when
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // value
            this.value = itemNav.SelectSingleNode("value").ValueAsDouble;

            // measurement-method
            this.measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // measurement-flags
            this.measurementFlags =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-flags");
        }

        /// <summary>
        /// Writes the blood oxygen saturation data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the blood oxygen saturation data to.
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

            // <blood-oxygen-saturation>
            writer.WriteStartElement("blood-oxygen-saturation");

            // <when>
            this.when.WriteXml("when", writer);

            // <value>
            writer.WriteElementString(
                "value",
                XmlConvert.ToString(this.value));

            // <measurement-method>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-method",
                this.measurementMethod);

            // <measurement-flags>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-flags",
                this.measurementFlags);

            // </blood-oxygen-saturation>
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
        /// Gets or sets the measured blood oxygen saturation percentage.
        /// </summary>
        ///
        /// <value>
        /// The measured blood oxygen saturation percentage.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public double Value
        {
            get { return this.value; }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Value), Resources.BloodOxygenSaturationValueOutOfRange);
                }

                this.value = value;
            }
        }

        private double value;

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
        /// for this value is "blood-oxygen-saturation-measurement-method".
        /// </remarks>
        ///
        public CodableValue MeasurementMethod
        {
            get { return this.measurementMethod; }
            set { this.measurementMethod = value; }
        }

        private CodableValue measurementMethod;

        /// <summary>
        /// Gets or sets the additional information about the measurement.
        /// </summary>
        ///
        /// <value>
        /// The additional information about the measurement
        /// </value>
        ///
        /// <remarks>
        /// Examples: Incomplete measurement, irregular heartbeat. If the value is not known,
        /// it will be set to <b>null</b>. The preferred vocabulary for this value is
        /// "blood-oxygen-saturation-measurement-method".
        /// </remarks>
        ///
        public CodableValue MeasurementFlags
        {
            get { return this.measurementFlags; }
            set { this.measurementFlags = value; }
        }

        private CodableValue measurementFlags;

        /// <summary>
        /// Gets the description of a blood oxygen instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the blood oxygen item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    Resources.Percent,
                    (this.value * 100.0).ToString(CultureInfo.CurrentCulture));
        }
    }
}
