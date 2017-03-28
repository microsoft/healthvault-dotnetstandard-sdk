// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Transport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// A stream for sending and receiving binary data associated with a thing.
    /// </summary>
    ///
    /// <remarks>
    /// The stream can be written only for BLOBs created using
    /// <see cref="BlobStore.NewBlob(string, string)"/>.
    /// If the blob has data retrieved from HealthVault
    /// it will not be writable. Instead, use <see cref="BlobStore.NewBlob(string, string)"/>
    /// with the same name as the BLOB you wish to replace and then get the <see cref="BlobStream"/>
    /// from that <see cref="Blob"/> instance and write to it.
    /// </remarks>
    ///
    public class BlobStream : Stream
    {
        private const int DefaultStreamBufferSize = 1 << 20; // 1MB

        internal BlobStream(HealthRecordAccessor record, Blob blob)
        {
            this.record = record;
            this.blob = blob;
            this.length = blob.ContentLength;
            this.CanWrite = true;
        }

        internal BlobStream(
            Blob blob,
            Uri blobReferenceUrl,
            long? length)
        {
            this.blob = blob;
            this.url = blobReferenceUrl;
            this.length = length;

            this.CanRead = true;
        }

        internal BlobStream(
            Blob blob,
            byte[] inlineData,
            long? length)
        {
            this.blob = blob;
            this.inlineData = inlineData;
            this.length = length;

            this.CanRead = true;
        }

        private readonly HealthRecordAccessor record;
        private readonly Blob blob;
        private readonly byte[] inlineData;
        private long? length;
        private readonly Uri url;

        private int currentBufferRequestIndex;
        private byte[] chunkBuffer;

        private readonly List<byte[]> blockHashes = new List<byte[]>();
        private BlobHasher blobHasher;

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        ///
        public override bool CanRead { get; }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        ///
        /// <value>
        /// True if the length of the stream is known, or false otherwise.
        /// </value>
        ///
        public override bool CanSeek => this.length != null;

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        ///
        public override bool CanTimeout => true;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        ///
        public override bool CanWrite { get; }

        /// <summary>
        /// Finalizes an instance of the <see cref="BlobStream"/> class.
        /// Releases all resources held by the <see cref="BlobStream"/>.
        /// </summary>
        ///
        ~BlobStream()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the stream object and optionally releases the managed
        /// resources.
        /// </summary>
        ///
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If no data has been written to the stream.
        /// </exception>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing && this.CanWrite)
            {
                // if we haven't written anything and we don't have anything to write, throw
                // an exception...
                if (!this.triedToWrite)
                {
                    throw new InvalidOperationException(Resources.BlobStreamNoData);
                }

                this.SendChunks(true);
            }

            this.disposed = true;
        }

        private bool disposed;

        /// <summary>
        /// Releases all resources used by the stream.
        /// </summary>
        ///
        public new void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to
        /// HealthVault.
        /// </summary>
        ///
        /// <exception cref="HealthServiceException">
        /// If a failure occurs calling HealthVault.
        /// </exception>
        ///
        public override void Flush()
        {
        }

        /// <summary>
        /// Gets the length BLOB.
        /// </summary>
        ///
        /// <exception cref="NotSupportedException">
        /// If <see cref="CanSeek"/> is false or the length of the stream is not known.
        /// </exception>
        ///
        public override long Length
        {
            get
            {
                if (!this.CanSeek)
                {
                    throw new NotSupportedException();
                }

                if (this.length == null)
                {
                    throw new NotSupportedException(Resources.BlobStreamLengthNotSupported);
                }

                return this.length.Value;
            }
        }

        /// <summary>
        /// Gets the position of the stream.
        /// </summary>
        ///
        /// <remarks>
        /// Set is not supported.
        /// </remarks>
        ///
        /// <exception cref="NotSupportedException">
        /// If <see cref="CanSeek"/> is false when the setter is called.
        /// </exception>
        ///
        public override long Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }

        private long position;

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the
        /// stream by the number of bytes read.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte
        /// array with the values between offset and (offset + count - 1) replaced by the bytes
        /// read from the current source.
        /// </param>
        ///
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read from
        /// the current stream.
        /// </param>
        ///
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        ///
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of
        /// bytes requested if that many bytes are not currently available, or zero (0) if the
        /// end of the stream has been reached.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If the stream does not support reading.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the
        /// <paramref name="buffer"/> length.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        ///
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there was a failure reading the data from HealthVault.
        /// </exception>
        ///
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!this.CanRead)
            {
                throw new NotSupportedException();
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(BlobStream));
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(Resources.BlobStreamBufferLengthTooSmall, nameof(buffer));
            }

            int result = 0;
            if (this.inlineData != null)
            {
                result = this.ReadInlineData(buffer, offset, count);
            }
            else if (this.url != null)
            {
                result = this.ReadStreamedData(buffer, offset, count);
            }

            return result;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte,
        /// or returns -1 if at the end of the stream.
        /// </summary>
        ///
        /// <returns>
        /// The unsigned byte cast to an <see cref="int"/>, or -1 if at the end of the stream.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// If the stream does not support reading.
        /// </exception>
        ///
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there was a failure reading the data from HealthVault.
        /// </exception>
        ///
        public override int ReadByte()
        {
            if (!this.CanRead)
            {
                throw new NotSupportedException();
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(BlobStream));
            }

            int result = -1;

            // Check to see if there is still something in the buffer
            if (this.bufferList.Count > 0)
            {
                BufferRequest buffer = this.bufferList[0];
                result = buffer.Buffer[buffer.Offset];
                buffer.Count--;
                buffer.Offset++;

                if (buffer.Count == 0)
                {
                    this.bufferList.RemoveAt(0);
                }
            }
            else
            {
                // Get a new chunk

                int bufferSize = (int)(this.length ?? DefaultStreamBufferSize);
                byte[] newBuffer = new byte[bufferSize];
                int count = this.Read(newBuffer, 0, bufferSize);

                if (count > 0)
                {
                    result = newBuffer[0];

                    // Note, this makes a copy of the buffer.
                    this.bufferList.Add(
                        new BufferRequest(newBuffer, 1, count - 1));
                }
            }

            return result;
        }

        #region Read helpers

        private int ReadInlineData(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count;
            if (this.length != null)
            {
                bytesToRead = (int)Math.Min(this.length.Value - this.position, count);
            }

            Array.Copy(this.inlineData, (int)this.position, buffer, offset, bytesToRead);
            this.position += bytesToRead;
            return bytesToRead;
        }

        private int ReadStreamedData(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            HttpResponseMessage request = this.ExecuteGetRequest(this.position, count);
            if (request.IsSuccessStatusCode)
            {
                HttpContent responseStream = request.Content;
                var array = responseStream.ReadAsByteArrayAsync().Result;
                array.CopyTo(buffer, offset);
            }
            else
            {
                this.ThrowRequestFailedException(request);
            }

            return totalBytesRead;
        }

        #endregion Read helpers

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt
        /// to read before timing out.
        /// </summary>
        ///
        /// <value>
        /// A value, in milliseconds, that determines how long the stream will attempt to read
        /// before timing out.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to 0 or a negative number to request that there is no timeout.
        /// </remarks>
        ///
        public override int ReadTimeout
        {
            get { return this.readTimeout; }
            set { this.readTimeout = value; }
        }

        private int readTimeout;

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        ///
        /// <param name="offset">
        /// A byte offset relative to the <paramref name="origin"/> parameter.
        /// </param>
        ///
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain
        /// the new position.
        /// </param>
        ///
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        ///
        /// <exception cref="IOException">
        /// If <paramref name="offset"/> causes the position to be set before the beginning or
        /// after the end of the stream.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <see cref="CanSeek"/> is false or <see cref="Length"/> is unknown and
        /// <paramref name="origin"/> is <see cref="SeekOrigin.End"/>.
        /// </exception>
        ///
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!this.CanSeek)
            {
                throw new NotSupportedException();
            }

            long newPosition = this.position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;

                case SeekOrigin.Current:
                    newPosition = this.position + offset;
                    break;

                case SeekOrigin.End:
                    if (this.length == null)
                    {
                        throw new NotSupportedException(Resources.BlobStreamSeekFromEndNullLength);
                    }

                    newPosition = (this.length.Value - 1) + offset;
                    break;
            }

            if (newPosition < 0)
            {
                throw new IOException(Resources.BlobStreamPositionIsNegative);
            }

            if (this.length != null && newPosition > this.length - 1)
            {
                throw new IOException(Resources.BlobStreamPositionPastEnd);
            }

            this.position = newPosition;

            return this.position;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        ///
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position
        /// within this stream by the number of bytes written.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current stream.
        /// </param>
        ///
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the current
        /// stream.
        /// </param>
        ///
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="buffer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the
        /// <paramref name="buffer"/> length.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// If the stream does not support writing.
        /// </exception>
        ///
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there was a failure writing the data to HealthVault.
        /// </exception>
        ///
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!this.CanWrite)
            {
                throw new NotSupportedException();
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(BlobStream));
            }

            if (this.record == null)
            {
                throw new NotSupportedException(Resources.BlobMustHaveRecord);
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(Resources.BlobStreamBufferLengthTooSmall, nameof(buffer));
            }

            this.triedToWrite = true;

            this.EnsureBeginPutBlob();
            this.AugmentBufferList(new BufferRequest(buffer, offset, count));
            this.SendChunks(false);
        }

        private void AugmentBufferList(BufferRequest buffer)
        {
            this.bufferList.Add(buffer);
            this.bytesInBuffer += buffer.Count;
        }

        private void SendChunks(bool sendPartialChunk)
        {
            if (this.bytesInBuffer > 0)
            {
                this.EnsureBeginPutBlob();
                while (this.bytesInBuffer >= this.blobPutParameters.ChunkSize)
                {
                    this.WriteChunk(this.blobPutParameters.ChunkSize, false);
                }
            }

            if (sendPartialChunk)
            {
                this.EnsureBeginPutBlob();

                this.WriteChunk(this.bytesInBuffer, true);
                this.CalculateBlobHash();
            }
        }

        private void WriteChunk(int chunkSizeToWrite, bool uploadComplete)
        {
            int bytesInWebRequest = 0;
            long start = 0;
            byte[] webRequestBuffer = null;
            if (chunkSizeToWrite > 0)
            {
                int writeBufferOffset = 0;
                while (writeBufferOffset < chunkSizeToWrite)
                {
                    BufferRequest curRequestBuffer = this.bufferList[this.currentBufferRequestIndex];

                    // Get the number of bytes in the current request buffer up to a chunk.
                    int numBytesToCopy = Math.Min(curRequestBuffer.Count, chunkSizeToWrite);

                    // Ensure we don't write past the end of the chunk buffer.
                    numBytesToCopy = Math.Min(
                        numBytesToCopy,
                        this.chunkBuffer.Length - writeBufferOffset);

                    // Copy the bytes to be sent over in to the chunk buffer;
                    Array.Copy(
                        curRequestBuffer.Buffer,
                        curRequestBuffer.Offset,
                        this.chunkBuffer,
                        writeBufferOffset,
                        numBytesToCopy);

                    writeBufferOffset += numBytesToCopy;
                    curRequestBuffer.Offset += numBytesToCopy;
                    curRequestBuffer.Count -= numBytesToCopy;

                    if (curRequestBuffer.Count < 1)
                    {
                        this.currentBufferRequestIndex++;
                    }
                }

                if (this.currentBufferRequestIndex > 0)
                {
                    this.bufferList.RemoveRange(0, this.currentBufferRequestIndex);
                    this.currentBufferRequestIndex = 0;
                }

                IList<byte[]> blockHashes =
                    this.blobHasher.CalculateBlockHashes(this.chunkBuffer, 0, chunkSizeToWrite);

                this.blockHashes.AddRange(blockHashes);

                bytesInWebRequest = chunkSizeToWrite;
                start = this.position;
                webRequestBuffer = this.chunkBuffer;
            }

            if (webRequestBuffer != null)
            {
                HttpResponseMessage request = this.ExecutePutRequest(start, bytesInWebRequest, uploadComplete, webRequestBuffer);
                if (!request.IsSuccessStatusCode)
                {
                    this.ThrowRequestFailedException(request);
                }

                this.bytesInBuffer -= chunkSizeToWrite;
            }

            this.position += chunkSizeToWrite;
        }

        private void ThrowRequestFailedException(HttpResponseMessage request)
        {
            HttpStatusCode response = request.StatusCode;

            // RequestedRangeNotSatisfiable means that we have finished reading the blob,
            // so ignore the error.
            if (response != HttpStatusCode.RequestedRangeNotSatisfiable)
            {
                throw new HttpRequestException($"Request failed with status code {response}");
            }
        }

        private List<BufferRequest> bufferList = new List<BufferRequest>();
        private int bytesInBuffer;

        private bool triedToWrite;

        private class BufferRequest
        {
            internal BufferRequest(byte[] buffer, int offset, int count)
            {
                this.Buffer = new byte[count];
                Array.Copy(buffer, offset, this.Buffer, 0, count);
                this.Count = count;
            }

            internal BufferRequest(byte buffer)
            {
                this.Buffer = new byte[1];
                this.Buffer[0] = buffer;
                this.Count = 1;
            }

            internal byte[] Buffer { get; }

            internal int Offset { get; set; }

            internal int Count { get; set; }
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within
        /// the stream by one byte.
        /// </summary>
        ///
        /// <param name="value">
        /// The byte to write to the stream.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// If the stream does not support reading.
        /// </exception>
        ///
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there was a failure writing the data to HealthVault.
        /// </exception>
        ///
        public override void WriteByte(byte value)
        {
            if (!this.CanWrite)
            {
                throw new NotSupportedException();
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(BlobStream));
            }

            this.AugmentBufferList(new BufferRequest(value));
            this.SendChunks(false);
        }

        /// <summary>
        /// Gets or sets the timeout for write requests.
        /// </summary>
        ///
        /// <remarks>
        /// Set the value to zero or negative to request no timeout.
        /// </remarks>
        ///
        public override int WriteTimeout
        {
            get { return this.writeTimeout; }
            set { this.writeTimeout = value; }
        }

        private int writeTimeout;

        #region Helpers

        private HttpResponseMessage ExecuteGetRequest(long position, int count)
        {
            HttpMessageHandler handler = new HttpClientHandler();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, this.blobPutParameters.Url);
            HttpClient request = new HttpClient(handler);

            message.Headers.Range = new RangeHeaderValue((int)position, (int)position + count);

            if (this.readTimeout > 0)
            {
                request.Timeout = new TimeSpan(this.writeTimeout);
            }

            return request.SendAsync(message).Result;
        }

        private HttpResponseMessage ExecutePutRequest(
            long startPosition,
            int count,
            bool uploadComplete,
            byte[] webRequestBuffer)
        {
            HttpMessageHandler handler = new HttpClientHandler();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, this.blobPutParameters.Url);
            HttpClient request = new HttpClient(handler);

            message.Headers.TransferEncodingChunked = true;

            if (this.writeTimeout > 0)
            {
                request.Timeout = new TimeSpan(this.writeTimeout);
            }

            message.Content = new ByteArrayContent(webRequestBuffer);

            if (count > 0)
            {
                message.Headers.Add(
                    "Content-Range",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "bytes {0}-{1}/*",
                        startPosition,
                        startPosition + count - 1));
            }

            if (uploadComplete)
            {
                message.Headers.Add("x-hv-blob-complete", "1");
            }

            return request.SendAsync(message).Result;
        }

        private void EnsureBeginPutBlob()
        {
            if (this.blobPutParameters == null)
            {
                this.blobPutParameters = this.BeginPutBlobAsync().Result;

                this.blob.Url = this.blobPutParameters.Url;
                this.chunkBuffer = new byte[this.blobPutParameters.ChunkSize];
                this.blobHasher = new BlobHasher(
                    this.blobPutParameters.BlobHashAlgorithm,
                    this.blobPutParameters.HashBlockSize);
            }
        }

        private BlobPutParameters blobPutParameters;

        /// <summary>
        /// Calls the BeginPutBlob HealthVault method.
        /// </summary>
        ///
        /// <returns>
        /// The result of the BeginPutBlob method call as a BlobPutParameters instance.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// If the call to HealthVault fails in some way.
        /// </exception>
        ///
        private async Task<BlobPutParameters> BeginPutBlobAsync()
        {
            HealthServiceResponseData responseData = await this.record.Connection.ExecuteAsync(HealthVaultMethods.BeginPutBlob, 1).ConfigureAwait(false);

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator,
                    "BeginPutBlob");

            XPathNavigator infoNav =
                responseData.InfoNavigator.SelectSingleNode(infoPath);

            Uri blobReferenceUrl = new Uri(infoNav.SelectSingleNode("blob-ref-url").Value);
            int chunkSize = infoNav.SelectSingleNode("blob-chunk-size").ValueAsInt;
            long maxBlobSize = infoNav.SelectSingleNode("max-blob-size").ValueAsLong;
            string blobHashAlgString = infoNav.SelectSingleNode("blob-hash-algorithm").Value;
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

            int blocksize = 0;
            XPathNavigator blockNav = infoNav.SelectSingleNode("blob-hash-parameters/block-size");
            if (blockNav != null)
            {
                blocksize = blockNav.ValueAsInt;
            }

            return new BlobPutParameters(
                blobReferenceUrl,
                chunkSize,
                maxBlobSize,
                blobHashAlg,
                blocksize);
        }

        private void CalculateBlobHash()
        {
            byte[] blobHash = this.blobHasher.CalculateBlobHash(this.blockHashes);

            this.blob.HashInfo = new BlobHashInfo(
                this.blobHasher.BlobHashAlgorithm,
                this.blobHasher.BlockSize,
                blobHash);
        }

        #endregion Helpers
    }
}
