// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;
using System.Web;

namespace Microsoft.HealthVault.Web.Providers
{
    /// <summary>
    /// Manages WebConnectionInfo related to HealthVault authentication
    /// </summary>
    internal interface IWebConnectionInfoProvider
    {
        /// <summary>
        /// Creates web connection info from the token and instance id.
        /// Makes call to platform to get the HealthVault instance, also
        /// retrieves associated person info.
        /// </summary>
        /// <param name="token">token from shell after authentication</param>
        /// <param name="instanceId">instance id of the platform where the user data is stored</param>
        /// <returns></returns>
        Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId);

        /// <summary>
        /// Load WebConnectionInfo from the request conext cookie
        /// </summary>
        /// <param name="httpContext">httpcontext</param>
        /// <returns>WebConnectionInfo stroed in the cookie. Can return null in case the cookie is not present</returns>
        WebConnectionInfo TryLoad(HttpContextBase httpContext);

        /// <summary>
        /// Save webconnectioninfo to a cookie
        /// </summary>
        /// <param name="httpConext">httpcontext</param>
        /// <param name="webConnectionInfo">webconnectionInfo</param>
        void Save(HttpContextBase httpConext, WebConnectionInfo webConnectionInfo);

        /// <summary>
        /// Clear the cookie contianing webconnectioninfo
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        void Clear(HttpContextBase httpContext);
    }
}
