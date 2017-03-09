using Microsoft.HealthVault.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web
{
    public interface IWebConfiguration : IConfiguration
    {
        string CookieName { get; }

        bool UseAspSession { get; }

        string AllowedRedirectSites { get; }

    }
}
