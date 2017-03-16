// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Indicates the type of the audit action template being used for a
    /// <see cref="ThingDataGrid"/>.
    /// </summary>
    ///
    public enum TemplateType
    {
        /// <summary>
        /// The template is being used as a header.
        /// </summary>
        ///
        Header,

        /// <summary>
        /// The template is being used for an item.
        /// </summary>
        ///
        Item
    }
}
