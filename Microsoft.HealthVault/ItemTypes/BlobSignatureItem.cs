// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a BLOB that is part of a digital signature.
    /// </summary>
    /// <remarks>
    /// For more details please see <see cref="HealthRecordItem" />.
    /// </remarks>
    ///
    internal class BlobSignatureItem
    {
        /// <summary>
        /// Constructs a BlobSignatureItem object with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="contentType">The type of the item</param>
        /// <param name="hashInfo">The hash information for the BLOB</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/> are <b>null</b>.
        /// </exception>
        internal BlobSignatureItem(string name, string contentType, BlobHashInfo hashInfo)
        {
            Validator.ThrowIfArgumentNull(name, "name", "ArgumentNull");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "ArgumentNull");

            this.Name = name;
            this.ContentType = contentType;
            this.HashInfo = hashInfo;
        }

        /// <summary>
        /// Gets the name of the BLOB.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the content type of the BLOB.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Get the hash information for the BLOB.
        /// </summary>
        public BlobHashInfo HashInfo { get; }
    }
}