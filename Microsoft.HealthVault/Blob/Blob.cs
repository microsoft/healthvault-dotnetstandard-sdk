// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents binary data that can be associated with a person's health record.
    /// </summary>
    ///
    public class Blob
    {
        /// <summary>
        /// Constructs an instance of the Blob class with the specified values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB. It can be <see cref="String.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="currentContentEncoding">
        /// The current content encoding of the BLOB or <b>null</b> if the BLOB is not encoded.
        /// </param>
        ///
        /// <param name="legacyContentEncoding">
        /// The previous content encoding of the BLOB (if any).
        /// </param>
        ///
        /// <param name="record">
        /// The health record to write the BLOB data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        internal Blob(
            string name,
            string contentType,
            string currentContentEncoding,
            string legacyContentEncoding,
            HealthRecordAccessor record)
            : this(name, contentType, currentContentEncoding, legacyContentEncoding, null, record)
        {
        }

        /// <summary>
        /// Constructs an instance of the Blob class with the specified values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB. It can be <see cref="String.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="currentContentEncoding">
        /// The current content encoding of the BLOB or <b>null</b> if the BLOB is not encoded.
        /// </param>
        ///
        /// <param name="legacyContentEncoding">
        /// The previous content encoding of the BLOB (if any).
        /// </param>
        ///
        /// <param name="hashInfo">
        /// The hash information for the BLOB.
        /// </param>
        ///
        /// <param name="record">
        /// The health record to write the BLOB data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        internal Blob(
            string name,
            string contentType,
            string currentContentEncoding,
            string legacyContentEncoding,
            BlobHashInfo hashInfo,
            HealthRecordAccessor record)
        {
            Validator.ThrowIfArgumentNull(name, "name", "StringNull");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "StringNull");

            _name = name;
            _contentType = contentType;
            _contentEncoding = currentContentEncoding;
            _legacyContentEncoding = legacyContentEncoding;
            _blobHashInfo = hashInfo;
            _record = record;
        }

        /// <summary>
        /// Constructs an instance of the Blob class with the specified values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB. It can be <see cref="String.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="currentContentEncoding">
        /// The current content encoding of the BLOB or <b>null</b> if the BLOB is not encoded.
        /// </param>
        ///
        /// <param name="legacyContentEncoding">
        /// The previous content encoding of the BLOB (if any).
        /// </param>
        ///
        /// <param name="hashInfo">
        /// The hash information for the BLOB.
        /// </param>
        ///
        /// <param name="connectPackageParameters">
        /// The creation parameters for the <see cref="ConnectPackage"/> that will host the
        /// <see cref="HealthRecordItem"/> that contains this blob.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/>
        /// or <paramref name="connectPackageParameters"/> is <b>null</b>.
        /// </exception>
        ///
        internal Blob(
            string name,
            string contentType,
            string currentContentEncoding,
            string legacyContentEncoding,
            BlobHashInfo hashInfo)
        {
            Validator.ThrowIfArgumentNull(name, "name", "StringNull");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "StringNull");

            _name = name;
            _contentType = contentType;
            _contentEncoding = currentContentEncoding;
            _legacyContentEncoding = legacyContentEncoding;
            _blobHashInfo = hashInfo;
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
        /// Gets the hash information for the BLOB.
        /// </summary>
        public BlobHashInfo HashInfo
        {
            get { return _blobHashInfo; }
            internal set { _blobHashInfo = value; }
        }
        private BlobHashInfo _blobHashInfo;

        private HealthRecordAccessor _record;

        /// <summary>
        /// Gets the content encoding of the BLOB.
        /// </summary>
        ///
        /// <remarks>
        /// This is only available for BLOBs that were added to HealthVault prior to streaming
        /// support being added.
        /// </remarks>
        ///
        public string ContentEncoding
        {
            get { return _contentEncoding; }
            internal set { _contentEncoding = value; }
        }
        private string _contentEncoding;

        internal string LegacyContentEncoding
        {
            get { return _legacyContentEncoding; }
        }
        private string _legacyContentEncoding;

        /// <summary>
        /// Gets the length of the content of the BLOB.
        /// </summary>
        ///
        /// <remarks>
        /// In some cases the content length can't be determined by
        /// HealthVault until the data is retrieved. In this case, the property
        /// will be null. For instance, when the Blob represents data that was
        /// streamed to HealthVault for a <see cref="Package.ConnectPackage"/>
        /// the data is encrypted so the true size of the Blob is not known.
        /// </remarks>
        ///
        public long? ContentLength
        {
            get { return _length; }
            internal set { _length = value; }
        }
        private long? _length;

        #region Write

        /// <summary>
        /// Gets a stream for writing data to a HealthVault BLOB.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="BlobStream"/> instance to write data to.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob already has data that was retrieved from HealthVault.
        /// </exception>
        ///
        public BlobStream GetWriterStream()
        {
            if (_inlineData != null || _url != null)
            {
                throw new NotSupportedException();
            }

            IsDirty = true;

            return _record != null ?
                    new BlobStream(_record, this) : null;
        }

        /// <summary>
        /// Writes the specified string to the blob with UTF-8 encoding.
        /// </summary>
        ///
        /// <param name="data">
        /// The data to write to the blob.
        /// </param>
        ///
        /// <remarks>
        /// The BLOB data is written into the XML request during the
        /// <see cref="HealthRecordAccessor.NewItem"/> or <see cref="HealthRecordAccessor.UpdateItem"/>
        /// rather than being streamed to HealthVault. This is limited to about 3.5MB of data.
        /// Use <see cref="GetWriterStream"/> to write more data.
        /// </remarks>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="data"/> is <b>null</b>.
        /// </exception>
        ///
        public void WriteInline(string data)
        {
            WriteInline(data, Encoding.UTF8);
        }

        /// <summary>
        /// Writes the specified string to the blob with the specified encoding.
        /// </summary>
        ///
        /// <param name="data">
        /// The data to write to the blob.
        /// </param>
        ///
        /// <param name="encoding">
        /// The encoding to use when writing the data.
        /// </param>
        ///
        /// <remarks>
        /// The BLOB data is written into the XML request during the
        /// <see cref="HealthRecordAccessor.NewItem"/> or <see cref="HealthRecordAccessor.UpdateItem"/>
        /// rather than being streamed to HealthVault. This is limited to about 3.5MB of data.
        /// Use <see cref="GetWriterStream"/> to write more data.
        /// </remarks>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="data"/> is <b>null</b>
        /// <br/>-or-<br/>
        /// <paramref name="encoding"/> is <b>null</b>.
        /// </exception>
        ///
        public void WriteInline(string data, Encoding encoding)
        {
            Validator.ThrowIfArgumentNull(data, "data", "StringNull");
            Validator.ThrowIfArgumentNull(encoding, "encoding", "ArgumentNull");

            WriteNewInlineData(encoding.GetBytes(data));
        }

        /// <summary>
        /// Writes the specified bytes to the blob inline with the data XML of the type.
        /// </summary>
        ///
        /// <param name="bytes">
        /// The bytes to write to the blob.
        /// </param>
        ///
        /// <remarks>
        /// The BLOB data is written into the XML request during the
        /// <see cref="HealthRecordAccessor.NewItem"/> or <see cref="HealthRecordAccessor.UpdateItem"/>
        /// rather than being streamed to HealthVault. This is limited to about 3.5MB of data.
        /// Use <see cref="GetWriterStream"/> to write more data.
        /// </remarks>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="bytes"/> is <b>null</b>.
        /// </exception>
        ///
        public void WriteInline(byte[] bytes)
        {
            Validator.ThrowIfArgumentNull(bytes, "bytes", "ArgumentNull");

            WriteNewInlineData(bytes);
        }

        private void WriteNewInlineData(byte[] bytes)
        {
            BlobHasher inlineHasher = BlobHasher.InlineBlobHasher;
            byte[] blobHash = inlineHasher.CalculateBlobHash(bytes, 0, bytes.Length);

            _url = null;
            _inlineData = bytes;
            _length = bytes.Length;

            _blobHashInfo = new BlobHashInfo(
                inlineHasher.BlobHashAlgorithm,
                inlineHasher.BlockSize,
                blobHash);

            IsDirty = true;
        }

        /// <summary>
        /// Writes the bytes from the specified stream.
        /// </summary>
        ///
        /// <param name="stream">
        /// The stream to get the bytes from to write to the blob.
        /// </param>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="WebException">
        /// If there is an error writing the data to HealthVault.
        /// </exception>
        ///
        public void Write(Stream stream)
        {
            Validator.ThrowIfArgumentNull(stream, "stream", "ArgumentNull");

            int bufferSize = _defaultStreamBufferSize;
            if (stream.CanSeek)
            {
                bufferSize = (int)Math.Min(stream.Length, (long)_defaultStreamBufferSize);
            }

            int bytesRead = 0;
            byte[] bytes = new byte[bufferSize];

            using (BlobStream blobStream = GetWriterStream())
            {
                while ((bytesRead = stream.Read(bytes, 0, bufferSize)) > 0)
                {
                    blobStream.Write(bytes, 0, bytesRead);
                }
            }
            IsDirty = true;
        }

        #endregion Write

        #region Read

        /// <summary>
        /// Gets a stream for reading data from a HealthVault BLOB.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="BlobStream"/> instance to read data from.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If there is no data to read from the BLOB.
        /// </exception>
        ///
        public BlobStream GetReaderStream()
        {
            BlobStream stream = null;
            if (_url != null)
            {
                stream = new BlobStream(this, _url, _length);
            }
            else if (_inlineData != null)
            {
                stream = new BlobStream(this, _inlineData, _length);
            }
            else
            {
                throw new NotSupportedException();
            }
            return stream;
        }

        /// <summary>
        /// Saves the BLOB data to the specified file.
        /// </summary>
        ///
        /// <param name="fileName">
        /// The path to the file to save the data to.
        /// </param>
        ///
        /// <remarks>
        /// This method will not overwrite an existing file.
        /// Exceptions that can occur by calling <see cref="System.IO.File.Open(string, FileMode, FileAccess)"/> may also be
        /// thrown by this method.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public void SaveToFile(string fileName)
        {
            SaveToFile(fileName, FileMode.CreateNew);
        }

        /// <summary>
        /// Saves the BLOB data to the specified file.
        /// </summary>
        ///
        /// <param name="fileName">
        /// The path to the file to save the data to.
        /// </param>
        ///
        /// <param name="mode">
        /// A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist,
        /// and determines whether the contents of existing files are retained or overwritten.
        /// </param>
        ///
        /// <remarks>
        /// Exceptions that can occur by calling <see cref="System.IO.File.Open(string, FileMode, FileAccess)"/> may also be
        /// thrown by this method.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public void SaveToFile(string fileName, FileMode mode)
        {
            Validator.ThrowIfStringNullOrEmpty(fileName, "filename");

            using (FileStream file = File.Open(fileName, mode, FileAccess.Write))
            {
                SaveToStream(file);
            }
        }

        /// <summary>
        /// Saves the BLOB data to the specified file.
        /// </summary>
        ///
        /// <param name="stream">
        /// The stream to save the data to.
        /// </param>
        ///
        /// <remarks>
        /// The stream will not be closed by this method.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public void SaveToStream(Stream stream)
        {
            Validator.ThrowIfArgumentNull(stream, "stream", "ArgumentNull");

            using (BlobStream blob = GetReaderStream())
            {
                int bufferSize = (int)(ContentLength ?? Int32.MaxValue);
                bufferSize = Math.Min(bufferSize, _defaultStreamBufferSize);

                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                while ((bytesRead = blob.Read(buffer, 0, bufferSize)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
        }

        /// <summary>
        /// Fetches the BLOB as a UTF-8 string.
        /// </summary>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public string ReadAsString()
        {
            return ReadAsString(Encoding.UTF8);
        }

        /// <summary>
        /// Fetches the BLOB as a string with the specified encoding.
        /// </summary>
        ///
        /// <param name="encoding">
        /// The encoding used to convert the BLOB to a string.
        /// </param>
        ///
        /// <returns>
        /// A string representation of the BLOB.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public string ReadAsString(Encoding encoding)
        {
            String result;
            using (MemoryStream memoryStream = new MemoryStream(1000))
            {
                SaveToStream(memoryStream);
                memoryStream.Flush();

                result = encoding.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Position);
            }

            return result;
        }

        /// <summary>
        /// Gets all the bytes of the BLOB.
        /// </summary>
        ///
        /// <returns>
        /// All the bytes of the BLOB.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob doesn't have data that was retrieved from HealthVault.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is a failure reading the data from HealthVault.
        /// </exception>
        ///
        public byte[] ReadAllBytes()
        {
            byte[] result = null;
            int bufferSize = (int)(ContentLength ?? Int32.MaxValue);
            bufferSize = Math.Min(bufferSize, _defaultStreamBufferSize);

            using (MemoryStream memoryStream = new MemoryStream(bufferSize))
            {
                SaveToStream(memoryStream);
                memoryStream.Flush();

                result = memoryStream.ToArray();
            }

            return result;
        }

        #endregion Read

        /// <summary>
        /// Gets a URL that can be used to read or write the Blob data directly using HTTP GET and PUT.
        /// </summary>
        ///
        /// <returns>
        /// A URL for direct access to the Blob data.
        /// </returns>
        ///
        /// <remarks>
        /// The token used on the URL is short-lived so the URL is only valid for a short period
        /// of time.<br/>
        /// An application storing this URL, whether in a cookie or by other means, should encrypt
        /// it to avoid tampering.
        /// </remarks>
        ///
        /// <exception cref="NotSupportedException">
        /// If the Blob was retieved inline or the Blob was created for writing.
        /// </exception>
        ///
        public Uri Url
        {
            get { return _url; }
            internal set { _url = value; }
        }
        private Uri _url;

        internal byte[] InlineData
        {
            get { return _inlineData; }
            set { _inlineData = value; }
        }
        private byte[] _inlineData;

        private const int _defaultStreamBufferSize = 1 << 20; // 1MB

        /// <summary>
        /// Gets or sets if the Blob instance has been modified.
        /// </summary>
        ///
        /// <remarks>
        /// Normally this property is maintained by the members of the class, however,
        /// if <see cref="HealthRecordItem.GetItemXml()"/> is used to serialize an item
        /// that contains this Blob before the item is committed to HealthVault, you will
        /// need to mark the Blob as dirty before trying to commit the item to HealthVault.
        /// </remarks>
        ///
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
        private bool _isDirty;
    }
}
