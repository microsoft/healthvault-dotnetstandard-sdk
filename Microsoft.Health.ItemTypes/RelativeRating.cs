// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a relative rating for attributes such as emotion or activity.
    /// </summary>
    /// 
    public enum RelativeRating
    {
        /// <summary>
        /// The relative rating is not known.
        /// </summary>
        /// 
        None = 0,

        /// <summary>
        /// The rating is very low.
        /// </summary>
        /// 
        VeryLow = 1,

        /// <summary>
        /// The rating is low.
        /// </summary>
        /// 
        Low = 2,

        /// <summary>
        /// The rating is moderate.
        /// </summary>
        /// 
        Moderate = 3,

        /// <summary>
        /// The rating is high.
        /// </summary>
        /// 
        High = 4,

        /// <summary>
        /// The rating is very high.
        /// </summary>
        /// 
        VeryHigh = 5
    }
}
