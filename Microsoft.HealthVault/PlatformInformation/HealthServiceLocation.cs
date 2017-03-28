// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides methods that retrieve URLs of important locations for the
    /// HealthVault service.
    /// </summary>
    ///
    internal static class HealthServiceLocation
    {
        private static HealthVaultConfiguration configuration = Ioc.Get<HealthVaultConfiguration>();

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault service Shell
        /// URL redirector, given the specified redirect parameters.
        /// </summary>
        ///
        /// <param name="redirectParameters">
        /// Parameters used to construct the redirect URL.
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
                configuration.MultiInstanceAware;

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
            return GetHealthServiceShellUrl(configuration.DefaultHealthVaultShellUrl, targetLocation);
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
                configuration.DefaultHealthVaultShellUrl,
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
