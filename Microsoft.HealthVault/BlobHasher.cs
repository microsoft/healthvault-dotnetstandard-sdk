// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Used to calculate BLOB hashes.
    /// </summary>
    internal class BlobHasher
    {
        /// <summary>
        /// Constructs the BlobHasher for calculating BLOB hashes.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="blockSize"></param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="algorithm"/> is not a supported blob hash algorithm.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="blockSize"/> is not a positive value.
        /// </exception>
        ///
        /// <exception cref="CryptographicUnexpectedOperationException">
        /// If the hash algorithm's hash size has a partial byte length.
        /// </exception>
        ///
        internal BlobHasher(BlobHashAlgorithm algorithm, int blockSize)
        {
            switch (algorithm)
            {
                case BlobHashAlgorithm.SHA256Block:
                    _baseHashAlgorithm = CryptoConfiguration.CreateHashAlgorithm("SHA256");
                    break;
                default:
                    throw new ArgumentException(
                        ResourceRetriever.FormatResourceString(
                            "BlobHashAlgorithmUnsupported",
                             algorithm),
                        "algorithm");
            }

            Validator.ThrowArgumentOutOfRangeIf(blockSize < 1, "blockSize", "BlockSizeMustBePositive");

            if ((_baseHashAlgorithm.HashSize % 8) != 0)
            {
                throw new CryptographicUnexpectedOperationException(
                    ResourceRetriever.FormatResourceString(
                        "AlgorithmHashSizePartialByteLength",
                        _baseHashAlgorithm.HashSize));
            }

            _blockSize = blockSize;
            _blobHashAlgorithm = algorithm;
            _hashSizeBytes = _baseHashAlgorithm.HashSize / 8;
        }

        /// <summary>
        /// Calculates the series of block hashes from the data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>The block hashes.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="data"/> is null.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the
        /// <paramref name="data"/> length.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="offset"/> or <paramref name="count"/> is negative
        /// </exception>
        ///
        internal IList<byte[]> CalculateBlockHashes(byte[] data, int offset, int count)
        {
            Validator.ThrowIfArgumentNull(data, "data", "ArgumentNull");

            Validator.ThrowArgumentOutOfRangeIf(
                offset < 0,
                "offset",
                "CalculateBlockHashesOffsetNegative");

            Validator.ThrowArgumentOutOfRangeIf(
                count < 0,
                "count",
                "CalculateBlockHashesCountNegative");

            Validator.ThrowArgumentExceptionIf(
                data.Length - offset < count,
                "data",
                "CalculateBlockHashesDataLengthTooSmall");

            int numBlocks = (int)Math.Ceiling((double)count / _blockSize);
            List<byte[]> blockHashes = new List<byte[]>(numBlocks);

            int currentOffset = offset;
            while (currentOffset < offset + count)
            {
                int numBytesToHash = Math.Min(_blockSize, (offset + count) - currentOffset);

                byte[] blockHash =
                    _baseHashAlgorithm.ComputeHash(data, currentOffset, numBytesToHash);

                blockHashes.Add(blockHash);

                currentOffset = currentOffset + _blockSize;
            }

            return blockHashes;
        }

        /// <summary>
        /// Calculates the BLOB hash from a series of block hashes.
        /// </summary>
        /// <param name="blockHashes"></param>
        /// <returns>The BLOB hash.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="blockHashes"/> is null.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="blockHashes"/> is empty.
        /// </exception>
        internal byte[] CalculateBlobHash(IList<byte[]> blockHashes)
        {
            Validator.ThrowIfArgumentNull(blockHashes, "blockHashes", "ArgumentNull");

            Validator.ThrowArgumentOutOfRangeIf(
                blockHashes.Count < 1,
                "blockHashes",
                "CalculateBlobHashBlockHashCountMustBePositive");

            byte[] blockHashBuffer = new byte[blockHashes.Count * _hashSizeBytes];

            int writeOffset = 0;
            foreach (byte[] blockHash in blockHashes)
            {
                blockHash.CopyTo(blockHashBuffer, writeOffset);
                writeOffset += _hashSizeBytes;
            }

            return _baseHashAlgorithm.ComputeHash(blockHashBuffer);
        }

        /// <summary>
        /// Used for calculating the blob hash for inline blobs.
        /// </summary>
        internal byte[] CalculateBlobHash(byte[] data, int offset, int count)
        {
            return CalculateBlobHash(CalculateBlockHashes(data, offset, count));
        }

        internal static BlobHasher InlineBlobHasher
        {
            get { return _inlineBlobHasher; }
        }
        private static BlobHasher _inlineBlobHasher = new BlobHasher(
            BlobHashAlgorithm.SHA256Block,
            HealthApplicationConfiguration.Current.InlineBlobHashBlockSize);

        internal const int DefaultInlineBlobHashBlockSizeBytes = 1 << 21; // 2Mb.

        internal BlobHashAlgorithm BlobHashAlgorithm
        {
            get { return _blobHashAlgorithm; }
        }
        private BlobHashAlgorithm _blobHashAlgorithm;

        private HashAlgorithm _baseHashAlgorithm;

        internal int BlockSize
        {
            get { return _blockSize; }
        }
        private int _blockSize;

        internal int HashSizeBytes
        {
            get { return _hashSizeBytes; }
        }
        private int _hashSizeBytes;
    }
}