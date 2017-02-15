// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
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
        public BlobHashInfo(BlobHashAlgorithm blobHashAlgorithm, Int32 blobBlockSizeBytes, byte[] hash)
        {
            _blobHashAlgorithm = blobHashAlgorithm;
            _blobHashAlgorithmString = blobHashAlgorithm.ToString();
            _blockSizeBytes = blobBlockSizeBytes;
            _hash = hash;
        }

        internal void Parse(XPathNavigator blobHashNav)
        {
            String blobHashAlgString = blobHashNav.SelectSingleNode("algorithm").Value;
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
            _blobHashAlgorithmString = blobHashAlgString;

            Int32 blockSize = blobHashNav.SelectSingleNode("params/block-size").ValueAsInt;

            XPathNavigator hashNav = blobHashNav.SelectSingleNode("hash");

            Byte[] blobHash = null;
            if (hashNav != null)
            {
                String blobHashStr = blobHashNav.SelectSingleNode("hash").Value;
                blobHash = Convert.FromBase64String(blobHashStr);
            }

            _blobHashAlgorithm = blobHashAlg;
            _blockSizeBytes = blockSize;
            _hash = blobHash;
        }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("hash-info");

            writer.WriteElementString("algorithm", _blobHashAlgorithmString);
            writer.WriteStartElement("params");

            writer.WriteElementString(
                "block-size",
                _blockSizeBytes.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();

            if (_hash != null)
            {
                writer.WriteElementString("hash", Convert.ToBase64String(_hash));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// The algorithm used to calculate the BLOB hash.
        /// </summary>
        public BlobHashAlgorithm BlobHashAlgorithm
        {
            get { return _blobHashAlgorithm; }
        }
        private BlobHashAlgorithm _blobHashAlgorithm;

        private string _blobHashAlgorithmString;

        /// <summary>
        /// The block size in bytes used by the <see cref="BlobHashAlgorithm" /> to
        /// calculate the BLOB hash.
        /// </summary>
        public int BlockSizeBytes
        {
            get { return _blockSizeBytes; }
        }
        private int _blockSizeBytes;

        /// <summary>
        /// Represents the BLOB hash as calculated by the <see cref="BlobHashAlgorithm" />
        /// and <see cref="BlockSizeBytes" />
        /// </summary>
        public byte[] Hash
        {
            get { return _hash; }
        }
        private byte[] _hash;
    }
}