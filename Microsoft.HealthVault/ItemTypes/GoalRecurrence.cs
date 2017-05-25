// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// This sub-type allows specifying recurrence for Goals.
    /// </summary>
    ///
    public class GoalRecurrence : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GoalRecurrence"/> class with default values.
        /// </summary>
        ///
        public GoalRecurrence()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GoalRecurrence"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="interval">
        /// Specifies the recurrence interval of the goal. For example, day, week, year, etc.
        /// </param>
        /// <param name="timesInInterval">
        /// Specifies the number of times the goal's target is intended to be achieved during the interval.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="interval"/> is <b>null</b>.
        /// </exception>
        ///
        public GoalRecurrence(
            CodableValue interval,
            int timesInInterval)
        {
            Interval = interval;
            TimesInInterval = timesInInterval;
        }

        /// <summary>
        /// Populates this <see cref="GoalRecurrence"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the GoalRecurrence data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException(
                    nameof(navigator),
                    Resources.ParseXmlNavNull);
            }

            _interval = new CodableValue();
            _interval.ParseXml(navigator.SelectSingleNode("interval"));
            int? timesInInterval = XPathHelper.GetOptNavValueAsInt(navigator, "times-in-interval");
            Validator.ThrowInvalidIfNull(timesInInterval, Resources.TimesInIntervalNull);
            timesInInterval = timesInInterval.Value;
        }

        /// <summary>
        /// Writes the XML representation of the GoalRecurrence into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the GoalRecurrence should be
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Interval"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException(
                    Resources.WriteXmlEmptyNodeName,
                    nameof(nodeName));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(
                    nameof(writer),
                    Resources.WriteXmlNullWriter);
            }

            if (_interval == null)
            {
                throw new ThingSerializationException(
                    Resources.GoalRecurrenceIntervalNullValue);
            }

            writer.WriteStartElement(nodeName);

            _interval.WriteXml("interval", writer);
            XmlWriterHelper.WriteOptInt(writer, "times-in-interval", _timesInInterval);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets specifies the recurrence interval of the goal. For example, day, week, year, etc.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about interval the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Interval
        {
            get
            {
                return _interval;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value",
                        Resources.GoalRecurrenceIntervalNullValue);
                }

                _interval = value;
            }
        }

        private CodableValue _interval;

        /// <summary>
        /// Gets or sets specifies the number of times the goal's target is intended to be achieved during the interval.
        /// </summary>
        ///
        /// <remarks>
        /// For example, the goal "exercise for 30 minutes, 4 times per week" would be represented as: an interval of a "week", a times-in-interval of 4, and a goal target of 30 minutes.
        /// </remarks>
        ///
        public int TimesInInterval
        {
            get
            {
                return _timesInInterval;
            }

            set
            {
                _timesInInterval = value;
            }
        }

        private int _timesInInterval;

        /// <summary>
        /// Gets a string representation of the GoalRecurrence.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the GoalRecurrence.
        /// </returns>
        ///
        public override string ToString()
        {
            return string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.GoalRecurrenceFormat,
                        TimesInInterval.ToString(CultureInfo.CurrentCulture),
                        Interval.Text);
        }
    }
}
