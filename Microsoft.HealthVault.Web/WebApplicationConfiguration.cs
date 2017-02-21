// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Contains the application configuration as retrieved from
    /// web.config or as constants.
    /// </summary>
    ///
    public sealed class WebApplicationConfiguration
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
        /// The name of the cookie for use by applications and the base
        /// HealthServicePage to store authentication information.
        /// </summary>
        ///
        public static readonly string CookieName =
            HttpContext.Current == null ? String.Empty :
                (HttpRuntime.AppDomainAppVirtualPath + "_wcpage")
                .Substring(1);

        /// <summary>
        /// Shell auth page location including the application id.
        /// </summary>
        ///
        public static Uri ShellAuthenticationUrl => HealthWebApplicationConfiguration.Current.HealthVaultShellAuthenticationUrl;

        /// <summary>
        /// Application id, from web config.
        /// </summary>
        ///
        public static readonly Guid AppId = HealthWebApplicationConfiguration.Current.ApplicationId;

        /// <summary>
        /// Shell url, from web config.
        /// </summary>
        ///
        public static Uri ShellUrl => HealthWebApplicationConfiguration.Current.HealthVaultShellUrl;

        /// <summary>
        /// Cookie domain, from web config.
        /// </summary>
        ///
        public static readonly string CookieDomain = HealthWebApplicationConfiguration.Current.CookieDomain;

        /// <Summary>
        /// Cookie path, from web config.
        /// </Summary>
        ///
        public static readonly string CookiePath = HealthWebApplicationConfiguration.Current.CookiePath;

        /// <summary>
        /// Cookie timeout in minutes, from web config.
        /// </summary>
        ///
        public static readonly int CookieTimeoutMinutes = HealthWebApplicationConfiguration.Current.CookieTimeoutMinutes;

        /// <summary>
        /// The maximum cookie timeout in minutes, from web config.
        /// </summary>
        ///
        /// <remarks>
        /// The cookie timeout is defaulted in the web config but the
        /// application configuration in HealthVault can allow for persistent
        /// tokens which require a longer cookie life. The cookie will take
        /// on the life of the token when the page receives a "tokenTtl"
        /// query string parameter when receiving the authentication token.
        /// The MaxCookieTimeoutMinutes limits the cookie timeout to a
        /// maximum value even when the CookieTimeoutMinutes or the token TTL
        /// from the applications configuration is larger.
        /// </remarks>
        ///
        public static readonly int MaxCookieTimeoutMinutes = HealthWebApplicationConfiguration.Current.MaxCookieTimeoutMinutes;

        /// <summary>
        /// The 32 byte key used to encrypt cookies for privacy.
        /// </summary>
        /// <remarks>
        /// The cookie encryption key is an optional string consisting of 32 hex digits (0-F).
        /// When a key is assigned to an application, any existing unencrypted cookies are still readable.
        /// </remarks>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2105:ArrayFieldsShouldNotBeReadOnly",
            Justification = "Each access returns a new array")]
        public static readonly byte[] CookieEncryptionKey = HealthWebApplicationConfiguration.Current.CookieEncryptionKey;

        /// <summary>
        /// Whether to secure the connection with SSL, from web config.
        /// </summary>
        public static readonly bool UseSslForSecurity = HealthWebApplicationConfiguration.Current.UseSslForSecurity;

        /// <summary>
        /// Use the asp session rather than a cookie, from web config.
        /// </summary>
        ///
        public static readonly bool UseAspSession = HealthWebApplicationConfiguration.Current.UseAspSession;

        /// <summary>
        /// If true, the application is a multi-record application.
        /// </summary>
        ///
        public static readonly bool IsMra = HealthWebApplicationConfiguration.Current.IsMultipleRecordApplication;

        /// <summary>
        /// Indicates whether a signup code is required for a user to create
        /// a HealthVault account.
        /// </summary>
        ///
        public static readonly bool IsSignupCodeRequired = HealthWebApplicationConfiguration.Current.IsSignupCodeRequired;

        /// <summary>
        /// HealthVault url, from web config.
        /// </summary>
        ///
        public static Uri HealthServiceUrl => HealthWebApplicationConfiguration.Current.HealthVaultUrl;

        /// <summary>
        /// HealthVault root url, from web config.
        /// </summary>
        public static Uri HealthServiceRootUrl => HealthWebApplicationConfiguration.Current.HealthVaultUrl;

        /// <summary>
        /// Shell redirector url, derived from web config.
        /// </summary>
        ///
        public static Uri ShellRedirectorUrl => UrlPathAppend(HealthWebApplicationConfiguration.Current.HealthVaultShellUrl, ShellRedirectorLocation);

        private const string ShellRedirectorLocation = "redirect.aspx?target=";

        /// <summary>
        /// HealthVault handler url, derived from web config.
        /// </summary>
        ///
        public static Uri HealthServiceHandlerUrl => HealthWebApplicationConfiguration.Current.GetHealthVaultMethodUrl();

        /// <summary>
        /// Gets the URL for the specified application action.
        /// </summary>
        ///
        /// <param name="action">
        /// The action to get the URL for.
        /// </param>
        ///
        /// <returns>
        /// The URL the application configured for the specified action.
        /// </returns>
        ///
        /// <remarks>
        /// Applications that use the <see cref="HealthServiceActionPage"/>
        /// can configure URLs for each action in the web.config file. They
        /// should have a key prefix of "WCPage_Action" with the action name
        /// appended. For example, for a the EULA action page, the
        /// web.config should contain a key of WCPage_ActionEula with a
        /// value containing the URL of the EULA page.
        /// </remarks>
        ///
        public static string GetActionUrl(string action)
        {
            return HealthWebApplicationConfiguration.Current.GetActionUrl(action).ToString();
        }

        /// <summary>
        /// Gets the list of allowed redirect sites.
        /// </summary>
        ///
        ///
        /// <returns>
        /// A comma-separated list of allowed url redirection sites.
        /// </returns>
        ///
        /// <remarks>
        /// Applications that use the <see cref="HealthServiceActionPage"/>
        /// can configure a list of allowed redirect sites.
        ///
        /// When a HealthVault shell redirect happens to a
        /// <see cref="HealthServiceActionPage"/>, it will be allowed if the actionqs parameter
        /// is a relative URL or a URL that points to the site the application is running on.
        /// All other sites will note be allowed.
        /// To allow another site, the web.config should contain a key of
        /// WCPage_AllowedRedirectSites that has the site as part of a comma-
        /// separated list.
        /// </remarks>
        ///
        public static string AllowedRedirectSites => HealthWebApplicationConfiguration.Current.AllowedRedirectSites;

        /// <summary>
        /// Non-production action url redirect override.
        /// </summary>
        ///
        public static string ActionUrlRedirectOverride => HealthWebApplicationConfiguration.Current.ActionUrlRedirectOverride.ToString();

        #region DataGrid

        /// <summary>
        /// Gets the configured number of items that should be shown for a
        /// <see cref="HealthRecordItemDataGrid"/> page.
        /// </summary>
        ///
        public static int DataGridItemsPerPage => HealthWebApplicationConfiguration.Current.DataGridItemsPerPage;

        #endregion DataGrid

        /// <summary>
        /// Appends the specified path to the URL after trimming the path.
        /// </summary>
        /// <param name="baseUrl">The base URL to trim and append the path to.
        /// </param>
        /// <param name="path">The path to append to the URL.</param>
        /// <returns>The combined URL and path.</returns>
        private static Uri UrlPathAppend(Uri baseUrl, string path)
        {
            return new Uri(baseUrl, path);
        }
    }
}
