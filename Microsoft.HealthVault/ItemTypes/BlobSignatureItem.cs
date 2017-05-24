// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a BLOB that is part of a digital signature.
    /// </summary>
    /// <remarks>
    /// For more details please see <see cref="ThingBase" />.
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
            Validator.ThrowIfArgumentNull(name, nameof(name), Resources.ArgumentNull);
            Validator.ThrowIfArgumentNull(contentType, nameof(contentType), Resources.ArgumentNull);

            Name = name;
            ContentType = contentType;
            HashInfo = hashInfo;
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