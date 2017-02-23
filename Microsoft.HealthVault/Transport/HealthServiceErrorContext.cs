// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains the error context of the service when the error occurred.
    /// </summary>
    ///
    public class HealthServiceErrorContext
    {
        /// <summary>
        /// Gets the name of the server that was handling the request when
        /// the error occurred.
        /// </summary>
        ///
        /// <value>
        /// A string representing the server name.
        /// </value>
        ///
        public string ServerName { get; internal set; }

        /// <summary>
        /// Gets the IP addresses of the server that was handling the request
        /// when the error occurred.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection of IP addresses.
        /// </value>
        ///
        public ReadOnlyCollection<IPAddress> ServerIPAddresses { get; private set; }

        internal void SetServerIpAddresses(IList<IPAddress> ipAddresses)
        {
            this.ServerIPAddresses =
                new ReadOnlyCollection<IPAddress>(ipAddresses);
        }

        /// <summary>
        /// Gets the exception message and stack trace of the exception
        /// from the server.
        /// </summary>
        ///
        /// <value>
        /// A string representing the exception message and stack trace.
        /// </value>
        ///
        /// <remarks>
        /// This is the ToString() of the exception that occurred while
        /// handling the request.
        /// </remarks>
        ///
        public string InnerException { get; internal set; }

        /// <summary>
        /// Retrieves the string representation of the <see cref="HealthServiceErrorContext"/>
        /// object.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the contents of the <see cref="HealthServiceErrorContext"/>
        /// object.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = this.ServerName;

            foreach (IPAddress serverIP in this.ServerIPAddresses)
            {
                result = string.Join(" ", result, serverIP.ToString());
            }

            result = string.Join(" ", result, this.InnerException);

            return result;
        }
    }
}
