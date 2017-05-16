using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Thrown when the record could not be found.
    /// </summary>
    public class HealthRecordNotFoundException : HealthVaultException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthRecordNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HealthRecordNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthRecordNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception to wrap.</param>
        public HealthRecordNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
