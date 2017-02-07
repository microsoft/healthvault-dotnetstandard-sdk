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
    /// Provides values representing the status of a goal.
    /// </summary>
    /// 
    public enum GoalStatus
    {
        /// <summary>
        /// The goal status is unknown or cannot be understood by this client.
        /// </summary>
        /// 
        Unknown = 0,

        /// <summary>
        /// The goal is being actively pursued.
        /// </summary>
        /// 
        Active,

        /// <summary>
        /// The goal has been achieved.
        /// </summary>
        /// 
        Achieved,

        /// <summary>
        /// The goal has been abandoned and is no longer being pursued.
        /// </summary>
        /// 
        Abandoned
    }
}
