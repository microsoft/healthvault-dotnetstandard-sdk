// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml.XPath;
using Microsoft.Health.Package;

namespace Microsoft.Health
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
            _canWrite = true;
        }

        internal BlobStream(ConnectPackageCreationParameters connectPackageParameters, Blob blob)
        {
            _connectPackageParameters = connectPackageParameters;
            _blob = blob;
            _length = blob.ContentLength;
            _canWrite = true;
        }

        internal BlobStream(
            Blob blob,
            Uri blobReferenceUrl,
            long? length)
        {
            _blob = blob;
            _url = blobReferenceUrl;
            _length = length;

            _canRead = true;
        }

        internal BlobStream(
            Blob blob,
            byte[] inlineData,
            long? length)
        {
            _blob = blob;
            _inlineData = inlineData;
            _length = length;

            _canRead = true;
        }

        private HealthRecordAccessor _record;
        private ConnectPackageCreationParameters _connectPackageParameters;
        private Blob _blob;
        private byte[] _inlineData;
        private long? _length;
        private Uri _url;

        private int _currentBufferRequestIndex;
        private byte[] _chunkBuffer;

        private List<byte[]> _blockHashes = new List<byte[]>();
        private BlobHasher _blobHasher;

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer"/>
        /// <param name="offset"/>
        /// <param name="count"/>
        /// <param name="callback"/>
        /// <param name="state"/>
        /// <exception cref="NotSupportedException"/>
        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer"/>
        /// <param name="offset"/>
        /// <param name="count"/>
        /// <param name="callback"/>
        /// <param name="state"/>
        /// <exception cref="NotSupportedException"/>
        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// 
        public override bool CanRead
        {
            get { return _canRead; }
        }
        private bool _canRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// 
        /// <value>
        /// True if the length of the stream is known, or false otherwise.
        /// </value>
        /// 
        public override bool CanSeek
        {
            get
            {
                bool result = true;
                if (_length == null)
                {
                    result = false;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// 
        public override bool CanTimeout
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// 
        public override bool CanWrite
        {
            get { return _canWrite; }
        }
        private bool _canWrite;

        /// <summary>
        /// Closes the current stream and releases any resources associated with the current stream.
        /// </summary>
        /// 
        public override void Close()
        {
            Dispose(true);
        }

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
        /// Not supported.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException"/>
        /// 
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException"/>
        /// 
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
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
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
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
                bytesToRead = (int)Math.Min(_length.Value - (long)_position, (long)count);
            }
            Array.Copy(_inlineData, _position, buffer, offset, bytesToRead);
            _position += bytesToRead;
            return bytesToRead;
        }

        private int ReadStreamedData(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            HttpWebRequest request = CreateGetRequest(_position, count);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        int bytesRead = 0;
                        int bytesToRead = count - bytesRead;
                        int position = offset;
                        while (
                            (0 < bytesToRead)
                            &&
                            (bytesRead =
                                 responseStream.Read(
                                    buffer,
                                    position,
                                    bytesToRead)) != 0)
                        {
                            _position += bytesRead;
                            position += bytesRead;
                            bytesToRead -= bytesRead;
                            totalBytesRead += bytesRead;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;

                // RequestedRangeNotSatisfiable means that we have finished reading the blob,
                // so ignore the error.
                if (response == null ||
                    response.StatusCode != HttpStatusCode.RequestedRangeNotSatisfiable)
                {
                    throw;
                }
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
        /// <remarks>
        /// If the request bytes to write don't make a full chunk, they will be buffered and
        /// written when a full chunk size is written or <see cref="Flush"/> or <see cref="Close"/>
        /// is called.
        /// </remarks>
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
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (_disposed)
            {
                throw new ObjectDisposedException("BlobStream");
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

                if (_connectPackageParameters != null)
                {
                    long startChunkSequenceNumber = _position / _blobPutParameters.ChunkSize;
                    start = startChunkSequenceNumber * _blobPutParameters.PostEncryptionChunkSize;

                    using (ICryptoTransform transform =
                        _connectPackageParameters.BlobChunkEncryptionAlgorithm.CreateEncryptor())
                    {
                        webRequestBuffer =
                            transform.TransformFinalBlock(webRequestBuffer, 0, chunkSizeToWrite);
                    }

                    bytesInWebRequest = webRequestBuffer.Length;
                }
            }

            HttpWebRequest request = CreatePutRequest(start, bytesInWebRequest, uploadComplete);
            using (Stream stream = request.GetRequestStream())
            {
                if (webRequestBuffer != null)
                {
                    stream.Write(webRequestBuffer, 0, bytesInWebRequest);
                    _bytesInBuffer -= chunkSizeToWrite;
                    stream.Flush();
                }

                stream.Close();
            }

            request.GetResponse();
            _position += chunkSizeToWrite;
        }

        private List<BufferRequest> _bufferList = new List<BufferRequest>();
        private int _bytesInBuffer;

        bool _triedToWrite;

        private class BufferRequest
        {
            internal BufferRequest(byte[] buffer, int offset, int count)
            {
                _buffer = new byte[count];
                Array.Copy(buffer, offset, _buffer, 0, count);
                _count = count;
            }

            internal BufferRequest(byte buffer)
            {
                _buffer = new byte[1];
                _buffer[0] = buffer;
                _count = 1;
            }

            internal byte[] Buffer
            {
                get { return _buffer; }
            }
            private byte[] _buffer;

            internal int Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }
            private int _offset;

            internal int Count
            {
                get { return _count; }
                set { _count = value; }
            }
            private int _count;
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
        /// <remarks>
        /// The bytes are buffered until the BLOB chunk size is obtained or <see cref="Flush"/> or
        /// <see cref="Close"/> is called.
        /// </remarks>
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

        private HttpWebRequest CreateGetRequest(long position, int count)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            request.AddRange((int)position, (int)position + count);

            if (_readTimeout > 0)
            {
                request.Timeout = _readTimeout;
            }
            return request;
        }

        private HttpWebRequest CreatePutRequest(
            long startPosition,
            int count,
            bool uploadComplete)
        {
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(_blobPutParameters.Url);
            request.Method = "POST";
            request.AllowWriteStreamBuffering = false;
            request.ServicePoint.Expect100Continue = false;
            request.SendChunked = true;

            if (_writeTimeout > 0)
            {
                request.Timeout = _writeTimeout;
            }

            if (count > 0)
            {
                request.Headers.Add(
                    "Content-Range",
                    String.Format(
                        CultureInfo.InvariantCulture,
                        "bytes {0}-{1}/*",
                        startPosition,
                        startPosition + (long)count - 1));
            }

            if (uploadComplete)
            {
                request.Headers.Add("x-hv-blob-complete", "1");
            }

            return request;
        }

        private void EnsureBeginPutBlob()
        {
            if (_blobPutParameters == null)
            {
                _blobPutParameters =
                    _record != null ?
                    BeginPutBlob() :
                    BeginPutConnectPackageBlob();

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
        private BlobPutParameters BeginPutBlob()
        {
            HealthServiceRequest request =
                new HealthServiceRequest(_record.Connection, "BeginPutBlob", 1, _record);
            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "BeginPutBlob");

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(infoPath);

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

        /// <summary>
        /// Calls the BeginPutBlob HealthVault method.
        /// </summary>
        /// 
        /// <returns>
        /// The result of the BeginPutConnectPackageBlob method call as a 
        /// BlobPutParameters instance.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// If the call to HealthVault fails in some way.
        /// </exception>
        /// 
        private BlobPutParameters BeginPutConnectPackageBlob()
        {
            HealthServiceRequest request =
                new HealthServiceRequest(_connectPackageParameters.Connection, "BeginPutConnectPackageBlob", 1);

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "BeginPutConnectPackageBlob");

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(infoPath);

            Uri blobReferenceUrl = new Uri(infoNav.SelectSingleNode("blob-ref-url").Value);
            int chunkSize = infoNav.SelectSingleNode("blob-pre-encryption-chunk-size").ValueAsInt;
            int postEncryptionChunkSize =
                infoNav.SelectSingleNode("blob-post-encryption-chunk-size").ValueAsInt;
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
                postEncryptionChunkSize,
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
