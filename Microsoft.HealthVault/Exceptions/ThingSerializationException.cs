// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the serialization of a
    /// type-specific thing fails or if a mandatory element in
    /// the thing is missing.
    /// </summary>
    ///
    public class ThingSerializationException : HealthVaultException
    {
        /// <summary>
        /// Creates an instance of the <see cref="ThingSerializationException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        public ThingSerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ThingSerializationException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The exception that occurred in the thing type
        /// serializer.
        /// </param>
        ///
        public ThingSerializationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
