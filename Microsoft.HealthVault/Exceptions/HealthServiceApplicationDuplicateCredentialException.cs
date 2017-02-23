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
    /// <see cref = "HealthServiceStatusCode.DuplicateCredentialFound"/>.
    /// </summary>
    ///
    [Serializable]
    public sealed class HealthServiceApplicationDuplicateCredentialException
        : HealthServiceException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceApplicationDuplicateCredentialException"/>
        /// class with the specified error information to represent a
        /// HealthVault error code of
        /// <see cref = "HealthServiceStatusCode.DuplicateCredentialFound"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        ///
        internal HealthServiceApplicationDuplicateCredentialException(
            HealthServiceResponseError error)
            : base(HealthServiceStatusCode.DuplicateCredentialFound, error)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceApplicationDuplicateCredentialException"/>
        /// class with default values.
        /// </summary>
        ///
        public HealthServiceApplicationDuplicateCredentialException()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceApplicationDuplicateCredentialException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public HealthServiceApplicationDuplicateCredentialException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceApplicationDuplicateCredentialException"/>
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
        public HealthServiceApplicationDuplicateCredentialException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors
    }
}
