// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Encapsulates a Positive Airway Pressure (PAP) Session data.
    /// </summary>
    ///
    /// <remarks>
    /// A common use for PAP therapy is in the treatment of sleep apnea.
    /// </remarks>
    ///
    public class PapSession : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PapSession"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public PapSession()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PapSession"/> class specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time of when the session was started.
        /// </param>
        /// <param name="durationMinutes">
        /// The number of minutes in the session.
        /// </param>
        /// <param name="apneaHypopneaIndex">
        /// The number of Apnea and Hypopnea events per hour.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// If <paramref name="durationMinutes"/> is <b>null</b>.
        /// If <paramref name="apneaHypopneaIndex"/> is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Hypopnea is a valid element name in PAP session.")]
        public PapSession(
            HealthServiceDateTime when,
            double durationMinutes,
            double apneaHypopneaIndex)
                : base(TypeId)
        {
            this.When = when;
            this.DurationMinutes = durationMinutes;
            this.ApneaHypopneaIndex = apneaHypopneaIndex;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("9085cad9-e866-4564-8a91-7ad8685d204d");

        /// <summary>
        /// Populates this <see cref="PapSession"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the PAP session data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a PAP session node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, "typeSpecificXml", "ParseXmlNavNull");

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("pap-session");

            Validator.ThrowInvalidIfNull(itemNav, "PapSessionUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.durationMinutes = itemNav.SelectSingleNode("duration-minutes").ValueAsDouble;
            this.apneaHypopneaIndex = itemNav.SelectSingleNode("apnea-hypopnea-index").ValueAsDouble;

            this.apneaIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "apnea-index");
            this.hypopneaIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "hypopnea-index");
            this.oxygenDesaturationIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "oxygen-desaturation-index");

            this.pressure = XPathHelper.GetOptNavValue<PapSessionMeasurements<PressureMeasurement>>(itemNav, "pressure");
            this.leakRate = XPathHelper.GetOptNavValue<PapSessionMeasurements<FlowMeasurement>>(itemNav, "leak-rate");
            this.tidalVolume = XPathHelper.GetOptNavValue<PapSessionMeasurements<VolumeMeasurement>>(itemNav, "tidal-volume");
            this.minuteVentilation = XPathHelper.GetOptNavValue<PapSessionMeasurements<VolumeMeasurement>>(itemNav, "minute-ventilation");
            this.respiratoryRate = XPathHelper.GetOptNavValue<PapSessionMeasurements<RespiratoryRateMeasurement>>(itemNav, "respiratory-rate");
        }

        /// <summary>
        /// Writes the XML representation of the PAP session into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the PAP session should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, "WhenNullValue");

            writer.WriteStartElement("pap-session");

            this.when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptDouble(writer, "duration-minutes", this.durationMinutes);
            XmlWriterHelper.WriteOptDouble(writer, "apnea-hypopnea-index", this.apneaHypopneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "apnea-index", this.apneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "hypopnea-index", this.hypopneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "oxygen-desaturation-index", this.oxygenDesaturationIndex);

            XmlWriterHelper.WriteOpt(writer, "pressure", this.pressure);
            XmlWriterHelper.WriteOpt(writer, "leak-rate", this.leakRate);
            XmlWriterHelper.WriteOpt(writer, "tidal-volume", this.tidalVolume);
            XmlWriterHelper.WriteOpt(writer, "minute-ventilation", this.minuteVentilation);
            XmlWriterHelper.WriteOpt(writer, "respiratory-rate", this.respiratoryRate);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of when the session was started.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return this.when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

        /// <summary>
        /// Gets or sets the number of minutes in the session.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0.
        /// </exception>
        ///
        public double DurationMinutes
        {
            get
            {
                return this.durationMinutes;
            }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0.0,
                    "DurationMinutes",
                    "DurationMinutesNegative");
                this.durationMinutes = value;
            }
        }

        private double durationMinutes;

        /// <summary>
        /// Gets or sets the number of Apnea and Hypopnea events per hour.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Hypopnea is a valid element name in PAP session.")]
        public double ApneaHypopneaIndex
        {
            get
            {
                return this.apneaHypopneaIndex;
            }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0.0,
                    "ApneaHypopneaIndex",
                    "ApneaHypopneaIndexNegative");
                this.apneaHypopneaIndex = value;
            }
        }

        private double apneaHypopneaIndex;

        /// <summary>
        /// Gets or sets the number of Apnea events per hour.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the Apnea Index the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0.
        /// </exception>
        ///
        public double? ApneaIndex
        {
            get
            {
                return this.apneaIndex;
            }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0.0,
                    "ApneaIndex",
                    "ApneaIndexNegative");
                this.apneaIndex = value;
            }
        }

        private double? apneaIndex;

        /// <summary>
        /// Gets or sets the number of Hypopnea events per hour.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the Hypopnea Index the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Hypopnea is a valid element name in PAP session.")]
        public double? HypopneaIndex
        {
            get
            {
                return this.hypopneaIndex;
            }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0.0,
                    "HypopneaIndex",
                    "HypopneaIndexNegative");
                this.hypopneaIndex = value;
            }
        }

        private double? hypopneaIndex;

        /// <summary>
        /// Gets or sets the number of oxygen desaturation events per hour.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the Oxygen Desaturation Index the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Oxygen desaturation index is a valid element name in PAP session.")]
        public double? OxygenDesaturationIndex
        {
            get
            {
                return this.oxygenDesaturationIndex;
            }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0.0,
                    "OxygenDesaturationIndex",
                    "OxygenDesaturationIndexNegative");
                this.oxygenDesaturationIndex = value;
            }
        }

        private double? oxygenDesaturationIndex;

        /// <summary>
        /// Gets or sets the pressure measurements during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the pressure the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PapSessionMeasurements<PressureMeasurement> Pressure
        {
            get { return this.pressure; }
            set { this.pressure = value; }
        }

        private PapSessionMeasurements<PressureMeasurement> pressure;

        /// <summary>
        /// Gets or sets the leak rate measurements during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the leak rate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PapSessionMeasurements<FlowMeasurement> LeakRate
        {
            get { return this.leakRate; }
            set { this.leakRate = value; }
        }

        private PapSessionMeasurements<FlowMeasurement> leakRate;

        /// <summary>
        /// Gets or sets the tidal volume measurements during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the tidal volume the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PapSessionMeasurements<VolumeMeasurement> TidalVolume
        {
            get { return this.tidalVolume; }
            set { this.tidalVolume = value; }
        }

        private PapSessionMeasurements<VolumeMeasurement> tidalVolume;

        /// <summary>
        /// Gets or sets the minute ventilation measurements during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the minute ventilation the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PapSessionMeasurements<VolumeMeasurement> MinuteVentilation
        {
            get { return this.minuteVentilation; }
            set { this.minuteVentilation = value; }
        }

        private PapSessionMeasurements<VolumeMeasurement> minuteVentilation;

        /// <summary>
        /// Gets or sets the respiratory rate measurements during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the respiratory rate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PapSessionMeasurements<RespiratoryRateMeasurement> RespiratoryRate
        {
            get { return this.respiratoryRate; }
            set { this.respiratoryRate = value; }
        }

        private PapSessionMeasurements<RespiratoryRateMeasurement> respiratoryRate;

        /// <summary>
        /// Gets a string representation of the PAP session.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the PAP session.
        /// </returns>
        ///
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                ResourceRetriever.GetResourceString("PapSessionToStringFormat"),
                this.ApneaHypopneaIndex.ToString(CultureInfo.CurrentCulture));
        }
    }
}
