// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores a Clinical Document Architecture (CDA) document.
    /// </summary>
    ///
    /// <remarks>
    /// The CDA XML can be accessed through the TypeSpecificXml property.
    /// </remarks>
    ///
    public class CDA : HealthRecordItem
    {
        /// <summary>
        /// Initializes an instance of the <see cref="CDA"/> class,
        /// with default values.
        /// </summary>
        ///
        public CDA()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="CDA"/> class,
        /// with specific data.
        /// </summary>
        ///
        public CDA(IXPathNavigable typeSpecificData)
            : base(TypeId, typeSpecificData)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("1ed1cba6-9530-44a3-b7b5-e8219690ebcf");
    }
}
