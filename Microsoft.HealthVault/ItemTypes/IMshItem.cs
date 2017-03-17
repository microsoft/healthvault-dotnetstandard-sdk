// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Contract to make sure wrapped object passes key value
    /// of <see cref="ThingBase"/> class and type name.
    /// </summary>
    public interface IMshItem
    {
        /// <summary>
        /// Versionstamp used for concurrency check (same as ETag in azure table)
        /// </summary>
        Guid VersionStamp { get; set; }

        /// <summary>
        /// Id of wrapped thing object
        /// </summary>
        Guid ThingId { get; set; }

        /// <summary>
        /// Base64-encoded json of wrapped object
        /// </summary>
        string Base64EncodedJson { get; }

        /// <summary>
        /// Full type name of wrapped object
        /// </summary>
        string WrappedTypeName { get; }
    }
}
