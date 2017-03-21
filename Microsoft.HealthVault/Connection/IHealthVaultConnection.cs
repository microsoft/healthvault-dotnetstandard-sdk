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
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Represents a connection for an application to the HealthVault service
    /// for operations
    /// </summary>
    public interface IHealthVaultConnection
    {
        /// <summary>
        /// The HealthVault web-service instance.
        /// </summary>
        HealthServiceInstance ServiceInstance { get; }

        /// <summary>
        /// Gets or sets the session credential.
        /// </summary>
        /// <value>
        /// The session credential.
        /// </value>
        SessionCredential SessionCredential { get; }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        Guid ApplicationId { get; }

        /// <summary>
        /// Gets the person information for this account.
        /// </summary>
        /// <remarks>This includes the list of authorized records for the application instance.</remarks>
        PersonInfo PersonInfo { get; }

        /// <summary>
        /// Gets a client of a given type.
        /// </summary>
        /// <typeparam name="TClient">The type of the client to retrieve</typeparam>
        /// <returns>A client instance</returns>
        TClient GetClient<TClient>()
            where TClient : IClient;

        /// <summary>
        /// A client that can be used to access information about the platform.
        /// </summary>
        IPlatformClient PlatformClient { get; }

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        IPersonClient PersonClient { get; }

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        IVocabularyClient VocabularyClient { get; }

        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        /// <param name="record">The record to associate the thing client with</param>
        /// <returns>An instance implementing IThingClient</returns>
        IThingClient GetThingClient(HealthRecordInfo record);

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <param name="record">The record to associate the action plan client with</param>
        /// <returns>An instance implementing IActionPlanClient</returns>
        IActionPlanClient GetActionPlanClient(HealthRecordInfo record);

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
        /// <returns>HealthServiceResponseData</returns>
        Task<HealthServiceResponseData> ExecuteAsync(
            HealthVaultMethods method,
            int methodVersion,
            string parameters = null,
            Guid? recordId = null);
    }
}