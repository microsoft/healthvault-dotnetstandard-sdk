// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Provides access to both application-level settings and
    /// methods to both specify and construct cryptographic primitives.
    /// </summary>
    ///
    public static class CryptoConfiguration
    {
        #region public crypto config settings

        /// <summary>
        /// Gets the preferred application-wide Hash Message Authentication Code
        /// (HMAC) algorithm name.
        /// </summary>
        ///
        /// <remarks>
        /// The application-wide algorithm name may be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// This algorithm name can be used to construct an HMAC primitive
        /// using <see cref="CreateHmac(string)"/>.
        /// </remarks>
        ///
        /// <returns>
        /// The HMAC algorithm name.
        /// </returns>
        ///
        public static string HmacAlgorithmName
        {
            get { return _hmacAlgorithmName; }
        }
        private static string _hmacAlgorithmName =
            HealthApplicationConfiguration.Current.HmacAlgorithmName;

        /// <summary>
        /// Generates an HMAC shared secret for the default HMAC algorithm.
        ///  </summary>
        ///
        /// <returns>
        /// A byte array representing the HMAC shared secret.
        /// </returns>
        ///
        public static byte[] GenerateHmacSharedSecret()
        {
            byte[] keyMaterial = new byte[256];
            CryptoUtil.GetRandomBytes(keyMaterial);
            return keyMaterial;
        }

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
        /// <see cref="CreateHashAlgorithm(string)"/>.
        /// </remarks>
        ///
        public static string HashAlgorithmName
        {
            get { return _hashAlgorithmName; }
        }
        private static string _hashAlgorithmName =
            HealthApplicationConfiguration.Current.HashAlgorithmName;

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
        /// <see cref="CreateHashAlgorithm(string)"/>.
        /// </remarks>
        ///
        public static string SignatureHashAlgorithmName
        {
            get
            {
                return HealthApplicationConfiguration.Current.SignatureHashAlgorithmName;
            }
        }

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
        public static string SignatureAlgorithmName
        {
            get
            {
                return HealthApplicationConfiguration.Current.SignatureAlgorithmName;
            }
        }

        /// <summary>
        /// Gets the preferred application-wide symmetric algorithm name.
        /// </summary>
        ///
        /// <remarks>
        /// The application-wide algorithm name can be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// The symmetric algorithm name can be used to construct a
        /// symmetric algorithm via <see cref="CreateSymmetricAlgorithm"/>.
        /// </remarks>
        ///
        /// <returns>
        /// A string representing the default symmetric algorithm name.
        /// </returns>
        ///
        public static string SymmetricAlgorithmName
        {
            get { return _symmetricAlgorithmName; }
        }
        private static string _symmetricAlgorithmName =
            HealthApplicationConfiguration.Current.SymmetricAlgorithmName;

        #endregion

        #region alg creation helpers

        /// <summary>
        /// Creates a new Hash Message Authentication Code (HMAC) instance
        /// using the specified <paramref name="algorithmName"/> and
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
        public static HMAC CreateHmac(
            string algorithmName,
            byte[] keyMaterial)
        {
            Validator.ThrowArgumentExceptionIf(
                keyMaterial == null || keyMaterial.Length == 0,
                "keyMaterial",
                "CryptoKeyMaterialNullOrEmpty");

            VerifyKeyMaterialNotEmpty(keyMaterial);

            HMAC Hmac = null;

            try
            {
                Hmac = CreateHmac(algorithmName);
                Hmac.Key = keyMaterial;

                return Hmac;
            }
            catch
            {
                if (Hmac != null)
                {
                    Hmac.Dispose();
                }

                throw;
            }
        }

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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'hmac' variable is returned to the caller")]
        public static HMAC CreateHmac(string algorithmName)
        {
            Validator.ThrowIfStringNullOrEmpty(algorithmName, "algorithmName");

            HMAC hmac = CreateHashAlgorithm(algorithmName) as HMAC;

            Validator.ThrowArgumentExceptionIf(
                hmac == null,
                "algorithmName",
                "CryptoConfigNotHMACAlgorithmName");

            return hmac;
        }

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
        /// A new HMAC instance using <see cref="HmacAlgorithmName"/>
        /// and the provided <paramref name="keyMaterial"/>.
        /// </returns>
        ///
        /// <seealso cref="HmacAlgorithmName"/>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="keyMaterial"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public static HMAC CreateHmac(byte[] keyMaterial)
        {
            Validator.ThrowArgumentExceptionIf(
                keyMaterial == null || keyMaterial.Length == 0,
                "keyMaterial",
                "CryptoConfigEmptyKeyMaterial");

            return CreateHmac(HmacAlgorithmName, keyMaterial);
        }

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
        [SuppressMessage("Microsoft.Security.Cryptography", "CA5354:SHA1CannotBeUsed", Justification = "Required for backward compatibility.")]
        public static HashAlgorithm CreateHashAlgorithm(string algorithmName)
        {
            HashAlgorithm hash = null;

            switch (algorithmName)
            {
                case "SHA1":
                    hash = new SHA1CryptoServiceProvider();
                    break;
                case "SHA256":
                    try
                    {
                        hash = new Sha256();
                    }
                    catch (CryptographicException)
                    {
                        hash = new SHA256Managed();
                    }
                    break;
                case "HMACSHA1":
                    hash = new HMACSHA1();
                    break;
                case "HMACSHA256":
                    try
                    {
                        hash = new Sha256Hmac();
                    }
                    catch (CryptographicException)
                    {
                        hash = new HMACSHA256();
                    }
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
        /// Creates a new hash algorithm with default values.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="HashAlgorithmName"/>.
        /// </returns>
        ///
        /// <seealso cref="HashAlgorithmName"/>
        ///
        public static HashAlgorithm CreateHashAlgorithm()
        {
            return CreateHashAlgorithm(HashAlgorithmName);
        }

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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'key' variable is returned to the caller")]
        public static SymmetricAlgorithm CreateSymmetricAlgorithm(
            string algorithmName,
            byte[] keyMaterial)
        {
            VerifyKeyMaterialNotEmpty(keyMaterial);

            RijndaelManaged key = new RijndaelManaged();

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

        /// <summary>
        /// Verifies that the provided key material is not empty or have a value of
        /// zero.
        /// </summary>
        ///
        /// <exception name="ArgumentException">
        /// The <paramref name="keyMaterial"/> parameter is empty or has a
        /// value of zero.
        /// </exception>
        ///
        private static void VerifyKeyMaterialNotEmpty(byte[] keyMaterial)
        {
            for (int i = 0; i < keyMaterial.Length; i++)
            {
                if (keyMaterial[i] != 0)
                {
                    return;
                }
            }

            throw Validator.ArgumentException("keyMaterial", "CryptoKeyMaterialAllZeros");
        }

        private const String _defaultCryptoServiceProviderName =
            "Microsoft Enhanced RSA and AES Cryptographic Provider";

        private const String _xpCryptoServiceProviderName =
            "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";

        /// <summary>
        /// Gets the name of the crypto service provider for the currently
        /// running operating system version.
        ///
        /// </summary>
        /// <returns></returns>
        public static String CryptoServiceProviderName
        {
            get
            {
                // Windows versioning:
                //
                // Name                     Major   Minor
                //
                // Windows 7                6       1
                // Windows Server 2008 R2   6       1
                // Windows Server 2008      6       0
                // Windows Vista            6       0
                // Windows Server 2003 R2   5       2
                // Windows Server 2003      5       2
                // Windows XP               5       1
                // Windows 2000             5       0

                String providerName = null;
                OperatingSystem os = System.Environment.OSVersion;

                if (os.Version.Major < 5 || os.Platform != PlatformID.Win32NT)
                {
                    throw Validator.InvalidOperationException("CryptoConfigOSNotSupported");
                }

                if (os.Version.Major == 5)
                {
                    if (os.Version.Minor == 0)
                    {
                        // Windows 2000
                        throw Validator.InvalidOperationException("OSNotSupported");
                    }
                    else if (os.Version.Minor == 1)
                    {
                        // Windows XP
                        providerName = _xpCryptoServiceProviderName;
                    }
                    else
                    {
                        // Windows Server 2003 and later
                        providerName = _defaultCryptoServiceProviderName;
                    }
                }
                else
                {
                    // Windows Vista and later
                    providerName = _defaultCryptoServiceProviderName;
                }

                return providerName;
            }
        }

        #endregion
    }
}
