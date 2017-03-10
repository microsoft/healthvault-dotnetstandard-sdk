using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Client
{
    internal class ClientHealthVaultConnection : ConnectionInternalBase
    {
        public ClientHealthVaultConnection(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override async Task AuthenticateAsync()
        {
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
        }
    }
}
