// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.RestApi.Generated;
using Microsoft.Rest;

namespace Microsoft.HealthVault.RestApi
{
    /// <summary>
    /// Provides extension methods for creating <see cref="IMicrosoftHealthVaultRestApi"/>
    /// </summary>
    public static class HealthVaultRestClientExtensions
    {
        /// <summary>
        /// Creates a client for accessing the Microsoft HealthVault REST API.
        /// </summary>
        /// <param name="connection">The HealthVault connection.</param>
        /// <param name="recordId">The record identifier.</param>
        public static IMicrosoftHealthVaultRestApi CreateMicrosoftHealthVaultRestApi(this IHealthVaultConnection connection, Guid recordId)
        {
            Uri restUrl = Ioc.Get<HealthVaultConfiguration>().RestHealthVaultUrl;
            ServiceClientCredentials credentials = new HealthVaultRestCredentials(connection, recordId);
            return new MicrosoftHealthVaultRestApi(restUrl, credentials, (connection as IMessageHandlerFactory)?.Create());
        }
    }
}