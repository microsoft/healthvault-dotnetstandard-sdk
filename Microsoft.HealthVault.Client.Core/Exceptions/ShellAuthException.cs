using System;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Client.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication through shell fails.
    /// </summary>
    public class ShellAuthException : HealthVaultException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAuthException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ShellAuthException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAuthException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception to wrap.</param>
        public ShellAuthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
