// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.HealthVault.Rest
{
    /// <summary>
    /// Defines a message that will be sent to the HealthVault endpoint
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class HealthVaultRestMessage<TResponse> : IHealthVaultRestMessage<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthVaultRestContentMessage{TResponse}" /> class.
        /// </summary>
        /// <param name="path">The path of the REST API.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="httpMethod">The HTTP method to be used.</param>
        public HealthVaultRestMessage(Uri path, Guid recordId, HttpMethod httpMethod)
        {
            Path = path;
            RecordId = recordId;
            HttpMethod = httpMethod;
            CustomHeaders = new Dictionary<string, string>();
        }

        public Uri Path { get; }

        public HttpMethod HttpMethod { get; }

        public int ApiVersion { get; set; } = 1;

        public Guid RecordId { get; }

        public IDictionary<string, string> CustomHeaders { get; }
    }
}