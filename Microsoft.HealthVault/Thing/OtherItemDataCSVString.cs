// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents a string entry in the CSV list
    /// </summary>
    internal class OtherItemDataCsvString : OtherItemDataCsvItem
    {
        /// <summary>
        /// Create an instance of the <see cref="OtherItemDataCsvString"/> instance with a string value.
        /// </summary>
        /// <param name="value">The string value of this entry.</param>
        internal OtherItemDataCsvString(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value of the string entry.
        /// </summary>
        internal string Value { get; set; }
    }
}