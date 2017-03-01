using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// <inheritdoc cref="ICryptoService"/>
    /// </summary>
    internal class CryptoService : ICryptoService
    {
        private readonly ICryptoConfiguration cryptoConfiguration;

        /// <summary>
        /// Constructor for a base implementation of the Cryptography service 
        /// </summary>
        /// <param name="cryptoConfiguration">Returns a CryptoService with the user supplied parameters</param>
        public CryptoService(ICryptoConfiguration cryptoConfiguration)
        {
            Validator.ThrowIfArgumentNull(
                cryptoConfiguration,
                nameof(cryptoConfiguration), 
                "CryptoConfigurationNull");

            this.cryptoConfiguration = cryptoConfiguration;
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.GenerateHmacSharedSecret"/>
        /// </summary>
        public byte[] GenerateHmacSharedSecret()
        {
            byte[] keyMaterial = new byte[256];

            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(keyMaterial);
            }

            return keyMaterial;
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateHmac(string,byte[])"/>
        /// </summary>
        public HMAC CreateHmac(string algorithmName, byte[] keyMaterial)
        {
            Validator.ThrowArgumentExceptionIf(
                keyMaterial == null || keyMaterial.Length == 0,
                "keyMaterial",
                "CryptoKeyMaterialNullOrEmpty");

            this.VerifyKeyMaterialNotEmpty(keyMaterial);

            HMAC hmac = null;

            try
            {
                hmac = this.CreateHmac(algorithmName);
                hmac.Key = keyMaterial;
                return hmac;
            }
            catch
            {
                hmac?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateHmac(string)"/>
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'hmac' variable is returned to the caller")]
        public HMAC CreateHmac(string algorithmName)
        {
            Validator.ThrowIfStringNullOrEmpty(algorithmName, "algorithmName");

            HMAC hmac = this.CreateHashAlgorithm(algorithmName) as HMAC;

            Validator.ThrowArgumentExceptionIf(
                hmac == null,
                "algorithmName",
                "CryptoConfigNotHMACAlgorithmName");

            return hmac;
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateHmac(byte[])"/>
        /// </summary>
        public HMAC CreateHmac(byte[] keyMaterial)
        {
            Validator.ThrowArgumentExceptionIf(
                keyMaterial == null || keyMaterial.Length == 0,
                "keyMaterial",
                "CryptoConfigEmptyKeyMaterial");

            return this.CreateHmac(this.cryptoConfiguration.HmacAlgorithmName, keyMaterial);
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateHmac(string)"/>
        /// </summary>
        [SuppressMessage("Microsoft.Security.Cryptography", "CA5354:SHA1CannotBeUsed", Justification = "Required for backward compatibility.")]
        public HashAlgorithm CreateHashAlgorithm(string algorithmName)
        {
            HashAlgorithm hash;

            switch (algorithmName)
            {
                case "SHA1":
                    hash = SHA1.Create();
                    break;
                case "SHA256":
                    hash = SHA256.Create();
                    break;
                case "HMACSHA1":
                    hash = new HMACSHA1();
                    break;
                case "HMACSHA256":
                    hash = new HMACSHA256();
                    break;
                case "HMACSHA512":
                    hash = new HMACSHA512();
                    break;

                default:
                    throw Validator.ArgumentException(
                            "algorithmName",
                            "CryptoConfigUnsupportedAlgorithmName");
            }

            return hash;
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateHashAlgorithm()"/>
        /// </summary>
        public HashAlgorithm CreateHashAlgorithm()
        {
            return this.CreateHashAlgorithm(this.cryptoConfiguration.HashAlgorithmName);
        }

        /// <summary>
        /// <inheritdoc cref="ICryptoService.CreateSymmetricAlgorithm(string, byte[])"/>
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'key' variable is returned to the caller")]
        public SymmetricAlgorithm CreateSymmetricAlgorithm(string algorithmName, byte[] keyMaterial)
        {
            this.VerifyKeyMaterialNotEmpty(keyMaterial);

            // Note, previous SDK used RijndaelManaged key = new RijndaelManaged();
            // and is being repalced by AES
            var key = Aes.Create();

            switch (algorithmName)
            {
                case "AES128":
                    if (keyMaterial.Length * 8 != 128)
                    {
                        throw Validator.ArgumentException("keyMaterial", "CryptoConfigInvalidKeyMaterial");
                    }

                    key.KeySize = 128;
                    key.Key = keyMaterial;
                    break;

                case "AES192":
                    if (keyMaterial.Length * 8 != 192)
                    {
                        throw Validator.ArgumentException("keyMaterial", "CryptoConfigInvalidKeyMaterial");
                    }

                    key.KeySize = 192;
                    key.Key = keyMaterial;
                    break;

                case "AES256":
                    if (keyMaterial.Length * 8 != 256)
                    {
                        throw Validator.ArgumentException("keyMaterial", "CryptoConfigInvalidKeyMaterial");
                    }

                    key.KeySize = 256;
                    key.Key = keyMaterial;
                    break;

                default:
                    throw Validator.ArgumentException(
                            "algorithmName",
                            "CryptoConfigUnsupportedAlgorithmName");
            }

            return key;
        }

        private void VerifyKeyMaterialNotEmpty(byte[] keyMaterial)
        {
            if (keyMaterial.Any(t => t != 0))
            {
                return;
            }

            throw Validator.ArgumentException("keyMaterial", "CryptoKeyMaterialAllZeros");
        }
    }
}
