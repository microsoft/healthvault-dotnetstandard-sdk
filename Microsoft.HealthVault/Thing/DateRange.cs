// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents the range of time between two dates.
    /// </summary>
    internal class DateRange
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
            if (start > end)
            {
                throw new ArgumentException(Resources.DateRangeMinLessThanMax, nameof(start));
            }

            Start = start;
            End = end;
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
