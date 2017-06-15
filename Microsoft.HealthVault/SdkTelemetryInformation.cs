namespace Microsoft.HealthVault
{
    /// <summary>
    /// Telemetry information that will be logged from clients
    /// using the SDK as part of the method call to HealthVault Platform <see cref="HealthServiceMessage.BuildRequestXml()"/>
    /// </summary>
    /// <remarks>
    /// Request from Clients are of format - {Category} / {FileVersion} {OsInformation}
    ///
    /// Category values are specified in SdkTelemetryInformationCategories
    ///
    /// For example Xamarin based android telemetry information will be:
    ///
    /// HV-Xamarin-Android / 1.0.0.0 Android 7.1
    ///
    /// </remarks>
    internal class SdkTelemetryInformation
    {
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Assembly file version
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// Operating system information
        /// </summary>
        public string OsInformation { get; set; }
    }
}
