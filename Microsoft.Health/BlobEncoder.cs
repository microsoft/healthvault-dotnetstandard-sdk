// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.Health
{
    /// <summary>
    /// Used to encode BLOBs in various encoding schemes.
    /// </summary>
    /// 
    internal static class BlobEncoder
    {
        internal static byte[] Encode(byte[] rawBlobPayload, string contentEncoding)
        {
            if (string.IsNullOrEmpty(contentEncoding))
            {
                return rawBlobPayload;
            }

            string[] encodings = SDKHelper.SplitAndTrim(contentEncoding, c_encodingSeperator);

            return Encode(rawBlobPayload, encodings);
        }

        internal static byte[] Decode(byte[] encodedBlobPayload, string contentEncoding)
        {
            if (string.IsNullOrEmpty(contentEncoding))
            {
                return encodedBlobPayload;
            }

            string[] encodings = SDKHelper.SplitAndTrim(contentEncoding, c_encodingSeperator);

            return Decode(encodedBlobPayload, encodings);
        }

        private static byte[] Encode(byte[] rawBlobPayloadBytes, string[] encodings)
        {
            byte[] encodedPayload = rawBlobPayloadBytes;

            for (int i = 0; i < encodings.Length; ++i)
            {
                switch (encodings[i])
                {
                    case "base64":
                        encodedPayload = EncodeAsBase64Bytes(encodedPayload);
                        break;
                    case "base-64":
                        encodedPayload = EncodeAsBase64Bytes(encodedPayload);
                        break;
                    case "gzip":
                        encodedPayload = EncodeAsGzipBytes(encodedPayload);
                        break;
                    case "deflate":
                        encodedPayload = EncodeAsDeflateBytes(encodedPayload);
                        break;
                    default:
                        break;
                }
            }
            return encodedPayload;
        }

        private static byte[] Decode(byte[] encodedPayload, string[] encodings)
        {
            Byte[] decodedPayload = encodedPayload;
            List<string> contentEncodingsUsed = new List<string>();
            try
            {
                for (Int32 i = encodings.Length - 1; i >= 0; --i)
                {
                    if (contentEncodingsUsed.Contains(encodings[i]))
                    {
                        throw new HealthServiceException(
                            HealthServiceStatusCode.UnsupportedContentEncoding);
                    }

                    switch (encodings[i])
                    {
                        case "base64":
                            decodedPayload = DecodeAsBase64Bytes(decodedPayload);
                            break;

                        case "base-64":
                            decodedPayload = DecodeAsBase64Bytes(decodedPayload);
                            break;

                        case "deflate":
                            decodedPayload = DecodeAsDeflateBytes(decodedPayload);
                            break;

                        case "gzip":
                            decodedPayload = DecodeAsGzipBytes(decodedPayload);
                            break;

                        default:
                            throw new HealthServiceException(
                                HealthServiceStatusCode.UnsupportedContentEncoding);
                    }
                    contentEncodingsUsed.Add(encodings[i]);
                }
                VerifyPayloadExists(decodedPayload);
            }
            catch (HealthServiceException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new HealthServiceException(
                    HealthServiceStatusCode.ContentEncodingDataMismatch);
            }
            return decodedPayload;
        }

        private static void VerifyPayloadExists(byte[] encodedData)
        {
            if (encodedData == null || encodedData.Length == 0)
            {
                throw new HealthServiceException(
                    HealthServiceStatusCode.ContentEncodingDataMismatch);
            }
        }

        private static byte[] EncodeAsBase64Bytes(byte[] rawPayload)
        {
            return Encoding.UTF8.GetBytes(EncodeAsBase64(rawPayload));
        }

        private static byte[] DecodeAsBase64Bytes(byte[] encodedPayload)
        {
            VerifyPayloadExists(encodedPayload);
            return Convert.FromBase64String(Encoding.UTF8.GetString(encodedPayload));
        }

        private static string EncodeAsBase64(byte[] rawPayload)
        {
            return Convert.ToBase64String(rawPayload);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes code more readable")]
        private static byte[] EncodeAsGzipBytes(byte[] rawPayload)
        {
            using (MemoryStream source = new MemoryStream(rawPayload))
            {
                using (MemoryStream encodedBytesStream = new MemoryStream())
                {
                    using (GZipStream destination = new GZipStream(
                            encodedBytesStream,
                            CompressionMode.Compress,
                            true))
                    {
                        CopyData(source, destination);
                    }
                    return encodedBytesStream.ToArray();
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes code more readable")]
        private static byte[] DecodeAsGzipBytes(byte[] encodedPayload)
        {
            VerifyPayloadExists(encodedPayload);
            using (MemoryStream sourceStream = new MemoryStream(encodedPayload, 0, encodedPayload.Length))
            {
                using (GZipStream source = new GZipStream(sourceStream, CompressionMode.Decompress))
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        CopyData(source, destination);
                        return destination.ToArray();
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes code more readable")]
        private static byte[] EncodeAsDeflateBytes(byte[] rawPayload)
        {
            using (MemoryStream source = new MemoryStream(rawPayload))
            {
                using (MemoryStream encodedBytesStream = new MemoryStream())
                {
                    using (DeflateStream destination = new DeflateStream(
                            encodedBytesStream,
                            CompressionMode.Compress,
                            true))
                    {
                        CopyData(source, destination);
                    }
                    return encodedBytesStream.ToArray();
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes code more readable")]
        private static byte[] DecodeAsDeflateBytes(byte[] encodedPayload)
        {
            VerifyPayloadExists(encodedPayload);
            using (MemoryStream sourceStream = new MemoryStream(encodedPayload, 0, encodedPayload.Length))
            {
                using (DeflateStream source = new DeflateStream(sourceStream, CompressionMode.Decompress))
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        CopyData(source, destination);
                        return destination.ToArray();
                    }
                }
            }
        }

        private static void CopyData(Stream inputStream, Stream outputStream)
        {
            byte[] buffer = new byte[1024];

            int count = inputStream.Read(buffer, 0, buffer.Length);
            while (count != 0)
            {
                outputStream.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, buffer.Length);
            }
        }
        private const char c_encodingSeperator = ',';
    }
}