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
    /// <see cref = "HealthServiceStatusCode.RecordQuotaExceeded"/>.
    /// </summary>
    ///
    [Serializable]
    public sealed class HealthServiceRecordQuotaExceededException : HealthServiceException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceRecordQuotaExceededException"/>
        /// class with the specified error information to represent a
        /// HealthVault error code of
        /// <see cref = "HealthServiceStatusCode.RecordQuotaExceeded"/>.
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
        internal HealthServiceRecordQuotaExceededException(
            HealthServiceResponseError error)
            : base(HealthServiceStatusCode.RecordQuotaExceeded, error)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceRecordQuotaExceededException"/>
        /// class with default values.
        /// </summary>
        ///
        public HealthServiceRecordQuotaExceededException()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceRecordQuotaExceededException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public HealthServiceRecordQuotaExceededException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceRecordQuotaExceededException"/>
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
        public HealthServiceRecordQuotaExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors
    }
}
