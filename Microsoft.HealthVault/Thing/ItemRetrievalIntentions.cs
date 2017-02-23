// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The usage intentions for items being retrieved.
    /// </summary>
    [Flags]
    public enum ItemRetrievalIntentions
    {
        /// <summary>
        /// None of the specifiable intentions are applicable.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Intentions for retrieved items are unspecified in the request.
        /// </summary>
        /// <remarks>
        /// When intentions are unspecified, HealthVault may
        /// infer a default intention such as "user view" based
        /// on the context of the request.
        /// </remarks>
        Unspecified = 0x1,

        /// <summary>
        /// Retrieved items are intended for immediate user view.
        /// </summary>
        View = 0x2,

        /// <summary>
        /// Retrieved items are intended for immediate user download.
        /// </summary>
        Download = 0x4,

        /// <summary>
        /// Retrieved items are intended for immediate transmission via the Direct protocol.
        /// </summary>
        Transmit = 0x8
    }
}