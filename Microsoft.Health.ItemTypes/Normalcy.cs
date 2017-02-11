// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Indicates how a value compares to normal values of the same type.
    /// </summary>
    /// 
    public enum Normalcy
    {
        /// <summary>
        /// The server returned a value that is unknown to this client.
        /// </summary>
        /// 
        /// <remarks>
        /// This value can occur when an update to the server has been made
        /// to add a new value but the client has not been updated. All data
        /// will persist correctly but may not be programmatically accessible.
        /// </remarks>
        /// 
        Unknown = 0,

        /// <summary>
        /// The value is well below the norm when compared to values of 
        /// the same type.
        /// </summary>
        /// 
        WellBelowNormal = 1,

        /// <summary>
        /// The value is below the norm when compared to values of 
        /// the same type.
        /// </summary>
        /// 
        BelowNormal = 2,

        /// <summary>
        /// The value is normal when compared to values of 
        /// the same type.
        /// </summary>
        /// 
        Normal = 3,

        /// <summary>
        /// The value is above the norm when compared to values of 
        /// the same type.
        /// </summary>
        /// 
        AboveNormal = 4,

        /// <summary>
        /// The value is well above the norm when compared to values of 
        /// the same type.
        /// </summary>
        /// 
        WellAboveNormal = 5,
    }
}
