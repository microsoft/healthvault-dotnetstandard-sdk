// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents information about a BLOB that is part of a digital signature.
    /// </summary>
    /// <remarks>
    /// For more details please see <see cref="HealthRecordItemSignature" />.
    /// </remarks>
    ///
    public class BlobSignatureItem
    {
        /// <summary>
        /// Constructs a BlobSignatureItem object with the specified parameters.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contentType"></param>
        /// <param name="hashInfo"></param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/> are <b>null</b>.
        /// </exception>
        internal BlobSignatureItem(string name, string contentType, BlobHashInfo hashInfo)
        {
            Validator.ThrowIfArgumentNull(name, "name", "ArgumentNull");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "ArgumentNull");

            _name = name;
            _contentType = contentType;
            _hashInfo = hashInfo;
        }

        /// <summary>
        /// Gets the name of the BLOB.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        private string _name;

        /// <summary>
        /// Gets the content type of the BLOB.
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
        }
        private string _contentType;

        /// <summary>
        /// Get the hash information for the BLOB.
        /// </summary>
        public BlobHashInfo HashInfo
        {
            get { return _hashInfo; }
        }
        private BlobHashInfo _hashInfo;
    }
}