// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.AspNetCore
{
    public sealed class HealthVaultAuthenticationOptions : RemoteAuthenticationOptions
    {
        public HealthVaultAuthenticationOptions(HealthVaultConfiguration config) : this()
        {
            Configuration = config;
        }

        public HealthVaultAuthenticationOptions()
        {
            Description.DisplayName = "HealthVault";
            CallbackPath = new PathString("/signin-healthvault");
            SignInScheme = HealthVaultAuthenticationDefaults.AuthenticationScheme;
            AuthenticationScheme = HealthVaultAuthenticationDefaults.AuthenticationScheme;
            AutomaticChallenge = true;
            AutomaticAuthenticate = true;
        }

        /// <summary>
        /// Gets the filename for the application certificate.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_ApplicationCertificateFilename" configuration value when reading from web.config.
        /// </remarks>
        public string ApplicationCertificateFileName { get; set; }

        /// <summary>
        /// Gets the password for the application certificate.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_ApplicationCertificatePassword" configuration value when reading from web.config.
        /// </remarks>
        public string ApplicationCertificatePassword { get; set; }

        /// <summary>
        /// Gets the Certificate Subject.
        /// </summary>
        /// <remarks>
        /// This property corresponds to the "HV_AppCertSubject" configuration value when reading from web.config.
        /// </remarks>
        public string CertSubject { get; set; }

        /// <summary>
        /// Gets a value indicating whether a signup code is required when a user
        /// signs up for a HealthVault account.
        /// </summary>
        /// <remarks>
        /// A signup code is only required under certain conditions. For instance,
        /// the account is being created from outside the United States.
        /// This property corresponds to the "HV_IsSignupCodeRequired" configuration value when reading from web.config.
        /// The value defaults to "false".
        /// </remarks>
        public bool IsSignupCodeRequired { get; set; }

        public HealthVaultConfiguration Configuration { get; set; }

        internal ICookieManager CookieManager { get; set; } = new ChunkingCookieManager();

        internal TicketDataFormat StateDataFormat { get; set; }

        public string CookieDomain { get; set; }

        public bool CookieHttpOnly { get; set; } = true;

        public string CookiePath { get; set; }

        public CookieSecurePolicy CookieSecure { get; set; } = CookieSecurePolicy.SameAsRequest;
    }
}