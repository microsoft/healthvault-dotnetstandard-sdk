// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Core;

namespace Microsoft.HealthVault.Client
{
    internal class NetFrameworkSecretStore : ISecretStore
    {
        private const string SaltString = "d219a6c4e0a78d74f2ff309061d473424646e43fc72f88aca5801ff09c33da9c";

        private static readonly byte[] Salt = StringToByteArray(SaltString);

        private static readonly string StoreFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), HealthVaultConstants.Storage.DirectoryName);

        public async Task WriteAsync(string key, string contents)
        {
            try
            {
                string filePath = Path.Combine(StoreFolder, key);

                EnsureDirectory();

                byte[] contentBytes = Encoding.UTF8.GetBytes(contents);
                byte[] encryptedContentBytes = ProtectedData.Protect(contentBytes, Salt, DataProtectionScope.CurrentUser);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await fileStream.WriteAsync(encryptedContentBytes, 0, encryptedContentBytes.Length).ConfigureAwait(false);
                }
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionWrite, key), e);
            }
        }

        public async Task<string> ReadAsync(string key)
        {
            try
            {
                string filePath = Path.Combine(StoreFolder, key);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] encryptedContentBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(encryptedContentBytes, 0, (int)fileStream.Length).ConfigureAwait(false);
                    byte[] contentBytes = ProtectedData.Unprotect(encryptedContentBytes, Salt, DataProtectionScope.CurrentUser);

                    return Encoding.UTF8.GetString(contentBytes);
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionRead, key), e);
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                string filePath = Path.Combine(StoreFolder, key);
                File.Delete(filePath);
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionDelete, key), e);
            }
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(StoreFolder))
            {
                Directory.CreateDirectory(StoreFolder);
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            int charCount = hex.Length;
            byte[] bytes = new byte[charCount / 2];
            for (int i = 0; i < charCount; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}

