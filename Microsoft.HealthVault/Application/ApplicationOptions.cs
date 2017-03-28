// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Application
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
    internal enum ApplicationOptions
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
