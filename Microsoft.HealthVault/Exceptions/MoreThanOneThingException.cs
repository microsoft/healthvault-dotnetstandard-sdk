using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Thrown when we expect one thing to be returned, but instead get multiple things.
    /// </summary>
    public class MoreThanOneThingException : HealthVaultException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoreThanOneThingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MoreThanOneThingException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreThanOneThingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception to wrap.</param>
        public MoreThanOneThingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
