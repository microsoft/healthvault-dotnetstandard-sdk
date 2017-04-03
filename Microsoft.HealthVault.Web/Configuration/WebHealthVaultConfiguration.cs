// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.Web.Configuration
{
    public class WebHealthVaultConfiguration : HealthVaultConfiguration
    {
        /// <summary>
        /// Constant to indicate logon required
        /// </summary>
        ///
        public const bool LogOnRequired = true;

        /// <summary>
        /// Constant to indicate logon not required
        /// </summary>
        ///
        public const bool NoLogOnRequired = false;

        /// <summary>
        /// Gets a URL to use in place of the action URL in testing environments.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_NonProductionActionUrlRedirectOverride" configuration value when reading from web.config.
        /// </remarks>
        ///
        public Uri ActionUrlRedirectOverride { get; internal set; }

        /// <summary>
        /// Gets the list of allowed redirect sites.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_AllowedRedirectSites" configuration value when reading from web.config.
        /// </remarks>
        ///
        public string AllowedRedirectSites { get; internal set; }

        /// <summary>
        /// Gets the filename for the application certificate.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_ApplicationCertificateFilename" configuration value when reading from web.config.
        /// </remarks>
        public string ApplicationCertificateFileName { get; internal set; }

        /// <summary>
        /// Gets the password for the application certificate.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_ApplicationCertificatePassword" configuration value when reading from web.config.
        /// </remarks>
        public string ApplicationCertificatePassword { get; internal set; }

        /// <summary>
        /// Gets the Certificate Subject.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_AppCertSubject" configuration value when reading from web.config.
        /// </remarks>
        public string CertSubject { get; internal set; }

        /// <summary>
        /// Gets the domain that will be used for the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_CookieDomain" configuration value when reading from web.config.
        /// The value defaults to "".
        /// </remarks>
        ///
        public string CookieDomain { get; internal set; }

        /// <summary>
        /// Gets the key used to encrypt the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_CookieEncryptionKey" configuration value when reading from web.config.
        /// </remarks>
        ///
        public byte[] CookieEncryptionKey { get; internal set; }

        /// <summary>
        /// Gets the name to use for the cookie which stores login information for the
        /// user.
        /// </summary>
        ///
        /// <remarks>
        /// The value defaults to <see cref="HttpRuntime.AppDomainAppVirtualPath"/> + "_HV".
        /// </remarks>
        ///
        public string CookieName { get; internal set; }

        /// <summary>
        /// Gets the path to be used for the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_CookiePath" configuration value when reading from web.config.
        /// The value defaults to "".
        /// </remarks>
        ///
        public string CookiePath { get; internal set; }

        /// <summary>
        /// Gets the time a cookie will be stored.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_CookieTimeoutMinutes" configuration value when reading from web.config.
        /// The value defaults to 20.
        /// </remarks>
        ///
        public TimeSpan CookieTimeoutDuration { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether a signup code is required when a user
        /// signs up for a HealthVault account.
        /// </summary>
        ///
        /// <remarks>
        /// A signup code is only required under certain conditions. For instance,
        /// the account is being created from outside the United States.
        /// This property corresponds to the "HV_IsSignupCodeRequired" configuration value when reading from web.config.
        /// The value defaults to "false".
        /// </remarks>
        ///
        public bool IsSignupCodeRequired { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the HealthVault token should be stored
        /// in the ASP.NET session rather than a cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_UseAspSession" configuration value when reading from web.config.
        /// The value defaults to "false".
        /// </remarks>
        ///
        public bool UseAspSession { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="HealthServicePage"/> should automatically
        /// redirect to SSL ports when reached through an unsecured port.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "HV_SSLForSecure" configuration value when reading from web.config.
        /// The value defaults to "true".
        /// </remarks>
        ///
        public bool UseSslForSecurity { get; internal set; }

        /// <summary>
        /// Gets the action URL prefix
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_Action*" configuration values when reading from web.config.
        /// </remarks>
        internal Dictionary<string, Uri> ActionPageUrls { get; set; }
    }
}
