// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
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
        : ItemBase,
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
            Year = year;
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
            Month = month;
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
            Day = day;
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
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

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
                if (value < 1000 || value > 9999)
                {
                    throw new ArgumentOutOfRangeException(nameof(Year), Resources.DateYearOutOfRange);
                }

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
                if (value < 1 || value > 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(Month), Resources.DateMonthOutOfRange);
                }

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
                if (value < 1 || value > 31)
                {
                    throw new ArgumentOutOfRangeException(nameof(Day), Resources.DateDayOutOfRange);
                }

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
            if (obj == null)
            {
                return 1;
            }

            ApproximateDate hsDate = obj as ApproximateDate;
            if (hsDate == null)
            {
                try
                {
                    DateTime dt = (DateTime)obj;
                    return CompareTo(dt);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(Resources.ApproximateDateCompareToInvalidType, nameof(obj));
                }
            }

            return CompareTo(hsDate);
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
            if (other == null)
            {
                return 1;
            }

            if (Year > other.Year)
            {
                return 1;
            }

            if (Year < other.Year)
            {
                return -1;
            }

            int result = ApproximateTime.CompareOptional(Month, other.Month);

            if (result != 0)
            {
                return result;
            }

            return ApproximateTime.CompareOptional(Day, other.Day);
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
            if (Year > other.Year)
            {
                return 1;
            }

            if (Year < other.Year)
            {
                return -1;
            }

            int result = ApproximateTime.CompareOptional(Month, other.Month);

            if (result != 0)
            {
                return result;
            }

            return ApproximateTime.CompareOptional(Day, other.Day);
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        ///
        public static bool operator !=(ApproximateDate date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        ///
        public static bool operator >(ApproximateDate date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter is not an <see cref="ApproximateDate"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        ///
        public static bool operator <(ApproximateDate date, object secondInstance)
        {
            if (date == null)
            {
                return secondInstance != null;
            }

            return date.CompareTo(secondInstance) < 0;
        }

        #endregion Operators

        internal string ToString(IFormatProvider formatProvider)
        {
            if (Month == null)
            {
                return string.Format("{0:D4}", Year);
            }

            if (Day != null)
            {
                DateTime date = new DateTime(Year, Month.Value, Day.Value);
                return date.ToString("d", formatProvider);
            }
            else
            {
                DateTime date = new DateTime(Year, Month.Value, 1);
                return date.ToString("Y", formatProvider);
            }
        }
    }
}
