using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Microsoft.HealthVault.Client.Core;
using AndroidApp = Android.App;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// The Android implementation of ISecretStore
    /// </summary>
    internal class AndroidSecretStore : ISecretStore
    {
        private static string s_encryptionKeyName = "HealthVaultStoreKey";
        private static string s_encryptionFlavor = "AES";

        private IEncryptionKeyService _keyService;
        private Dictionary<string, AsyncLock> _fileLocks = new Dictionary<string, AsyncLock>();
        private AsyncLock _globalLock = new AsyncLock();
        private string _storePath;
        private byte[] _encryptionKey;

        public AndroidSecretStore(IEncryptionKeyService keyService)
        {
            _keyService = keyService;

            string storageDir;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                storageDir = AndroidApp.Application.Context.NoBackupFilesDir.AbsolutePath;
            }
            else
            {
                storageDir = AndroidApp.Application.Context.FilesDir.AbsolutePath;
            }

            _storePath = Path.Combine(new[] { storageDir, HealthVaultConstants.Storage.DirectoryName });
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                using (await LockAndInitialize(key).ConfigureAwait(false))
                {
                    File.Delete(GetFilePath(key));
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
                    // read file
                    string file = GetFilePath(key);
                    byte[] fileContents = File.ReadAllBytes(file);

                    // decrypt the file
                    SecretKeySpec keySpec = new SecretKeySpec(_encryptionKey, s_encryptionFlavor);
                    Cipher cipher = Cipher.GetInstance(s_encryptionFlavor);
                    cipher.Init(CipherMode.DecryptMode, keySpec);
                    byte[] decrypted = cipher.DoFinal(fileContents);

                    return Encoding.UTF8.GetString(decrypted);
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
                    // encrypt the contents
                    SecretKeySpec keySpec = new SecretKeySpec(_encryptionKey, s_encryptionFlavor);
                    Cipher cipher = Cipher.GetInstance(s_encryptionFlavor);
                    cipher.Init(CipherMode.EncryptMode, keySpec);
                    byte[] encrypted = cipher.DoFinal(Encoding.UTF8.GetBytes(contents));

                    // write file
                    string file = GetFilePath(key);
                    File.WriteAllBytes(file, encrypted);
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
                if (_encryptionKey == null)
                {
                    _encryptionKey = await _keyService.GetOrMakeEncryptionKeyAsync(s_encryptionKeyName).ConfigureAwait(false);

                    if (!Directory.Exists(_storePath))
                    {
                        Directory.CreateDirectory(_storePath);
                    }
                }

                if (!_fileLocks.TryGetValue(key, out fileLock))
                {
                    fileLock = new AsyncLock();
                    _fileLocks[key] = fileLock;
                }
            }

            return await fileLock.LockAsync().ConfigureAwait(false);
        }

        private string GetFilePath(string key)
        {
            return Path.Combine(_storePath, key);
        }
    }
}