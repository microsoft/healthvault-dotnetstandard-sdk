// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents access restrictions for a <see cref="ThingBase"/>.
    /// </summary>
    ///
    [Flags]
    public enum ThingFlags
    {
        /// <summary>
        /// There are no special access restrictions in place for
        /// the <see cref="ThingBase"/>.
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// Access to the <see cref="ThingBase"/> is
        /// restricted to custodians only.
        /// </summary>
        ///
        Personal = 0x1,

        /// <summary>
        /// The <see cref="ThingBase"/> is down-versioned.
        /// </summary>
        ///
        DownVersioned = 0x2,

        /// <summary>
        /// The <see cref="ThingBase"/> is up-versioned.
        /// </summary>
        ///
        UpVersioned = 0x4,

        /// <summary>
        /// The <see cref="ThingBase"/> is read-only.
        /// </summary>
        ///
        ReadOnly = 0x10
    }
}
