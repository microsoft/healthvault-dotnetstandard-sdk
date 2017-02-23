namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Provides Crypto Configuration
    /// </summary>
    public interface ICryptoConfiguration
    {
        /// <summary>
        /// Gets the preferred application-wide Hash Message Authentication Code 
        /// (HMAC) algorithm name.
        /// </summary>
        /// 
        /// <remarks>
        /// The application-wide algorithm name may be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// This algorithm name can be used to construct an HMAC primitive 
        /// using <see cref="ICryptoService.CreateHmac(string)"/>
        /// </remarks>
        /// 
        /// <returns>
        /// The HMAC algorithm name.
        /// </returns>
        string HmacAlgorithmName { get; set; }

        /// <summary>
        /// Gets the preferred application-wide hash algorithm name.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the algorithm name.
        /// </returns>
        /// 
        /// <remarks>
        /// The application-wide algorithm name can be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// This algorithm name can be used to construct a hash primitive using
        /// <see cref="ICryptoService.CreateHashAlgorithm(string)"/>
        ///      .
        ///  </remarks>
        string HashAlgorithmName { get; set; }

        /// <summary>
        /// Gets the preferred application-wide hash algorithm name for 
        /// computing digests to be used for signature generation.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the algorithm name.
        /// </returns>
        /// 
        /// <remarks>
        /// The application-wide algorithm name can be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// This algorithm name can be used to construct a hash primitive using
        /// <see cref="ICryptoService.CreateHashAlgorithm(string)"/>.
        /// </remarks>
        /// 
        string SignatureHashAlgorithmName { get; set; }

        /// <summary>
        /// Gets the preferred application-wide signature algorithm name.
        /// </summary>
        /// 
        /// <remarks>
        /// The application-wide algorithm name can be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// The signature signing algorithm is currently RSA. The RSA algorithm
        /// name is prepended to the default 
        /// <seealso cref="SignatureHashAlgorithmName"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// A string representing the signature algorithm name.
        /// </returns>
        /// 
        string SignatureAlgorithmName { get; set; }

        /// <summary>
        /// Gets the preferred application-wide symmetric algorithm name.
        /// </summary>
        /// 
        /// <remarks>
        /// The application-wide algorithm name can be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// The symmetric algorithm name can be used to construct a
        /// symmetric algorithm via <see cref="ICryptoService.CreateSymmetricAlgorithm"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// A string representing the default symmetric algorithm name.
        /// </returns>
        /// 
        string SymmetricAlgorithmName { get; set; }
    }
}
