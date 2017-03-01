using System;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Service to provide cryptography
    /// </summary>
    internal interface ICryptoService
    {
        /// <summary>
        /// Generates an HMAC shared secret for the default HMAC algorithm.
        ///  </summary>
        /// 
        /// <returns>
        /// A byte array representing the HMAC shared secret.
        /// </returns>
        /// 
        byte[] GenerateHmacSharedSecret();

        /// <summary>
        /// Creates a new Hash Message Authentication Code (HMAC) instance 
        /// using the specified <paramref name="algorithmName"/> and s
        /// <paramref name="keyMaterial"/>.
        /// </summary>
        /// 
        /// <param name="algorithmName">
        /// The well-known algorithm name that specifies the HMAC primitive.
        /// </param>
        /// 
        /// <param name="keyMaterial">
        /// The provided key material to be used as the HMAC key.
        /// </param>
        /// 
        /// <returns>
        /// A new HMAC instance.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="algorithmName"/> parameter is <b>null</b> or empty.
        /// The <paramref name="algorithmName"/> parameter is not of type HMAC.
        /// The <paramref name="keyMaterial"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        HMAC CreateHmac(string algorithmName, byte[] keyMaterial);

        /// <summary>
        /// Creates a new Hash Message Authentication Code (HMAC) instance 
        /// based on the specified <paramref name="algorithmName"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// Since this method does not take user-specified keyMaterial,
        /// the caller must set the key after this call and before using the 
        /// HMAC algorithm.
        /// </remarks>
        /// 
        /// <param name="algorithmName">
        /// The well-known algorithm name which specifies the HMAC primitive.
        /// </param>
        /// 
        /// <returns>
        /// A new HMAC instance of type <paramref name="algorithmName"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="algorithmName"/> parameter is <b>null</b> or empty.
        /// The <paramref name="algorithmName"/> parameter is not of type HMAC.
        /// </exception>
        /// 
        HMAC CreateHmac(string algorithmName);

        /// <summary>
        /// Creates a new Hash Message Authentication Code (HMAC) based on 
        /// the current key.
        /// </summary>
        /// 
        /// <param name="keyMaterial">
        /// The provided key material to be used as the HMAC key.
        /// </param>
        /// 
        /// <returns>
        /// A new HMAC instance using <see cref="ICryptoConfiguration.HmacAlgorithmName"/>
        /// and the provided <paramref name="keyMaterial"/>.
        /// </returns>
        /// 
        /// <seealso cref="ICryptoConfiguration.HmacAlgorithmName"/>
        /// <exception cref="ArgumentException">
        /// The <paramref name="keyMaterial"/> parameter is <b>null</b> or empty.
        /// </exception>
        HMAC CreateHmac(byte[] keyMaterial);

        /// <summary>
        /// Creates a new hash algorithm based on the specified 
        /// <paramref name="algorithmName"/>.
        /// </summary>
        /// 
        /// <param name="algorithmName">
        /// The well-known algorithm name that specifies the Hash Message 
        /// Authentication Code (HMAC) primitive.
        /// </param>
        /// 
        /// <returns>
        /// A new hash algorithm of type <paramref name="algorithmName"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="algorithmName"/> parameter is not supported.
        /// </exception>
        HashAlgorithm CreateHashAlgorithm(string algorithmName);

        /// <summary>
        /// Creates a new hash algorithm with default values.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="HashAlgorithmName"/>.
        /// </returns>
        /// 
        /// <seealso cref="HashAlgorithmName"/>
        /// 
        HashAlgorithm CreateHashAlgorithm();

        /// <summary>
        /// Constructs a symmetric key algorithm based on the specified 
        /// <paramref name="algorithmName"/> and <paramref name="keyMaterial"/>.
        /// </summary>
        /// 
        /// <param name="algorithmName">
        /// The well-known algorithm name that specifies the symmetric 
        /// algorithm primitive.
        /// </param>
        /// 
        /// <param name="keyMaterial">
        /// The provided key material to be used as the Hash Message 
        /// Authentication Code (HMAC) key.
        /// </param>
        /// 
        /// <returns>
        /// A new symmetric key of type <paramref name="algorithmName"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="algorithmName"/> parameter is not supported,
        /// or the <paramref name="keyMaterial"/> parameter is invalid.
        /// </exception>
        /// 
        SymmetricAlgorithm CreateSymmetricAlgorithm(
            string algorithmName,
            byte[] keyMaterial);
    }
}
