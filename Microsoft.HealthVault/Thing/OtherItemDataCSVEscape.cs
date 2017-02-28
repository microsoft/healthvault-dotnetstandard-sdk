// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// Information about a single escape entity from an OtherDataHelper object.
    /// </summary>
    /// <remarks>
    /// Escapes are stored using a "name=value" format in the other data text string.
    /// </remarks>
    internal class OtherItemDataCsvEscape : OtherItemDataCsvItem
    {
        /// <summary>
        /// Create an OtherDataEscape instance.
        /// </summary>
        /// <param name="name">The name of the escape value.</param>
        /// <param name="value">The value.</param>
        internal OtherItemDataCsvEscape(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        private string name;

        /// <summary>
        /// Gets or sets the name of the escape.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If the name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the name contains the "=" character.
        /// </exception>
        internal string Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "CSVNameNull");

                Validator.ThrowArgumentExceptionIf(
                    value.Contains("="),
                    "Name",
                    "CSVNameInvalid");

                this.name = value;
            }
        }

        private string value;

        /// <summary>
        /// Gets or sets the value of the escape.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If the value is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the value contains the "=" character.
        /// </exception>
        internal string Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "CSVValueNull");

                Validator.ThrowArgumentExceptionIf(
                    value.Contains("="),
                    "Name",
                    "CSVValueInvalid");

                this.value = value;
            }
        }

        /// <summary>
        /// Gets the escape in "Name=value" format.
        /// </summary>
        internal string NameEqualsValue => this.name + "=" + this.value;
    }
}
