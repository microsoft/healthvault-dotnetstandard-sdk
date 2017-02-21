// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// A stream for sending and receiving binary data associated with a health record item.
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
        internal BlobStream(HealthRecordAccessor record, Blob blob)
        {
            _record = record;
            _blob = blob;
            _length = blob.ContentLength;
            CanWrite = true;
        }

        internal BlobStream(
            Blob blob,
            Uri blobReferenceUrl,
            long? length)
        {
            _blob = blob;
            _url = blobReferenceUrl;
            _length = length;

            CanRead = true;
        }

        internal BlobStream(
            Blob blob,
            byte[] inlineData,
            long? length)
        {
            _blob = blob;
            _inlineData = inlineData;
            _length = length;

            CanRead = true;
        }

        private HealthRecordAccessor _record;
        private Blob _blob;
        private byte[] _inlineData;
        private long? _length;
        private Uri _url;

        private int _currentBufferRequestIndex;
        private byte[] _chunkBuffer;

        private List<byte[]> _blockHashes = new List<byte[]>();
        private BlobHasher _blobHasher;

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
        public override bool CanSeek => _length != null;

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
        /// Releases all resources held by the <see cref="BlobStream"/>.
        /// </summary>
        ///
        ~BlobStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the stream object and optionally releases the managed
        /// resources.
        /// </summary>
        ///
        /// <param name="disposing">
        /// true to release both managed and unmanaged resoures; false to release only unmanaged
        /// resoureces.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If no data has been written to the stream.
        /// </exception>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing && CanWrite)
            {
                // if we haven't written anything and we don't have anything to write, throw
                // an exception...
                if (!_triedToWrite)
                {
                    throw Validator.InvalidOperationException("BlobStreamNoData");
                }
                SendChunks(true);
            }
            _disposed = true;
        }

        private bool _disposed;

        /// <summary>
        /// Releases all resources used by the stream.
        /// </summary>
        ///
        public new void Dispose()
        {
            Dispose(true);
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
                if (!CanSeek)
                {
                    throw new NotSupportedException();
                }

                if (_length == null)
                {
                    throw Validator.NotSupportedException("BlobStreamLengthNotSupported");
                }
                return _length.Value;
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
                return _position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }
        private long _position;

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
            if (!CanRead)
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

            if (_disposed)
            {
                throw new ObjectDisposedException("BlobStream");
            }

            Validator.ThrowArgumentExceptionIf(
                buffer.Length - offset < count,
                "buffer",
                "BlobStreamBufferLengthTooSmall");

            int result = 0;
            if (_inlineData != null)
            {
                result = ReadInlineData(buffer, offset, count);
            }
            else if (_url != null)
            {
                result = ReadStreamedData(buffer, offset, count);
            }

            return result;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte,
        /// or returns -1 if at the end of the stream.
        /// </summary>
        ///
        /// <returns>
        /// The unsigned byte cast to an <see cref="Int32"/>, or -1 if at the end of the stream.
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
            if (!CanRead)
            {
                throw new NotSupportedException();
            }

            if (_disposed)
            {
                throw new ObjectDisposedException("BlobStream");
            }

            int result = -1;

            // Check to see if there is still something in the buffer
            if (_bufferList.Count > 0)
            {
                BufferRequest buffer = _bufferList[0];
                result = buffer.Buffer[buffer.Offset];
                buffer.Count--;
                buffer.Offset++;

                if (buffer.Count == 0)
                {
                    _bufferList.RemoveAt(0);
                }
            }
            else
            {
                // Get a new chunk

                int bufferSize = (int)(_length ?? _defaultStreamBufferSize);
                byte[] newBuffer = new byte[bufferSize];
                int count = Read(newBuffer, 0, bufferSize);

                if (count > 0)
                {
                    result = newBuffer[0];

                    // Note, this makes a copy of the buffer.
                    _bufferList.Add(
                        new BufferRequest(newBuffer, 1, count - 1));
                }
            }
            return result;
        }

        private const int _defaultStreamBufferSize = 1 << 20; // 1MB

        #region Read helpers

        private int ReadInlineData(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count;
            if (_length != null)
            {
                bytesToRead = (int)Math.Min(_length.Value - _position, count);
            }
            Array.Copy(_inlineData, (int)_position, buffer, offset, bytesToRead);
            _position += bytesToRead;
            return bytesToRead;
        }

        private int ReadStreamedData(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            HttpResponseMessage request = ExecuteGetRequest(_position, count);
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
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }
        private int _readTimeout;

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        ///
        /// <param name="offset">
        /// A byte offset releative to the <paramref name="origin"/> parameter.
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
            if (!CanSeek)
            {
                throw new NotSupportedException();
            }

            long newPosition = _position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;

                case SeekOrigin.Current:
                    newPosition = _position + offset;
                    break;

                case SeekOrigin.End:
                    if (_length == null)
                    {
                        throw Validator.NotSupportedException("BlobStreamSeekFromEndNullLength");
                    }
                    newPosition = (_length.Value - 1) + offset;
                    break;
            }

            if (newPosition < 0)
            {
                throw Validator.IOException("BlobStreamPositionIsNegative");
            }

            if (_length != null && newPosition > _length - 1)
            {
                throw Validator.IOException("BlobStreamPositionPastEnd");
            }
            _position = newPosition;

            return _position;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        ///
        /// <exception cref="NotSupportedException"/>
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
            if (!CanWrite)
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

            if (_disposed)
            {
                throw new ObjectDisposedException("BlobStream");
            }

            if (_record == null)
            {
                throw new NotSupportedException("Currently blobs for records are only supported");
            }

            Validator.ThrowArgumentExceptionIf(
                buffer.Length - offset < count,
                "buffer",
                "BlobStreamBufferLengthTooSmall");

            _triedToWrite = true;

            EnsureBeginPutBlob();
            AugmentBufferList(new BufferRequest(buffer, offset, count));
            SendChunks(false);
        }

        private void AugmentBufferList(BufferRequest buffer)
        {
            _bufferList.Add(buffer);
            _bytesInBuffer += buffer.Count;
        }

        private void SendChunks(Boolean sendPartialChunk)
        {
            if (_bytesInBuffer > 0)
            {
                EnsureBeginPutBlob();
                while (_bytesInBuffer >= _blobPutParameters.ChunkSize)
                {
                    WriteChunk(_blobPutParameters.ChunkSize, false);
                }
            }

            if (sendPartialChunk)
            {
                EnsureBeginPutBlob();

                WriteChunk(_bytesInBuffer, true);
                CalculateBlobHash();
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
                    BufferRequest curRequestBuffer = _bufferList[_currentBufferRequestIndex];
                    // Get the number of bytes in the current request buffer up to a chunk.
                    int numBytesToCopy = Math.Min(curRequestBuffer.Count, chunkSizeToWrite);

                    // Ensure we don't write past the end of the chunk buffer.
                    numBytesToCopy = Math.Min(
                        numBytesToCopy,
                        _chunkBuffer.Length - writeBufferOffset);

                    // Copy the bytes to be sent over in to the chunk buffer;
                    Array.Copy(
                        curRequestBuffer.Buffer,
                        curRequestBuffer.Offset,
                        _chunkBuffer,
                        writeBufferOffset,
                        numBytesToCopy);

                    writeBufferOffset += numBytesToCopy;
                    curRequestBuffer.Offset += numBytesToCopy;
                    curRequestBuffer.Count -= numBytesToCopy;

                    if (curRequestBuffer.Count < 1)
                    {
                        _currentBufferRequestIndex++;
                    }
                }

                if (_currentBufferRequestIndex > 0)
                {
                    _bufferList.RemoveRange(0, _currentBufferRequestIndex);
                    _currentBufferRequestIndex = 0;
                }

                IList<byte[]> blockHashes =
                    _blobHasher.CalculateBlockHashes(_chunkBuffer, 0, chunkSizeToWrite);

                _blockHashes.AddRange(blockHashes);

                bytesInWebRequest = chunkSizeToWrite;
                start = _position;
                webRequestBuffer = _chunkBuffer;
            }

            if (webRequestBuffer != null)
            {
                HttpResponseMessage request = ExecutePutRequest(start, bytesInWebRequest, uploadComplete, webRequestBuffer);
                if (!request.IsSuccessStatusCode)
                {
                    this.ThrowRequestFailedException(request);
                }
                _bytesInBuffer -= chunkSizeToWrite;

            }
            _position += chunkSizeToWrite;
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

        private List<BufferRequest> _bufferList = new List<BufferRequest>();
        private int _bytesInBuffer;

        private bool _triedToWrite;

        private class BufferRequest
        {
            internal BufferRequest(byte[] buffer, int offset, int count)
            {
                Buffer = new byte[count];
                Array.Copy(buffer, offset, Buffer, 0, count);
                Count = count;
            }

            internal BufferRequest(byte buffer)
            {
                Buffer = new byte[1];
                Buffer[0] = buffer;
                Count = 1;
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
        /// Methods were called after the stram was closed.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there was a failure writing the data to HealthVault.
        /// </exception>
        ///
        public override void WriteByte(byte value)
        {
            if (!CanWrite)
            {
                throw new NotSupportedException();
            }

            if (_disposed)
            {
                throw new ObjectDisposedException("BlobStream");
            }

            AugmentBufferList(new BufferRequest(value));
            SendChunks(false);
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
            get { return _writeTimeout; }
            set { _writeTimeout = value; }
        }
        private int _writeTimeout;

        #region Helpers

        private HttpResponseMessage ExecuteGetRequest(long position, int count)
        {
            HttpMessageHandler handler = new HttpClientHandler();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, _blobPutParameters.Url);
            HttpClient request = new HttpClient(handler);

            message.Headers.Range = new RangeHeaderValue((int)position, (int)position + count);

            if (_readTimeout > 0)
            {
                request.Timeout = new TimeSpan(_writeTimeout);
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
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _blobPutParameters.Url);
            HttpClient request = new HttpClient(handler);

            message.Headers.TransferEncodingChunked = true;

            if (_writeTimeout > 0)
            {
                request.Timeout = new TimeSpan(_writeTimeout);
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
            if (_blobPutParameters == null)
            {
                _blobPutParameters = BeginPutBlobAsync().Result;

                _blob.Url = _blobPutParameters.Url;
                _chunkBuffer = new byte[_blobPutParameters.ChunkSize];
                _blobHasher = new BlobHasher(
                    _blobPutParameters.BlobHashAlgorithm,
                    _blobPutParameters.HashBlockSize);
            }
        }

        private BlobPutParameters _blobPutParameters;

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
            HealthServiceRequest request =
                new HealthServiceRequest(_record.Connection, "BeginPutBlob", 1, _record);
            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator,
                    "BeginPutBlob");

            XPathNavigator infoNav =
                responseData.InfoNavigator.SelectSingleNode(infoPath);

            Uri blobReferenceUrl = new Uri(infoNav.SelectSingleNode("blob-ref-url").Value);
            int chunkSize = infoNav.SelectSingleNode("blob-chunk-size").ValueAsInt;
            long maxBlobSize = infoNav.SelectSingleNode("max-blob-size").ValueAsLong;
            String blobHashAlgString = infoNav.SelectSingleNode("blob-hash-algorithm").Value;
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
            Byte[] blobHash = _blobHasher.CalculateBlobHash(_blockHashes);

            _blob.HashInfo = new BlobHashInfo(
                _blobHasher.BlobHashAlgorithm,
                _blobHasher.BlockSize,
                blobHash);
        }

        #endregion Helpers
    }
}
