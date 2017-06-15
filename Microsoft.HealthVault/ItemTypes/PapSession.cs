// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
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
    public class PapSession : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PapSession"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
            When = when;
            DurationMinutes = durationMinutes;
            ApneaHypopneaIndex = apneaHypopneaIndex;
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
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("pap-session");

            Validator.ThrowInvalidIfNull(itemNav, Resources.PapSessionUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _durationMinutes = itemNav.SelectSingleNode("duration-minutes").ValueAsDouble;
            _apneaHypopneaIndex = itemNav.SelectSingleNode("apnea-hypopnea-index").ValueAsDouble;

            _apneaIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "apnea-index");
            _hypopneaIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "hypopnea-index");
            _oxygenDesaturationIndex = XPathHelper.GetOptNavValueAsDouble(itemNav, "oxygen-desaturation-index");

            _pressure = XPathHelper.GetOptNavValue<PapSessionMeasurements<PressureMeasurement>>(itemNav, "pressure");
            _leakRate = XPathHelper.GetOptNavValue<PapSessionMeasurements<FlowMeasurement>>(itemNav, "leak-rate");
            _tidalVolume = XPathHelper.GetOptNavValue<PapSessionMeasurements<VolumeMeasurement>>(itemNav, "tidal-volume");
            _minuteVentilation = XPathHelper.GetOptNavValue<PapSessionMeasurements<VolumeMeasurement>>(itemNav, "minute-ventilation");
            _respiratoryRate = XPathHelper.GetOptNavValue<PapSessionMeasurements<RespiratoryRateMeasurement>>(itemNav, "respiratory-rate");
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);

            writer.WriteStartElement("pap-session");

            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptDouble(writer, "duration-minutes", _durationMinutes);
            XmlWriterHelper.WriteOptDouble(writer, "apnea-hypopnea-index", _apneaHypopneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "apnea-index", _apneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "hypopnea-index", _hypopneaIndex);
            XmlWriterHelper.WriteOptDouble(writer, "oxygen-desaturation-index", _oxygenDesaturationIndex);

            XmlWriterHelper.WriteOpt(writer, "pressure", _pressure);
            XmlWriterHelper.WriteOpt(writer, "leak-rate", _leakRate);
            XmlWriterHelper.WriteOpt(writer, "tidal-volume", _tidalVolume);
            XmlWriterHelper.WriteOpt(writer, "minute-ventilation", _minuteVentilation);
            XmlWriterHelper.WriteOpt(writer, "respiratory-rate", _respiratoryRate);

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
                return _when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when;

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
                return _durationMinutes;
            }

            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(DurationMinutes), Resources.DurationMinutesNegative);
                }

                _durationMinutes = value;
            }
        }

        private double _durationMinutes;

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
                return _apneaHypopneaIndex;
            }

            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ApneaHypopneaIndex), Resources.ApneaHypopneaIndexNegative);
                }

                _apneaHypopneaIndex = value;
            }
        }

        private double _apneaHypopneaIndex;

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
                return _apneaIndex;
            }

            set
            {
                if (value != null && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ApneaIndex), Resources.ApneaIndexNegative);
                }

                _apneaIndex = value;
            }
        }

        private double? _apneaIndex;

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
                return _hypopneaIndex;
            }

            set
            {
                if (value != null && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(HypopneaIndex), Resources.HypopneaIndexNegative);
                }

                _hypopneaIndex = value;
            }
        }

        private double? _hypopneaIndex;

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
                return _oxygenDesaturationIndex;
            }

            set
            {
                if (value != null && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(OxygenDesaturationIndex), Resources.OxygenDesaturationIndexNegative);
                }

                _oxygenDesaturationIndex = value;
            }
        }

        private double? _oxygenDesaturationIndex;

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
            get { return _pressure; }
            set { _pressure = value; }
        }

        private PapSessionMeasurements<PressureMeasurement> _pressure;

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
            get { return _leakRate; }
            set { _leakRate = value; }
        }

        private PapSessionMeasurements<FlowMeasurement> _leakRate;

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
            get { return _tidalVolume; }
            set { _tidalVolume = value; }
        }

        private PapSessionMeasurements<VolumeMeasurement> _tidalVolume;

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
            get { return _minuteVentilation; }
            set { _minuteVentilation = value; }
        }

        private PapSessionMeasurements<VolumeMeasurement> _minuteVentilation;

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
            get { return _respiratoryRate; }
            set { _respiratoryRate = value; }
        }

        private PapSessionMeasurements<RespiratoryRateMeasurement> _respiratoryRate;

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
                Resources.PapSessionToStringFormat,
                ApneaHypopneaIndex.ToString(CultureInfo.CurrentCulture));
        }
    }
}
