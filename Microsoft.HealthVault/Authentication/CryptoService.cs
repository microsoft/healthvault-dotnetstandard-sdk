using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// <inheritdoc cref="ICryptoService"/>
    /// </summary>
    public class CryptoService : ICryptoService
    {
        public byte[] GenerateHmacSharedSecret()
        {
            byte[] keyMaterial = new byte[256];

            using (RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider())
            {
                Gen.GetBytes(buffer);
            }

            return keyMaterial;
        }

        public HMAC CreateHmac(string algorithmName, byte[] keyMaterial)
        {
            throw new System.NotImplementedException();
        }

        public HMAC CreateHmac(string algorithmName)
        {
            throw new System.NotImplementedException();
        }

        public HMAC CreateHmac(byte[] keyMaterial)
        {
            throw new System.NotImplementedException();
        }

        public HashAlgorithm CreateHashAlgorithm(string algorithmName)
        {
            throw new System.NotImplementedException();
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            throw new System.NotImplementedException();
        }

        public SymmetricAlgorithm CreateSymmetricAlgorithm(string algorithmName, byte[] keyMaterial)
        {
            throw new System.NotImplementedException();
        }
    }
}
