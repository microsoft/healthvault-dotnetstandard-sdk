// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents the range of time between two dates.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DateRange"/> class
        /// with the specified start and end dates.
        /// </summary>
        ///
        /// <param name="start">
        /// The start date of the date range.
        /// </param>
        ///
        /// <param name="end">
        /// The end date of the date range.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="start"/> parameter is greater than the
        /// <paramref name="end"/> parameter.
        /// </exception>
        ///
        public DateRange(DateTime start, DateTime end)
        {
            Validator.ThrowArgumentExceptionIf(
                start > end,
                "dateMin",
                "DateRangeMinLessThanMax");

            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Gets the minimum date of the range.
        /// </summary>
        ///
        /// <value>
        /// A DateTime value.
        /// </value>
        ///
        public DateTime Start { get; } = DateTime.MinValue;

        /// <summary>
        /// Gets the maximum date of the range.
        /// </summary>
        ///
        /// <value>
        /// A DateTime value.
        /// </value>
        ///
        public DateTime End { get; } = DateTime.MaxValue;
    }
}
