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
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Core;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Windows implementation of the <see cref="ISecretStore"/> interface that encrypts data
    /// to the local user and saves to a file in the local cache folder.
    /// </summary>
    internal class UwpSecretStore : ISecretStore
    {
        private const string LocalUserAuth = "LOCAL=user";

        private DataProtectionProvider _protector;
        private DataProtectionProvider _unprotector;
        private Dictionary<string, AsyncLock> _fileLocks = new Dictionary<string, AsyncLock>();
        private AsyncLock _globalLock = new AsyncLock();
        private StorageFolder _folder;

        public async Task DeleteAsync(string key)
        {
            try
            {
                using (await LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await _folder.GetFileAsync(key).AsTask().ConfigureAwait(false);
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionDelete, key), e);
            }
        }

        public async Task<string> ReadAsync(string key)
        {
            try
            {
                using (await LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await _folder.GetFileAsync(key).AsTask().ConfigureAwait(false);
                    var protectedBuffer = await FileIO.ReadBufferAsync(file).AsTask().ConfigureAwait(false);
                    return await Unprotect(protectedBuffer).ConfigureAwait(false);
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionRead, key), e);
            }
        }

        public async Task WriteAsync(string key, string contents)
        {
            try
            {
                using (await LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await _folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                    var protectedBuffer = await Protect(contents).ConfigureAwait(false);
                    await FileIO.WriteBufferAsync(file, protectedBuffer).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception e) when (!(e is IOException))
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionWrite, key), e);
            }
        }

        private async Task<IDisposable> LockAndInitialize(string key)
        {
            AsyncLock fileLock;
            using (await _globalLock.LockAsync().ConfigureAwait(false))
            {
                if (_folder == null)
                {
                    _folder = await ApplicationData.Current.LocalCacheFolder
                        .CreateFolderAsync(HealthVaultConstants.Storage.DirectoryName, CreationCollisionOption.OpenIfExists)
                        .AsTask().ConfigureAwait(false);
                    _protector = new DataProtectionProvider(LocalUserAuth);
                    _unprotector = new DataProtectionProvider();
                }

                if (!_fileLocks.TryGetValue(key, out fileLock))
                {
                    fileLock = new AsyncLock();
                    _fileLocks.Add(key, fileLock);
                }
            }

            return await fileLock.LockAsync().ConfigureAwait(false);
        }

        private async Task<IBuffer> Protect(string input)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
            var protectedbuffer = await _protector.ProtectAsync(buffer);
            return protectedbuffer;
        }

        private async Task<string> Unprotect(IBuffer input)
        {
            var unprotectedBuffer = await _unprotector.UnprotectAsync(input).AsTask().ConfigureAwait(false);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, unprotectedBuffer);
        }
    }
}
