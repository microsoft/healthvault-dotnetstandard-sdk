// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information pertaining to a portion of the overall exercise.
    /// </summary>
    ///
    /// <remarks>
    /// This is typically used to store information about separate laps in a race or individual events within
    /// a triathlon.
    /// </remarks>
    ///
    public class ExerciseSegment : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseSegment"/> class
        /// with default values.
        /// </summary>
        ///
        public ExerciseSegment()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseSegment"/> class
        /// with the specified values.
        /// </summary>
        /// <param name="activity">The type of activity for this segment.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="activity"/> is <b>null</b>.
        /// </exception>
        public ExerciseSegment(CodableValue activity)
        {
            this.Activity = activity;
        }

        /// <summary>
        /// Populates the data for the lap from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the lap.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // activity
            this.activity = XPathHelper.GetOptNavValue<CodableValue>(navigator, "activity");

            // title
            this.title =
                XPathHelper.GetOptNavValue(navigator, "title");

            // distance
            this.distance = XPathHelper.GetOptNavValue<Length>(navigator, "distance");

            // duration
            this.duration =
                XPathHelper.GetOptNavValueAsDouble(navigator, "duration");

            // offset
            this.offset =
                XPathHelper.GetOptNavValueAsDouble(navigator, "offset");

            // details
            XPathNodeIterator detailsIterator = navigator.Select("detail");

            this.details.Clear();
            foreach (XPathNavigator detailsNavigator in detailsIterator)
            {
                ExerciseDetail exerciseDetail = new ExerciseDetail();
                exerciseDetail.ParseXml(detailsNavigator);

                this.details.Add(exerciseDetail.Name.Value, exerciseDetail);
            }
        }

        /// <summary>
        /// Writes the lap to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the lap.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the lap data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The Activity property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.activity, "ExerciseSegmentActivityNotSet");

            writer.WriteStartElement(nodeName);

            // <activity>
            XmlWriterHelper.WriteOpt(
                writer,
                "activity",
                this.activity);

            // <title>
            XmlWriterHelper.WriteOptString(
                writer,
                "title",
                this.title);

            // <distance>
            if (this.distance != null)
            {
                this.distance.WriteXml("distance", writer);
            }

            // <duration>
            if (this.duration != null)
            {
                XmlWriterHelper.WriteOptDouble(writer, "duration", this.duration);
            }

            // <offset>
            if (this.offset != null)
            {
                XmlWriterHelper.WriteOptDouble(writer, "offset", this.offset);
            }

            // <details>
            foreach (ExerciseDetail exerciseDetail in this.details.Values)
            {
                exerciseDetail.WriteXml("detail", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the type of activity for this segment of the exercise.
        /// </summary>
        ///
        /// <remarks>
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
                Validator.ThrowIfArgumentNull(value, "Activity", "ExerciseActivityMandatory");
                this.activity = value;
            }
        }

        private CodableValue activity;

        /// <summary>
        /// Gets or sets a descriptive title for this segment.
        /// </summary>
        ///
        /// <value>
        /// String.
        /// </value>
        ///
        /// <remarks>
        /// Examples: Lap 1, bicycle leg, first half.
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
        /// Gets or sets the distance covered in the segment.
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
        /// Gets or sets the duration of the segment in minutes.
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
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && (double)value <= 0.0,
                    "Duration",
                    "ExerciseDurationNotPositive");
                this.duration = value;
            }
        }

        private double? duration;

        /// <summary>
        /// Gets or sets the offset in minutes of the segment from the start of exercise.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if there is no offset.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is negative or zero when set.
        /// </exception>
        ///
        public double? Offset
        {
            get { return this.offset; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && (double)value <= 0.0,
                    "Offset",
                    "ExerciseOffsetNotPositive");
                this.offset = value;
            }
        }

        private double? offset;

        /// <summary>
        /// Gets additional information about the segment.
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
        /// Gets a string representation of the ExerciseSegment item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the ExerciseSegment item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.title != null)
            {
                return
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "ExerciseSegmentToStringFormat"),
                        this.title,
                        this.activity.Text);
            }

            return this.activity.Text;
        }
    }
}
