// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Exceptions;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Runs through authorization UI with a browser.
    /// </summary>
    public interface IBrowserAuthBroker
    {
        /// <summary>
        /// Runs through authorization UI with a browser for the given URL.
        /// </summary>
        /// <param name="startUrl">The URL of the page to load in the browser.</param>
        /// <param name="stopUrlPrefix">The stop URL minus any query parameters.</param>
        /// <returns>The URL of the success page.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the flow by pressing the back button or closing
        /// the browser window.</exception>
        /// <exception cref="BrowserAuthException">Thrown if the authorization fails.</exception>
        Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix);
    }
}
