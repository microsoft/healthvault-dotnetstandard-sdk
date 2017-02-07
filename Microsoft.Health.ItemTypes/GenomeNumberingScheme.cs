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
    /// Provides values for genome numbering scheme.
    /// </summary>
    /// 
    /// <remarks>
    /// It can be either 0 based numbering scheme or 1 based numbering scheme.
    /// </remarks>
    /// 
    public enum GenomeNumberingScheme
    {
        /// <summary>
        /// 0 based numbering scheme.
        /// </summary>
        /// 
        ZeroBased = 0,

        /// <summary>
        /// 1 based number scheme. 
        /// </summary>
        /// 
        OneBased = 1,

        /// <summary>
        /// Unknown numbering scheme. 
        /// </summary>
        /// 
        Unknown = 2
    }
}
