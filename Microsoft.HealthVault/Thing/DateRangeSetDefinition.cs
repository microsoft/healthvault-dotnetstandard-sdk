// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a set of things for authorization
    /// purposes whose effective date falls within a specified range.
    /// </summary>
    ///
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or
    /// effective date, or explicitly by setting the system set.) This class
    /// serves as a set of things that have effective dates
    /// falling within the specified range. Other types of authorization
    /// sets include <see cref="TypeIdSetDefinition"/>.
    /// </remarks>
    ///
    /// <seealso cref="AuthorizationSetDefinition"/>
    /// <seealso cref="TypeIdSetDefinition"/>
    ///
    internal class DateRangeSetDefinition : AuthorizationSetDefinition
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DateRangeSetDefinition"/> class
        /// with the specified minimum and maximum dates.
        /// </summary>
        ///
        /// <param name="dateMin">
        /// The minimum effective date of the things included in
        /// the set.
        /// </param>
        ///
        /// <param name="dateMax">
        /// The maximum effective date of things included in
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
            if (dateMin > dateMax)
            {
                throw new ArgumentException(Resources.DateRangeMinLessThanMax, nameof(dateMin));
            }

            this.DateMin = dateMin;
            this.DateMax = dateMax;
        }

        /// <summary>
        /// Gets the minimum effective date of the things in
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
        public DateTime DateMin { get; } = DateTime.MinValue;

        /// <summary>
        /// Gets the maximum effective date of the things in
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
        public DateTime DateMax { get; } = DateTime.MaxValue;

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
                SDKHelper.XmlFromDateTime(this.DateMin) +
                "</date-min>" +
                "<date-max>" +
                SDKHelper.XmlFromDateTime(this.DateMax) +
                "</date-max>" +
                "</date-range>";
        }
    }
}
