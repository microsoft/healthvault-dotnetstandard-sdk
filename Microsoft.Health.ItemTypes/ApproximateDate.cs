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
    /// Represents an approximation of a date.
    /// </summary>
    /// 
    /// <remarks>
    /// An approximation of a date must have a year. The month, day, or both 
    /// are optional.
    /// </remarks>
    /// 
    public class ApproximateDate 
        : HealthRecordItemData,
            IComparable,
            IComparable<ApproximateDate>,
            IComparable<DateTime>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDate"/> class 
        /// using default values.
        /// </summary>
        /// 
        public ApproximateDate()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDate"/> class 
        /// with the specified year.
        /// </summary>
        /// 
        /// <param name="year">
        /// A year between 1000 and 9999.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="year"/> parameter is less than 1000 or greater 
        /// than 9999.
        /// </exception>
        /// 
        public ApproximateDate(int year)
        {
            this.Year = year;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDate"/> class 
        /// with the specified year and month.
        /// </summary>
        /// 
        /// <param name="year">
        /// A year between 1000 and 9999.
        /// </param>
        /// 
        /// <param name="month">
        /// A month between 1 and 12.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="year"/> parameter is less than 1000 or greater 
        /// than 9999, or the <paramref name="month"/> parameter is less than 
        /// 1 or greater than 12.
        /// </exception>
        /// 
        public ApproximateDate(int year, int month)
            : this(year)
        {
            this.Month = month;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDate"/> class 
        /// with the specified year, month, and day.
        /// </summary>
        /// 
        /// <param name="year">
        /// A year between 1000 and 9999.
        /// </param>
        /// 
        /// <param name="month">
        /// A month between 1 and 12.
        /// </param>
        /// 
        /// <param name="day">
        /// A day between 1 and 31.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="year"/> parameter is less than 1000 or greater than
        /// 9999, or the <paramref name="month"/> parameter is less than 1 or 
        /// greater than 12, or the <paramref name="day"/> parameter is less 
        /// than 1 or greater than 31.
        /// </exception>
        /// 
        public ApproximateDate(int year, int month, int day)
            : this(year, month)
        {
            this.Day = day;
        }

        
        /// <summary> 
        /// Populates the data for the approximate date from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the approximate date.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _year = navigator.SelectSingleNode("y").ValueAsInt;

            XPathNavigator monthNav = navigator.SelectSingleNode("m");
            if (monthNav != null)
            {
                _month = monthNav.ValueAsInt;
            }

            XPathNavigator dayNav = navigator.SelectSingleNode("d");
            if (dayNav != null)
            {
                _day = dayNav.ValueAsInt;
            }
        }

        /// <summary> 
        /// Writes the approximate date to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the approximate date.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the approximate date to.
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
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            writer.WriteStartElement(nodeName);

            writer.WriteElementString(
                "y", _year.ToString(CultureInfo.InvariantCulture));

            if (_month != null)
            {
                writer.WriteElementString(
                    "m", 
                    ((int)_month).ToString(CultureInfo.InvariantCulture));
            }
            if (_day != null)
            {
                writer.WriteElementString(
                    "d", 
                    ((int)_day).ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the year of the date approximation.
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the year.
        /// </returns>
        /// 
        /// <remarks>
        /// This value defaults to the current year.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1000 or greater than 9999.
        /// </exception>
        /// 
        public int Year
        {
            get { return _year; }
            set 
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 1000 || value > 9999,
                    "Year",
                    "DateYearOutOfRange");
                _year = value;
            }
        }
        private int _year = DateTime.Now.Year;

        /// <summary>
        /// Gets or sets the month of the date approximation.
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the month.
        /// </returns>
        /// 
        /// <remarks>
        /// If the month is unknown, it can be set to <b>null</b>. This value 
        /// defaults to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1 or greater than 12.
        /// </exception>
        /// 
        public int? Month
        {
            get { return _month; }
            set 
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 1 || value > 12,
                    "Month",
                    "DateMonthOutOfRange");
                _month = value;
            }
        }
        private int? _month;

        /// <summary>
        /// Gets or sets the day of the date approximation.
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the day.
        /// </returns>
        /// 
        /// <remarks>
        /// If the day is unknown, it can be set to <b>null</b>. This value 
        /// defaults to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1 or greater than 31.
        /// </exception>
        /// 
        public int? Day
        {
            get { return _day; }
            set 
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 1 || value > 31,
                    "Day",
                    "DateDayOutOfRange");
                _day = value;
            }
        }
        private int? _day;

        #region IComparable

        /// <summary>
        /// Compares the specified object to this ApproximateDate object.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the 
        /// objects being compared. If the result is less than zero, the 
        /// instance is less than <paramref name="obj"/>. If the result is zero, 
        /// the instance is equal to <paramref name="obj"/>. If the result is
        /// greater than zero, the instance is greater than 
        /// <paramref name="obj"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not an <see cref="ApproximateDate"/>
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

                ApproximateDate hsDate = obj as ApproximateDate;
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
                        throw Validator.ArgumentException("obj", "ApproximateDateCompareToInvalidType");
                    }
                }

                result = CompareTo(hsDate);
            } while (false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this <see cref="ApproximateDate"/> object.
        /// </summary>
        /// 
        /// <param name="other">
        /// The date to be compared.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the 
        /// objects being compared. If the result is less than zero, the 
        /// instance is less than <paramref name="other"/>. If the result is zero, 
        /// the instance is equal to <paramref name="other"/>. If the result is
        /// greater than zero, the instance is greater than 
        /// <paramref name="other"/>.
        /// </returns>
        /// 
        public int CompareTo(ApproximateDate other)
        {
            int result = 0;

            do // false loop
            {
                if (other == null)
                {
                    result = 1;
                    break;
                }

                if (this.Year > other.Year)
                {
                    result = 1;
                    break;
                }

                if (this.Year < other.Year)
                {
                    result = -1;
                    break;
                }

                result = ApproximateTime.CompareOptional(this.Month, other.Month);

                if (result != 0)
                {
                    break;
                }
                result = ApproximateTime.CompareOptional(this.Day, other.Day);

                if (result != 0)
                {
                    break;
                }

            } while (false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this <see cref="DateTime"/> object.
        /// </summary>
        /// 
        /// <param name="other">
        /// The date to be compared.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the 
        /// objects being compared. If the result is less than zero, the 
        /// instance is less than <paramref name="other"/>. If the result is zero, 
        /// the instance is equal to <paramref name="other"/>. If the result is
        /// greater than zero, the instance is greater than 
        /// <paramref name="other"/>.
        /// </returns>
        /// 
        public int CompareTo(DateTime other)
        {
            int result = 0;

            do // false loop
            {
                if (this.Year > other.Year)
                {
                    result = 1;
                    break;
                }

                if (this.Year < other.Year)
                {
                    result = -1;
                    break;
                }

                result = ApproximateTime.CompareOptional(this.Month, other.Month);

                if (result != 0)
                {
                    break;
                }
                result = ApproximateTime.CompareOptional(this.Day, other.Day);

                if (result != 0)
                {
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
        /// <b>true</b> if the <paramref name="obj"/> is an 
        /// <see cref="ApproximateDate"/> object and the year, month, and
        /// day exactly match the year, month, and day of this object; otherwise,
        /// <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
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
        /// Gets a value indicating whether the specified object is equal to the 
        /// specified date.
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
        /// exactly match the year, month, and day of <paramref name="secondInstance"/>; otherwise,
        /// <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or 
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator ==(ApproximateDate date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator !=(ApproximateDate date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator >(ApproximateDate date, object secondInstance)
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
        /// Gets a value indicating whether the specified date is less than the s
        /// pecified
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or 
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator <(ApproximateDate date, object secondInstance)
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

        /// <summary>
        /// Gets a string representation of the approximate date.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the approximate date.
        /// </returns>
        /// 
        /// <remarks>
        /// Note, this is not culture sensitive.
        /// </remarks>
        /// 
        public override string ToString()
        {
            return ToString(Thread.CurrentThread.CurrentCulture);
        }
        
        internal string ToString(IFormatProvider formatProvider)
        {
            String result = null;

            if (Month == null)
            {
                result = String.Format("{0:D4}", Year);
            }
            else
            {
                if (Day != null)
                {
                    DateTime date = new DateTime(Year, Month.Value, Day.Value);
                    result = date.ToString("d", formatProvider);
                }
                else
                {
                    DateTime date = new DateTime(Year, Month.Value, 1);
                    result = date.ToString("Y", formatProvider);
                }
            }

            return result;
        }
    }

}
