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
    internal class WebConfigurationFactory
    {
        public static WebConfiguration CreateConfiguration()
        {
            // TODO: This should instantiate the WebConfiguration instance and then set it's properties based on the WebConfig.
            NameValueCollection appSettings = WebConfigurationManager.AppSettings;
            WebConfiguration config = new WebConfiguration
            {
                ApplicationId = appSettings.GetGuid("ApplicationId"),
                DefaultHealthVaultShellUrl = appSettings.GetUrl("ShellUrl", true)
               
            };

            return config;
        }
    }
}
