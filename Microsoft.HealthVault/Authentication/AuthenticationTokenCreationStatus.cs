// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Provides authentication token creation status codes.
    /// </summary>
    ///
    public enum AuthenticationTokenCreationStatus
    {
        /// <summary>
        /// The server returned a value that this client cannot read.
        /// </summary>
        ///
        Unknown = 0,

        /// <summary>
        /// The authentication token has been successfully returned.
        /// </summary>
        ///
        Success = 1,

        /// <summary>
        /// The person is not authorized for the specified application.
        /// </summary>
        ///
        PersonNotAuthorizedForApp = 2,

        /// <summary>
        /// The application requires acceptance by the person.
        /// </summary>
        ///
        PersonAppAcceptanceRequired = 3,

        /// <summary>
        /// The credential was not found.
        /// </summary>
        CredentialNotFound = 4,

        /// <summary>
        /// The person requires two factor authentication.
        /// </summary>
        SecondFactorAuthenticationRequired = 5
    }
}
