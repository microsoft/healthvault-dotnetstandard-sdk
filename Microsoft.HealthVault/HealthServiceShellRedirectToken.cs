// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents the redirect information that can be supplied along with the Shell redirect
    /// URL to access specific functionalities in the Shell.
    /// </summary>
    ///
    public class HealthServiceShellRedirectToken
    {
        internal HealthServiceShellRedirectToken(
            string token,
            string description,
            IList<string> queryStringParameters)
        {
            _token = token;
            _description = description;
            _queryStringParameters = queryStringParameters;
        }

        /// <summary>
        /// Gets a string token used to redirect to specific parts within the
        /// Shell, accessing specific functions.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the redirect token.
        /// </returns>
        ///
        public string Token
        {
            get { return _token; }
        }
        private string _token;

        /// <summary>
        /// Gets a localized text description of the Shell functionality
        /// accessible by using the token.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the Shell functionality description.
        /// </returns>
        ///
        public string Description
        {
            get { return _description; }
        }
        private string _description;

        /// <summary>
        /// Gets a collection of parameters that must be supplied in the query
        /// string in addition to the redirect token.
        /// </summary>
        ///
        /// <returns>
        /// A read-only collection containing the parameters.
        /// </returns>
        ///
        public ReadOnlyCollection<string> QueryStringParameters
        {
            get
            {
                return new ReadOnlyCollection<string>(_queryStringParameters);
            }
        }
        private IList<string> _queryStringParameters;
    }
}
