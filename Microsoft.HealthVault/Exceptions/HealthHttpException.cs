// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Net;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when a failure occurs during a
    /// cloud call.
    /// </summary>
    ///
    public class HealthHttpException : HealthVaultException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthHttpException"/>
        /// class with the specified message and status code.
        /// </summary>
        /// 
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// 
        /// <param name="statusCode">
        /// The HTTP error code corresponding with the error
        /// </param>
        public HealthHttpException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthHttpException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The exception message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The exception that occurred during the certificate operation.
        /// </param>
        ///
        public HealthHttpException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// The error status code that was returned resulting in this exception
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
    }
}
