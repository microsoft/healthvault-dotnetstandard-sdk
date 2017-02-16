// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents the categories of information that can be specified when creating a
    /// <see cref="ServiceInfo"/> object via
    /// <see cref="PlatformPrimitives.HealthVaultPlatformInformation.GetServiceDefinition(HealthServiceConnection, ServiceInfoSections)"/> or
    /// <see cref="PlatformPrimitives.HealthVaultPlatformInformation.GetServiceDefinition(HealthServiceConnection, ServiceInfoSections, DateTime)"/>.
    /// If any categories are specified, only the information corresponding to those categories
    /// will be filled out in the <see cref="ServiceInfo"/> object; otherwise, all information
    /// will be filled out.
    /// </summary>
    [Flags]
    public enum ServiceInfoSections
    {
        /// <summary>
        /// Corresponds to <see cref="ServiceInfo.HealthServiceUrl"/>,
        /// <see cref="ServiceInfo.Version"/>, and
        /// <see cref="ServiceInfo.ConfigurationValues"/>.
        /// </summary>
        Platform = 0x1,

        /// <summary>
        /// Corresponds to <see cref="ServiceInfo.HealthServiceShellInfo"/>.
        /// </summary>
        Shell = 0x2,

        /// <summary>
        /// Corresponds to <see cref="ServiceInfo.ServiceInstances"/> and
        /// <see cref="ServiceInfo.CurrentInstance"/>.
        /// </summary>
        Topology = 0x4,

        /// <summary>
        /// Corresponds to <see cref="ServiceInfo.Methods"/> and
        /// <see cref="ServiceInfo.IncludedSchemaUrls"/>.
        /// </summary>
        XmlOverHttpMethods = 0x8,

        /// <summary>
        /// Corresponds to <see cref="ServiceInfo.MeaningfulUseInfo"/>.
        /// </summary>
        MeaningfulUse = 0x10,

        /// <summary>
        /// Retrieve all sections.
        /// </summary>
        All = Platform | Shell | Topology | XmlOverHttpMethods | MeaningfulUse
    }
}
