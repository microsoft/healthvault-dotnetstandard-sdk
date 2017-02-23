// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// The exception representing an error while trying to log a user on to an application that
    /// has an authorized record which no longer meets the minimum requirements for that application
    /// or the application has changed its minimum authorization requirements.
    /// </summary>
    ///
    [Serializable]
    public sealed class HealthRecordReauthorizationRequiredException : HealthServiceException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordReauthorizationRequiredException"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        internal HealthRecordReauthorizationRequiredException()
            : base(ResourceRetriever.GetResourceString("RecordReauthorizationRequired"))
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordReauthorizationRequiredException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public HealthRecordReauthorizationRequiredException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordReauthorizationRequiredException"/>
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
        public HealthRecordReauthorizationRequiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors
    }
}
