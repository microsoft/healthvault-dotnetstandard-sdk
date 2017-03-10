using System;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Client.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication through shell fails.
    /// </summary>
    public class ShellAuthException : HealthServiceException
    {
        public ShellAuthException(string message)
            : base(message)
        {
        }

        public ShellAuthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
