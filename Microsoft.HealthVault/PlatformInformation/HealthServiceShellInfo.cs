// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about the HealthVault Shell.
    /// </summary>
    ///
    public class HealthServiceShellInfo
    {
        /// <summary>
        /// Constructs a <see cref="HealthServiceShellInfo"/> object from the
        /// supplied XML.
        /// </summary>
        ///
        /// <param name="nav">
        /// An XPathNavigator to access the XML from which the
        /// <see cref="HealthServiceShellInfo"/> object will be constructed.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthServiceShellInfo"/> object.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// A URL string returned by HealthVault is
        /// invalid.
        /// </exception>
        ///
        internal static HealthServiceShellInfo CreateShellInfo(
            XPathNavigator nav)
        {
            Uri baseUrl = new Uri(nav.SelectSingleNode("url").Value);
            Uri redirectUrl =
                new Uri(nav.SelectSingleNode("redirect-url").Value);

            XPathNodeIterator tokenNavs = nav.Select("redirect-token");

            List<HealthServiceShellRedirectToken> redirectTokens =
                new List<HealthServiceShellRedirectToken>();

            foreach (XPathNavigator tokenNav in tokenNavs)
            {
                string token = tokenNav.SelectSingleNode("token").Value;
                string description =
                    tokenNav.SelectSingleNode("description").Value;

                string queryParams = tokenNav.SelectSingleNode(
                    "querystring-parameters").Value;

                string[] queryStringParameters =
                    queryParams.Split(',');

                HealthServiceShellRedirectToken redirectToken =
                    new HealthServiceShellRedirectToken(token, description, queryStringParameters);

                redirectTokens.Add(redirectToken);
            }

            HealthServiceShellInfo shellInfo =
                new HealthServiceShellInfo(baseUrl, redirectUrl, redirectTokens);

            return shellInfo;
        }

        /// <summary>
        /// Construct an instance of the class.
        /// </summary>
        /// <param name="baseUrl">The Shell base URL.</param>
        /// <param name="redirectUrl">The Shell redirect URL.</param>
        /// <param name="redirectTokens">A list of redirect information.</param>
        public HealthServiceShellInfo(
            Uri baseUrl,
            Uri redirectUrl,
            IList<HealthServiceShellRedirectToken> redirectTokens)
        {
            BaseUrl = baseUrl;
            RedirectUrl = redirectUrl;
            _redirectTokens = redirectTokens;
        }

        /// <summary>
        /// Gets the Shell base URL.
        /// </summary>
        ///
        /// <returns>
        /// A Uri representing the Shell base URL.
        /// </returns>
        ///
        /// <remarks>
        /// The URL used to access the HealthVault Shell.
        /// </remarks>
        ///
        public Uri BaseUrl { get; }

        /// <summary>
        /// Gets the Shell redirect URL.
        /// </summary>
        ///
        /// <returns>
        /// A Uri representing the Shell redirect URL.
        /// </returns>
        ///
        /// <remarks>
        /// The URL used to redirect to specific functions within the
        /// HealthVault Shell.
        /// </remarks>
        ///
        public Uri RedirectUrl { get; }

        /// <summary>
        /// Gets a collection of the possible redirect information that can be
        /// supplied along with the shell redirect URL to access specific
        /// functionalities in the Shell.
        /// </summary>
        ///
        /// <returns>
        /// A read-only collection containing the redirect information.
        /// </returns>
        ///
        public ReadOnlyCollection<HealthServiceShellRedirectToken> RedirectTokens => new ReadOnlyCollection<HealthServiceShellRedirectToken>(_redirectTokens);

        private IList<HealthServiceShellRedirectToken> _redirectTokens;
    }
}
