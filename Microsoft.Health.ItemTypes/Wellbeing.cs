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
    /// Describes the general wellbeing of a person from sick to healthy.
    /// </summary>
    /// 
    public enum Wellbeing
    {
        /// <summary>
        /// The wellbeing of the person is unknown.
        /// </summary>
        /// 
        None = 0,

        /// <summary>
        /// The person is sick.
        /// </summary>
        /// 
        Sick = 1,

        /// <summary>
        /// The person is not functioning at a normal level. 
        /// </summary>
        /// 
        Impaired = 2,

        /// <summary>
        /// The person is functioning at a normal level but might still have
        /// symptoms.
        /// </summary>
        /// 
        Able = 3,

        /// <summary>
        /// The person is functioning at a normal level without any
        /// symptoms.
        /// </summary>
        /// 
        Healthy = 4,

        /// <summary>
        /// The person is functioning beyond their normal level. 
        /// </summary>
        /// 
        Vigorous = 5
    }
}
