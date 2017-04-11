// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using Microsoft.HealthVault.Transport.MessageFormatters.InfoFormatters;

namespace Microsoft.HealthVault.Transport.MessageFormatters.HeaderFormatters
{
    internal class AppIdFormatter : IAuthSessionOrAppId
    {
        private readonly HealthVaultMethods method;
        private readonly Guid? masterApplicationId;
        private readonly Guid? applicationId;

        public AppIdFormatter(HealthVaultMethods method, Guid? masterApplicationId, Guid? applicationId)
        {
            this.method = method;
            this.masterApplicationId = masterApplicationId;
            this.applicationId = applicationId;
        }

        public void Write(XmlWriter writer)
        {
            Guid appId;
            if (this.method == HealthVaultMethods.NewApplicationCreationInfo || this.method == HealthVaultMethods.GetServiceDefinition)
            {
                // These always use the Master app ID from configuration
                appId = this.masterApplicationId.GetValueOrDefault();
            }
            else
            {
                // Otherwise use app instance ID from connection.
                appId = this.applicationId.GetValueOrDefault();
            }

            writer.WriteElementString("app-id", appId.ToString());
        }

        public void WriteHash(XmlWriter writer, InfoFormatter info)
        {
            // No hash is provided when using AppId
        }
    }
}