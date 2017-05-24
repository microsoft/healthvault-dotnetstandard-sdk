// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            BlobHashAlgorithm = blobHashAlgorithm;
            _blobHashAlgorithmString = blobHashAlgorithm.ToString();
            BlockSizeBytes = blobBlockSizeBytes;
            Hash = hash;
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

            _blobHashAlgorithmString = blobHashAlgString;

            int blockSize = blobHashNav.SelectSingleNode("params/block-size").ValueAsInt;

            XPathNavigator hashNav = blobHashNav.SelectSingleNode("hash");

            byte[] blobHash = null;
            if (hashNav != null)
            {
                string blobHashStr = blobHashNav.SelectSingleNode("hash").Value;
                blobHash = Convert.FromBase64String(blobHashStr);
            }

            BlobHashAlgorithm = blobHashAlg;
            BlockSizeBytes = blockSize;
            Hash = blobHash;
        }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("hash-info");

            writer.WriteElementString("algorithm", _blobHashAlgorithmString);
            writer.WriteStartElement("params");

            writer.WriteElementString(
                "block-size",
                BlockSizeBytes.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();

            if (Hash != null)
            {
                writer.WriteElementString("hash", Convert.ToBase64String(Hash));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// The algorithm used to calculate the BLOB hash.
        /// </summary>
        public BlobHashAlgorithm BlobHashAlgorithm { get; private set; }

        private string _blobHashAlgorithmString;

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