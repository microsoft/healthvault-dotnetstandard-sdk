// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Used to calculate BLOB hashes.
    /// </summary>
    internal class BlobHasher
    {
        private static IConfiguration configuration = Ioc.Get<IConfiguration>();

        /// <summary>
        /// Constructs the BlobHasher for calculating BLOB hashes.
        /// </summary>
        /// <param name="algorithm">The algorith to use to calculate the blob hasher</param>
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
                    this.baseHashAlgorithm = Ioc.Get<ICryptoService>().CreateHashAlgorithm("SHA256");
                    break;
                default:
                    throw new ArgumentException(
                        ResourceRetriever.FormatResourceString(
                            "BlobHashAlgorithmUnsupported",
                             algorithm),
                        nameof(algorithm));
            }

            Validator.ThrowArgumentOutOfRangeIf(blockSize < 1, "blockSize", "BlockSizeMustBePositive");

            if (this.baseHashAlgorithm.HashSize % 8 != 0)
            {
                throw new CryptographicUnexpectedOperationException(
                    ResourceRetriever.FormatResourceString(
                        "AlgorithmHashSizePartialByteLength",
                        this.baseHashAlgorithm.HashSize));
            }

            this.BlockSize = blockSize;
            this.BlobHashAlgorithm = algorithm;
            this.HashSizeBytes = this.baseHashAlgorithm.HashSize / 8;
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

            int numBlocks = (int)Math.Ceiling((double)count / this.BlockSize);
            List<byte[]> blockHashes = new List<byte[]>(numBlocks);

            int currentOffset = offset;
            while (currentOffset < offset + count)
            {
                int numBytesToHash = Math.Min(this.BlockSize, (offset + count) - currentOffset);

                byte[] blockHash =
                    this.baseHashAlgorithm.ComputeHash(data, currentOffset, numBytesToHash);

                blockHashes.Add(blockHash);

                currentOffset = currentOffset + this.BlockSize;
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
            Validator.ThrowIfArgumentNull(blockHashes, "blockHashes", "ArgumentNull");

            Validator.ThrowArgumentOutOfRangeIf(
                blockHashes.Count < 1,
                "blockHashes",
                "CalculateBlobHashBlockHashCountMustBePositive");

            byte[] blockHashBuffer = new byte[blockHashes.Count * this.HashSizeBytes];

            int writeOffset = 0;
            foreach (byte[] blockHash in blockHashes)
            {
                blockHash.CopyTo(blockHashBuffer, writeOffset);
                writeOffset += this.HashSizeBytes;
            }

            return this.baseHashAlgorithm.ComputeHash(blockHashBuffer);
        }

        /// <summary>
        /// Used for calculating the blob hash for inline blobs.
        /// </summary>
        internal byte[] CalculateBlobHash(byte[] data, int offset, int count)
        {
            return this.CalculateBlobHash(this.CalculateBlockHashes(data, offset, count));
        }

        internal static BlobHasher InlineBlobHasher { get; } = new BlobHasher(
            BlobHashAlgorithm.SHA256Block,
            configuration.InlineBlobHashBlockSize);

        internal const int DefaultInlineBlobHashBlockSizeBytes = 1 << 21; // 2Mb.

        internal BlobHashAlgorithm BlobHashAlgorithm { get; }

        private HashAlgorithm baseHashAlgorithm;

        internal int BlockSize { get; }

        internal int HashSizeBytes { get; }
    }
}