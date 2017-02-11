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
    /// Provides values representing how awake a person felt during the day.
    /// </summary>
    /// 
    public enum Sleepiness
    {
        /// <summary>
        /// The sleepiness state is unknown.
        /// </summary>
        /// 
        /// <remarks>
        /// This is not a valid state and will cause an exception if used.
        /// </remarks>
        /// 
        Unknown=0,

        /// <summary>
        /// The person was very sleepy throughout the day.
        /// </summary>
        /// 
        VerySleepy=1,

        /// <summary>
        /// The person was somewhat tired throughout the day.
        /// </summary>
        /// 
        Tired=2,

        /// <summary>
        /// The person was fairly alert throughout the day.
        /// </summary>
        /// 
        Alert=3,

        /// <summary>
        /// The person was wide awake throughout the day.
        /// </summary>
        /// 
        WideAwake=4
    }
}
