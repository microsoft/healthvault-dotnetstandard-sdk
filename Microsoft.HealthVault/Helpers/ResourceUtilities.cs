using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Helpers
{
    internal static class ResourceUtilities
    {
        public static ResourceManager ResourceManager { get; } = new ResourceManager(typeof(Resources));
    }
}
