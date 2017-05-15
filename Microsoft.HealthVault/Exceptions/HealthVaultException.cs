using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Base class for all HealthVault exceptions thrown by the SDK.
    /// </summary>
    /// <remarks>HealthVault APIs may also throw system exceptions such as <see cref="InvalidOperationException"/>,
    /// <see cref="OperationCanceledException"/> or <see cref="ArgumentException"/> when appropriate.</remarks>
    public class HealthVaultException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthVaultException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HealthVaultException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthVaultException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception to wrap.</param>
        public HealthVaultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
