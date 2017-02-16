// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace Microsoft.HealthVault
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
                    queryParams.Split(new Char[] { ',' });

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
            _baseUrl = baseUrl;
            _redirectUrl = redirectUrl;
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
        public Uri BaseUrl
        {
            get { return _baseUrl; }
        }
        private Uri _baseUrl;

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
        public Uri RedirectUrl
        {
            get { return _redirectUrl; }
        }
        private Uri _redirectUrl;

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
        public ReadOnlyCollection<HealthServiceShellRedirectToken> RedirectTokens
        {
            get
            {
                return new ReadOnlyCollection<HealthServiceShellRedirectToken>(_redirectTokens);
            }
        }
        private IList<HealthServiceShellRedirectToken> _redirectTokens;
    }
}
