// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about the HealthVault assemblies.
    /// </summary>
    ///
    public class HealthServiceAssemblyInfo
    {
        internal static HealthServiceAssemblyInfo CreateAssemblyInfo(
            XPathNavigator nav)
        {
            Uri url = new Uri(nav.SelectSingleNode("url").Value);
            string version = nav.SelectSingleNode("version").Value;
            Uri docUrl = new Uri(nav.SelectSingleNode("doc-url").Value);

            HealthServiceAssemblyInfo assemblyInfo =
                new HealthServiceAssemblyInfo(url, version, docUrl);

            return assemblyInfo;
        }

        private HealthServiceAssemblyInfo(
            Uri url,
            string version,
            Uri documentationUrl)
        {
            this.Url = url;
            this.Version = version;
            this.DocumentationUrl = documentationUrl;
        }

        /// <summary>
        /// Gets the HealthVault URL.
        /// </summary>
        ///
        /// <value>
        /// An instance of Uri representing the HealthVault URL.
        /// </value>
        ///
        /// <remarks>
        /// This is the URL to the HealthVault.ashx, which is used to call the
        /// HealthVault XML methods.
        /// </remarks>
        ///
        public Uri Url { get; }

        /// <summary>
        /// Gets the version of the HealthVault assembly.
        /// </summary>
        ///
        /// <value>
        /// A string representing the version.
        /// </value>
        ///
        public string Version { get; }

        /// <summary>
        /// Gets the URL of the documentation for the assembly.
        /// </summary>
        /// <value>
        /// An instance of Uri representing the documentation URL.
        /// </value>
        ///
        public Uri DocumentationUrl { get; }
    }
}
