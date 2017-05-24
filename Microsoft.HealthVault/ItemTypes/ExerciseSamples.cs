// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores a set of samples related to an exercise.
    /// </summary>
    /// <remarks>
    /// Examples: Heart rate samples, speed samples, position samples.
    ///
    /// Exercise samples are related to exercises through related items.
    ///
    /// The samples are stored in the other-data section of the object, and must
    /// be fetched by specifying HealthItemRecordSections.BlobPayload.
    ///
    /// The format of the other-data section is the HealthVault comma-separated format. It should be accessed
    /// using the ExerciseSamplesData property.
    /// </remarks>
    ///
    public class ExerciseSamples : ThingBase
    {
        /// <summary>
        /// Creates an instance of <see cref="ExerciseSamples"/> with default values.
        /// </summary>
        ///
        public ExerciseSamples()
            : base(TypeId)
        {
            Sections |= ThingSections.BlobPayload;
        }

        /// <summary>
        /// Creates an instance of <see cref="ExerciseSamples"/> with specified parameters.
        /// </summary>
        ///
        /// <param name="when">The date and time the samples were recorded.</param>
        /// <param name="name">The kind of information that is stored in this set of samples.</param>
        /// <param name="unit">The unit of measure for the samples.</param>
        /// <param name="samplingInterval">The time interval between samples in seconds.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// </exception>
        ///
        public ExerciseSamples(ApproximateDateTime when, CodableValue name, CodableValue unit, double samplingInterval)
            : base(TypeId)
        {
            When = when;
            Name = name;
            Unit = unit;
            SamplingInterval = samplingInterval;
            Sections |= ThingSections.BlobPayload;
        }

        /// <summary>
        /// Retrieves the unique identifier for the exercise samples item type.
        /// </summary>
        public static new readonly Guid TypeId =
                    new Guid("e1f92d7f-9699-4483-8223-8442874ec6d9");

        /// <summary>
        /// Populates this <see cref="ExerciseSamples"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the exercise samples data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// an "exercise-samples" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("exercise-samples");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ExerciseSamplesUnexpectedNode);

            // when
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // name
            _name = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "name");

            // unit
            _unit = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "unit");

            // sampling interval
            _samplingInterval = itemNav.SelectSingleNode("sampling-interval").ValueAsDouble;
        }

        /// <summary>
        /// Writes the exercise samples data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the exercise samples data to.
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
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes code more readable.")]
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(_name, Resources.ExerciseSampleNameNotSet);
            Validator.ThrowSerializationIfNull(_unit, Resources.ExerciseSampleUnitNotSet);

            if (double.IsNaN(_samplingInterval))
            {
                throw new ThingSerializationException(Resources.ExerciseSampleSamplingIntervalNotSet);
            }

            if (_sampleData != null && _sampleData.SingleValuedSamples.Count > 0)
            {
                // This ensures that the data gets written to a string in _sampleData.Data
                using (MemoryStream stream = new MemoryStream(1024))
                {
                    using (XmlWriter unusedWriter = XmlWriter.Create(stream))
                    {
                        _sampleData.WriteXml(unusedWriter);
                    }
                }

                Blob blob =
                    GetBlobStore(default(HealthRecordAccessor)).NewBlob(
                        string.Empty,
                        _sampleData.ContentType);
                blob.WriteInline(_sampleData.Data);
            }

            // <exercise-samples>
            writer.WriteStartElement("exercise-samples");

            // <when>
            _when.WriteXml("when", writer);

            // <name>
            XmlWriterHelper.WriteOpt(
                writer,
                "name",
                _name);

            // <unit>
            XmlWriterHelper.WriteOpt(
                writer,
                "unit",
                _unit);

            // <sampling-interval>
            writer.WriteElementString(
                "sampling-interval",
                XmlConvert.ToString(_samplingInterval));

            // </exercise-samples>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time when the samples were collected.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="ApproximateDateTime"/> instance representing the date
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
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets the kind of information that is stored in this sample set.
        /// </summary>
        ///
        /// <remarks>
        /// The name encodes both the type of information that is stored and the units in
        /// which it is stored.
        ///
        /// The preferred vocabulary is exercise-sample-names.
        ///
        /// Example sample types and units. See the preferred vocabulary for others.
        /// Heartrate_BPM
        /// Distance_meters
        /// Position_LatLong
        /// Speed_m-per-s
        /// Pace_s-per-100m
        /// Power_watts
        /// Torque_Nm
        /// Cadence_RPM
        /// Temperature_celsius
        /// Altitude_meters
        /// AirPressure_kPa
        /// Steps_count
        /// AerobicSteps_count
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public CodableValue Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.ExerciseSampleNameMandatory);
                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets the unit of measure for the samples.
        /// </summary>
        ///
        /// <remarks>
        /// The preferred vocabulary is exercise-units.
        ///
        /// The appropriate units are defined by the code used for the Name property. For example, if the
        /// name property is set to the entry Power_watts, the Unit should be coded to watts.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public CodableValue Unit
        {
            get { return _unit; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Unit), Resources.ExerciseSampleUnitMandatory);
                _unit = value;
            }
        }

        private CodableValue _unit;

        /// <summary>
        /// Gets or sets the initial sampling interval between samples, in seconds.
        /// </summary>
        /// <summary>
        /// The samples data format supports modifying the sampling interval in the middle of a
        /// set of samples. See the <see cref="ExerciseSamplesData"/> class for more information.
        /// </summary>
        ///
        /// <returns>
        /// An double representing the sampling interval in seconds.
        /// </returns>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public double SamplingInterval
        {
            get { return _samplingInterval; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(SamplingInterval), Resources.SamplingIntervalMustBePositive);
                }

                _samplingInterval = value;
            }
        }

        private double _samplingInterval = double.NaN;

        /// <summary>
        /// Gets the description of a exercise samples instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the exercise samples item.
        /// </returns>
        ///
        public override string ToString()
        {
            return _name.Text;
        }

        /// <summary>
        /// The sample data.
        /// </summary>
        /// <remarks>
        /// The sample data is exposed as a <see cref="ExerciseSamplesData"/> instance.
        ///
        /// To get the sample data when fetching an instance of the ExerciseSamples thing type, you must specify that the other-data section
        /// be returned to access the ExerciseSamplesData.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// If the sampling interval has not be set before the property is referenced.
        /// </exception>
        public ExerciseSamplesData ExerciseSamplesData => _sampleData ?? (_sampleData = CreateExerciseSampleData());

        private ExerciseSamplesData _sampleData;

        /// <summary>
        /// Sample name for Heartrate_BPM.
        /// </summary>
        public const string HeartrateBPM = "Heartrate_BPM";

        /// <summary>
        /// Sample name for Distance_meters.
        /// </summary>
        public const string DistanceMeters = "Distance_meters";

        /// <summary>
        /// Sample name for Position_LatLong.
        /// </summary>
        public const string PositionLatLong = "Position_LatLong";

        /// <summary>
        /// Sample name for Speed_m-per-s.
        /// </summary>
        public const string SpeedMPerS = "Speed_m-per-s";

        /// <summary>
        /// Sample name for Pace_s-per-100m.
        /// </summary>
        public const string PaceSPer100m = "Pace_s-per-100m";

        /// <summary>
        /// Sample name for Power_watts.
        /// </summary>
        public const string PowerWatts = "Power_watts";

        /// <summary>
        /// Sample name for Torque_Nm.
        /// </summary>
        public const string TorqueNm = "Torque_Nm";

        /// <summary>
        /// Sample name for Cadence_RPM.
        /// </summary>
        public const string CadenceRPM = "Cadence_RPM";

        /// <summary>
        /// Sample name for Temperature_celsius.
        /// </summary>
        public const string TemperatureCelsius = "Temperature_celsius";

        /// <summary>
        /// Sample name for Altitude_meters.
        /// </summary>
        public const string AltitudeMeters = "Altitude_meters";

        /// <summary>
        /// Sample name for AirPressure_kPa.
        /// </summary>
        public const string AirPressureKPa = "AirPressure_kPa";

        /// <summary>
        /// Sample name for Steps_count.
        /// </summary>
        public const string StepsCount = "Steps_count";

        /// <summary>
        /// Sample name for AerobicSteps_count.
        /// </summary>
        public const string AerobicStepsCount = "AerobicSteps_count";

        private ExerciseSamplesData CreateExerciseSampleData()
        {
            if (double.IsNaN(SamplingInterval))
            {
                throw new InvalidOperationException(Resources.SamplingIntervalMustBeSet);
            }

            ExerciseSamplesData sampleData;

            BlobStore store = GetBlobStore(default(HealthRecordAccessor));
            Blob blob = store[string.Empty];

            if (blob == null)
            {
                sampleData = new ExerciseSamplesData(null, string.Empty, "text/csv") { SamplingInterval = SamplingInterval };
            }
            else
            {
                sampleData = new ExerciseSamplesData(
                    blob.ReadAsString(),
                    blob.ContentEncoding,
                    blob.ContentType)
                { SamplingInterval = SamplingInterval };
            }

            return sampleData;
        }
    }
}
