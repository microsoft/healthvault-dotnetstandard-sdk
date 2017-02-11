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
    /// Represents the exception thrown when the deserialization of a
    /// type-specific health record item fails.
    /// </summary>
    /// 
    [Serializable]
    public class HealthRecordItemDeserializationException : Exception
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemDeserializationException"/> 
        /// class with the specified message and inner exception.
        /// </summary>
        /// 
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// 
        /// <param name="innerException">
        /// The exception that occurred in the health record item type 
        /// deserializer.
        /// </param>
        /// 
        public HealthRecordItemDeserializationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemDeserializationException"/> 
        /// class with default values.
        /// </summary>
        /// 
        public HealthRecordItemDeserializationException()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemDeserializationException"/> 
        /// class with the specified message.
        /// </summary>
        /// 
        /// <param name="message">
        /// The error message.
        /// </param>
        /// 
        public HealthRecordItemDeserializationException(string message)
            : base(message)
        {
        }

        #region Serialization

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemDeserializationException"/> 
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
        protected HealthRecordItemDeserializationException(
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
