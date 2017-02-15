// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Microsoft.HealthVault
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
        public string ServerName
        {
            get { return _serverName; }
            internal set { _serverName = value; }
        }
        private string _serverName;

        /// <summary>
        /// Gets the IP addresses of the server that was handling the request
        /// when the error occurred.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection of IP addresses.
        /// </value>
        ///
        public ReadOnlyCollection<IPAddress> ServerIPAddresses
        {
            get { return _serverIPAddresses; }
        }
        private ReadOnlyCollection<IPAddress> _serverIPAddresses;

        internal void SetServerIpAddresses(IList<IPAddress> ipAddresses)
        {
            _serverIPAddresses =
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
        public string InnerException
        {
            get { return _innerException; }
            internal set { _innerException = value; }
        }
        private string _innerException;

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
            string result = _serverName;

            foreach (IPAddress serverIP in _serverIPAddresses)
            {
                result = String.Join(" ",
                    new string[] { result, serverIP.ToString() });
            }

            result = String.Join(" ",
                new string[] { result, _innerException });

            return result;
        }
    }
}
