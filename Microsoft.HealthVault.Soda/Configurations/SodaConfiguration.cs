using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Configurations
{
    public class SodaConfiguration : BaseConfiguration
    {
        private static readonly object instanceLock = new object();

        /// <summary>
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public static SodaConfiguration Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (_current == null)
                    {
                        var applicationConfiguration = new BaseConfiguration();

                        _current = new SodaConfiguration
                        {
                            ApplicationConfiguration = applicationConfiguration
                        };

                        BaseConfiguration.Current = applicationConfiguration;
                    }

                    return _current;
                }
            }

            set
            {
                lock (instanceLock)
                {
                    _current = value;
                }
            }
        }

        private static volatile SodaConfiguration _current;

        /// <summary>
        /// Gets the health application configuration.
        /// </summary>
        /// <value>
        /// The health application configuration.
        /// </value>
        public virtual IConfiguration ApplicationConfiguration { get; set; }


        public bool AllowInstanceBounce { get; set; } = true;
    }
}
