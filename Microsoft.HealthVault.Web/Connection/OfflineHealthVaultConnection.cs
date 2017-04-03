// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;

namespace Microsoft.HealthVault.Web.Connection
{
    /// <summary>
    /// <see cref="IOfflineHealthVaultConnection"/>
    /// </summary>
    internal class OfflineHealthVaultConnection : WebHealthVaultConnectionBase, IOfflineHealthVaultConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineHealthVaultConnection"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="sessionCredential">The session credential.</param>
        /// <param name="offlinePersionId">The offline persion identifier.</param>
        public OfflineHealthVaultConnection(IServiceLocator serviceLocator,
            HealthServiceInstance serviceInstance = null,
            SessionCredential sessionCredential = null,
            string offlinePersionId = null) 
            : base(serviceLocator, serviceInstance, sessionCredential)
        {
            Guid offlinePersonId;

            if (Guid.TryParse(offlinePersionId, out offlinePersonId))
            {
                this.OfflinePersonId = offlinePersonId;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the offline person who granted
        /// permissions to the calling application to perform certain
        /// operations.
        /// </summary>
        public Guid OfflinePersonId { get; internal set; }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            writer.WriteStartElement("auth-session");
            writer.WriteElementString("auth-token", this.SessionCredential.Token);

            if (this.OfflinePersonId != Guid.Empty)
            {
                // person-specific request, but person is offline as far as
                // HealthVault is concerned, so pass in offline info...
                // <offline-person-info>
                writer.WriteStartElement("offline-person-info");

                // <offline-person-id>
                writer.WriteElementString(
                    "offline-person-id",
                    this.OfflinePersonId.ToString());

                // </offline-person-info>
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override Task<PersonInfo> GetPersonInfoAsync()
        {
            throw new NotSupportedException("OfflineHealthVaultConnection is a specialized anonymous connection with no PersonInfo");
        }

        public override string GetRestAuthSessionHeader(Guid? recordId)
        {
            throw new NotImplementedException();
        }
    }
}
