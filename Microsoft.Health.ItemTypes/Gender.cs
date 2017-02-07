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
    /// Provides values representing a person's gender.
    /// </summary>
    /// 
    public enum Gender
    {
        /// <summary>
        /// The gender is unknown or has a value that is not understood by
        /// this client.
        /// </summary>
        /// 
        Unknown = 0,

        /// <summary>
        /// The person is male.
        /// </summary>
        /// 
        Male = 1,

        /// <summary>
        /// The person is female.
        /// </summary>
        /// 
        Female = 2
    }
}
