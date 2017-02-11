// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

using Microsoft.Health.Authentication;

namespace Microsoft.Health.Web.Authentication
{
    /// <summary>
    /// A keyset used to create authenticated sessions with the 
    /// HealthVault Service.
    /// </summary>
    /// 
    internal sealed class AuthenticatedSessionKeySet
    {
        #region properties

        internal CryptoHmac HMAC
        {
            get { return _hmac; }
        }
        private CryptoHmac _hmac;

        private string HmacAlgorithmName
        {
            get { return _hmacAlgorithmName; }
            set { _hmacAlgorithmName = value; }
        }
        private string _hmacAlgorithmName;

        private byte[] HmacKeyMaterial
        {
            get { return _hmac.KeyMaterial; }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="AuthenticatedSessionKeySet"/> 
        /// class using the supplied Hash Message Authentication Code (HMAC) 
        /// key material.
        /// </summary>
        /// 
        /// <param name="hmacAlgorithmName">
        /// The well-known algorithm name for the HMAC primitive to be used to 
        /// ensure envelope integrity and authentication.
        /// </param>
        /// 
        /// <param name="hmacKeyMaterial">
        /// The key material that will be used to construct the HMAC primitive.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="hmacAlgorithmName"/> or 
        /// <paramref name="hmacKeyMaterial"/> parameter is <b>null</b> or empty,
        /// or the key material bytes are all zeros.
        /// </exception>
        /// 
        internal AuthenticatedSessionKeySet(
            string hmacAlgorithmName,
            byte[] hmacKeyMaterial)
        {
            HmacAlgorithmName = hmacAlgorithmName;

            _hmac = new CryptoHmac(HmacAlgorithmName, hmacKeyMaterial);
        }

        #endregion

        #region methods

        internal AuthenticatedSessionKeySet Clone()
        {
            return
                new AuthenticatedSessionKeySet(
                    this.HmacAlgorithmName,
                    this.HmacKeyMaterial);
        }

        #endregion
    }
}


