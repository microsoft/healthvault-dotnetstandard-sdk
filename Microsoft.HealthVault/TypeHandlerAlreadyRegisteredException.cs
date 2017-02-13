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
    /// Indicates that a health record item type handler has already been registered for 
    /// the specified item type.
    /// </summary>
    /// 
    [Serializable]
    public sealed class TypeHandlerAlreadyRegisteredException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TypeHandlerAlreadyRegisteredException"/>
        /// class with default values.
        /// </summary>
        /// 
        public TypeHandlerAlreadyRegisteredException()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeHandlerAlreadyRegisteredException"/>
        /// class with the specified message.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// 
        public TypeHandlerAlreadyRegisteredException(string message)
            : base(message)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates a new instance of the <see cref="TypeHandlerAlreadyRegisteredException"/>
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
        public TypeHandlerAlreadyRegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #region Serialization

        /// <summary>
        /// Creates a new instance of the <see cref="TypeHandlerAlreadyRegisteredException"/>
        /// class with the specified serialization information.
        /// </summary>
        /// 
        /// <param name="info">
        /// Serialized information about this exception.
        /// </param>
        /// 
        /// <param name="context">
        /// The stream context of the serialized information.
        /// </param>
        /// 
        private TypeHandlerAlreadyRegisteredException(
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
        /// The serialization context.
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
