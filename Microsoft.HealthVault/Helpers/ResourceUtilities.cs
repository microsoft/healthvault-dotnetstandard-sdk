using System;
using System.Resources;

namespace Microsoft.HealthVault.Helpers
{
    internal static class ResourceUtilities
    {
        public static ResourceManager ResourceManager { get; } = new ResourceManager(typeof(Resources));
    }
}
