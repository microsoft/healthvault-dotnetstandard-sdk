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
        private readonly ILocalObjectStore localObjectStore;
        private readonly IShellAuthService shellAuthService;

        public ClientHealthVaultConnection(IServiceLocator serviceLocator, ILocalObjectStore localObjectStore, IShellAuthService shellAuthService)
            : base(serviceLocator)
        {
            this.localObjectStore = localObjectStore;
            this.shellAuthService = shellAuthService;
        }

        public override async Task AuthenticateAsync()
        {
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
        }
    }
}
