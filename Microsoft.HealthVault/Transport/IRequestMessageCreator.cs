// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Creates request xml which will be used to send to HealthVault service
    /// </summary>
    interface IRequestMessageCreator
    {
        /// <summary>
        /// Create request xml
        /// </summary>
        /// <param name="method">HealthVault method</param>
        /// <param name="methodVersion">HealhtVault method version</param>
        /// <param name="isMethodAnonymous">In case the method is anonymous, then the request xml won't put any auth information</param>
        /// <param name="parameters">Method parameters, which will become infoxml</param>
        /// <param name="recordId">RecordId, in case the method is record specfic, like "GetThings"</param>
        /// <param name="appId">ApplicationId, will be used when the method doesn't need authentication, like NewApplicationInfo, GetServiceDefintion methods</param>
        /// <returns></returns>
        string Create(
            HealthVaultMethods method,
            int methodVersion,
            bool isMethodAnonymous,
            string parameters = null,
            Guid? recordId = null,
            Guid? appId = null);
    }
}
