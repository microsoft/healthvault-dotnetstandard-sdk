// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
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
        : HealthRecordItemData, 
            IComparable,
            IComparable<HealthServiceDateTime>,
            IComparable<DateTime>
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
            this.Date = date;
            this.Time = null;
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
            this.Date = date;
            this.Time = time;
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
            this.Date = date;
            this.Time = time;
            this.TimeZone = timeZone;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDateTime"/> 
        /// class from the specified DateTime instance.
        /// </summary>
        /// 
        /// <param name="dateTime">
        /// The date and time used to construct the HealthVault date and time.
        /// </param>
        /// 
        /// <remarks>
        /// The time zone is ignored.
        /// </remarks>
        /// 
        public HealthServiceDateTime(DateTime dateTime)
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
                    dateTime.Second);
            if (dateTime.Millisecond != 0)
            {
                _time.Millisecond = dateTime.Millisecond;
            }
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
        /// Casts the <see cref="HealthServiceDateTime"/> instance to a System.DateTime object.
        /// </summary>
        /// 
        /// <param name="wcDateTime">
        /// The <see cref="HealthServiceDateTime"/> instance to cast.
        /// </param>
        /// 
        /// <returns>
        /// A DateTime instance with the appropriate fields populated by the
        /// <see cref="HealthServiceDateTime"/> values.
        /// </returns>
        /// 
        public static explicit operator DateTime(HealthServiceDateTime wcDateTime) 
        {
            int year = wcDateTime.Date.Year;
            int month = wcDateTime.Date.Month;
            int day = wcDateTime.Date.Day;

            DateTime result;
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

                result = new DateTime(year, month, day, hour, minute, second, millisecond);
            }
            else
            {
                result = new DateTime(year, month, day);
            }
            return result;
        }

        /// <summary>
        /// Converts the <see cref="HealthServiceDateTime"/> instance to a System.DateTime object.
        /// </summary>
        /// 
        /// <returns>
        /// A DateTime instance with the appropriate fields populated by the
        /// <see cref="HealthServiceDateTime"/> values.
        /// </returns>
        /// 
        public DateTime ToDateTime()
        {
            return (DateTime)this;
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
                Validator.ThrowIfArgumentNull(value, "Date", "HealthServiceDateTimeDateNull");
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

        /// <summary>
        /// Gets the date and time as a string.
        /// </summary>
        /// 
        /// <returns>
        /// The date and time represented as a string.
        /// </returns>
        /// 
        /// <remarks>
        /// This method is not yet culture aware.
        /// </remarks>
        /// 
        public override string ToString()
        {
            return ToString(Thread.CurrentThread.CurrentCulture);
        }

        internal string ToString(IFormatProvider formatProvider)
        {
            StringBuilder result = new StringBuilder(40);
            string space = ResourceRetriever.GetSpace("sdkerrors");

            result.Append(Date.ToString(formatProvider));

            if (Time != null)
            {
                string time = Time.ToString(formatProvider);

                if (!String.IsNullOrEmpty(time))
                {
                    result.Append(space);
                    result.Append(time);
                }
            }

            if (TimeZone != null)
            {
                result.Append(space);
                result.Append(TimeZone.ToString());
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
        /// or <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public int CompareTo(object obj)
        {
            int result = 0;

            do // false loop
            {
                if (obj == null)
                {
                    result = 1;
                    break;
                }

                HealthServiceDateTime hsDate = obj as HealthServiceDateTime;
                if (hsDate == null)
                {
                    try
                    {
                        DateTime dt = (DateTime)obj;
                        result = CompareTo(dt);
                        break;
                    }
                    catch (InvalidCastException)
                    {
                        throw Validator.ArgumentException("obj", "DateCompareToInvalidType");
                    }
                }

                result = CompareTo(hsDate);
            } while (false);
            return result;
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
            int result = 0;

            do // false loop
            {
                if (other == null)
                {
                    result = 1;
                    break;
                }

                result = this.Date.CompareTo(other.Date);

                if (result != 0 ||
                    this.Time == null ||
                    (this.Time.Hour == 0 &&
                     this.Time.Minute == 0 &&
                     this.Time.Second == null))
                {
                    break;
                }

                result = this.Time.CompareTo(other.Time);
            } while (false);
            return result;
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
        public int CompareTo(DateTime other)
        {
            int result = 0;

            do // false loop
            {
                if (this.Date.Year > other.Year)
                {
                    result = 1;
                    break;
                }

                if (this.Date.Year < other.Year)
                {
                    result = -1;
                    break;
                }

                if (this.Date.Month > other.Month)
                {
                    result = 1;
                    break;
                }

                if (this.Date.Month < other.Month)
                {
                    result = -1;
                    break;
                }

                if (this.Date.Day > other.Day)
                {
                    result = 1;
                    break;
                }

                if (this.Date.Day < other.Day)
                {
                    result = -1;
                    break;
                }

            } while (false);
            return result;
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
            return this.CompareTo(obj) == 0;
        }

        /// <summary>
        /// See the base class documentation.
        /// </summary>
        /// 
        /// <returns>
        /// See the base class documentation.
        /// </returns>
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
            bool result = true;

            do // false loop
            {
                if ((object)date == null)
                {
                    if (secondInstance == null)
                    {
                        break;
                    }
                    result = false;
                    break;
                }

                result = date.Equals(secondInstance);
            } while (false);

            return result;
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
            bool result = false;

            do // false loop
            {
                if (date == null)
                {
                    if (secondInstance == null)
                    {
                        break;
                    }
                    result = true;
                    break;
                }

                result = !date.Equals(secondInstance);
            } while (false);

            return result;
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
            bool result = false;

            do // false loop
            {
                if (date == null)
                {
                    if (secondInstance == null)
                    {
                        break;
                    }
                    result = true;
                    break;
                }

                result = date.CompareTo(secondInstance) > 0;
            } while (false);

            return result;
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
            bool result = false;

            do // false loop
            {
                if (date == null)
                {
                    if (secondInstance == null)
                    {
                        break;
                    }
                    result = true;
                    break;
                }

                result = date.CompareTo(secondInstance) < 0;
            } while (false);

            return result;
        }

        #endregion Operators


    }

}
