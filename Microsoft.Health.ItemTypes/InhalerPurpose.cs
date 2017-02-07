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
    /// Provides values representing the intended purpose for the inhaler.
    /// </summary>
    /// 
    public enum InhalerPurpose
    {
        /// <summary>
        /// Purpose is not known.
        /// </summary>
        /// 
        /// <remarks>
        /// This value causes an exception to be thrown.
        /// </remarks>
        /// 
        None = 0,

        /// <summary>
        /// The intended purpose is to control the condition.
        /// </summary>
        /// 
        Control,

        /// <summary>
        /// The intended purpose is to meet an emergency condition.
        /// </summary>
        /// 
        Rescue,

        /// <summary>
        /// The intended purpose combines both control and rescue.
        /// </summary>
        /// 
        Combination,
    }
}
