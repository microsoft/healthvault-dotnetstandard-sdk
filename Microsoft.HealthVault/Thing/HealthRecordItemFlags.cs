// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// Represents access restrictions for a <see cref="HealthRecordItem"/>.
    /// </summary>
    ///
    [Flags]
    public enum HealthRecordItemFlags
    {
        /// <summary>
        /// There are no special access restrictions in place for
        /// the <see cref="HealthRecordItem"/>.
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// Access to the <see cref="HealthRecordItem"/> is
        /// restricted to custodians only.
        /// </summary>
        ///
        Personal = 0x1,

        /// <summary>
        /// The <see cref="HealthRecordItem"/> is down-versioned.
        /// </summary>
        ///
        DownVersioned = 0x2,

        /// <summary>
        /// The <see cref="HealthRecordItem"/> is up-versioned.
        /// </summary>
        ///
        UpVersioned = 0x4,

        /// <summary>
        /// The <see cref="HealthRecordItem"/> is read-only.
        /// </summary>
        ///
        ReadOnly = 0x10
    }
}
