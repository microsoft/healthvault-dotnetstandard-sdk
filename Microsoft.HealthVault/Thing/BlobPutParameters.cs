// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Blob parameters returned by BeginPutBlob
    /// </summary>
    ///
    internal class BlobPutParameters
    {
        /// <summary>
        /// Constructs a BlobPutParameters instance with the specified parameters.
        /// </summary>
        ///
        /// <param name="blobReferenceUrl">
        /// The BLOB reference URL.
        /// </param>
        ///
        /// <param name="chunkSize">
        /// The chunk size, in bytes, of the portion of a BLOB that should be sent to HealthVault
        /// in each request.
        /// </param>
        ///
        /// <param name="maxBlobSize">
        /// The maximum overall size of a BLOB in bytes.
        /// </param>
        ///
        /// <param name="blobHashAlgorithm">
        /// The hash algorithm to use for calculating the hash of this BLOB.
        /// </param>
        ///
        /// <param name="blockSize">
        /// The block size, in bytes, used by the BLOB hash algorithm.
        /// </param>
        internal BlobPutParameters(
            Uri blobReferenceUrl,
            int chunkSize,
            long maxBlobSize,
            BlobHashAlgorithm blobHashAlgorithm,
            int blockSize)
        {
            Url = blobReferenceUrl;
            ChunkSize = chunkSize;
            PostEncryptionChunkSize = chunkSize;
            MaxBlobSize = maxBlobSize;
            BlobHashAlgorithm = blobHashAlgorithm;
            HashBlockSize = blockSize;
        }

        /// <summary>
        /// Constructs a BlobPutParameters instance with the specified parameters.
        /// </summary>
        ///
        /// <param name="blobReferenceUrl">
        /// The BLOB reference URL.
        /// </param>
        ///
        /// <param name="chunkSize">
        /// The chunk size, in bytes, of the portion of a BLOB that should be sent to HealthVault
        /// in each request.
        /// </param>
        ///
        /// <param name="postEncryptionChunkSize">
        /// The chunk size, in bytes, of the portion of a BLOB that should be sent to HealthVault
        /// in each request, after the connect package encryption has been applied to the portion.
        /// </param>
        ///
        /// <param name="maxBlobSize">
        /// The maximum overall size of a BLOB in bytes.
        /// </param>
        ///
        /// <param name="blobHashAlgorithm">
        /// The hash algorithm to use for calculating the hash of this BLOB.
        /// </param>
        ///
        /// <param name="blockSize">
        /// The block size, in bytes, used by the BLOB hash algorithm.
        /// </param>
        internal BlobPutParameters(
            Uri blobReferenceUrl,
            int chunkSize,
            int postEncryptionChunkSize,
            long maxBlobSize,
            BlobHashAlgorithm blobHashAlgorithm,
            int blockSize)
        {
            Url = blobReferenceUrl;
            ChunkSize = chunkSize;
            PostEncryptionChunkSize = postEncryptionChunkSize;
            MaxBlobSize = maxBlobSize;
            BlobHashAlgorithm = blobHashAlgorithm;
            HashBlockSize = blockSize;
        }

        /// <summary>
        /// Gets the BLOB reference url.
        /// </summary>
        ///
        internal Uri Url { get; }

        /// <summary>
        /// Gets the put chunk size.
        /// </summary>
        ///
        internal int ChunkSize { get; }

        /// <summary>
        /// Gets the put chunk size for the bytes after the connect package encryption has been
        /// applied.
        /// </summary>
        ///
        internal int PostEncryptionChunkSize { get; }

        /// <summary>
        /// Gets the maximum size of the BLOB.
        /// </summary>
        ///
        internal long MaxBlobSize { get; }

        /// <summary>
        /// Gets the BLOB hash algorithm used to hash the BLOB.
        /// </summary>
        internal BlobHashAlgorithm BlobHashAlgorithm { get; }

        /// <summary>
        /// Gets the block size to use for the BLOB hash algorithm.
        /// </summary>
        internal int HashBlockSize { get; }
    }
}
