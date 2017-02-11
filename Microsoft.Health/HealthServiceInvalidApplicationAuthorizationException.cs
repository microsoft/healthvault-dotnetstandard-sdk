// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Health
{
    /// <summary>
    /// The exception representing a HealthVault error code of 
    /// <see cref="Microsoft.Health.HealthServiceStatusCode.InvalidApplicationAuthorization"/>.
    /// </summary>
    /// 
    [Serializable]
    public sealed class HealthServiceInvalidApplicationAuthorizationException
        : HealthServiceException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceInvalidApplicationAuthorizationException"/> 
        /// class with the specified error information to represent a 
        /// HealthVault error code of 
        /// <see cref="Microsoft.Health.HealthServiceStatusCode.InvalidApplicationAuthorization"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers 
        /// using the SDK should catch instances of this exception instead of 
        /// throwing new exceptions of this type. The error indicates that the 
        /// application does not have an entry in the authorized application 
        /// information and as such is not authorized to perform the task.
        /// </remarks>
        /// 
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        /// 
        internal HealthServiceInvalidApplicationAuthorizationException(HealthServiceResponseError error)
            : base(HealthServiceStatusCode.InvalidApplicationAuthorization, error)
        {
        }
        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceInvalidApplicationAuthorizationException"/> 
        /// class with default values.
        /// </summary>
        /// 
        public HealthServiceInvalidApplicationAuthorizationException()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceInvalidApplicationAuthorizationException"/> 
        /// class with the specified message.
        /// </summary>
        /// 
        /// <param name="message">
        /// The error message.
        /// </param>
        /// 
        public HealthServiceInvalidApplicationAuthorizationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceInvalidApplicationAuthorizationException"/> 
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
        public HealthServiceInvalidApplicationAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #region Serialization

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceInvalidApplicationAuthorizationException"/> 
        /// class with the specified serialization information and context.
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
        private HealthServiceInvalidApplicationAuthorizationException(
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
