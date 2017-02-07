// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary> 
    /// Represents a HealthVault date.
    /// </summary>
    /// 
    /// <remarks>
    /// A HealthVault date differs from a <see cref="System.DateTime"/> in 
    /// that it pertains to dates only, not times. The year, month, and day
    /// must be specified.
    /// </remarks>
    /// 
    public class HealthServiceDate 
        : HealthRecordItemData, 
            IComparable,
            IComparable<HealthServiceDate>,
            IComparable<DateTime>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDate"/> class 
        /// with values defaulting to the current year, month, and day.
        /// </summary>
        /// 
        public HealthServiceDate()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceDate"/> class 
        /// with the specified year, month, and day.
        /// </summary>
        /// 
        /// <param name="year">
        /// The year between 1000 and 9999.
        /// </param>
        /// 
        /// <param name="month">
        /// The month between 1 and 12.
        /// </param>
        /// 
        /// <param name="day">
        /// The day between 1 and 31.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="year"/> parameter is less than 1000 or greater than 9999, 
        /// or the <paramref name="month"/> parameter is less than 1 or greater than 12, 
        /// or the <paramref name="day"/> parameter is less than 1 or greater than 31.
        /// </exception>
        /// 
        public HealthServiceDate(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        /// <summary> 
        /// Populates the data for the date from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the date.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator yearNav = navigator.SelectSingleNode("y");
            if (yearNav != null)
            {
                _year = yearNav.ValueAsInt;
            }

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
        /// Writes the date to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the date.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the date to.
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

            writer.WriteElementString(
                "y", 
                _year.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString(
                "m", 
                _month.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString(
                "d", 
                _day.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the year of the date.
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
        /// Gets or sets the month of the date.
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the month.
        /// </returns> 
        /// 
        /// <remarks>
        /// This value defaults to the current month.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1 or greater than 12.
        /// </exception>
        /// 
        public int Month
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
        private int _month = DateTime.Now.Month;

        /// <summary>
        /// Gets or sets the day of the date.
        /// </summary>
        /// 
        /// <returns>
        /// An integer representing the day.
        /// </returns> 
        /// 
        /// <remarks>
        /// This value defaults to the current day.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1 or greater than 31.
        /// </exception>
        /// 
        public int Day
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
        private int _day = DateTime.Now.Day;

        #region IComparable

        /// <summary>
        /// Compares the specified object to this HealthServiceDate object.
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
        /// The <paramref name="obj"/> parameter is not a <see cref="HealthServiceDate"/>
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

                HealthServiceDate hsDate = obj as HealthServiceDate;
                if (hsDate == null)
                {
                    try
                    {
                        DateTime dt = (DateTime) obj;
                        result = CompareTo(dt);
                        break;
                    }
                    catch (InvalidCastException)
                    {
                        throw Validator.ArgumentException("secondInstance", "DateCompareToInvalidType");
                    }
                }

                result = CompareTo(hsDate);
            } while(false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this HealthServiceDate object.
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
        public int CompareTo(HealthServiceDate other)
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

                if (this.Month > other.Month)
                {
                    result = 1;
                    break;
                }

                if (this.Month < other.Month)
                {
                    result = -1;
                    break;
                }

                if (this.Day > other.Day)
                {
                    result = 1;
                    break;
                }

                if (this.Day < other.Day)
                {
                    result = -1;
                    break;
                }

            } while (false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this HealthServiceDate object.
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

                if (this.Month > other.Month)
                {
                    result = 1;
                    break;
                }

                if (this.Month < other.Month)
                {
                    result = -1;
                    break;
                }

                if (this.Day > other.Day)
                {
                    result = 1;
                    break;
                }

                if (this.Day < other.Day)
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
        /// Retrieves a value indicating whether the specified object is equal to this object.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared.
        /// </param>
        /// 
        /// <returns>
        /// <b>true</b> if the <paramref name="obj"/> is a 
        /// <see cref="HealthServiceDate"/> object and the year, month, and
        /// day exactly match the year, month, and day of this object; otherwise, 
        /// <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not a <see cref="HealthServiceDate"/>
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
        /// Retrieves a value indicating whether the specified object is equal 
        /// to the specified date.
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
        /// exactly match the year, month, and day of <paramref name="secondInstance"/> ;
        /// otherwise, <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a 
        /// <see cref="HealthServiceDate"/> object.
        /// </exception>
        /// 
        public static bool operator ==(HealthServiceDate date, object secondInstance)
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
        /// Retrieves a value indicating whether the specified object is not 
        /// equal to the specified date.
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
        /// exactly match the year, month, and day of <paramref name="secondInstance"/> ; 
        /// otherwise, <b>true</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a <see cref="HealthServiceDate"/> object.
        /// </exception>
        /// 
        public static bool operator !=(HealthServiceDate date, object secondInstance)
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
        /// Retrieves a value indicating whether the specified date is greater 
        /// than the specified object.
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
        /// The <paramref name="secondInstance"/> parameter is not a <see cref="HealthServiceDate"/> object.
        /// </exception>
        /// 
        public static bool operator >(HealthServiceDate date, object secondInstance)
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
        /// Retrieves a value indicating whether the specified date is less than 
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
        /// is less than the year, month, and day of <paramref name="secondInstance"/>; 
        /// otherwise, <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="secondInstance"/> parameter is not a <see cref="HealthServiceDate"/> object.
        /// </exception>
        /// 
        public static bool operator <(HealthServiceDate date, object secondInstance)
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
        /// Gets a string representation of the date.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the date.
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
            DateTime dt = new DateTime(Year, Month, Day);

            return dt.ToString("d", formatProvider);
        }

    }

}
