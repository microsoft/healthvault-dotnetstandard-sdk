using Microsoft.HealthVault.Connection;
using System;
using System.Text;
using System.Security.Cryptography;

namespace Microsoft.HealthVault
{
    internal class Cryptographer : ICryptographer
    {
        public CryptoData Hash(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);

                CryptoData cryptoData = new CryptoData
                {
                    Algorithm = HealthVaultConstants.Cryptography.HashAlgorithm,
                    Value = Convert.ToBase64String(hash)
                };

                return cryptoData;
            }
        }

        public CryptoData Hmac(string keyMaterial, byte[] data)
        {
            using (HMAC hmac = new HMACSHA256(Convert.FromBase64String(keyMaterial)))
            {
                byte[] hash = hmac.ComputeHash(data);

                CryptoData cryptoData = new CryptoData
                {
                    Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm,
                    Value = Convert.ToBase64String(hash)
                };

                return cryptoData;
            }
        }
    }
}
