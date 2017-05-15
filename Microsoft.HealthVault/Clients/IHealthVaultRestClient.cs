// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.HealthVault.Rest;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// Defines the contract for a rest client that communicates with HealthVault
    /// </summary>
    public interface IHealthVaultRestClient : IClient
    {
        /// <summary>
        /// Adds authorization headers to a http message.
        /// </summary>
        /// <param name="message">The message that will be sent to HealthVault.</param>
        /// <param name="recordId">The record identifier.</param>
        void AuthorizeRestRequest(HttpRequestMessage message, Guid recordId);

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        /// <typeparam name="T">The returned model type.</typeparam>
        /// <param name="request">The request.</param>
        Task<T> ExecuteAsync<T>(IHealthVaultRestMessage<T> request);
    }
}
