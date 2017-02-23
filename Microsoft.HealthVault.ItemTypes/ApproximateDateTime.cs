// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
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

            this.approximateDate = approximateDate;
            this.approximateTime = null;
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
            this.approximateTime = approximateTime;
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
            this.timeZone = timeZone;
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
            this.description = description;
            this.approximateDate = null;
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
            this.approximateDate = new ApproximateDate(dateTime.Year, dateTime.Month, dateTime.Day);
            this.approximateTime = new ApproximateTime(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
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
                this.approximateDate = new ApproximateDate();
                this.approximateDate.ParseXml(
                    structuredNav.SelectSingleNode("date"));

                XPathNavigator timeNav =
                    structuredNav.SelectSingleNode("time");
                if (timeNav != null)
                {
                    this.approximateTime = new ApproximateTime();
                    this.approximateTime.ParseXml(timeNav);
                }
                else
                {
                    this.approximateTime = null;
                }

                XPathNavigator tzNav =
                    structuredNav.SelectSingleNode("tz");

                if (tzNav != null)
                {
                    this.timeZone = new CodableValue();
                    this.timeZone.ParseXml(tzNav);
                }
            }
            else
            {
                this.description =
                    navigator.SelectSingleNode("descriptive").Value;
                this.approximateDate = null;
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

            if (this.approximateDate == null && string.IsNullOrEmpty(this.description))
            {
                throw Validator.HealthRecordItemSerializationException("ApproximateDateTimeMissingMandatory");
            }

            writer.WriteStartElement(nodeName);

            if (this.approximateDate != null)
            {
                // <structured>
                writer.WriteStartElement("structured");

                this.approximateDate.WriteXml("date", writer);

                if (this.approximateTime != null)
                {
                    this.approximateTime.WriteXml("time", writer);
                }

                this.timeZone?.WriteXml("tz", writer);

                // </structured>
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteElementString("descriptive", this.description);
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
            get { return this.approximateDate; }

            set
            {
                this.approximateDate = value;
                if (value != null)
                {
                    this.description = null;
                }
            }
        }

        private ApproximateDate approximateDate = new ApproximateDate();

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
            get { return this.approximateTime; }
            set { this.approximateTime = value; }
        }

        private ApproximateTime approximateTime = new ApproximateTime();

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
            get { return this.timeZone; }
            set { this.timeZone = value; }
        }

        private CodableValue timeZone;

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
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");

                this.description = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.approximateDate = null;
                    this.approximateTime = null;
                    this.timeZone = null;
                }
            }
        }

        private string description;

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
            if (obj == null)
            {
                return 1;
            }

            ApproximateDateTime hsDate = obj as ApproximateDateTime;
            if (hsDate == null)
            {
                try
                {
                    DateTime dt = (DateTime)obj;
                    return this.CompareTo(dt);
                }
                catch (InvalidCastException)
                {
                    throw Validator.ArgumentException("obj", "ApproximateDateTimeCompareToInvalidType");
                }
            }

            return this.CompareTo(hsDate);
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
            if (other == null)
            {
                return 1;
            }

            if (this.ApproximateDate == null)
            {
                if (other.ApproximateDate != null)
                {
                    return -1;
                }

                return this.Description.CompareTo(other.Description);
            }

            int result = this.ApproximateDate.CompareTo(other.ApproximateDate);
            if (result != 0)
            {
                return result;
            }

            if (this.ApproximateTime == null)
            {
                if (other.ApproximateTime != null)
                {
                    return -1;
                }

                return 0;
            }

            return this.ApproximateTime.CompareTo(other.ApproximateTime);
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
            if (this.ApproximateDate == null)
            {
                return -1;
            }

            int result = this.ApproximateDate.CompareTo(other);
            if (result != 0)
            {
                return result;
            }

            if (this.ApproximateTime == null)
            {
                return -1;
            }

            return this.ApproximateTime.CompareTo(other);
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
            if ((object)date == null)
            {
                return secondInstance == null;
            }

            return date.Equals(secondInstance);
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
        /// The <paramref name="secondInstance"/> parameter
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        ///
        public static bool operator >(ApproximateDateTime date, object secondInstance)
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
        /// The <paramref name="secondInstance"/> parameter
        /// is not an <see cref="ApproximateDateTime"/> or
        /// <see cref="System.DateTime"/> object.
        /// </exception>
        ///
        public static bool operator <(ApproximateDateTime date, object secondInstance)
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
            StringBuilder result = new StringBuilder(50);
            string space = ResourceRetriever.GetSpace("sdkerrors");

            if (string.IsNullOrEmpty(this.Description))
            {
                result.Append(this.ApproximateDate.ToString(formatProvider));

                if (this.ApproximateTime != null &&
                    this.ApproximateTime.HasValue)
                {
                    string time = this.ApproximateTime.ToString(formatProvider);

                    if (!string.IsNullOrEmpty(time))
                    {
                        result.Append(space);
                        result.Append(time);
                    }
                }

                if (this.TimeZone != null)
                {
                    result.Append(space);
                    result.Append(this.TimeZone.Text);
                }
            }
            else
            {
                result.Append(this.Description);
            }

            return result.ToString();
        }
    }
}
