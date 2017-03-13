using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Utilities
{
    internal static class UrlUtilities
    {
        public static Uri GetFullPlatformUrl(Uri partialPlatformUrl)
        {
            string newUri = partialPlatformUrl.AbsoluteUri;
            if (!newUri.EndsWith("/", StringComparison.Ordinal))
            {
                newUri = newUri + "/wildcat.ashx";
            }
            else
            {
                newUri = newUri + "wildcat.ashx";
            }

            return new Uri(newUri);
        }
    }
}
