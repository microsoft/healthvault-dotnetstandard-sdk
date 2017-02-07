// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary> 
    /// An approximation of a date and time.
    /// </summary>
    /// 
    /// <remarks>
    /// To use this class, you must specify either an approximate date or a 
    /// descriptive date such as "as a baby."
    /// </remarks>
    /// 
    public class ApproximateDateTime 
        : HealthRecordItemData,
            IComparable,
            IComparable<ApproximateDateTime>,
            IComparable<DateTime>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class with default values.
        /// </summary>
        /// 
        public ApproximateDateTime()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class with the specified date.
        /// </summary>
        /// 
        /// <param name="approximateDate">
        /// The approximation of the date.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="approximateDate"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public ApproximateDateTime(ApproximateDate approximateDate)
        {
            Validator.ThrowIfArgumentNull(approximateDate, "approximateDate", "ApproximateDateTimeDateNull");

            _approximateDate = approximateDate;
            _approximateTime = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class with the specified date and time.
        /// </summary>
        /// 
        /// <param name="approximateDate">
        /// The approximation of the date.
        /// </param>
        /// 
        /// <param name="approximateTime">
        /// The approximation of the time.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="approximateDate"/> is <b>null</b>.
        /// </exception>
        /// 
        public ApproximateDateTime(
            ApproximateDate approximateDate,
            ApproximateTime approximateTime)
            : this(approximateDate)
        {
            _approximateTime = approximateTime;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class with the specified date, time, and time zone.
        /// </summary>
        /// 
        /// <param name="approximateDate">
        /// The approximation of the date.
        /// </param>
        /// 
        /// <param name="approximateTime">
        /// The approximation of the time.
        /// </param>
        /// 
        /// <param name="timeZone">
        /// The time zone of the approximate time.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="approximateDate"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public ApproximateDateTime(
            ApproximateDate approximateDate,
            ApproximateTime approximateTime,
            CodableValue timeZone)
            : this(approximateDate, approximateTime)
        {
            _timeZone = timeZone;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class with the specified description.
        /// </summary>
        /// 
        /// <param name="description">
        /// The description of the approximate date.
        /// </param>
        /// 
        /// <remarks>
        /// The description and approximate date/time are mutually exclusive. A description
        /// is the approximation of a date without using year, month, day.  For instance,
        /// "when I was a toddler" can be used as an approximate date.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="description"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public ApproximateDateTime(string description)
        {
            Validator.ThrowIfStringNullOrEmpty(description, "description");
            _description = description;
            _approximateDate = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApproximateDateTime"/>
        /// class from a DateTime instance.
        /// </summary>
        /// 
        /// <param name="dateTime">
        /// The DateTime instance
        /// </param>
        /// 
        /// <remarks>
        /// The time zone is not set by this constructor.
        /// </remarks>
        /// 
        public ApproximateDateTime(DateTime dateTime)
        {
            _approximateDate = new ApproximateDate(dateTime.Year, dateTime.Month, dateTime.Day);
            _approximateTime = new ApproximateTime(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }

        /// <summary> 
        /// Populates the data for the approximate date and time from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the approximate date and time.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator structuredNav = 
                navigator.SelectSingleNode("structured");

            if (structuredNav != null)
            {
                _approximateDate = new ApproximateDate();
                _approximateDate.ParseXml(
                    structuredNav.SelectSingleNode("date"));

                XPathNavigator timeNav =
                    structuredNav.SelectSingleNode("time");
                if (timeNav != null)
                {
                    _approximateTime = new ApproximateTime();
                    _approximateTime.ParseXml(timeNav);
                }
                else
                {
                    _approximateTime = null;
                }

                XPathNavigator tzNav =
                    structuredNav.SelectSingleNode("tz");

                if (tzNav != null)
                {
                    _timeZone = new CodableValue();
                    _timeZone.ParseXml(tzNav);
                }
            }
            else
            {
                _description = 
                    navigator.SelectSingleNode("descriptive").Value;
                _approximateDate = null;
            }
        }

        /// <summary> 
        /// Writes the approximate date and time to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the approximate date and time.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the approximate date and time to.
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
        /// The <see cref="ApproximateDate"/> is <b>null</b> and 
        /// <see cref="Description"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            if (_approximateDate == null && String.IsNullOrEmpty(_description))
            {
                throw Validator.HealthRecordItemSerializationException("ApproximateDateTimeMissingMandatory");
            }

            writer.WriteStartElement(nodeName);

            if (_approximateDate != null)
            {
                // <structured>
                writer.WriteStartElement("structured");

                _approximateDate.WriteXml("date", writer);

                if (_approximateTime != null)
                {
                    _approximateTime.WriteXml("time", writer);
                }

                if (_timeZone != null)
                {
                    _timeZone.WriteXml("tz", writer);
                }

                // </structured>
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteElementString("descriptive", _description);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the approximate date.
        /// </summary>
        /// 
        /// <remarks>
        /// The <paramref name="value"/> parameter is <b>null</b> when set, and 
        /// <see cref="Description"/> must be set to a non-<b>null</b> value before
        /// calling <see cref="WriteXml"/>.
        /// <br/><br/>
        /// The property value defaults to the current year.
        /// </remarks>
        /// 
        public ApproximateDate ApproximateDate
        {
            get { return _approximateDate; }
            set
            {
                _approximateDate = value;
                if (value != null)
                {
                    _description = null;
                }
            }
        }
        private ApproximateDate _approximateDate = new ApproximateDate();

        /// <summary>
        /// Gets or sets the approximate time.
        /// </summary>
        /// 
        /// <remarks>
        /// This value is only used if <see cref="ApproximateDate"/> is set
        /// to a non-<b>null</b> value.
        /// </remarks>
        /// 
        public ApproximateTime ApproximateTime
        {
            get { return _approximateTime; }
            set { _approximateTime = value; }
        }
        private ApproximateTime _approximateTime = new ApproximateTime();

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// 
        /// <remarks>
        /// This value is only used if <see cref="ApproximateDate"/> is set
        /// to a non-<b>null</b> value.
        /// HealthVault does not interpret this value or adjust dates accordingly.
        /// It is up to the application to decide how this value is used.
        /// </remarks>
        /// 
        public CodableValue TimeZone
        {
            get { return _timeZone; }
            set { _timeZone = value; }
        }
        private CodableValue _timeZone;

        /// <summary>
        /// Gets or sets the descriptive form of the approximate date.
        /// </summary>
        /// 
        /// <remarks>
        /// The descriptive form of the approximate date is a value such as 
        /// "As a baby...".
        /// <br/><br/>
        /// If <paramref name="value"/> is <b>null</b> when set, 
        /// <see cref="ApproximateDate"/> must be set to a non-<b>null</b> value 
        /// before calling <see cref="WriteXml"/>.
        /// If <paramref name="value"/> is not <b>null</b> when set, 
        /// <see cref="ApproximateDate"/> is set to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Description
        {
            get { return _description; }
            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                
                _description = value;
                if (!String.IsNullOrEmpty(value))
                {
                    _approximateDate = null;
                    _approximateTime = null;
                    _timeZone = null;
                }
            }
        }
        private string _description;

        #region IComparable

        /// <summary>
        /// Compares the specified object to this <see cref="ApproximateDateTime"/> 
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
        /// instance is less than <paramref name="obj"/>. If the result is zero, 
        /// the instance is equal to <paramref name="obj"/>. If the result is
        /// greater than zero, the instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not an <see cref="ApproximateDateTime"/>
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

                ApproximateDateTime hsDate = obj as ApproximateDateTime;
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
                        throw Validator.ArgumentException("obj", "ApproximateDateTimeCompareToInvalidType");
                    }
                }

                result = CompareTo(hsDate);
            } while (false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this <see cref="ApproximateDateTime"/> 
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
        public int CompareTo(ApproximateDateTime other)
        {
            int result = 0;

            do // false loop
            {
                if (other == null)
                {
                    result = 1;
                    break;
                }

                if (this.ApproximateDate == null)
                {
                    if (other.ApproximateDate != null)
                    {
                        result = -1;
                        break;
                    }

                    result = this.Description.CompareTo(other.Description);
                    break;
                }

                result = this.ApproximateDate.CompareTo(other.ApproximateDate);
                if (result != 0)
                {
                    break;
                }

                if (this.ApproximateTime == null)
                {
                    if (other.ApproximateTime != null)
                    {
                        result = -1;
                        break;
                    }
                }
                else
                {
                    result =
                        this.ApproximateTime.CompareTo(other.ApproximateTime);
                }
            } while (false);
            return result;
        }

        /// <summary>
        /// Compares the specified object to this <see cref="ApproximateDateTime"/> 
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
                if (this.ApproximateDate == null)
                {
                    result = -1;
                    break;
                }

                result = this.ApproximateDate.CompareTo(other);
                if (result != 0)
                {
                    break;
                }

                if (this.ApproximateTime == null)
                {
                    result = -1;
                    break;
                }
                else
                {
                    result =
                        this.ApproximateTime.CompareTo(other);
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
        /// <see cref="ApproximateDateTime"/> object and the year, month, and
        /// day exactly match the year, month, and day of this object; otherwise, 
        /// <b>false</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="obj"/> parameter is not an <see cref="ApproximateDateTime"/>
        /// or <see cref="System.DateTime"/> object.
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
        /// Gets a value indicating whether the specified object is equal to 
        /// the specified date.
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
        /// The <paramref name="secondInstance"/>
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator ==(ApproximateDateTime date, object secondInstance)
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
        /// Gets a value indicating whether the specified object is not equal 
        /// to the specified
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
        /// The <paramref name="secondInstance"/> parameter 
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator !=(ApproximateDateTime date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator >(ApproximateDateTime date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter 
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        /// 
        public static bool operator <(ApproximateDateTime date, object secondInstance)
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
        /// Gets a string representation of the approximate date/time.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the approximate date/time.
        /// </returns>
        /// 
        /// <remarks>
        /// This method is not culture aware.
        /// </remarks>
        /// 
        public override string ToString()
        {
            return ToString(Thread.CurrentThread.CurrentCulture);
        }

        internal string ToString(IFormatProvider formatProvider)
        {
            StringBuilder result = new StringBuilder(50);
            string space = ResourceRetriever.GetSpace("sdkerrors");

            if (String.IsNullOrEmpty(Description))
            {
                result.Append(ApproximateDate.ToString(formatProvider));

                if (ApproximateTime != null &&
                    ApproximateTime.HasValue)
                {
                    string time = ApproximateTime.ToString(formatProvider);

                    if (!String.IsNullOrEmpty(time))
                    {
                        result.Append(space);
                        result.Append(time);
                    }
                }

                if (TimeZone != null)
                {
                    result.Append(space);
                    result.Append(TimeZone.Text);
                }
            }
            else
            {
                result.Append(Description);
            }
            return result.ToString();
        }
    }

}
