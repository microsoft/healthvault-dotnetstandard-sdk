// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.HealthVault
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
            : base()
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

        #region Serialization

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceAuthenticatedSessionTokenExpiredException"/> 
        /// class with the specified serialization information.
        /// </summary>
        /// 
        /// <param name="info">
        /// Serialized information about this exception.
        /// </param>
        /// 
        /// <param name="context">
        /// The source and destination of the serialized information.
        /// </param>
        /// 
        private HealthServiceAuthenticatedSessionTokenExpiredException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Serializes the exception.
        /// </summary>
        /// 
        /// <param name="info">
        /// The serialization information.
        /// </param>
        /// 
        /// <param name="context">
        /// The source and destination of the serialized information.
        /// </param>
        [SecurityCritical]
        [SecurityPermission(
            SecurityAction.LinkDemand,
            SerializationFormatter = true)]
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion Serialization

        #endregion FxCop required ctors
    }
}
