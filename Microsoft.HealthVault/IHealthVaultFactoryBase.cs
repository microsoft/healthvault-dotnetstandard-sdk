using Microsoft.HealthVault.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault
{
    public interface IHealthVaultFactoryBase
    {
        void RegisterClientType<TClient>(Func<TClient, TClient> func)
            where TClient : IClient;
    }
}
