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
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a weekly alert schedule.
    /// </summary>
    ///
    public class Alert : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Alert"/> class with default values.
        /// </summary>
        ///
        public Alert()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Alert"/> class with the
        /// specified days and times.
        /// </summary>
        ///
        /// <param name="daysOfWeek">
        /// The days of the week the alert should be triggered.
        /// </param>
        ///
        /// <param name="times">
        /// The times in those days that the alert should be triggered.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="daysOfWeek"/>  or <paramref name="times"/>
        /// parameter is <b>null</b>.
        /// </exception>
        ///
        public Alert(
            IEnumerable<DayOfWeek> daysOfWeek,
            IEnumerable<ApproximateTime> times)
        {
            Validator.ThrowIfArgumentNull(daysOfWeek, nameof(daysOfWeek), Resources.AlertDOWNull);
            Validator.ThrowIfArgumentNull(times, nameof(times), Resources.AlertTimesNull);

            foreach (DayOfWeek dow in daysOfWeek)
            {
                _daysOfWeek.Add(dow);
            }

            foreach (ApproximateTime time in times)
            {
                _times.Add(time);
            }
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the address information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNodeIterator dowIterator = navigator.Select("dow");

            foreach (XPathNavigator dowNav in dowIterator)
            {
                _daysOfWeek.Add((DayOfWeek)(dowNav.ValueAsInt - 1));
            }

            XPathNodeIterator timeIterator = navigator.Select("time");

            foreach (XPathNavigator timeNav in timeIterator)
            {
                ApproximateTime time = new ApproximateTime();
                time.ParseXml(timeNav);
                _times.Add(time);
            }
        }

        /// <summary>
        /// Writes the XML representation of the alert into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the alert.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the alert should be
        /// written.
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="DaysOfWeek"/> property has no days set, or the <see cref="Times"/> property contains no times.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            if (_daysOfWeek.Count == 0)
            {
                throw new ThingSerializationException(Resources.AlertDaysNotSet);
            }

            if (_times.Count == 0)
            {
                throw new ThingSerializationException(Resources.AlertTimesNotSet);
            }

            writer.WriteStartElement(nodeName);

            foreach (DayOfWeek dow in _daysOfWeek)
            {
                // The DayOfWeek enum starts at 0 whereas the XSD starts at
                // 1.

                writer.WriteElementString(
                    "dow",
                    ((int)dow + 1).ToString(CultureInfo.InvariantCulture));
            }

            foreach (ApproximateTime time in _times)
            {
                time.WriteXml("time", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a collection of the days of the week which the alert applies
        /// to.
        /// </summary>
        ///
        /// <value>
        /// A collection of days of the week.
        /// </value>
        ///
        /// <remarks>
        /// To add days to be alerted, add <see cref="System.DayOfWeek"/>
        /// values to the returned collection.
        /// </remarks>
        ///
        public Collection<DayOfWeek> DaysOfWeek => _daysOfWeek;

        private readonly Collection<DayOfWeek> _daysOfWeek =
            new Collection<DayOfWeek>();

        /// <summary>
        /// Gets a collection of the times in each of the specified days to
        /// be alerted.
        /// </summary>
        ///
        /// <value>
        /// A collection of times.
        /// </value>
        ///
        /// <remarks>
        /// To add times to be alerted, add <see cref="ApproximateTime"/>
        /// instances to the returned collection.
        /// </remarks>
        ///
        public Collection<ApproximateTime> Times => _times;

        private readonly Collection<ApproximateTime> _times =
            new Collection<ApproximateTime>();

        /// <summary>
        /// Gets a string representation of the alert.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the alert.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder daysOfWeek = new StringBuilder(50);

            for (int daysIndex = 0; daysIndex < DaysOfWeek.Count; ++daysIndex)
            {
                if (daysIndex == 0)
                {
                    daysOfWeek.Append(ResourceUtilities.ResourceManager.GetString(DaysOfWeek[daysIndex].ToString()));
                }
                else
                {
                    daysOfWeek.AppendFormat(
                        Resources.ListFormat,
                        ResourceUtilities.ResourceManager.GetString(DaysOfWeek[daysIndex].ToString()));
                }
            }

            StringBuilder times = new StringBuilder(200);
            for (int index = 0; index < Times.Count; ++index)
            {
                if (index == 0)
                {
                    times.AppendFormat(
                        Resources.AlertTimeFormat,
                         Times[index].ToString());
                }
                else
                {
                    times.AppendFormat(
                        Resources.ListFormat,
                         Times[index].ToString());
                }
            }

            return string.Format(
                Resources.AlertToStringFormat,
                daysOfWeek.ToString(),
                times.ToString());
        }
    }
}
