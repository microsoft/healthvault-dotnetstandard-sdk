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
    /// Stores a Continuity of Care Record (CCR). 
    /// </summary>
    /// 
    /// <remarks>
    /// The CCR XML can be accessed through the TypeSpecificXml property.
    /// </remarks>
    /// 
    public class CCR : HealthRecordItem
    {
        /// <summary>
        /// Initializes an instance of the <see cref="CCR"/> class, 
        /// with default values.
        /// </summary>
        /// 
        public CCR()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="CCR"/> class, 
        /// with specific data.
        /// </summary>
        /// 
        public CCR(IXPathNavigable typeSpecificData)
            : base(TypeId, typeSpecificData)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("1e1ccbfc-a55d-4d91-8940-fa2fbf73c195");
    }
}
