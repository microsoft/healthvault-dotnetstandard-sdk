// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.HealthVault.Certificate
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
        #region private variables
        private string _distinguishedName = null;
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
            this._distinguishedName = distinguishedName;
            return;
        }

        /// <summary>
        /// Gets a CryptoApiBlob for this name.
        /// </summary>
        [SecurityCritical]
        internal CryptoApiBlob GetCryptoApiBlob()
        {
            byte[] encodedName = GetEncodedName();
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
            Debug.Assert(!String.IsNullOrEmpty(_distinguishedName));

            int encodingSize = 0;
            StringBuilder errorString = null;

            // first figure out how big of a buffer is needed
            NativeMethods.CertStrToName(
                NativeMethods.CertEncodingType.X509AsnEncoding | NativeMethods.CertEncodingType.PKCS7AsnEncoding,
                DistinguishedName,
                NativeMethods.StringType.OIDNameString | NativeMethods.StringType.ReverseFlag,
                IntPtr.Zero,
                null,
                ref encodingSize,
                ref errorString);

            // allocate the buffer, and then do the conversion
            byte[] encodedBytes = new byte[encodingSize];
            bool ok =
                NativeMethods.CertStrToName(
                    NativeMethods.CertEncodingType.X509AsnEncoding | NativeMethods.CertEncodingType.PKCS7AsnEncoding,
                    DistinguishedName,
                    NativeMethods.StringType.OIDNameString | NativeMethods.StringType.ReverseFlag,
                    IntPtr.Zero,
                    encodedBytes,
                    ref encodingSize,
                    ref errorString);

            // if the conversion failed, throw an exception
            if (!ok)
            {
                string lastError = Util.GetLastErrorMessage();
                throw new CryptographicException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        ResourceRetriever.GetResourceString(
                            "CertificateNameConversionFailed"),
                            lastError,
                            errorString));
            }

            return encodedBytes;
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
                Debug.Assert(!String.IsNullOrEmpty(_distinguishedName));
                return _distinguishedName;
            }
        }
        #endregion

    }
}
