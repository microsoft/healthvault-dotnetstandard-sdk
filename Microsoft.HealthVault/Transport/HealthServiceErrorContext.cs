// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains the error context of the service when the error occurred.
    /// </summary>
    ///
    internal class HealthServiceErrorContext
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
            ServerIPAddresses =
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
            string result = ServerName;

            foreach (IPAddress serverIP in ServerIPAddresses)
            {
                result = string.Join(" ", result, serverIP.ToString());
            }

            result = string.Join(" ", result, InnerException);

            return result;
        }
    }
}
