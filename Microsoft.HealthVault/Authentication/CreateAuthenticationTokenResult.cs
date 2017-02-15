// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Authentication
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
            return (result != null
                    && result.Status == AuthenticationTokenCreationStatus.Success
                    && result.ApplicationRecordAuthorizationAction ==
                        ApplicationRecordAuthorizationAction.NoActionRequired
                    && !String.IsNullOrEmpty(result.AuthenticationToken));
        }
    }
}
