// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the deserialization of a
    /// type-specific thing fails.
    /// </summary>
    ///
    [Serializable]
    public class ThingDeserializationException : HealthVaultException
    {
        /// <summary>
        /// Creates an instance of the <see cref="ThingDeserializationException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The exception that occurred in the thing type
        /// deserializer.
        /// </param>
        ///
        public ThingDeserializationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="ThingDeserializationException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public ThingDeserializationException(string message)
            : base(message)
        {
        }

        #endregion FxCop required ctors
    }
}
