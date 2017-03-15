using Microsoft.HealthVault.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// A factory for the WebConfiguration
    /// </summary>
    internal class WebConfigurationFactory
    {
        public static WebConfiguration CreateConfiguration()
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings;
            WebConfiguration config = new WebConfiguration
            {
                MasterApplicationId = appSettings.GetGuid("ApplicationId"),
                DefaultHealthVaultShellUrl = appSettings.GetUrl("ShellUrl", true)
               
                // TODO: finish populating the config!
            };

            return config;
        }
    }
}
