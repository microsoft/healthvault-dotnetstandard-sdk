using System;
using System.IO;
using System.Threading.Tasks;
using Android.Provider;
using Java.Security;
using Javax.Crypto;
using AndroidApp = Android.App;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Implementation of IEncryptionKeyService
    /// </summary>
    internal class EncryptionKeyService : IEncryptionKeyService
    {
        private const string _keyStoreFileName = "HealthVault.Android.keystore";
        private string _keyStoreDirectory;
        private AsyncLock _keystoreLock = new AsyncLock();

        public EncryptionKeyService()
        {
            string storageDir;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                storageDir = AndroidApp.Application.Context.NoBackupFilesDir.AbsolutePath;
            }
            else
            {
                storageDir = AndroidApp.Application.Context.FilesDir.AbsolutePath;
            }

            _keyStoreDirectory = Path.Combine(storageDir, HealthVaultConstants.Storage.DirectoryName);
        }

        #region IEncryptionKeyService

        public async Task<byte[]> GetOrMakeEncryptionKeyAsync(string keyName)
        {
            using (await _keystoreLock.LockAsync().ConfigureAwait(false))
            {
                byte[] key = await GetEncryptionKeyLockedAsync(keyName).ConfigureAwait(false);
                if (key == null)
                {
                    key = await CreateAndStoreEncryptionKeyLockedAsync(keyName).ConfigureAwait(false);
                }

                return key;
            }
        }

        public async Task<byte[]> GetEncryptionKeyAsync(string keyName)
        {
            using (await _keystoreLock.LockAsync().ConfigureAwait(false))
            {
                return await GetEncryptionKeyLockedAsync(keyName).ConfigureAwait(false);
            }
        }

        public async Task<byte[]> CreateAndStoreEncryptionKeyAsync(string keyName)
        {
            using (await _keystoreLock.LockAsync().ConfigureAwait(false))
            {
                return await CreateAndStoreEncryptionKeyLockedAsync(keyName).ConfigureAwait(false);
            }
        }

        #endregion

        #region Private

        private async Task<byte[]> GetEncryptionKeyLockedAsync(string keyName)
        {
            byte[] key = null;

            try
            {
                char[] deviceId = GetDeviceId().ToCharArray();
                KeyStore keyStore = await GetOrCreateKeyStoreAsync(deviceId).ConfigureAwait(false);

                KeyStore.IProtectionParameter protectionParameter = new KeyStore.PasswordProtection(deviceId);
                KeyStore.SecretKeyEntry secretKeyEntry = (KeyStore.SecretKeyEntry)keyStore.GetEntry(keyName, protectionParameter);

                if (secretKeyEntry != null)
                {
                    ISecretKey secretKey = secretKeyEntry.SecretKey;
                    if (secretKey != null)
                    {
                        key = secretKey.GetEncoded();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // If the file isn't found, it's not a big deal and should mean it's just the first run.
                // The caller or the GetOrCreate method above will need to create it if we don't find it here.
            }

            return key;
        }

        private async Task<KeyStore> GetOrCreateKeyStoreAsync(char[] password)
        {
            KeyStore keyStore = KeyStore.GetInstance("BKS");
            try
            {
                using (Stream inputStream = await OpenKeyStoreFileForInputAsync().ConfigureAwait(false))
                {
                    keyStore.Load(inputStream, password);
                }
            }
            catch (Exception)
            {
                keyStore.Load(null);
            }

            return keyStore;
        }

        private async Task<byte[]> CreateAndStoreEncryptionKeyLockedAsync(string keyName)
        {
            byte[] keyBytes = null;

            KeyGenerator keyGenerator = KeyGenerator.GetInstance("AES");
            keyGenerator.Init(256, new SecureRandom());
            ISecretKey key = keyGenerator.GenerateKey();
            await StoreKeyAsync(keyName, key).ConfigureAwait(false);
            keyBytes = key.GetEncoded();

            return keyBytes;
        }

        private async Task StoreKeyAsync(string keyName, ISecretKey key)
        {
            char[] deviceId = GetDeviceId().ToCharArray();
            KeyStore keyStore = await GetOrCreateKeyStoreAsync(deviceId).ConfigureAwait(false);

            KeyStore.IProtectionParameter protectionParameter = new KeyStore.PasswordProtection(deviceId);

            // Create the SecretKeyEntry wrapper around the SecretKey
            KeyStore.SecretKeyEntry secretKeyEntry = new KeyStore.SecretKeyEntry(key);

            // Store the SecretKeyEntry in the KeyStore
            keyStore.SetEntry(keyName, secretKeyEntry, protectionParameter);

            // Write the KeyStore to the app's directory with appropriate permissions
            // Note: we're using the same key to protect the KeyStore as we are to protect the entry inside it.
            using (Stream fileStream = await OpenKeyStoreFileForOutputAsync().ConfigureAwait(false))
            {
                keyStore.Store(fileStream, deviceId);
            }
        }

        private Task<Stream> OpenKeyStoreFileForOutputAsync()
        {
            if (!Directory.Exists(_keyStoreDirectory))
            {
                Directory.CreateDirectory(_keyStoreDirectory);
            }

            return Task.FromResult((Stream)File.Create(Path.Combine(_keyStoreDirectory, _keyStoreFileName)));
        }

        private Task<Stream> OpenKeyStoreFileForInputAsync()
        {
            return Task.FromResult((Stream)File.OpenRead(Path.Combine(_keyStoreDirectory, _keyStoreFileName)));
        }

        private string GetDeviceId()
        {
            var contentResolver = AndroidApp.Application.Context.ContentResolver;
            string deviceId = Settings.Secure.GetString(contentResolver, Settings.Secure.AndroidId);
            return deviceId;
        }

        #endregion
    }
}