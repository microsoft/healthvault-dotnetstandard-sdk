// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.AspNetCore.Connection
{
    /// <summary>
    /// Represents a connection for an application to HealthVault for 
    /// operations that are performed when a user is offline using the 
    /// permissions granted by the user to the application. 
    /// </summary>
    /// 
    /// <remarks>
    /// A connection must be made to HealthVault to access the
    /// Web methods that the service exposes. Implementers of this class 
    /// does not maintain an open connection to the service.  It uses XML over HTTP to 
    /// to make requests and receive responses from the service. The connection
    /// just maintains the data necessary to make the request.
    /// <br/><br/>
    /// For operations that require the user to be online, use the 
    /// <see cref="IWebHealthVaultConnection"/> class.
    /// </remarks>
    /// 
    public interface IOfflineHealthVaultConnection : IHealthVaultConnection
    {
        /// <summary>
        /// Gets or sets the unique identifier of the offline person who granted 
        /// permissions to the calling application to perform certain 
        /// operations.
        /// </summary>
        /// 
        /// <value>
        /// A GUID representing the offline person.
        /// </value>
        Guid OfflinePersonId { get; }
    }
}
