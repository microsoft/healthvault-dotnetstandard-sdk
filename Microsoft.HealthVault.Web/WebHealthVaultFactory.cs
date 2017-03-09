using Microsoft.HealthVault.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web
{
    public class WebHealthVaultFactory : HealthVaultFactoryBase
    {
        private static readonly object InstanceLock = new object();
    
        private static WebHealthVaultFactory current;

        /// <summary>
        /// Gets the current WebHealthVaultFactory instance.
        /// </summary>
        public static WebHealthVaultFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    return current ?? (current = new WebHealthVaultFactory());
                }
            }
        }

        private WebHealthVaultFactory()
        {
        }

        public IHealthVaultConnection GetWebApplicationConnection()
        {
            WebIoc.EnsureTypesRegistered();
            IHealthVaultConnection connection = Ioc.Get<IConnectionInternal>();
            connection.ApplicationConfiguration = Ioc.Get<WebConfiguration>();
            return connection;
        }
    }
}
