// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
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
        Desc = 1,
    }

    /// <summary>
    /// Specifies the order in which data is returned in GetThings request.
    /// </summary>
    public class HealthRecordItemsOrderByClause
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
