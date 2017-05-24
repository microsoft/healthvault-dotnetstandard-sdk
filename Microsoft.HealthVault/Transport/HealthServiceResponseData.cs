// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Net.Http.Headers;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains the response information from the HealthVault service after
    /// processing a request.
    /// </summary>
    ///
    public class HealthServiceResponseData
    {
        // Prevents creation of an instance.
        internal HealthServiceResponseData()
        {
        }

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceStatusCode"/> value.
        /// </value>
        ///
        /// <remarks>
        /// Any code other than <see cref="HealthServiceStatusCode.Ok"/> indicates
        /// an error. Use the <see cref="HealthServiceResponseData.Error"/> property
        /// to get more information about the error.
        /// </remarks>
        ///
        public HealthServiceStatusCode Code => HealthServiceStatusCodeManager.GetStatusCode(CodeId);

        /// <summary>
        /// Gets the integer identifier of the status code in the HealthVault
        /// response.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the status code ID.
        /// </value>
        ///
        /// <remarks>
        /// Use this property when the SDK is out of sync with the HealthVault
        /// status code set. You can look up the actual integer value of the
        /// status code for further investigation.
        /// </remarks>
        ///
        public int CodeId { get; internal set; }

        /// <summary>
        /// Gets the information about an error that occurred while processing
        /// the request.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceResponseError"/>.
        /// </value>
        /// <remarks>
        /// This property is <b>null</b> if
        /// <see cref="HealthServiceResponseData.Code"/> returns
        /// <see cref="HealthServiceStatusCode.Ok"/>.
        /// </remarks>
        ///
        public HealthServiceResponseError Error { get; internal set; }

        /// <summary>
        /// Gets the info section of the response XML.
        /// </summary>
        ///
        public XPathNavigator InfoNavigator { get; internal set; }

        /// <summary>
        /// Gets the headers on the response.
        /// </summary>
        public HttpResponseHeaders ResponseHeaders { get; internal set; }
    }
}
