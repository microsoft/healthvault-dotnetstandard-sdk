// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health
{
    /// <summary>
    /// Defines a set of health record items for authorization 
    /// purposes whose effective date falls within a specified range.
    /// </summary>
    /// 
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or 
    /// effective date, or explicitly by setting the system set.) This class
    /// serves as a set of health record items that have effective dates 
    /// falling within the specified range. Other types of authorization 
    /// sets include <see cref="TypeIdSetDefinition"/>.
    /// </remarks>
    /// 
    /// <seealso cref="AuthorizationSetDefinition"/>
    /// <seealso cref="TypeIdSetDefinition"/>
    /// 
    public class DateRangeSetDefinition : AuthorizationSetDefinition
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DateRangeSetDefinition"/> class 
        /// with the specified minimum and maximum dates.
        /// </summary>
        /// 
        /// <param name="dateMin">
        /// The minimum effective date of the health record items included in 
        /// the set.
        /// </param>
        /// 
        /// <param name="dateMax">
        /// The maximum effective date of health record items included in 
        /// the set.
        /// </param>
        /// 
        /// <remarks>
        /// All dates and times are considered to be in UTC time. The calling
        /// application must do any conversion from local time.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="dateMin"/> parameter is greater than the 
        /// <paramref name="dateMax"/> parameter.
        /// </exception>
        /// 
        public DateRangeSetDefinition(DateTime dateMin, DateTime dateMax)
            : base(SetType.DateRangeSet)
        {
            Validator.ThrowArgumentExceptionIf(
                dateMin > dateMax,
                "dateMin",
                "DateRangeMinLessThanMax");

            _dateMin = dateMin;
            _dateMax = dateMax;
        }

        /// <summary>
        /// Gets the minimum effective date of the health record items in 
        /// this set.
        /// </summary>
        /// 
        /// <value>
        /// A DateTime value in UTC.
        /// </value>
        /// 
        /// <remarks>
        /// The calling application is responsible for changing any local
        /// times to UTC.
        /// </remarks>
        /// 
        public DateTime DateMin
        {
            get { return _dateMin; }
        }
        private DateTime _dateMin = DateTime.MinValue;

        /// <summary>
        /// Gets the maximum effective date of the health record items in 
        /// this set.
        /// </summary>
        /// 
        /// <value>
        /// A DateTime value in UTC.
        /// </value>
        /// 
        /// <remarks>
        /// The calling application is responsible for changing any local 
        /// times to UTC time.
        /// </remarks>
        /// 
        public DateTime DateMax
        {
            get { return _dateMax; }
        }
        private DateTime _dateMax = DateTime.MaxValue;

        /// <summary>
        /// Gets the XML representation of the set.
        /// </summary>
        /// 
        /// <returns>
        /// The XML representation of the set as a string.
        /// </returns>
        /// 
        /// <remarks>
        /// The XML representation adheres to the schema required by the
        /// HealthVault methods.
        /// </remarks>
        /// 
        public override string GetXml()
        {
            return
                "<date-range>" +
                "<date-min>" +
                SDKHelper.XmlFromDateTime(DateMin) +
                "</date-min>" +
                "<date-max>" +
                SDKHelper.XmlFromDateTime(DateMax) +
                "</date-max>" +
                "</date-range>";
        }
    }

}
