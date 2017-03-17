// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Web.Certificate
{
    /// <summary>
    /// Wrapper around a disntinguished name for a certificate.
    /// </summary>
    ///
    /// <remarks>
    /// Class is seperate from X500DistinguishedName since that
    /// class does not allow public access to the encoded name.
    /// </remarks>
    internal sealed class CertificateName
    {
        private static ICertificateUtilities certificateUtilities;

        #region private variables

        private readonly string distinguishedName;

        #endregion

        #region public methods

        /// <summary>
        /// Creates a name for a given distinguished name.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="distinguishedName"/> is null or empty
        /// </exception>
        internal CertificateName(string distinguishedName)
        {
            Validator.ThrowIfStringNullOrEmpty(distinguishedName, "distinguishedName");
            this.distinguishedName = distinguishedName;
        }

        /// <summary>
        /// Gets a CryptoApiBlob for this name.
        /// </summary>
        [SecurityCritical]
        internal CryptoApiBlob GetCryptoApiBlob()
        {
            byte[] encodedName = this.GetEncodedName();
            return new CryptoApiBlob(encodedName);
        }

        /// <summary>
        /// Converts the distinguished name into a cert blob.
        /// </summary>
        /// <exception cref="CryptographicException">If the conversion cannot be performed</exception>
        /// <returns>encoded form of the name</returns>
        [SecuritySafeCritical]
        private byte[] GetEncodedName()
        {
            Debug.Assert(!string.IsNullOrEmpty(this.distinguishedName), "Cannot get encoded name without distinguished name");
            return certificateUtilities.GetEncodedName(this.distinguishedName);
        }

        #endregion

        #region public properties
        
        /// <summary>
        /// Gets the distinguished name of the certificate.
        /// </summary>
        public string DistinguishedName
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(this.distinguishedName), "Distinguished name is null or empty");
                return this.distinguishedName;
            }
        }
        #endregion
    }
}
