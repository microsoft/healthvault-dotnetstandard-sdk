// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Exposes functionality to retrieve service information from
    /// the HealthVault web-service.
    /// </summary>
    public interface IServiceInfoProvider
    {
        /// <summary>
        /// Returns the service information retrieved from the HealthVault web-service.
        /// </summary>
        /// 
        /// <returns>
        /// Service information retrieved from the HealthVault web-service.
        /// </returns>
        ServiceInfo GetServiceInfo();
    }
}