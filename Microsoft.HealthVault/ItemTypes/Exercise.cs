// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates an aerobic session.
    /// </summary>
    ///
    public class Exercise : ThingBase
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Exercise"/> class
        /// with default values.
        /// </summary>
        ///
        public Exercise()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Exercise"/> class with
        /// the specified date/time and activity.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the exercise occurred.
        /// </param>
        /// <param name="activity">
        /// The type of activity.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="activity"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Exercise(ApproximateDateTime when, CodableValue activity)
            : base(TypeId)
        {
            When = when;
            Activity = activity;
        }

        /// <summary>
        /// The unique identifier for the Exercise item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("85a21ddb-db20-4c65-8d30-33c899ccf612");

        /// <summary>
        /// Populates this <see cref="Exercise"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the exercise data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// an "exercise" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("exercise");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ExerciseUnexpectedNode);

            // when
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // activity
            _activity = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "activity");

            // title
            _title =
                XPathHelper.GetOptNavValue(itemNav, "title");

            _distance =
                    XPathHelper.GetOptNavValue<Length>(itemNav, "distance");

            // duration
            _duration =
                XPathHelper.GetOptNavValueAsDouble(itemNav, "duration");

            // detail
            XPathNodeIterator detailsIterator = itemNav.Select("detail");

            _details.Clear();
            foreach (XPathNavigator detailsNavigator in detailsIterator)
            {
                ExerciseDetail exerciseDetail = new ExerciseDetail();
                exerciseDetail.ParseXml(detailsNavigator);

                _details[exerciseDetail.Name.Value] = exerciseDetail;
            }

            // segment
            XPathNodeIterator segmentsIterator = itemNav.Select("segment");

            _segments.Clear();
            foreach (XPathNavigator segmentsNavigator in segmentsIterator)
            {
                ExerciseSegment exerciseSegment = new ExerciseSegment();
                exerciseSegment.ParseXml(segmentsNavigator);

                _segments.Add(exerciseSegment);
            }
        }

        /// <summary>
        /// Writes the exercise data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the exercise data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// If <see cref="Activity"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(_activity, Resources.ExerciseActivityNotSet);

            // <exercise>
            writer.WriteStartElement("exercise");

            // <when>
            _when.WriteXml("when", writer);

            // <activity>
            _activity.WriteXml("activity", writer);

            // <title>
            XmlWriterHelper.WriteOptString(
                writer,
                "title",
                _title);

            // <distance>
            if (_distance != null)
            {
                XmlWriterHelper.WriteOpt(writer, "distance", _distance);
            }

            // <duration>
            if (_duration != null)
            {
                XmlWriterHelper.WriteOptDouble(writer, "duration", _duration);
            }

            // <detail>
            foreach (ExerciseDetail exerciseDetail in _details.Values)
            {
                exerciseDetail.WriteXml("detail", writer);
            }

            // <segment>
            foreach (ExerciseSegment exerciseSegment in _segments)
            {
                exerciseSegment.WriteXml("segment", writer);
            }

            // </exercise>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the exercise occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="ApproximateDateTime"/> instance.
        /// The value defaults to the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
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

        private ApproximateDateTime _when = new ApproximateDateTime();

        /// <summary>
        /// Gets or sets the type of activity.
        /// </summary>
        ///
        /// <remarks>
        /// Stores the overall activity for the exercise period.
        ///
        /// Examples: Running, hiking, walking, golfing, dancing.
        /// The preferred vocabulary for route is "exercise-activities".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Activity
        {
            get { return _activity; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Activity), Resources.ExerciseActivityMandatory);
                _activity = value;
            }
        }

        private CodableValue _activity;

        /// <summary>
        /// Gets or sets a descriptive title for this activity.
        /// </summary>
        ///
        /// <value>
        /// String.
        /// </value>
        ///
        /// <remarks>
        /// Examples: Hiking up Mt. Baker, 3-day walk, Memorial day triathlon.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Title
        {
            get { return _title; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Title");
                _title = value;
            }
        }

        private string _title;

        /// <summary>
        /// Gets or sets the distance covered in the exercise.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> value representing the distance.
        /// </value>
        ///
        /// <remarks>
        /// Distances are stored in meters. The application
        /// must convert the distance entered by the user
        /// into meters and should also store the distance
        /// and units entered by the user in the display-value
        /// so that it can be displayed to the user in their
        /// preferred unit of measure when viewing the data.
        ///
        /// Set the value to <b>null</b> if there is no distance.
        /// </remarks>
        ///
        public Length Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        private Length _distance;

        /// <summary>
        /// Gets or sets the duration of the exercise in minutes.
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
            get { return _duration; }

            set
            {
                if (value != null && (double)value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Duration), Resources.ExerciseDurationNotPositive);
                }

                _duration = value;
            }
        }

        private double? _duration;

        /// <summary>
        /// Gets additional information about the exercise.
        /// </summary>
        ///
        /// <remarks>
        /// Details is a dictionary of the items, where the key is the
        /// string name of the item.
        /// </remarks>
        ///
        /// <value>
        /// A dictionary of <see cref="ExerciseDetail" /> items.
        /// </value>
        ///
        public IDictionary<string, ExerciseDetail> Details => _details;

        private readonly Dictionary<string, ExerciseDetail> _details = new Dictionary<string, ExerciseDetail>();

        /// <summary>
        /// Gets information pertaining to a portion of the overall exercise.
        /// </summary>
        ///
        /// <value>
        /// A collection of <see cref="ExerciseSegment" /> items.
        /// </value>
        ///
        public Collection<ExerciseSegment> Segments => _segments;

        private readonly Collection<ExerciseSegment> _segments = new Collection<ExerciseSegment>();

        /// <summary>
        /// Gets a string representation of the Exercise item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Exercise item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(128);

            result.Append(_activity.Text);

            if (_title != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    _title);
            }

            if (_distance != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    _distance.ToString());
            }

            if (_duration != null)
            {
                result.AppendFormat(
                    Resources.ExerciseToStringFormatDuration,
                    _duration.Value.ToString());
            }

            return result.ToString();
        }
    }
}
