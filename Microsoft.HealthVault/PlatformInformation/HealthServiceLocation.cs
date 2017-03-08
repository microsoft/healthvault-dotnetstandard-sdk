// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Configurations;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides methods that retrieve URLs of important locations for the
    /// HealthVault service.
    /// </summary>
    ///
    internal static class HealthServiceLocation
    {
        private static Lazy<IConfiguration> configuration = Ioc.Get<Lazy<IConfiguration>>();
        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service Shell
        /// URL redirector, given the specified redirect parameters.
        /// </summary>
        ///
        /// <param name="redirectParameters">
        /// Parameters used to contruct the redirect URL.
        /// </param>
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// The specified parameters construct an invalid URL.
        /// </exception>
        public static Uri GetHealthServiceShellUrl(ShellRedirectParameters redirectParameters)
        {
            ShellRedirectParameters paramsCopy = redirectParameters.Clone();

            // apply configuration
            // aib
            paramsCopy.AllowInstanceBounce = paramsCopy.AllowInstanceBounce ??
                configuration.Value.MultiInstanceAware;

            return paramsCopy.ConstructRedirectUrl();
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service Shell
        /// URL redirector, given the specified location.
        /// </summary>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> is passed as the target
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// The specific target location constructs an improper URL.
        /// </exception>
        ///
        public static Uri GetHealthServiceShellUrl(string targetLocation)
        {
            return GetHealthServiceShellUrl(configuration.Value.HealthVaultShellUrl, targetLocation);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service Shell
        /// URL redirector, given the specified location.
        /// </summary>
        ///
        /// <param name="shellUrl">
        /// The HealthVault Shell redirector URL.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> is passed as the target
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// The specific target location constructs an improper URL.
        /// </exception>
        ///
        public static Uri GetHealthServiceShellUrl(
            Uri shellUrl,
            string targetLocation)
        {
            return GetHealthServiceShellUrl(shellUrl, targetLocation, null);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service
        /// Shell URL redirector, given the specified location and query.
        /// </summary>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> is passed as the target
        /// parameter value to the redirector URL.
        /// The <paramref name="targetQuery"/> is URL-encoded and
        /// passed to the redirector URL as the target query string parameter
        /// value.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// The specific target location constructs an improper URL.
        /// </exception>
        ///
        public static Uri GetHealthServiceShellUrl(
            string targetLocation,
            string targetQuery)
        {
            return GetHealthServiceShellUrl(
                configuration.Value.HealthVaultShellUrl,
                targetLocation,
                targetQuery);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service
        /// Shell URL redirector, given the specified location and query.
        /// </summary>
        ///
        /// <param name="shellUrl">
        /// The HealthVault Shell redirector URL.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> is passed as the target
        /// parameter value to the redirector URL.
        /// The <paramref name="targetQuery"/> is URL-encoded and
        /// passed to the redirector URL as the target query string parameter
        /// value.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// The specific target location constructs an improper URL.
        /// </exception>
        ///
        public static Uri GetHealthServiceShellUrl(
            Uri shellUrl,
            string targetLocation,
            string targetQuery)
        {
            var redirect = new ShellRedirectParameters(shellUrl.OriginalString)
            {
                TargetLocation = targetLocation,
                TargetQueryString = targetQuery,
                TokenRedirectionMethod = "post"
            };

            return redirect.ConstructRedirectUrl();
        }

        internal static Uri GetServiceBaseUrl(Uri healthServiceUrl)
        {
            string url = healthServiceUrl.OriginalString;

            int index = url.LastIndexOf('/');
            if (index > 0)
            {
                url = url.Substring(0, index);
            }

            return new Uri(url);
        }
    }
}
