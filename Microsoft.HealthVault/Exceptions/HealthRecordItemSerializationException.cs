// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the serialization of a 
    /// type-specific health record item fails or if a mandatory element in 
    /// the health record item is missing.
    /// </summary>
    /// 
    [Serializable]
    public class HealthRecordItemSerializationException : Exception
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemSerializationException"/> 
        /// class with the specified message.
        /// </summary>
        /// 
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// 
        public HealthRecordItemSerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemSerializationException"/> 
        /// class with the specified message and inner exception.
        /// </summary>
        /// 
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// 
        /// <param name="innerException">
        /// The exception that occurred in the health record item type 
        /// serializer.
        /// </param>
        /// 
        public HealthRecordItemSerializationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthRecordItemSerializationException"/> 
        /// class  with default values.
        /// </summary>
        /// 
        public HealthRecordItemSerializationException()
        {
        }
        
        #endregion FxCop required ctors
    }

}
