// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Indicates that a thing type handler has already been registered for
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

        #endregion FxCop required ctors
    }
}
