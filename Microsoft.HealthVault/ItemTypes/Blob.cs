// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents binary data that can be associated with a person's health record.
    /// </summary>
    ///
    public class Blob
    {
        private const int DefaultStreamBufferSize = 1 << 20; // 1MB

        /// <summary>
        /// Constructs an instance of the Blob class with the specified values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
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
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
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
            Validator.ThrowIfArgumentNull(name, nameof(name), Resources.StringNull);
            Validator.ThrowIfArgumentNull(contentType, nameof(contentType), Resources.StringNull);

            this.Name = name;
            this.ContentType = contentType;
            this.ContentEncoding = currentContentEncoding;
            this.LegacyContentEncoding = legacyContentEncoding;
            this.HashInfo = hashInfo;
            this.record = record;
        }

        /// <summary>
        /// Constructs an instance of the Blob class with the specified values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
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
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> or <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        internal Blob(
            string name,
            string contentType,
            string currentContentEncoding,
            string legacyContentEncoding,
            BlobHashInfo hashInfo)
        {
            Validator.ThrowIfArgumentNull(name, nameof(name), Resources.StringNull);
            Validator.ThrowIfArgumentNull(contentType, nameof(contentType), Resources.StringNull);

            this.Name = name;
            this.ContentType = contentType;
            this.ContentEncoding = currentContentEncoding;
            this.LegacyContentEncoding = legacyContentEncoding;
            this.HashInfo = hashInfo;
        }

        /// <summary>
        /// Gets the name of the BLOB.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the content type of the BLOB.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the hash information for the BLOB.
        /// </summary>
        public BlobHashInfo HashInfo { get; internal set; }

        private readonly HealthRecordAccessor record;

        /// <summary>
        /// Gets the content encoding of the BLOB.
        /// </summary>
        ///
        /// <remarks>
        /// This is only available for BLOBs that were added to HealthVault prior to streaming
        /// support being added.
        /// </remarks>
        ///
        public string ContentEncoding { get; internal set; }

        internal string LegacyContentEncoding { get; }

        /// <summary>
        /// Gets the length of the content of the BLOB.
        /// </summary>
        ///
        /// <remarks>
        /// In some cases the content length can't be determined by
        /// HealthVault until the data is retrieved. In this case, the property
        /// will be null. 
        /// </remarks>
        ///
        public long? ContentLength { get; internal set; }

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
            if (this.InlineData != null || this.Url != null)
            {
                throw new NotSupportedException();
            }

            this.IsDirty = true;

            return this.record != null ?
                    new BlobStream(this.record, this) : null;
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
        /// <see cref="HealthRecordAccessor.NewItemAsync"/> or <see cref="HealthRecordAccessor.UpdateItemAsync"/>
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
            this.WriteInline(data, Encoding.UTF8);
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
        /// <see cref="HealthRecordAccessor.NewItemAsync"/> or <see cref="HealthRecordAccessor.UpdateItemAsync"/>
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
            Validator.ThrowIfArgumentNull(data, nameof(data), Resources.StringNull);
            Validator.ThrowIfArgumentNull(encoding, nameof(encoding), Resources.ArgumentNull);

            this.WriteNewInlineData(encoding.GetBytes(data));
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
        /// <see cref="HealthRecordAccessor.NewItemAsync"/> or <see cref="HealthRecordAccessor.UpdateItemAsync"/>
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
            Validator.ThrowIfArgumentNull(bytes, nameof(bytes), Resources.ArgumentNull);

            this.WriteNewInlineData(bytes);
        }

        private void WriteNewInlineData(byte[] bytes)
        {
            BlobHasher inlineHasher = BlobHasher.InlineBlobHasher;
            byte[] blobHash = inlineHasher.CalculateBlobHash(bytes, 0, bytes.Length);

            this.Url = null;
            this.InlineData = bytes;
            this.ContentLength = bytes.Length;

            this.HashInfo = new BlobHashInfo(
                inlineHasher.BlobHashAlgorithm,
                inlineHasher.BlockSize,
                blobHash);

            this.IsDirty = true;
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
        /// <exception cref="HealthHttpException">
        /// If there is an error writing the data to HealthVault.
        /// </exception>
        ///
        public void Write(Stream stream)
        {
            Validator.ThrowIfArgumentNull(stream, nameof(stream), Resources.ArgumentNull);

            int bufferSize = DefaultStreamBufferSize;
            if (stream.CanSeek)
            {
                bufferSize = (int)Math.Min(stream.Length, DefaultStreamBufferSize);
            }

            byte[] bytes = new byte[bufferSize];

            using (BlobStream blobStream = this.GetWriterStream())
            {
                int bytesRead;
                while ((bytesRead = stream.Read(bytes, 0, bufferSize)) > 0)
                {
                    blobStream.Write(bytes, 0, bytesRead);
                }
            }

            this.IsDirty = true;
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
            BlobStream stream;
            if (this.Url != null)
            {
                stream = new BlobStream(this, this.Url, this.ContentLength);
            }
            else if (this.InlineData != null)
            {
                stream = new BlobStream(this, this.InlineData, this.ContentLength);
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
        public void SaveToFile(string fileName, FileMode mode = FileMode.CreateNew)
        {
            Validator.ThrowIfStringNullOrEmpty(fileName, "filename");

            using (FileStream file = System.IO.File.Open(fileName, mode, FileAccess.Write))
            {
                this.SaveToStream(file);
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
            Validator.ThrowIfArgumentNull(stream, nameof(stream), Resources.ArgumentNull);

            using (BlobStream blob = this.GetReaderStream())
            {
                int bufferSize = (int)(this.ContentLength ?? int.MaxValue);
                bufferSize = Math.Min(bufferSize, DefaultStreamBufferSize);

                byte[] buffer = new byte[bufferSize];
                int bytesRead;
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
            return this.ReadAsString(Encoding.UTF8);
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
            string result;
            using (MemoryStream memoryStream = new MemoryStream(1000))
            {
                this.SaveToStream(memoryStream);
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
            byte[] result;
            int bufferSize = (int)(this.ContentLength ?? int.MaxValue);
            bufferSize = Math.Min(bufferSize, DefaultStreamBufferSize);

            using (MemoryStream memoryStream = new MemoryStream(bufferSize))
            {
                this.SaveToStream(memoryStream);
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
        public Uri Url { get; internal set; }

        internal byte[] InlineData { get; set; }

        /// <summary>
        /// Gets or sets if the Blob instance has been modified.
        /// </summary>
        ///
        /// <remarks>
        /// Normally this property is maintained by the members of the class, however,
        /// if <see cref="ThingBase.GetItemXml(string)"/> is used to serialize an item
        /// that contains this Blob before the item is committed to HealthVault, you will
        /// need to mark the Blob as dirty before trying to commit the item to HealthVault.
        /// </remarks>
        ///
        public bool IsDirty { get; set; }
    }
}
