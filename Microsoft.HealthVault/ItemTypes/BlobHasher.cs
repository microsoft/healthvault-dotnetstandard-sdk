// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Used to calculate BLOB hashes.
    /// </summary>
    internal class BlobHasher
    {
        private static HealthVaultConfiguration configuration = Ioc.Get<HealthVaultConfiguration>();

        /// <summary>
        /// Constructs the BlobHasher for calculating BLOB hashes.
        /// </summary>
        /// <param name="algorithm">The algorithm to use to calculate the blob hasher</param>
        /// <param name="blockSize">The block size to use in bytes</param>
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
                    _baseHashAlgorithm = SHA256.Create();
                    break;
                default:
                    throw new ArgumentException(
                        Resources.BlobHashAlgorithmUnsupported.FormatResource(algorithm),
                        nameof(algorithm));
            }

            if (blockSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockSize), Resources.BlockSizeMustBePositive);
            }

            if (_baseHashAlgorithm.HashSize % 8 != 0)
            {
                throw new CryptographicUnexpectedOperationException(Resources.AlgorithmHashSizePartialByteLength.FormatResource(_baseHashAlgorithm.HashSize));
            }

            BlockSize = blockSize;
            BlobHashAlgorithm = algorithm;
            HashSizeBytes = _baseHashAlgorithm.HashSize / 8;
        }

        /// <summary>
        /// Calculates the series of block hashes from the data.
        /// </summary>
        /// <param name="data">The data to calculate</param>
        /// <param name="offset">the offset that the hash starts at</param>
        /// <param name="count">the number of blocks to calculate</param>
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
            Validator.ThrowIfArgumentNull(data, nameof(data), Resources.ArgumentNull);

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), Resources.CalculateBlockHashesOffsetNegative);
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Resources.CalculateBlockHashesCountNegative);
            }

            if (data.Length - offset < count)
            {
                throw new ArgumentException(Resources.CalculateBlockHashesDataLengthTooSmall, nameof(data));
            }

            int numBlocks = (int)Math.Ceiling((double)count / BlockSize);
            List<byte[]> blockHashes = new List<byte[]>(numBlocks);

            int currentOffset = offset;
            while (currentOffset < offset + count)
            {
                int numBytesToHash = Math.Min(BlockSize, (offset + count) - currentOffset);

                byte[] blockHash =
                    _baseHashAlgorithm.ComputeHash(data, currentOffset, numBytesToHash);

                blockHashes.Add(blockHash);

                currentOffset = currentOffset + BlockSize;
            }

            return blockHashes;
        }

        /// <summary>
        /// Calculates the BLOB hash from a series of block hashes.
        /// </summary>
        /// <param name="blockHashes">The hashes to use in the calculations</param>
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
            Validator.ThrowIfArgumentNull(blockHashes, nameof(blockHashes), Resources.ArgumentNull);

            if (blockHashes.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHashes), Resources.CalculateBlobHashBlockHashCountMustBePositive);
            }

            byte[] blockHashBuffer = new byte[blockHashes.Count * HashSizeBytes];

            int writeOffset = 0;
            foreach (byte[] blockHash in blockHashes)
            {
                blockHash.CopyTo(blockHashBuffer, writeOffset);
                writeOffset += HashSizeBytes;
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

        internal static BlobHasher InlineBlobHasher { get; } = new BlobHasher(
            BlobHashAlgorithm.SHA256Block,
            configuration.InlineBlobHashBlockSize);

        internal const int DefaultInlineBlobHashBlockSizeBytes = 1 << 21; // 2Mb.

        internal BlobHashAlgorithm BlobHashAlgorithm { get; }

        private HashAlgorithm _baseHashAlgorithm;

        internal int BlockSize { get; }

        internal int HashSizeBytes { get; }
    }
}