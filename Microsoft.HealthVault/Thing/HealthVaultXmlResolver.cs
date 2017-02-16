// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Describes the schema and structure of a health record item type.
    /// </summary>
    ///
    internal class HealthVaultXmlResolver : XmlUrlResolver
    {
        /// <summary>
        /// Constructs the HealthVaultXmlResolver instance with the specified base
        /// URI.
        /// </summary>
        ///
        /// <param name="baseUri">
        /// The base URL of type schemas on the platform.
        /// </param>
        ///
        internal HealthVaultXmlResolver(Uri baseUri)
        {
            BaseUri = baseUri;
        }

        /// <summary>
        /// Gets or sets the base URL of the type schemas on the platform.
        /// </summary>
        internal Uri BaseUri { get; set; }

        /// <summary>
        /// Resolves the relativeUri relative to the baseUri.
        /// </summary>
        ///
        /// <param name="baseUri">
        /// The base path used to resolve the relative path.
        /// </param>
        ///
        /// <param name="relativeUri">
        /// The relative path.
        /// </param>
        ///
        /// <returns>
        /// The resolved path.
        /// </returns>
        ///
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (!baseUri.IsAbsoluteUri)
            {
                return new Uri(BaseUri, relativeUri);
            }

            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
