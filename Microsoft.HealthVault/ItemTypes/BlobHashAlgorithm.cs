// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Defines the set of supported blob hash algorithms.
    /// </summary>
    public enum BlobHashAlgorithm
    {
        /// <summary>
        /// Represents an unknown blob hash algorithm.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The block hash algorithm using SHA256.
        /// </summary>
        SHA256Block = 1
    }
}