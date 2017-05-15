using Microsoft.HealthVault.Client.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        private DataProtectionProvider protector;
        private DataProtectionProvider unprotector;
        private Dictionary<string, AsyncLock> fileLocks = new Dictionary<string, AsyncLock>();
        private AsyncLock globalLock = new AsyncLock();
        private StorageFolder folder;

        public async Task DeleteAsync(string key)
        {
            try
            {
                using (await this.LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await this.folder.GetFileAsync(key).AsTask().ConfigureAwait(false);
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
                using (await this.LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await this.folder.GetFileAsync(key).AsTask().ConfigureAwait(false);
                    var protectedBuffer = await FileIO.ReadBufferAsync(file).AsTask().ConfigureAwait(false);
                    return await this.Unprotect(protectedBuffer).ConfigureAwait(false);
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
                using (await this.LockAndInitialize(key).ConfigureAwait(false))
                {
                    var file = await this.folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                    var protectedBuffer = await this.Protect(contents).ConfigureAwait(false);
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
            using (await this.globalLock.LockAsync().ConfigureAwait(false))
            {
                if (this.folder == null)
                {
                    this.folder = await ApplicationData.Current.LocalCacheFolder
                        .CreateFolderAsync(HealthVaultConstants.Storage.DirectoryName, CreationCollisionOption.OpenIfExists)
                        .AsTask().ConfigureAwait(false);
                    this.protector = new DataProtectionProvider(LocalUserAuth);
                    this.unprotector = new DataProtectionProvider();
                }

                if (!this.fileLocks.TryGetValue(key, out fileLock))
                {
                    fileLock = new AsyncLock();
                    this.fileLocks.Add(key, fileLock);
                }
            }

            return await fileLock.LockAsync().ConfigureAwait(false);
        }

        private async Task<IBuffer> Protect(string input)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
            var protectedbuffer = await this.protector.ProtectAsync(buffer);
            return protectedbuffer;
        }

        private async Task<string> Unprotect(IBuffer input)
        {
            var unprotectedBuffer = await this.unprotector.UnprotectAsync(input).AsTask().ConfigureAwait(false);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, unprotectedBuffer);
        }
    }
}
