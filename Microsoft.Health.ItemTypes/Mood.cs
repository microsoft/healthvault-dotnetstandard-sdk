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
    /// Provides values describing the mood component of an emotional state.
    /// </summary>
    /// 
    public enum Mood
    {
        /// <summary>
        /// The person's mood is unknown.
        /// </summary>
        /// 
        None = 0,

        /// <summary>
        /// The person is depressed.
        /// </summary>
        /// 
        Depressed = 1,

        /// <summary>
        /// The person is sad.
        /// </summary>
        /// 
        Sad = 2,

        /// <summary>
        /// The person's mood is neutral.
        /// </summary>
        /// 
        Neutral = 3,

        /// <summary>
        /// The person is happy.
        /// </summary>
        /// 
        Happy = 4,

        /// <summary>
        /// The person is elated.
        /// </summary>
        /// 
        Elated = 5
    }
}
