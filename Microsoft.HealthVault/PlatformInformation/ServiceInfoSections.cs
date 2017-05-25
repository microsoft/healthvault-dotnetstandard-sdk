// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Represents the categories of information that can be specified when creating a
    /// <see cref="ServiceInfo"/> object via
    /// <see cref="HealthVaultPlatformInformation.GetServiceDefinitionAsync(IHealthVaultConnection, ServiceInfoSections)"/> or
    /// <see cref="HealthVaultPlatformInformation.GetServiceDefinitionAsync(IHealthVaultConnection, ServiceInfoSections, DateTime)"/>.
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
        /// Not currently used.
        /// </summary>
        MeaningfulUse = 0x10,

        /// <summary>
        /// Retrieve all sections.
        /// </summary>
        All = Platform | Shell | Topology | XmlOverHttpMethods | MeaningfulUse
    }
}
