// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
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
            _url = blobReferenceUrl;
            _chunkSize = chunkSize;
            _postEncryptionChunkSize = chunkSize;
            _maxBlobSize = maxBlobSize;
            _blobHashAlgorithm = blobHashAlgorithm;
            _hashBlockSize = blockSize;
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
            _url = blobReferenceUrl;
            _chunkSize = chunkSize;
            _postEncryptionChunkSize = postEncryptionChunkSize;
            _maxBlobSize = maxBlobSize;
            _blobHashAlgorithm = blobHashAlgorithm;
            _hashBlockSize = blockSize;
        }

        /// <summary>
        /// Gets the BLOB reference url.
        /// </summary>
        /// 
        internal Uri Url
        {
            get { return _url; }
        }
        private Uri _url;

        /// <summary>
        /// Gets the put chunk size.
        /// </summary>
        /// 
        internal int ChunkSize
        {
            get { return _chunkSize; }
        }
        private int _chunkSize;

        /// <summary>
        /// Gets the put chunk size for the bytes after the connect package encryption has been 
        /// applied.
        /// </summary>
        /// 
        internal int PostEncryptionChunkSize
        {
            get { return _postEncryptionChunkSize; }
        }
        private int _postEncryptionChunkSize;

        /// <summary>
        /// Gets the maximum size of the BLOB.
        /// </summary>
        /// 
        internal long MaxBlobSize
        {
            get { return _maxBlobSize; }
        }
        private long _maxBlobSize;

        /// <summary>
        /// Gets the BLOB hash algorithm used to hash the BLOB.
        /// </summary>
        internal BlobHashAlgorithm BlobHashAlgorithm
        {
            get { return _blobHashAlgorithm; }
        }
        private BlobHashAlgorithm _blobHashAlgorithm;

        /// <summary>
        /// Gets the block size to use for the BLOB hash algorithm.
        /// </summary>
        internal int HashBlockSize
        {
            get { return _hashBlockSize; }
        }
        private int _hashBlockSize;
    }
}
