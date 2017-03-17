// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Provides values describing how awake a person feels when they awake
    /// from sleep.
    /// </summary>
    ///
    public enum WakeState
    {
        /// <summary>
        /// The waking state is unknown.
        /// </summary>
        ///
        /// <remarks>
        /// This is not a valid state and will cause an exception if used.
        /// </remarks>
        ///
        Unknown = 0,

        /// <summary>
        /// The person awoke feeling refreshed.
        /// </summary>
        ///
        WideAwake = 1,

        /// <summary>
        /// The person awoke but was still somewhat tired.
        /// </summary>
        ///
        Tired = 2,

        /// <summary>
        /// The person awoke but was still very sleepy.
        /// </summary>
        ///
        Sleepy = 3
    }
}
