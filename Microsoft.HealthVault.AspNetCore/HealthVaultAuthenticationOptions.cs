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
            SignInScheme = HealthVaultAuthenticationMiddleware.AuthenticationScheme;
            AuthenticationScheme = HealthVaultAuthenticationMiddleware.AuthenticationScheme;
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
        ///
        /// <remarks>
        /// A signup code is only required under certain conditions. For instance,
        /// the account is being created from outside the United States.
        /// This property corresponds to the "HV_IsSignupCodeRequired" configuration value when reading from web.config.
        /// The value defaults to "false".
        /// </remarks>
        ///
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