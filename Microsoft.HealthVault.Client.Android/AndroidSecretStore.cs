using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Javax.Crypto;
using Javax.Crypto.Spec;
using AndroidApp = Android.App;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// The Android implementation of ISecretStore
    /// </summary>
    internal class AndroidSecretStore : ISecretStore
    {
        private static string encryptionKeyName = "HealthVaultStoreKey";
        private static string encryptionFlavor = "AES";

        private IEncryptionKeyService keyService;
        private Dictionary<string, AsyncLock> fileLocks = new Dictionary<string, AsyncLock>();
        private AsyncLock globalLock = new AsyncLock();
        private string storePath;
        private byte[] encryptionKey;

        public AndroidSecretStore(IEncryptionKeyService keyService)
        {
            this.keyService = keyService;

            string storageDir;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                storageDir = AndroidApp.Application.Context.NoBackupFilesDir.AbsolutePath;
            }
            else
            {
                storageDir = AndroidApp.Application.Context.FilesDir.AbsolutePath;
            }

            this.storePath = Path.Combine(new[] { storageDir, HealthVaultConstants.Storage.DirectoryName });
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                using (await this.LockAndInitialize(key).ConfigureAwait(false))
                {
                    File.Delete(this.GetFilePath(key));
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
                    // read file
                    string file = this.GetFilePath(key);
                    byte[] fileContents = File.ReadAllBytes(file);

                    // decrypt the file
                    SecretKeySpec keySpec = new SecretKeySpec(this.encryptionKey, encryptionFlavor);
                    Cipher cipher = Cipher.GetInstance(encryptionFlavor);
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
                using (await this.LockAndInitialize(key).ConfigureAwait(false))
                {
                    // encrypt the contents
                    SecretKeySpec keySpec = new SecretKeySpec(this.encryptionKey, encryptionFlavor);
                    Cipher cipher = Cipher.GetInstance(encryptionFlavor);
                    cipher.Init(CipherMode.EncryptMode, keySpec);
                    byte[] encrypted = cipher.DoFinal(Encoding.UTF8.GetBytes(contents));

                    // write file
                    string file = this.GetFilePath(key);
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
            using (await this.globalLock.LockAsync().ConfigureAwait(false))
            {
                if (this.encryptionKey == null)
                {
                    this.encryptionKey = await this.keyService.GetOrMakeEncryptionKeyAsync(encryptionKeyName).ConfigureAwait(false);

                    if (!Directory.Exists(storePath))
                    {
                        Directory.CreateDirectory(storePath);
                    }
                }

                if (!this.fileLocks.TryGetValue(key, out fileLock))
                {
                    fileLock = new AsyncLock();
                    this.fileLocks[key] = fileLock;
                }
            }

            return await fileLock.LockAsync().ConfigureAwait(false);
        }

        private string GetFilePath(string key)
        {
            return Path.Combine(storePath, key);
        }
    }
}