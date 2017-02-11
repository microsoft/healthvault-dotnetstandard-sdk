// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health.Authentication
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
        public Guid ApplicationId
        {
            get { return _applicationId; }
            internal set { _applicationId = value; }
        }
        private Guid _applicationId;

        /// <summary>
        /// Gets the authentication token creation status.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="AuthenticationTokenCreationStatus"/>.
        /// </returns>
        /// 
        public AuthenticationTokenCreationStatus Status
        {
            get { return _status; }
            internal set { _status = value; }
        }
        private AuthenticationTokenCreationStatus _status;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="AuthenticationToken"/>.
        /// </returns>
        /// 
        public string AuthenticationToken
        {
            get { return _authToken; }
            set { _authToken = value; }
        }
        private string _authToken;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="AuthenticationToken"/>.
        /// </returns>
        /// 
        public string StsTokenPayload
        {
            get { return _stsTokenPayload; }
            internal set { _stsTokenPayload = value; }
        }
        private string _stsTokenPayload;

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
        public ApplicationRecordAuthorizationAction ApplicationRecordAuthorizationAction
        {
            get { return _action; }
            internal set { _action = value; }
        }
        private ApplicationRecordAuthorizationAction _action;

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

