// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using NodaTime;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a date and time.
    /// </summary>
    ///
    /// <remarks>
    /// A <see cref="HealthServiceDateTime"/> is different from a <see cref="System.DateTime"/>
    /// in that the time is optional and it can be determined if it was
    /// specified. Also, the time zone is optional and is not interpreted in
    /// any way by HealthVault.
    /// </remarks>
    ///
    public class HealthServiceDateTime
        : ItemBase,
            IComparable,
            IComparable<HealthServiceDateTime>,
            IComparable<LocalDateTime>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/> class
        /// with the date set to the current date and the time not set.
        /// </summary>
        ///
        public HealthServiceDateTime()
        {
            DateTime now = DateTime.Now;
            _date =
                new HealthServiceDate(
                    now.Year,
                    now.Month,
                    now.Day);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="date">
        /// The date.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="date"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime(HealthServiceDate date)
        {
            Date = date;
            Time = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/>
        /// class with the specified date and time.
        /// </summary>
        ///
        /// <param name="date">
        /// The date.
        /// </param>
        ///
        /// <param name="time">
        /// The approximate time.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="date"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime(HealthServiceDate date, ApproximateTime time)
        {
            Date = date;
            Time = time;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/>
        /// class with the specified date, time, and time zone.
        /// </summary>
        ///
        /// <param name="date">
        /// The date.
        /// </param>
        ///
        /// <param name="time">
        /// The approximate time.
        /// </param>
        ///
        /// <param name="timeZone">
        /// The optional time zone for the <paramref name="time"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="date"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime(
            HealthServiceDate date,
            ApproximateTime time,
            CodableValue timeZone)
        {
            Date = date;
            Time = time;
            TimeZone = timeZone;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/>
        /// class from the specified <see cref="LocalDateTime" /> instance.
        /// </summary>
        /// <param name="dateTime">
        /// The date and time used to construct the HealthVault date and time.
        /// </param>
        public HealthServiceDateTime(LocalDateTime dateTime)
        {
            _date =
                new HealthServiceDate(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day);

            _time =
                new ApproximateTime(
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second,
                    dateTime.Millisecond);
        }

        /// <summary>
        /// Populates the data for the date and time from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the date and time.
        /// </param>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _date = new HealthServiceDate();
            _date.ParseXml(navigator.SelectSingleNode("date"));

            XPathNavigator timeNav = navigator.SelectSingleNode("time");
            if (timeNav != null)
            {
                _time = new ApproximateTime();
                _time.ParseXml(timeNav);
            }
            else
            {
                _time = null;
            }

            XPathNavigator tzNav =
                navigator.SelectSingleNode("tz");

            if (tzNav != null)
            {
                _timeZone = new CodableValue();
                _timeZone.ParseXml(tzNav);
            }
        }

        /// <summary>
        /// Writes the date and time to the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer element for the date and time.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the date and time to.
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
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            _date.WriteXml("date", writer);

            if (_time != null)
            {
                _time.WriteXml("time", writer);
            }

            if (_timeZone != null)
            {
                _timeZone.WriteXml("tz", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Casts the <see cref="HealthServiceDateTime"/> instance to a <see cref="LocalDateTime"/> object.
        /// </summary>
        /// <param name="wcDateTime">
        /// The <see cref="HealthServiceDateTime"/> instance to cast.
        /// </param>
        /// <returns>
        /// A LocalDateTime instance with the appropriate fields populated by the
        /// <see cref="HealthServiceDateTime"/> values.
        /// </returns>
        public static explicit operator LocalDateTime(HealthServiceDateTime wcDateTime)
        {
            int year = wcDateTime.Date.Year;
            int month = wcDateTime.Date.Month;
            int day = wcDateTime.Date.Day;

            LocalDateTime result;
            if (wcDateTime.Time != null)
            {
                int hour = wcDateTime.Time.Hour;
                int minute = wcDateTime.Time.Minute;
                int second =
                    wcDateTime.Time.Second == null ?
                        0 : (int)wcDateTime.Time.Second;

                int millisecond =
                    wcDateTime.Time.Millisecond == null ?
                        0 : (int)wcDateTime.Time.Millisecond;

                result = new LocalDateTime(year, month, day, hour, minute, second, millisecond);
            }
            else
            {
                result = new LocalDateTime(year, month, day, 0, 0);
            }

            return result;
        }

        /// <summary>
        /// Converts the <see cref="HealthServiceDateTime"/> instance to a <see cref="LocalDateTime"/> object.
        /// </summary>
        /// <returns>
        /// A <see cref="LocalDateTime"/> instance with the appropriate fields populated by the
        /// <see cref="HealthServiceDateTime"/> values.
        /// </returns>
        public LocalDateTime ToLocalDateTime()
        {
            return (LocalDateTime)this;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to the current year, month, and day.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> on set.
        /// </exception>
        ///
        public HealthServiceDate Date
        {
            get { return _date; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Date), Resources.HealthServiceDateTimeDateNull);
                _date = value;
            }
        }

        private HealthServiceDate _date;

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateTime"/> instance representing the time.
        /// </returns>
        ///
        /// <remarks>
        /// If the time isn't known it can be set to <b>null</b>. This value defaults
        /// to an absent time. The reference is valid but the time will not be
        /// stored unless the hour and minute are set.
        /// </remarks>
        ///
        public ApproximateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private ApproximateTime _time = new ApproximateTime();

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the time zone.
        /// </returns>
        ///
        /// <remarks>
        /// If the time zone is unknown, it can be set to <b>null</b>. This value defaults
        /// to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue TimeZone
        {
            get { return _timeZone; }
            set { _timeZone = value; }
        }

        private CodableValue _timeZone;

        internal string ToString(IFormatProvider formatProvider)
        {
            StringBuilder result = new StringBuilder(40);

            result.Append(Date.ToString(formatProvider));

            if (Time != null)
            {
                string time = Time.ToString(formatProvider);

                if (!string.IsNullOrEmpty(time))
                {
                    result.Append(" ");
                    result.Append(time);
                }
            }

            if (TimeZone != null)
            {
                result.Append(" ");
                result.Append(TimeZone);
            }

            return result.ToString();
        }

        #region IComparable

        /// <summary>
        /// Compares the specified object to this <see cref="HealthServiceDateTime"/>
        /// object.
        /// </summary>
        ///
        /// <param name="obj">
        /// The object to be compared.
        /// </param>
        ///
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the
        /// objects being compared. If the result is less than zero, the
        /// instance is less than <paramref name="obj"/>. If the result is zero
        /// the instance is equal to <paramref name="obj"/>. If the result is
        /// greater than zero, the instance is greater than
        /// <paramref name="obj"/>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not a <see cref="HealthServiceDateTime"/>
        /// or <see cref="LocalDateTime"/> object.
        /// </exception>
        ///
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            HealthServiceDateTime hsDate = obj as HealthServiceDateTime;
            if (hsDate == null)
            {
                try
                {
                    LocalDateTime dt = (LocalDateTime)obj;
                    return CompareTo(dt);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(Resources.DateCompareToInvalidType, nameof(obj));
                }
            }

            return CompareTo(hsDate);
        }

        /// <summary>
        /// Compares the specified object to this <see cref="HealthServiceDateTime"/>
        /// object.
        /// </summary>
        ///
        /// <param name="other">
        /// The date to be compared.
        /// </param>
        ///
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the
        /// objects being compared. If the result is less than zero, the
        /// instance is less than <paramref name="other"/>. If the result is zero
        /// the instance is equal to <paramref name="other"/>. If the result is
        /// greater than zero, the instance is greater than
        /// <paramref name="other"/>.
        /// </returns>
        ///
        public int CompareTo(HealthServiceDateTime other)
        {
            if (other == null)
            {
                return 1;
            }

            var result = Date.CompareTo(other.Date);

            if (result != 0 ||
                Time == null ||
                (Time.Hour == 0 &&
                    Time.Minute == 0 &&
                    Time.Second == null))
            {
                return result;
            }

            return Time.CompareTo(other.Time);
        }

        /// <summary>
        /// Compares the specified object to this HealthServiceDateTime object.
        /// </summary>
        ///
        /// <param name="other">
        /// The date to be compared.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the
        /// objects being compared. If the result is less than zero, the
        /// instance is less than <paramref name="other"/>. If the result is zero
        /// the instance is equal to <paramref name="other"/>. If the result is
        /// greater than zero, the instance is greater than
        /// <paramref name="other"/>.
        /// </returns>
        public int CompareTo(LocalDateTime other)
        {
            if (Date.Year > other.Year)
            {
                return 1;
            }

            if (Date.Year < other.Year)
            {
                return -1;
            }

            if (Date.Month > other.Month)
            {
                return 1;
            }

            if (Date.Month < other.Month)
            {
                return -1;
            }

            if (Date.Day > other.Day)
            {
                return 1;
            }

            if (Date.Day < other.Day)
            {
                return -1;
            }

            return Time.CompareTo(other.TimeOfDay);
        }

        #endregion IComparable

        #region Equals

        /// <summary>
        /// Gets a value indicating whether the specified object is equal to this object.
        /// </summary>
        ///
        /// <param name="obj">
        /// The object to be compared.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if the <paramref name="obj"/> is a
        /// <see cref="HealthServiceDateTime"/> object and the year, month, and
        /// day exactly match the year, month, and day of this object; otherwise,
        /// <b>false</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not a <see cref="HealthServiceDateTime"/>
        /// object.
        /// </exception>
        ///
        public override bool Equals(object obj)
        {
            Type objectType = obj?.GetType();
            if (objectType != this.GetType() && objectType != typeof(LocalDateTime))
            {
                return false;
            }

            return CompareTo(obj) == 0;
        }

        /// <summary>
        /// See the base class documentation.
        /// </summary>
        ///
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Equals

        #region Operators

        /// <summary>
        /// Gets a value indicating whether the specified object is equal to the specified
        /// date.
        /// </summary>
        ///
        /// <param name="date">
        /// The date object to be compared.
        /// </param>
        ///
        /// <param name="secondInstance">
        /// The second object to be compared.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if the year, month, and day of the <paramref name="date"/>
        /// exactly match the year, month, and day of <paramref name="secondInstance"/>;
        /// otherwise, <b>false</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a
        /// <see cref="HealthServiceDateTime"/> object.
        /// </exception>
        ///
        public static bool operator ==(HealthServiceDateTime date, object secondInstance)
        {
            if ((object)date == null)
            {
                return secondInstance == null;
            }

            return date.Equals(secondInstance);
        }

        /// <summary>
        /// Gets a value indicating whether the specified object is not equal to the specified
        /// date.
        /// </summary>
        ///
        /// <param name="date">
        /// The date object to be compared.
        /// </param>
        ///
        /// <param name="secondInstance">
        /// The second object to be compared.
        /// </param>
        ///
        /// <returns>
        /// <b>false</b> if the year, month, and day of the <paramref name="date"/>
        /// exactly match the year, month, and day of <paramref name="secondInstance"/>;
        /// otherwise, <b>true</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a
        /// <see cref="HealthServiceDateTime"/> object.
        /// </exception>
        ///
        public static bool operator !=(HealthServiceDateTime date, object secondInstance)
        {
            if (date == null)
            {
                return secondInstance != null;
            }

            return !date.Equals(secondInstance);
        }

        /// <summary>
        /// Gets a value indicating whether the specified date is greater than
        /// the specified object.
        /// </summary>
        ///
        /// <param name="date">
        /// The date object to be compared.
        /// </param>
        ///
        /// <param name="secondInstance">
        /// The second object to be compared.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if the year, month, and day of the <paramref name="date"/>
        /// is greater than the year, month, and day of <paramref name="secondInstance"/>;
        /// otherwise, <b>false</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a <see cref="HealthServiceDateTime"/> object.
        /// </exception>
        ///
        public static bool operator >(HealthServiceDateTime date, object secondInstance)
        {
            if (date == null)
            {
                return secondInstance != null;
            }

            return date.CompareTo(secondInstance) > 0;
        }

        /// <summary>
        /// Gets a value indicating whether the specified date is less than the specified
        /// object.
        /// </summary>
        ///
        /// <param name="date">
        /// The date object to be compared.
        /// </param>
        ///
        /// <param name="secondInstance">
        /// The second object to be compared.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if the year, month, and day of the <paramref name="date"/>
        /// is less than the year, month, and day of <paramref name="secondInstance"/>;
        /// otherwise, <b>false</b>.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a <see cref="HealthServiceDateTime"/> object.
        /// </exception>
        ///
        public static bool operator <(HealthServiceDateTime date, object secondInstance)
        {
            if (date == null)
            {
                return secondInstance != null;
            }

            return date.CompareTo(secondInstance) < 0;
        }

        #endregion Operators
    }
}
