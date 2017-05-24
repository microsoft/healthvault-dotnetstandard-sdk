// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
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
            HttpErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the error code from the browser.
        /// </summary>
        /// <remarks>&gt;= 300 means it's an HTTP error code. Otherwise it means there was a connection problem.</remarks>
        public int? HttpErrorCode { get; }
    }
}
