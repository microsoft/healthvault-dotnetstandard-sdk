using System;
using System.Security.Cryptography;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Web
{
    internal class WebCryptographer : ICryptographer
    {
        public CryptoData Hmac(string keyMaterial, byte[] data)
        {
            byte[] key = Convert.FromBase64String(keyMaterial);

            using (var hmac = new HMACSHA256(key))
            {
                byte[] hash = hmac.ComputeHash(data);
                string value = Convert.ToBase64String(hash);

                return new CryptoData() { Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm, Value = value };
            }
        }

        public CryptoData Hash(byte[] data)
        {
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(data);
                string value = Convert.ToBase64String(hash);

                sha.Clear();

                return new CryptoData() { Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm, Value = value };
            }
        }
    }
}