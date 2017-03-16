// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Encapsulates the authentication token creation results.
    /// </summary>
    ///
    public class CreateAuthenticationTokenResult
    {
        /// <summary>
        /// Gets the authentication token application id.
        /// </summary>
        ///
        /// <returns>
        /// The application id guid.
        /// </returns>
        /// 
        public Guid ApplicationId { get; internal set; }

        /// <summary>
        /// Gets the authentication token creation status.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="AuthenticationTokenCreationStatus"/>.
        /// </returns>
        /// 
        public AuthenticationTokenCreationStatus Status { get; internal set; }

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="AuthenticationToken"/>.
        /// </returns>
        /// 
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="AuthenticationToken"/>.
        /// </returns>
        /// 
        public string StsTokenPayload { get; internal set; }

        /// <summary>
        /// Gets the application record authorization action.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="ApplicationRecordAuthorizationAction"/>.
        /// </returns>
        ///
        /// <remarks>
        /// The application record authorization action only
        /// applies if the value of <see cref="Status"/> is Success.
        /// </remarks>
        ///
        /// <seealso cref="Status"/>
        /// 
        public ApplicationRecordAuthorizationAction ApplicationRecordAuthorizationAction { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the result contains a successfully
        /// authenticated token.
        /// </summary>
        ///
        /// <param name="result">
        /// The result to query.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if the result contains a successfully
        /// authenticated token; otherwise, <b>false</b>.
        /// </returns>
        ///
        internal static bool IsAuthenticated(CreateAuthenticationTokenResult result)
        {
            return result != null
                    && result.Status == AuthenticationTokenCreationStatus.Success
                    && result.ApplicationRecordAuthorizationAction == ApplicationRecordAuthorizationAction.NoActionRequired
                    && !string.IsNullOrEmpty(result.AuthenticationToken);
        }
    }
}
