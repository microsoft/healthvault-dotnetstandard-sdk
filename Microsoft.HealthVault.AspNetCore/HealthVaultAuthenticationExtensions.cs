using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.AspNetCore
{
    public static class HealthVaultAuthenticationExtensions
    {
        public static IApplicationBuilder UseHealthVault(this IApplicationBuilder app, HealthVaultAuthenticationOptions options)
        {
            WebIoc.EnsureTypesRegistered(options);
            //return app.UseMiddleware(typeof(HealthVaultAuthenticationMiddleware), app, options);
            return app.UseMiddleware<HealthVaultAuthenticationMiddleware>(Options.Create(options));
        }
    }

    public sealed class HealthVaultAuthenticationOptions : RemoteAuthenticationOptions
    {
        public HealthVaultAuthenticationOptions(HealthVaultConfiguration config)
            : this()
        {
            
            Configuration = config;
        }

        public HealthVaultAuthenticationOptions()
        {
            AutomaticAuthenticate = true;
            Description.DisplayName = "HealthVault";
            CallbackPath = new PathString("/signin-healthvault");
            SignInScheme = HealthVaultAuthenticationMiddleware.AuthenticationScheme;
            AuthenticationScheme = HealthVaultAuthenticationMiddleware.AuthenticationScheme;
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

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}
