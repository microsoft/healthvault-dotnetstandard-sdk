// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Things
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
            this.Url = blobReferenceUrl;
            this.ChunkSize = chunkSize;
            this.PostEncryptionChunkSize = chunkSize;
            this.MaxBlobSize = maxBlobSize;
            this.BlobHashAlgorithm = blobHashAlgorithm;
            this.HashBlockSize = blockSize;
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
            this.Url = blobReferenceUrl;
            this.ChunkSize = chunkSize;
            this.PostEncryptionChunkSize = postEncryptionChunkSize;
            this.MaxBlobSize = maxBlobSize;
            this.BlobHashAlgorithm = blobHashAlgorithm;
            this.HashBlockSize = blockSize;
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
