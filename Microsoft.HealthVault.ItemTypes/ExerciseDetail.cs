// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents additional information about the exercise.
    /// </summary>
    ///
    /// <remarks>
    /// The detail information typically stores information that is specific to the type of exercise activity
    /// and any device used to measure it.
    ///
    /// Examples: Average heart rate, average temperature, intensity.
    /// </remarks>
    ///
    public class ExerciseDetail : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseDetail"/> class
        /// with default values.
        /// </summary>
        ///
        public ExerciseDetail()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseDetail"/> class
        /// with specified values
        /// </summary>
        /// <param name="name">The name of the information stored in this exercise detail. </param>
        /// <param name="value">The value of the exercise detail</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> or <paramref name="value"/> is <b>null</b>.
        /// </exception>
        public ExerciseDetail(CodedValue name, StructuredMeasurement value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Populates the data for the exercise detail from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the exercise detail.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.name =
                XPathHelper.GetOptNavValue<CodedValue>(navigator, "name");

            this.value =
                XPathHelper.GetOptNavValue<StructuredMeasurement>(navigator, "value");
        }

        /// <summary>
        /// Writes the exercise detail to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the exercise detail.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the exercise detail data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, "ExerciseDetailNameNotSet");
            Validator.ThrowSerializationIfNull(this.value, "ExerciseDetailValueNotSet");

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt(
                writer,
                "name",
                this.name);

            XmlWriterHelper.WriteOpt(
                writer,
                "value",
                this.value);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the information stored in this exercise detail.
        /// </summary>
        ///
        /// <remarks>
        /// Names should be values contained in the exercise-detail-names vocabularies, and that
        /// vocabulary should be specified when adding new details.
        /// Constants that are equal to these names can be found as static members of the <see cref="ExerciseDetail"/> class.
        /// </remarks>
        ///
        /// <value>
        /// A CodedValue representing the detail name.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The name is being set to null.
        /// </exception>
        public CodedValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "ExerciseDetailNameNullValue");
                this.name = value;
            }
        }

        private CodedValue name;

        /// <summary>
        /// Gets or sets the value of the exercise detail.
        /// </summary>
        ///
        /// <remarks>
        /// For example, to store an average heartrate of 125, place 125 in the value element and
        /// set the unit to "BPM".
        /// Units should be coded using the exercise-units vocabulary.
        /// </remarks>
        /// <value>
        /// A <see cref="StructuredMeasurement"/> representing the detail value.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The value is being set to null.
        /// </exception>
        public StructuredMeasurement Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "ExerciseDetailNameNullValue");
                this.value = value;
            }
        }

        private StructuredMeasurement value;

        /// <summary>
        /// Gets a string representation of the ExerciseDetail item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the ExerciseDetail item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    ResourceRetriever.GetResourceString(
                        "NameEqualsValue"),
                    this.name.ToString(),
                    this.value.ToString());
        }

        /// <summary>
        /// Detail name for Intensity.
        /// </summary>
        public const string Intensity = "Intensity";

        /// <summary>
        /// Detail name for MinimumHeartrate_BPM.
        /// </summary>
        public const string MinimumHeartrateBPM = "MinimumHeartrate_BPM";

        /// <summary>
        /// Detail name for AverageHeartrate_BPM.
        /// </summary>
        public const string AverageHeartrateBPM = "AverageHeartrate_BPM";

        /// <summary>
        /// Detail name for MaximumHeartrate_BPM
        /// </summary>
        public const string MaximumHeartrateBPM = "MaximumHeartrate_BPM";

        /// <summary>
        /// Detail name for PeakHeartrate_BPM (use MaximumHeartrate_BPM instead)
        /// </summary>
        public const string PeakHeartrateBPM = "PeakHeartrate_BPM";

        /// <summary>
        /// Detail name for Work_kJ.
        /// </summary>
        /// <remarks>
        /// The work_kJ detail pertains to the actual amount of work done during the exercise.
        /// </remarks>
        public const string WorkKJ = "Work_kJ";

        /// <summary>
        /// Detail name for CaloriesBurned_calories.
        /// </summary>
        /// <remarks>
        /// The CaloriesBurned_calories detail expresses the number of food calories required to
        /// balance out the energy expended in the exercise.
        /// </remarks>
        public const string CaloriesBurnedCalories = "CaloriesBurned_calories";

        /// <summary>
        /// Detail name for FatCaloriesBurned_calories.
        /// </summary>
        /// <remarks>
        /// The FatCaloriesBurned_calories detail expresses the number of fat calories required to
        /// balance out the energy expended in the exercise.
        /// </remarks>
        public const string FatCaloriesBurnedCalories = "FatCaloriesBurned_calories";

        /// <summary>
        /// Detail name for EnergyOld_kJ.
        /// </summary>
        /// <remarks>
        /// This value corresponds to Energy_kJ value in the AerobicSession type.
        /// </remarks>
        public const string EnergyOldKJ = "EnergyOld_kJ";

        /// <summary>
        /// Detail name for EnergyFromFatOld_kJ.
        /// </summary>
        /// <remarks>
        /// This value corresponds to EnergyFromFat_kJ value in the AerobicSession type.
        /// </remarks>
        public const string EnergyFromFatOldKJ = "EnergyFromFatOld_kJ";

        /// <summary>
        /// Detail name for MinimumSpeed_m-per-s.
        /// </summary>
        public const string MinimumSpeedMPerS = "MinimumSpeed_m-per-s";

        /// <summary>
        /// Detail name for AverageSpeed_m-per-s.
        /// </summary>
        public const string AverageSpeedMPerS = "AverageSpeed_m-per-s";

        /// <summary>
        /// Detail name for MaximumSpeed_m-per-s.
        /// </summary>
        public const string MaximumSpeedMPerS = "MaximumSpeed_m-per-s";

        /// <summary>
        /// Detail name for MinimumPace_s-per-100m.
        /// </summary>
        public const string MinimumPaceSPer100m = "MinimumPace_s-per-100m";

        /// <summary>
        /// Detail name for AveragePace_s-per-100m.
        /// </summary>
        public const string AveragePaceSPer100m = "AveragePace_s-per-100m";

        /// <summary>
        /// Detail name for MaximumPace_s-per-100m.
        /// </summary>
        public const string MaximumPaceSPer100m = "MaximumPace_s-per-100m";

        /// <summary>
        /// Detail name for MinimumPower_watts.
        /// </summary>
        public const string MinimumPowerWatts = "MinimumPower_watts";

        /// <summary>
        /// Detail name for AveragePower_watts.
        /// </summary>
        public const string AveragePowerWatts = "AveragePower_watts";

        /// <summary>
        /// Detail name for MaximumPower_watts.
        /// </summary>
        public const string MaximumPowerWatts = "MaximumPower_watts";

        /// <summary>
        /// Detail name for MinimumTorque_Nm.
        /// </summary>
        public const string MinimumTorqueNm = "MinimumTorque_Nm";

        /// <summary>
        /// Detail name for AverageTorque_Nm.
        /// </summary>
        public const string AverageTorqueNm = "AverageTorque_Nm";

        /// <summary>
        /// Detail name for MaximumTorque_Nm.
        /// </summary>
        public const string MaximumTorqueNm = "MaximumTorque_Nm";

        /// <summary>
        /// Detail name for LeftRightBalance_percent.
        /// </summary>
        /// <remarks>
        /// This detail stores the balance between the left and right leg when cycling,
        /// and is a number from 0 to 1. The detail stores the percentage of work the left leg is performing,
        /// and the right leg percentage is 1 minus the left leg value.
        /// For example, if the this value is 0.45, the left leg is doing 45% of the work and the right leg is
        /// doing 55% of the work.
        /// </remarks>
        public const string LeftRightBalancePercent = "LeftRightBalance_percent";

        /// <summary>
        /// Detail name for MinimumCadence_RPM.
        /// </summary>
        public const string MinimumCadenceRPM = "MinimumCadence_RPM";

        /// <summary>
        /// Detail name for AverageCadence_RPM.
        /// </summary>
        public const string AverageCadenceRPM = "AverageCadence_RPM";

        /// <summary>
        /// Detail name for MaximumCadence_RPM.
        /// </summary>
        public const string MaximumCadenceRPM = "MaximumCadence_RPM";

        /// <summary>
        /// Detail name for MinimumTemp_celsius.
        /// </summary>
        public const string MinimumTempCelsius = "MinimumTemp_celsius";

        /// <summary>
        /// Detail name for AverageTemp_celsius.
        /// </summary>
        public const string AverageTempCelsius = "AverageTemp_celsius";

        /// <summary>
        /// Detail name for MaximumTemp_celsius.
        /// </summary>
        public const string MaximumTempCelsius = "MaximumTemp_celsius";

        /// <summary>
        /// Detail name for MaximumTemp_celsius.
        /// </summary>
        public const string MinimumAltitudeMeters = "MaximumTemp_celsius";

        /// <summary>
        /// Detail name for AverageAltitude_meters.
        /// </summary>
        public const string AverageAltitudeMeters = "AverageAltitude_meters";

        /// <summary>
        /// Detail name for MaximumAltitude_meters.
        /// </summary>
        public const string MaximumAltitudeMeters = "MaximumAltitude_meters";

        /// <summary>
        /// Detail name for ElevationGain_meters.
        /// </summary>
        public const string ElevationGainMeters = "ElevationGain_meters";

        /// <summary>
        /// Detail name for ElevationLoss_meters.
        /// </summary>
        public const string ElevationLossMeters = "ElevationLoss_meters";

        /// <summary>
        /// Detail name for Steps_count.
        /// </summary>
        public const string StepsCount = "Steps_count";

        /// <summary>
        /// Detail name for AerobicSteps_count.
        /// </summary>
        public const string AerobicStepsCount = "AerobicSteps_count";

        /// <summary>
        /// Detail name for AerobicStepDuration_minutes.
        /// </summary>
        public const string AerobicStepDurationMinutes = "AerobicStepDuration_minutes";

        /// <summary>
        /// Detail name for Odometer_meters.
        /// </summary>
        public const string OdometerMeters = "Odometer_meters";

        /// <summary>
        /// Detail name for MaximumVerticalSpeedAscending_m-per-s.
        /// </summary>
        public const string MaximumVerticalSpeedAscendingMPerS = "MaximumVerticalSpeedAscending_m-per-s";

        /// <summary>
        /// Detail name for MaximumVerticalSpeedDescending_m-per-s.
        /// </summary>
        public const string MaximumVerticalSpeedDescendingMPerS = "MaximumVerticalSpeedDescending_m-per-s";

        /// <summary>
        /// Detail name for AverageVerticalSpeedAscending_m-per-s.
        /// </summary>
        public const string AverageVerticalSpeedAscendingMPerS = "AverageVerticalSpeedAscending_m-per-s";

        /// <summary>
        /// Detail name for AverageVerticalSpeedDescending_m-per-s.
        /// </summary>
        public const string AverageVerticalSpeedDescendingMPerS = "AverageVerticalSpeedDescending_m-per-s";
    }
}