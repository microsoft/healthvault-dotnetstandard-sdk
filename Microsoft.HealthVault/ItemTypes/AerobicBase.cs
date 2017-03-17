// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents data about an aerobic session.
    /// </summary>
    ///
    /// <remarks>
    /// Aerobic data can represent the data for an entire session, the data
    /// for a single lap, or the desired aerobic session for an aerobic goal.
    /// </remarks>
    ///
    public class AerobicBase : ItemBase
    {
        /// <summary>
        /// Populates the data for the length from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the length.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator modeNav =
                navigator.SelectSingleNode("mode");

            this.mode.Clear();
            if (modeNav != null)
            {
                this.mode.ParseXml(modeNav);
            }

            XPathNavigator distanceNav =
                navigator.SelectSingleNode("distance");

            if (distanceNav != null)
            {
                this.distance = new Length();
                this.distance.ParseXml(distanceNav);
            }

            this.minutes =
                XPathHelper.GetOptNavValueAsDouble(navigator, "minutes");

            int? currentIntensity =
                XPathHelper.GetOptNavValueAsInt(navigator, "intensity");
            if (currentIntensity != null)
            {
                this.intensity = (RelativeRating)(int)currentIntensity;
            }

            this.peakHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "peak-heartrate");

            this.averageHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "avg-heartrate");

            this.minHr =
                XPathHelper.GetOptNavValueAsInt(navigator, "min-heartrate");

            this.energy =
                XPathHelper.GetOptNavValueAsDouble(navigator, "energy");

            this.energyFromFat =
                XPathHelper.GetOptNavValueAsDouble(navigator, "energy-from-fat");

            this.peakSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "peak-speed");

            this.averageSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "avg-speed");

            this.minSpeed =
                XPathHelper.GetOptNavValue<SpeedMeasurement>(
                    navigator,
                    "min-speed");

            this.peakPace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "peak-pace");

            this.averagePace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "avg-pace");

            this.minPace =
                XPathHelper.GetOptNavValue<PaceMeasurement>(
                    navigator,
                    "min-pace");

            this.peakPower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "peak-power");

            this.averagePower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "avg-power");

            this.minPower =
                XPathHelper.GetOptNavValue<PowerMeasurement>(
                    navigator,
                    "min-power");

            this.peakTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "peak-torque");

            this.averageTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "avg-torque");

            this.minTorque =
                XPathHelper.GetOptNavValue<TorqueMeasurement>(
                    navigator,
                    "min-torque");

            this.leftRightBalance =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "left-right-balance");

            this.peakCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "peak-cadence");

            this.averageCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "avg-cadence");

            this.minCadence =
                XPathHelper.GetOptNavValueAsDouble(navigator, "min-cadence");

            this.peakTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "peak-temperature");

            this.averageTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "avg-temperature");

            this.minTemperature =
                XPathHelper.GetOptNavValue<TemperatureMeasurement>(
                    navigator,
                    "min-temperature");

            this.peakAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "peak-altitude");

            this.averageAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "avg-altitude");

            this.minAltitude =
                XPathHelper.GetOptNavValue<AltitudeMeasurement>(
                    navigator,
                    "min-altitude");

            this.elevationGain =
                XPathHelper.GetOptNavValue<Length>(
                    navigator,
                    "elevation-gain");

            this.elevationLoss =
                XPathHelper.GetOptNavValue<Length>(
                    navigator,
                    "elevation-loss");

            this.numberOfSteps =
                XPathHelper.GetOptNavValueAsInt(
                    navigator,
                    "number-of-steps");

            this.numberOfAerobicSteps =
                XPathHelper.GetOptNavValueAsInt(
                    navigator,
                    "number-of-aerobic-steps");

            this.aerobicStepMinutes =
                XPathHelper.GetOptNavValueAsDouble(
                    navigator,
                    "aerobic-step-minutes");
        }

        /// <summary>
        /// Writes the aerobic data to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the aerobic data.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the aerobic data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");

            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            writer.WriteStartElement(nodeName);

            if (this.mode.Text != null)
            {
                // <mode>
                this.mode.WriteXml("mode", writer);
            }

            // <distance>
            this.distance?.WriteXml("distance", writer);

            if (this.minutes != null)
            {
                // <minutes>
                writer.WriteElementString(
                    "minutes",
                    XmlConvert.ToString(this.minutes.Value));
            }

            if (this.intensity != RelativeRating.None)
            {
                // <intensity>
                writer.WriteElementString(
                    "intensity",
                    ((int)this.intensity).ToString(CultureInfo.InvariantCulture));
            }

            if (this.peakHr != null)
            {
                // <peak-heartrate>
                writer.WriteElementString(
                    "peak-heartrate",
                    this.peakHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.averageHr != null)
            {
                // <avg-heartrate>
                writer.WriteElementString(
                    "avg-heartrate",
                    this.averageHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.minHr != null)
            {
                // <min-heartrate>
                writer.WriteElementString(
                    "min-heartrate",
                    this.minHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            XmlWriterHelper.WriteOptDouble(
                writer,
                "energy",
                this.energy);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "energy-from-fat",
                this.energyFromFat);

            this.peakSpeed?.WriteXml("peak-speed", writer);

            this.averageSpeed?.WriteXml("avg-speed", writer);

            this.minSpeed?.WriteXml("min-speed", writer);

            this.peakPace?.WriteXml("peak-pace", writer);

            this.averagePace?.WriteXml("avg-pace", writer);

            this.minPace?.WriteXml("min-pace", writer);

            this.peakPower?.WriteXml("peak-power", writer);

            this.averagePower?.WriteXml("avg-power", writer);

            this.minPower?.WriteXml("min-power", writer);

            this.peakTorque?.WriteXml("peak-torque", writer);

            this.averageTorque?.WriteXml("avg-torque", writer);

            this.minTorque?.WriteXml("min-torque", writer);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "left-right-balance",
                this.leftRightBalance);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "peak-cadence",
                this.peakCadence);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "avg-cadence",
                this.averageCadence);

            XmlWriterHelper.WriteOptDouble(
                writer,
                "min-cadence",
                this.minCadence);

            this.peakTemperature?.WriteXml("peak-temperature", writer);

            this.averageTemperature?.WriteXml("avg-temperature", writer);

            this.minTemperature?.WriteXml("min-temperature", writer);

            this.peakAltitude?.WriteXml("peak-altitude", writer);

            this.averageAltitude?.WriteXml("avg-altitude", writer);

            this.minAltitude?.WriteXml("min-altitude", writer);

            XmlWriterHelper.WriteOpt(writer, "elevation-gain", this.elevationGain);
            XmlWriterHelper.WriteOpt(writer, "elevation-loss", this.elevationLoss);
            XmlWriterHelper.WriteOptInt(writer, "number-of-steps", this.numberOfSteps);
            XmlWriterHelper.WriteOptInt(writer, "number-of-aerobic-steps", this.numberOfAerobicSteps);
            XmlWriterHelper.WriteOptDouble(writer, "aerobic-step-minutes", this.aerobicStepMinutes);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the mode by which the aerobic session occurred.
        /// </summary>
        ///
        /// <remarks>
        /// The mode of the aerobic session include actions such as bike, run, or
        /// swim and are defined by the HealthVault dictionary.
        /// </remarks>
        ///
        public CodableValue Mode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }

        private CodableValue mode = new CodableValue();

        /// <summary>
        /// Gets or sets the length traversed by this aerobic session.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> value representing the distance.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no distance.
        /// </remarks>
        ///
        public Length Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }

        private Length distance;

        private static void ThrowIfValueIsNegative(double? value, string message)
        {
            if (value != null && (double)value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), message);
            }
        }

        private static void ThrowIfValueIsNegative(int? value, string message)
        {
            if (value != null && (int)value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), message);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the session in minutes.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no duration.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? Duration
        {
            get { return this.minutes; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionDurationNotPositive);
                this.minutes = value;
            }
        }

        private double? minutes;

        /// <summary>
        /// Gets the duration of the aerobic session as a
        /// <see cref="System.TimeSpan"/>.
        /// </summary>
        ///
        public TimeSpan DurationAsTimeSpan
        {
            get
            {
                TimeSpan result;
                if (this.minutes != null)
                {
                    result =
                        new TimeSpan(
                            0,
                            (int)this.minutes.Value,
                            (int)(60.0 * (this.minutes - (int)this.minutes)));
                }
                else
                {
                    result = new TimeSpan();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the relative intensity of the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <see cref="RelativeRating.None"/> if there is
        /// no intensity.
        /// </remarks>
        ///
        public RelativeRating Intensity
        {
            get { return this.intensity; }
            set { this.intensity = value; }
        }

        private RelativeRating intensity;

        /// <summary>
        /// Gets or sets the peak heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero on set.
        /// </exception>
        ///
        public int? PeakHeartRate
        {
            get { return this.peakHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionPeakHrNotPositive);
                this.peakHr = value;
            }
        }

        private int? peakHr;

        /// <summary>
        /// Gets or sets the average heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero on set.
        /// </exception>
        ///
        public int? AverageHeartRate
        {
            get { return this.averageHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionAvgHrNotPositive);
                this.averageHr = value;
            }
        }

        private int? averageHr;

        /// <summary>
        /// Gets or sets the minimum heart rate for the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no heart rate data for the session, the value should
        /// be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to
        /// zero when set.
        /// </exception>
        ///
        public int? MinHeartRate
        {
            get { return this.minHr; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionMinHrNotPositive);
                this.minHr = value;
            }
        }

        private int? minHr;

        /// <summary>
        /// Gets or sets the food energy consumed during the session.
        /// </summary>
        ///
        /// <remarks>
        /// The energy consumed is measured in kilojoules.
        /// Set the value to <b>null</b> if there is no energy reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? Energy
        {
            get { return this.energy; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionEnergyNotPositive);
                this.energy = value;
            }
        }

        private double? energy;

        /// <summary>
        /// Gets or sets the energy from fat consumed during the session.
        /// </summary>
        ///
        /// <remarks>
        /// The energy consumed is measured in kilojoules.
        /// Set the value to <b>null</b> if there is no energy from fat reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? EnergyFromFat
        {
            get { return this.energyFromFat; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionEnergyFromFatNotPositive);
                this.energyFromFat = value;
            }
        }

        private double? energyFromFat;

        /// <summary>
        /// Gets or sets the peak speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement PeakSpeed
        {
            get { return this.peakSpeed; }
            set { this.peakSpeed = value; }
        }

        private SpeedMeasurement peakSpeed;

        /// <summary>
        /// Gets or sets the average speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement AverageSpeed
        {
            get { return this.averageSpeed; }
            set { this.averageSpeed = value; }
        }

        private SpeedMeasurement averageSpeed;

        /// <summary>
        /// Gets or sets the minimum speed, in meters per second, during the
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no speed reading.
        /// </remarks>
        ///
        public SpeedMeasurement MinSpeed
        {
            get { return this.minSpeed; }
            set { this.minSpeed = value; }
        }

        private SpeedMeasurement minSpeed;

        /// <summary>
        /// Gets or sets the peak pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement PeakPace
        {
            get { return this.peakPace; }
            set { this.peakPace = value; }
        }

        private PaceMeasurement peakPace;

        /// <summary>
        /// Gets or sets the average pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement AveragePace
        {
            get { return this.averagePace; }

            set
            {
                this.averagePace = value;
            }
        }

        private PaceMeasurement averagePace;

        /// <summary>
        /// Gets or sets the minimum pace, in seconds per 100 meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no pace reading.
        /// </remarks>
        ///
        public PaceMeasurement MinPace
        {
            get { return this.minPace; }

            set
            {
                this.minPace = value;
            }
        }

        private PaceMeasurement minPace;

        /// <summary>
        /// Gets or sets the peak power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement PeakPower
        {
            get { return this.peakPower; }

            set
            {
                this.peakPower = value;
            }
        }

        private PowerMeasurement peakPower;

        /// <summary>
        /// Gets or sets the average power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement AveragePower
        {
            get { return this.averagePower; }

            set
            {
                this.averagePower = value;
            }
        }

        private PowerMeasurement averagePower;

        /// <summary>
        /// Gets or sets the minimum power, in watts, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no power reading.
        /// </remarks>
        ///
        public PowerMeasurement MinPower
        {
            get { return this.minPower; }

            set
            {
                this.minPower = value;
            }
        }

        private PowerMeasurement minPower;

        /// <summary>
        /// Gets or sets the peak torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement PeakTorque
        {
            get { return this.peakTorque; }

            set
            {
                this.peakTorque = value;
            }
        }

        private TorqueMeasurement peakTorque;

        /// <summary>
        /// Gets or sets the average torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement AverageTorque
        {
            get { return this.averageTorque; }

            set
            {
                this.averageTorque = value;
            }
        }

        private TorqueMeasurement averageTorque;

        /// <summary>
        /// Gets or sets the minimum torque, in newton meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no torque reading.
        /// </remarks>
        ///
        public TorqueMeasurement MinTorque
        {
            get { return this.minTorque; }

            set
            {
                this.minTorque = value;
            }
        }

        private TorqueMeasurement minTorque;

        /// <summary>
        /// Gets or sets the balance between left and right strokes.
        /// </summary>
        ///
        /// <remarks>
        /// The value is a percentage where the indicated value is the
        /// percentage of the left stroke. The remaining percentage is
        /// the right stroke.
        /// Set the value to <b>null</b> if there is no balance reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or greater than one when set.
        /// </exception>
        ///
        public double? LeftRightBalance
        {
            get { return this.leftRightBalance; }

            set
            {
                if (value != null && ((double)value < 0.0 || (double)value > 1.0))
                {
                    throw new ArgumentOutOfRangeException(nameof(this.LeftRightBalance), Resources.AerobicSessionLeftRightBalanceNotPercentage);
                }
                
                this.leftRightBalance = value;
            }
        }

        private double? leftRightBalance;

        /// <summary>
        /// Gets or sets the peak cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? PeakCadence
        {
            get { return this.peakCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionPeakCadenceNotPositive);
                this.peakCadence = value;
            }
        }

        private double? peakCadence;

        /// <summary>
        /// Gets or sets the average cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? AverageCadence
        {
            get { return this.averageCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionAvgCadenceNotPositive);
                this.averageCadence = value;
            }
        }

        private double? averageCadence;

        /// <summary>
        /// Gets or sets the minimum cadence, in revolutions per minute (rpm),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no cadence reading.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? MinCadence
        {
            get { return this.minCadence; }

            set
            {
                ThrowIfValueIsNegative(value, Resources.AerobicSessionMinCadenceNotPositive);
                this.minCadence = value;
            }
        }

        private double? minCadence;

        /// <summary>
        /// Gets or sets the peak temperature, in degrees Celsius (C), during
        /// the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement PeakTemperature
        {
            get { return this.peakTemperature; }

            set
            {
                this.peakTemperature = value;
            }
        }

        private TemperatureMeasurement peakTemperature;

        /// <summary>
        /// Gets or sets the average temperature, in degrees Celsius (C),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement AverageTemperature
        {
            get { return this.averageTemperature; }

            set
            {
                this.averageTemperature = value;
            }
        }

        private TemperatureMeasurement averageTemperature;

        /// <summary>
        /// Gets or sets the minimum temperature, in degrees Celsius (C),
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no temperature reading.
        /// </remarks>
        ///
        public TemperatureMeasurement MinTemperature
        {
            get { return this.minTemperature; }
            set { this.minTemperature = value; }
        }

        private TemperatureMeasurement minTemperature;

        /// <summary>
        /// Gets or sets the peak altitude, in meters above sea level, during
        /// the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement PeakAltitude
        {
            get { return this.peakAltitude; }
            set { this.peakAltitude = value; }
        }

        private AltitudeMeasurement peakAltitude;

        /// <summary>
        /// Gets or sets the average altitude, in meters above sea level,
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement AverageAltitude
        {
            get { return this.averageAltitude; }
            set { this.averageAltitude = value; }
        }

        private AltitudeMeasurement averageAltitude;

        /// <summary>
        /// Gets or sets the minimum altitude, in meters above sea level,
        /// during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Negative values indicate the meters below sea level.
        /// Set the value to <b>null</b> if there is no altitude reading.
        /// </remarks>
        ///
        public AltitudeMeasurement MinAltitude
        {
            get { return this.minAltitude; }
            set { this.minAltitude = value; }
        }

        private AltitudeMeasurement minAltitude;

        /// <summary>
        /// Gets or sets the elevation gained, in meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no elevation gain reading.
        /// </remarks>
        ///
        public Length ElevationGain
        {
            get { return this.elevationGain; }
            set { this.elevationGain = value; }
        }

        private Length elevationGain;

        /// <summary>
        /// Gets or sets the elevation lossed, in meters, during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no elevation loss reading.
        /// </remarks>
        ///
        public Length ElevationLoss
        {
            get { return this.elevationLoss; }
            set { this.elevationLoss = value; }
        }

        private Length elevationLoss;

        /// <summary>
        /// Gets or sets the number of steps taken during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no step readings.
        /// </remarks>
        ///
        public int? NumberOfSteps
        {
            get { return this.numberOfSteps; }
            set { this.numberOfSteps = value; }
        }

        private int? numberOfSteps;

        /// <summary>
        /// Gets or sets the number of aerobic steps taken during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no aerobic step readings.
        /// </remarks>
        ///
        public int? NumberOfAerobicSteps
        {
            get { return this.numberOfAerobicSteps; }
            set { this.numberOfAerobicSteps = value; }
        }

        private int? numberOfAerobicSteps;

        /// <summary>
        /// Gets or sets the number of aerobic minutes during the session.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there are no readings indicating the number of
        /// aerobic minutes.
        /// </remarks>
        ///
        public double? AerobicStepMinutes
        {
            get { return this.aerobicStepMinutes; }
            set { this.aerobicStepMinutes = value; }
        }

        private double? aerobicStepMinutes;

        /// <summary>
        /// Gets a summary of the aerobic data.
        /// </summary>
        ///
        /// <returns>
        /// A string summary of the aerobic data.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Distance != null && this.Duration != null)
            {
                result = Resources.AerobicDataToStringFormatDistanceAndDuration.FormatResource(
                    this.Distance.ToString(),
                    this.Duration.Value.ToString(CultureInfo.CurrentCulture));
            }
            else if (this.Duration != null)
            {
                result = Resources.AerobicDataToStringFormatDuration.FormatResource(this.Duration.Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (this.Distance != null)
            {
                result = Resources.AerobicDataToStringFormatDistance.FormatResource(this.Distance.ToString());
            }

            return result;
        }
    }
}
