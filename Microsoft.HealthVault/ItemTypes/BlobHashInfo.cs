// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents hash information about a <see cref="Blob" />.
    /// </summary>
    public class BlobHashInfo
    {
        /// <summary>
        /// Creates an instance of the <see cref="BlobHashInfo"/> class
        /// with default values.
        /// </summary>
        public BlobHashInfo()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="BlobHashInfo"/> class
        /// for a given hashing algorithm, hashblock size and hash.
        /// </summary>
        ///
        /// <param name="blobHashAlgorithm">
        /// The hashing algorithm used.
        /// </param>
        ///
        /// <param name="blobBlockSizeBytes">
        /// The size of the BLOB block size used in calculating hashes
        /// </param>
        ///
        /// <param name="hash">
        /// Represents the BLOB hash value
        /// </param>
        public BlobHashInfo(BlobHashAlgorithm blobHashAlgorithm, int blobBlockSizeBytes, byte[] hash)
        {
            this.BlobHashAlgorithm = blobHashAlgorithm;
            this.blobHashAlgorithmString = blobHashAlgorithm.ToString();
            this.BlockSizeBytes = blobBlockSizeBytes;
            this.Hash = hash;
        }

        internal void Parse(XPathNavigator blobHashNav)
        {
            string blobHashAlgString = blobHashNav.SelectSingleNode("algorithm").Value;
            BlobHashAlgorithm blobHashAlg;
            try
            {
                blobHashAlg =
                    (BlobHashAlgorithm)Enum.Parse(
                        typeof(BlobHashAlgorithm), blobHashAlgString);
            }
            catch (ArgumentException)
            {
                blobHashAlg = BlobHashAlgorithm.Unknown;
            }

            this.blobHashAlgorithmString = blobHashAlgString;

            int blockSize = blobHashNav.SelectSingleNode("params/block-size").ValueAsInt;

            XPathNavigator hashNav = blobHashNav.SelectSingleNode("hash");

            byte[] blobHash = null;
            if (hashNav != null)
            {
                string blobHashStr = blobHashNav.SelectSingleNode("hash").Value;
                blobHash = Convert.FromBase64String(blobHashStr);
            }

            this.BlobHashAlgorithm = blobHashAlg;
            this.BlockSizeBytes = blockSize;
            this.Hash = blobHash;
        }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("hash-info");

            writer.WriteElementString("algorithm", this.blobHashAlgorithmString);
            writer.WriteStartElement("params");

            writer.WriteElementString(
                "block-size",
                this.BlockSizeBytes.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();

            if (this.Hash != null)
            {
                writer.WriteElementString("hash", Convert.ToBase64String(this.Hash));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// The algorithm used to calculate the BLOB hash.
        /// </summary>
        public BlobHashAlgorithm BlobHashAlgorithm { get; private set; }

        private string blobHashAlgorithmString;

        /// <summary>
        /// The block size in bytes used by the <see cref="BlobHashAlgorithm" /> to
        /// calculate the BLOB hash.
        /// </summary>
        public int BlockSizeBytes { get; private set; }

        /// <summary>
        /// Represents the BLOB hash as calculated by the <see cref="BlobHashAlgorithm" />
        /// and <see cref="BlockSizeBytes" />
        /// </summary>
        public byte[] Hash { get; private set; }
    }
}