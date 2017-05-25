// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.HealthVault.PlatformInformation
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
            Token = token;
            Description = description;
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
        public string Token { get; }

        /// <summary>
        /// Gets a localized text description of the Shell functionality
        /// accessible by using the token.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the Shell functionality description.
        /// </returns>
        ///
        public string Description { get; }

        /// <summary>
        /// Gets a collection of parameters that must be supplied in the query
        /// string in addition to the redirect token.
        /// </summary>
        ///
        /// <returns>
        /// A read-only collection containing the parameters.
        /// </returns>
        ///
        public ReadOnlyCollection<string> QueryStringParameters => new ReadOnlyCollection<string>(_queryStringParameters);

        private IList<string> _queryStringParameters;
    }
}
