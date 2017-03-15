// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Care plan task recurrence representation.
    /// </summary>
    ///
    /// <remarks>
    /// For recurrence rules that cannot be expressed using iCalendar recurrence format, use interval and times-in-interval fields. Ex: Two times in a week.
    /// </remarks>
    ///
    public class CarePlanTaskRecurrence : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="CarePlanTaskRecurrence"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the CarePlanTaskRecurrence data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.icalRecurrence = XPathHelper.GetOptNavValue(navigator, "ical-recurrence");
            this.interval = XPathHelper.GetOptNavValue<CodableValue>(navigator, "interval");
            this.timesInInterval = XPathHelper.GetOptNavValueAsInt(navigator, "times-in-interval");
        }

        /// <summary>
        /// Writes the XML representation of the CarePlanTaskRecurrence into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlanTaskRecurrence should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "WriteXmlEmptyNodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowInvalidIf(
                (this.interval != null && this.timesInInterval == null) ||
                (this.interval == null && this.timesInInterval != null),
                "CarePlanTaskRecurrenceIntervalAndTimesBothSet");

            writer.WriteStartElement("recurrence");
            {
                XmlWriterHelper.WriteOptString(writer, "ical-recurrence", this.icalRecurrence);
                XmlWriterHelper.WriteOpt(writer, "interval", this.interval);
                XmlWriterHelper.WriteOptInt(writer, "times-in-interval", this.timesInInterval);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the ical-recurrence
        /// </summary>
        /// <remarks>
        ///  Uses the iCalendar format for recurrence specification as per RFC 2445, Section 4.3.10.
        /// </remarks>
        ///
        public string IcalRecurrence
        {
            get
            {
                return this.icalRecurrence;
            }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "IcalRecurrence");
                Validator.ThrowIfStringIsWhitespace(value, "IcalRecurrence");

                this.icalRecurrence = value;
                this.interval = null;
                this.timesInInterval = null;
            }
        }

        private string icalRecurrence;

        /// <summary>
        /// Gets or sets the recurrence interval.
        /// </summary>
        /// <remarks>
        /// For example: day, month, year.
        /// </remarks>
        ///
        public CodableValue Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                this.interval = value;
                this.icalRecurrence = null;
            }
        }

        private CodableValue interval;

        /// <summary>
        /// Gets or sets the number of times in the interval.
        /// </summary>
        /// <remarks>
        /// Example: Two times in a week would be stored as interval = week, times-in-interval = 2.
        /// </remarks>
        ///
        public int? TimesInInterval
        {
            get
            {
                return this.timesInInterval;
            }

            set
            {
                Validator.ThrowArgumentExceptionIf(
                    value <= 0,
                    "TimesInInterval",
                    "CarePlanTaskRecurrenceInvalidTimeInInterval");

                this.timesInInterval = value;
                this.icalRecurrence = null;
            }
        }

        private int? timesInInterval;

        /// <summary>
        /// Gets a string representation of the CarePlanTaskRecurrence.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CarePlanTaskRecurrence.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.icalRecurrence != null)
            {
                return this.icalRecurrence;
            }

            if (this.interval != null &&
                this.timesInInterval != null)
            {
                return string.Format(
                    CultureInfo.CurrentUICulture,
                    ResourceRetriever.GetResourceString("CarePlanTaskRecurrenceFormat"),
                    this.timesInInterval,
                    this.interval.Text);
            }

            return string.Empty;
        }
    }
}
