using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Client.Exceptions
{
    /// <summary>
    /// Thrown from the browser auth flow when authentication fails.
    /// </summary>
    public class BrowserAuthException : HealthVaultException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserAuthException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public BrowserAuthException(int? errorCode)
            : base(Resources.BrowserAuthException)
        {
            this.HttpErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the error code from the browser.
        /// </summary>
        /// <remarks>&gt;= 300 means it's an HTTP error code. Otherwise it means there was a connection problem.</remarks>
        public int? HttpErrorCode { get; }
    }
}
