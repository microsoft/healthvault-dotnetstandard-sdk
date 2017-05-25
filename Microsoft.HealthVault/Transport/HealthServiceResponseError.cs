// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains error information for a response that has a code other
    /// than <see cref="HealthServiceStatusCode.Ok"/>.
    /// </summary>
    ///
    public class HealthServiceResponseError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        ///
        /// <value>
        /// A string representing the error message.
        /// </value>
        ///
        /// <remarks>
        /// The message contains localized text of why the request failed.
        /// This text should be added to application context information
        /// and suggestions of what to do before displaying it to the user.
        /// </remarks>
        ///
        public string Message
        {
            get { return _message; }
            internal set { _message = value; }
        }

        private string _message;

        /// <summary>
        /// Gets the context of the server in which the error occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceErrorContext"/> representing the server context.
        /// </value>
        ///
        /// <remarks>
        /// This information is available only when the service is configured
        /// in debugging mode. In all other cases, this property returns
        /// <b>null</b>.
        /// </remarks>
        ///
        internal HealthServiceErrorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private HealthServiceErrorContext _context;

        /// <summary>
        /// Gets the additional information specific to the method failure.
        /// </summary>
        ///
        /// <value>
        /// A string representing the additional error information.
        /// </value>
        ///
        /// <remarks>
        /// The text contains specific actionable information related to the failed request.
        /// It may be used in determining possible actions to circumvent the error condition
        /// before displaying an error to the user.
        /// </remarks>
        ///
        public string ErrorInfo
        {
            get { return _errorInfo; }
            internal set { _errorInfo = value; }
        }

        private string _errorInfo;

        /// <summary>
        /// Gets the string representation of the <see cref="HealthServiceErrorContext"/>
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
            string result =
                string.Join(" ", GetType().ToString(), _message);
            return result;
        }
    }
}
