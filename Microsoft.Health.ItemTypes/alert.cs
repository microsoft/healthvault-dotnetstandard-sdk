// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a weekly alert schedule.
    /// </summary>
    /// 
    public class Alert : HealthRecordItemData
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
            Validator.ThrowIfArgumentNull(daysOfWeek, "daysOfWeek", "AlertDOWNull");
            Validator.ThrowIfArgumentNull(times, "times", "AlertTimesNull");

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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="DaysOfWeek"/> property has no days set, or the 
        /// <see cref="Times"/> property contains no times.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {

            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            Validator.ThrowSerializationIf(
                _daysOfWeek.Count == 0,
                "AlertDaysNotSet");

            Validator.ThrowSerializationIf(
                _times.Count == 0,
                "AlertTimesNotSet");

            writer.WriteStartElement(nodeName);

            foreach (DayOfWeek dow in _daysOfWeek)
            {
                // The DayOfWeek enum starts at 0 whereas the XSD starts at
                // 1.

                writer.WriteElementString(
                    "dow",
                    (((int)dow) + 1).ToString(CultureInfo.InvariantCulture));
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
        public Collection<DayOfWeek> DaysOfWeek
        {
            get { return _daysOfWeek; }
        }
        private Collection<DayOfWeek> _daysOfWeek =
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
        public Collection<ApproximateTime> Times
        {
            get { return _times; }
        }
        private Collection<ApproximateTime> _times =
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
                    daysOfWeek.Append(
                        ResourceRetriever.GetResourceString(
                            DaysOfWeek[daysIndex].ToString()));
                }
                else
                {
                    daysOfWeek.AppendFormat(
                        ResourceRetriever.GetResourceString(
                            "ListFormat"),
                         ResourceRetriever.GetResourceString(
                            DaysOfWeek[daysIndex].ToString()));     
                }
            }

            StringBuilder times = new StringBuilder(200);
            for (int index=0; index < Times.Count; ++index)
            {
                if (index == 0)
                {
                    times.AppendFormat(
                        ResourceRetriever.GetResourceString(
                            "AlertTimeFormat"),
                         Times[index].ToString());
                }
                else
                {
                    times.AppendFormat(
                        ResourceRetriever.GetResourceString(
                            "ListFormat"),
                         Times[index].ToString());
                }
            }

            return String.Format(
                ResourceRetriever.GetResourceString(
                    "AlertToStringFormat"),
                daysOfWeek.ToString(),
                times.ToString());
        }
    }
}
