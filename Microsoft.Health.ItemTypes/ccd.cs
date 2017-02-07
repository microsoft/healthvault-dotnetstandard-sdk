// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Stores a Continuity of Care Document (CCD). 
    /// </summary>
    /// 
    /// <remarks>
    /// The CCD XML can be accessed through the TypeSpecificXml property.
    /// </remarks>
    /// 
    public class CCD : HealthRecordItem
    {
        /// <summary>
        /// Initializes an instance of the <see cref="CCD"/> class, 
        /// with default values.
        /// </summary>
        /// 
        public CCD()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="CCD"/> class, 
        /// with specific data.
        /// </summary>
        /// 
        public CCD(IXPathNavigable typeSpecificData)
            : base(TypeId, typeSpecificData)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("9c48a2b8-952c-4f5a-935d-f3292326bf54");
    }
}
