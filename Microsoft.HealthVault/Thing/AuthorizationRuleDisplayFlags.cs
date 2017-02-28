// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// A class representing the display strategies for optional rules
    /// </summary>
    ///
    /// <remarks>
    /// Optional rules may be displayed in the AppAuth page in several ways.
    /// This class specifies the configuration to control that display.
    /// </remarks>
    ///
    [Flags]
    public enum AuthorizationRuleDisplayFlags
    {
        /// <summary>
        /// No special display handling configured
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// If set, the rule is displayed when the person is initially
        /// authorizing the application.
        /// </summary>
        ///
        DisplayFirstTime = 0x1,

        /// <summary>
        /// If set, display the rule as checked when
        /// initially authorizing the application.
        /// </summary>
        ///
        CheckedFirstTime = 0x2,

        /// <summary>
        /// If set, the display the rule as checked.
        /// </summary>
        ///
        CheckedByDefault = 0x4
    }
}
