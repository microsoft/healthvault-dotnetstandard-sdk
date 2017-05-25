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
    /// Specifies if the order should be sorted in ascending or descending order.
    /// </summary>
    ///
    public enum OrderByDirection
    {
        /// <summary>
        /// ASC sorts from the lowest value to highest value.
        /// </summary>
        ///
        Asc = 0,

        /// <summary>
        /// DESC sorts from highest value to lowest value.
        /// </summary>
        ///
        Desc = 1
    }

    /// <summary>
    /// Specifies the order in which data is returned in GetThings request.
    /// </summary>
    public class ThingOrderByClause
    {
        /// <summary>
        /// Gets or sets the unique item type identifiers that the order by clause is set for.
        /// </summary>
        ///
        public Guid ThingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the item property to sort on.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies that the values in the specified property should be sorted in ascending or descending order.
        /// </summary>
        public OrderByDirection Direction { get; set; }
    }
}
