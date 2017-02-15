// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Optional configuration settings for HealthVault applications.
    /// </summary>
    ///
    /// <remarks>
    /// The default value is None.
    /// </remarks>
    ///
    [Flags]
    public enum ApplicationOptions
    {
        /// <summary>
        /// No optional application settings specified.
        /// </summary>
        ///
        None = 0,

        /// <summary>
        /// The application allows the user to stay logged in to the application across browser
        /// session.
        /// </summary>
        ///
        /// <remarks>
        /// This setting enables the "Keep me signed in on this computer" checkbox on the
        /// HealthVault login page.
        /// </remarks>
        ///
        PersistentTokensAllowed = 0x1,

        /// <summary>
        /// The application is required to notify HealthVault that a user has been authorized to
        /// use the application.
        /// This property will be soon removed. Please use RestrictApplicationUsers instead
        /// </summary>
        ApplicationAuthorizationRequired = 0x2,

        /// <summary>
        /// The application is required to notify HealthVault that a user has been authorized to
        /// use the application.
        /// </summary>
        ///
        /// <remarks>
        /// If this option is specified, the application must authorize HealthVault to allow the
        /// user to use the application. For example, if the application requires a subscription
        /// for use it could enable this option and then notify HealthVault when the status of the
        /// subscription has changed. In this case HealthVault will prevent the user from logging
        /// in to the application if the subscription has lapsed.
        /// </remarks>
        RestrictApplicationUsers = 0x8,

        /// <summary>
        /// The application will be published to the application directory for users to find.
        /// </summary>
        ///
        PublishApplication = 0x4,

        /// <summary>
        /// All application options are specified.
        /// </summary>
        ///
        All = PersistentTokensAllowed | ApplicationAuthorizationRequired | PublishApplication,

        /// <summary>
        /// The default is no options specified.
        /// </summary>
        ///
        Default = None
    }
}
