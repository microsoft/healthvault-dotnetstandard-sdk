// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// The exception representing a HealthVault error code of
    /// <see cref = "HealthServiceStatusCode.AuthenticatedSessionTokenExpired"/>.
    /// </summary>
    ///
    [Serializable]
    public sealed class HealthServiceAuthenticatedSessionTokenExpiredException : HealthServiceException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceAuthenticatedSessionTokenExpiredException"/>
        /// class with the specified error information representing a
        /// HealthVault error code of
        /// <see cref = "HealthServiceStatusCode.AuthenticatedSessionTokenExpired"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type. This error indicates that the
        /// authorization token supplied to HealthVault is malformed or
        /// otherwise faulty.
        /// </remarks>
        ///
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        ///
        internal HealthServiceAuthenticatedSessionTokenExpiredException(HealthServiceResponseError error)
            : base(HealthServiceStatusCode.AuthenticatedSessionTokenExpired, error)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceAuthenticatedSessionTokenExpiredException"/>
        /// class with default values.
        /// </summary>
        ///
        public HealthServiceAuthenticatedSessionTokenExpiredException()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceAuthenticatedSessionTokenExpiredException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// A string representing the error message.
        /// </param>
        ///
        public HealthServiceAuthenticatedSessionTokenExpiredException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceAuthenticatedSessionTokenExpiredException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        ///
        public HealthServiceAuthenticatedSessionTokenExpiredException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors
    }
}
