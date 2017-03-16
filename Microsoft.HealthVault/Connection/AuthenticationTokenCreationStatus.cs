// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Connection
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
