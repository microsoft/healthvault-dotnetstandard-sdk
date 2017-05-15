// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.HealthVault.Web.Cookie
{
    /// <summary>
    /// <see cref="ICookieDataManager"/>
    /// </summary>
    internal class CookieDataManager : ICookieDataManager
    {
        public string Compress(string data, out int bufferLength)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var bytes = Encoding.UTF8.GetBytes(data);

            using (var memoryStream = new MemoryStream())
            {
                using (var deflate = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflate.Write(bytes, 0, bytes.Length);
                }

                var bufferArray = memoryStream.ToArray();

                bufferLength = bufferArray.Length;

                return Convert.ToBase64String(bufferArray);
            }
        }

        public string Decompress(string compressedData)
        {
            if (String.IsNullOrEmpty(compressedData))
            {
                throw new ArgumentNullException(nameof(compressedData));
            }

            byte[] stringBytes = Convert.FromBase64String(compressedData);

            using (var memoryStream = new MemoryStream(stringBytes))
            {
                using (var deflate = new DeflateStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(deflate))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
