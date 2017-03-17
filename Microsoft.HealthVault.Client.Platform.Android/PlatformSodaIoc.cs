using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Android.OS;
using Grace.DependencyInjection;

namespace Microsoft.HealthVault.Client.Platform
{
    internal static class PlatformSodaIoc
    {
        public static void RegisterTypes(DependencyInjectionContainer container)
        {
            RegisterTelemetryInformation(container);
        }

        /// <summary>
        /// Register Telemetry information for SDK
        /// </summary>
        /// <param name="container">IOC container</param>
        private static void RegisterTelemetryInformation(DependencyInjectionContainer container)
        {
            string androidVersion = Regex.Replace(Build.VERSION.Release, "[^0-9.]", string.Empty);
            if (string.IsNullOrWhiteSpace(androidVersion))
            {
                // Preview SDKs do not have integer Release versions. Identify them uniquely using SDK version and preview SDK version.
                androidVersion = $"{(int) Build.VERSION.SdkInt}.{Build.VERSION.PreviewSdkInt}";
            }

            var sdkTelemetryInformation = new SdkTelemetryInformation
            {
                Category = HealthVaultConstants.SdkTelemetryInformationCategories.AndroidClient,
                FileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                OsInformation = $"Android {androidVersion}"
            };

            container.Configure(c => c.ExportInstance(sdkTelemetryInformation).As<SdkTelemetryInformation>());
        }
    }
}
