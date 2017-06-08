// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Represents a connection for an application to the HealthVault service
    /// for operations
    /// </summary>
    public interface IHealthVaultConnection : IHealthVaultRestAuthorizer
    {
        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        Guid? ApplicationId { get; }

        /// <summary>
        /// Gets the person information for this account.
        /// </summary>
        /// <remarks>This includes the list of authorized records for the application instance.</remarks>
        Task<PersonInfo> GetPersonInfoAsync();

        /// <summary>
        /// Authenticates the connection.
        /// </summary>
        /// <remarks> This should depend on application platform - SODA vs WEB </remarks>
        Task AuthenticateAsync();

        /// <summary>
        /// Makes Web request call to HealthVault service
        /// for specified method name and method version
        /// </summary>
        /// <param name="method">The method to execute.</param>
        /// <param name="methodVersion">The method version.</param>
        /// <param name="parameters">Method parameters</param>
        /// <param name="recordId">Record Id</param>
        /// <param name="correlationId">correlationId</param>
        /// <returns>HealthServiceResponseData</returns>
        Task<HealthServiceResponseData> ExecuteAsync(HealthVaultMethods method, int methodVersion, string parameters = null, Guid? recordId = null, Guid? correlationId = null);

        /// <summary>
        /// Gets SessionCredential
        /// </summary>
        SessionCredential SessionCredential { get; }

        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        ///
        /// <returns>
        /// An instance implementing IThingClient
        /// </returns>
        IThingClient CreateThingClient();

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        ///
        /// <returns>
        /// An instance implementing IVocabularyClient
        /// </returns>
        IVocabularyClient CreateVocabularyClient();

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        ///
        /// <returns>
        /// An instance implementing IPersonClient
        /// </returns>
        IPersonClient CreatePersonClient();

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        ///
        /// <returns>
        /// An instance implementing IPlatformClient
        /// </returns>
        IPlatformClient CreatePlatformClient();
    }
}