// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
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
            this.When = when;
            this.Activity = activity;
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
            this.when = new ApproximateDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // activity
            this.activity = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "activity");

            // title
            this.title =
                XPathHelper.GetOptNavValue(itemNav, "title");

            this.distance =
                    XPathHelper.GetOptNavValue<Length>(itemNav, "distance");

            // duration
            this.duration =
                XPathHelper.GetOptNavValueAsDouble(itemNav, "duration");

            // detail
            XPathNodeIterator detailsIterator = itemNav.Select("detail");

            this.details.Clear();
            foreach (XPathNavigator detailsNavigator in detailsIterator)
            {
                ExerciseDetail exerciseDetail = new ExerciseDetail();
                exerciseDetail.ParseXml(detailsNavigator);

                this.details[exerciseDetail.Name.Value] = exerciseDetail;
            }

            // segment
            XPathNodeIterator segmentsIterator = itemNav.Select("segment");

            this.segments.Clear();
            foreach (XPathNavigator segmentsNavigator in segmentsIterator)
            {
                ExerciseSegment exerciseSegment = new ExerciseSegment();
                exerciseSegment.ParseXml(segmentsNavigator);

                this.segments.Add(exerciseSegment);
            }
        }

        /// <summary>
        /// Writes the exercise data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the exericse data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// If <see cref="Activity"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(this.activity, Resources.ExerciseActivityNotSet);

            // <exercise>
            writer.WriteStartElement("exercise");

            // <when>
            this.when.WriteXml("when", writer);

            // <activity>
            this.activity.WriteXml("activity", writer);

            // <title>
            XmlWriterHelper.WriteOptString(
                writer,
                "title",
                this.title);

            // <distance>
            if (this.distance != null)
            {
                XmlWriterHelper.WriteOpt(writer, "distance", this.distance);
            }

            // <duration>
            if (this.duration != null)
            {
                XmlWriterHelper.WriteOptDouble(writer, "duration", this.duration);
            }

            // <detail>
            foreach (ExerciseDetail exerciseDetail in this.details.Values)
            {
                exerciseDetail.WriteXml("detail", writer);
            }

            // <segment>
            foreach (ExerciseSegment exerciseSegment in this.segments)
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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private ApproximateDateTime when = new ApproximateDateTime();

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
        /// If <paramref name="Activity"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Activity
        {
            get { return this.activity; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Activity), Resources.ExerciseActivityMandatory);
                this.activity = value;
            }
        }

        private CodableValue activity;

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
            get { return this.title; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Title");
                this.title = value;
            }
        }

        private string title;

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
            get { return this.distance; }
            set { this.distance = value; }
        }

        private Length distance;

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
            get { return this.duration; }

            set
            {
                if (value != null && (double)value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Duration), Resources.ExerciseDurationNotPositive);
                }

                this.duration = value;
            }
        }

        private double? duration;

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
        public IDictionary<string, ExerciseDetail> Details => this.details;

        private readonly Dictionary<string, ExerciseDetail> details = new Dictionary<string, ExerciseDetail>();

        /// <summary>
        /// Gets information pertaining to a portion of the overall exercise.
        /// </summary>
        ///
        /// <value>
        /// A collection of <see cref="ExerciseSegment" /> items.
        /// </value>
        ///
        public Collection<ExerciseSegment> Segments => this.segments;

        private readonly Collection<ExerciseSegment> segments = new Collection<ExerciseSegment>();

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

            result.Append(this.activity.Text);

            if (this.title != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.title);
            }

            if (this.distance != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    this.distance.ToString());
            }

            if (this.duration != null)
            {
                result.AppendFormat(
                    Resources.ExerciseToStringFormatDuration,
                    this.duration.Value.ToString());
            }

            return result.ToString();
        }
    }
}
