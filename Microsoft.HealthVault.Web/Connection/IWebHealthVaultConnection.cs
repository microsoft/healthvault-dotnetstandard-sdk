// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Web.Connection
{
    /// <summary>
    /// Represents an authenticated interface to HealthVault. 
    /// </summary>
    /// 
    /// <remarks>
    /// Most operations performed against the service require authentication. 
    /// A connection must be made to HealthVault to access the
    /// web methods that the service exposes. The implementation class does not maintain
    /// an open connection to the service. It uses XML over HTTP to 
    /// make requests and receive responses from the service. The connection
    /// just maintains the data necessary to make the request.
    /// </remarks>
    /// 
    public interface IWebHealthVaultConnection : IHealthVaultConnection
    {
        /// <summary>
        /// Gets the UserAuthToken.
        /// </summary>
        /// 
        /// <remarks>
        /// This is the credential token retrieved from the HealthVault 
        /// platform.  By specifying a User Auth Token, the web application
        /// credential can operate in the context of an authenticated user.
        /// </remarks>
        /// 
        /// <returns>
        /// A string representing the UserAuthToken.
        /// </returns>
        /// 
        string UserAuthToken { get; }
    }
}
