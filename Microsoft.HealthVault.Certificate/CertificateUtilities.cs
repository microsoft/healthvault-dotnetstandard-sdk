using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.HealthVault.Certificate
{
    internal class CertificateUtilities : ICertificateUtilities
    {

        #region ICertificateUtilities implementation

        [SuppressMessage(
          "Microsoft.Reliability",
          "CA2001:AvoidCallingProblematicMethods",
          Justification = "The call to DangerousGetHandle is acceptable here as handle is valid and used for creation of the certificate only. No reference is added.")]
        [SecurityCritical]
        public X509Certificate2 CreateCert(string certificateName, short numberOfYears, bool persist, StoreLocation storeLocation)
        {
            X509Certificate2 certificate = null;

            // convert the times to SystemTime structures
            NativeMethods.SystemTime beginTime = new NativeMethods.SystemTime(DateTime.Now);
            NativeMethods.SystemTime expireTime = new NativeMethods.SystemTime(DateTime.Now.AddYears(numberOfYears));

            // convert the name into a X500 name
            CertificateName certName = new CertificateName(ApplicationCertificate.MakeCertSubject(certificateName));

            using (KeyContainerHandle keyContainer = GenerateKeys(certificateName))
            {
                // create the certificate
                using (CryptoApiBlob nameBlob = certName.GetCryptoApiBlob())
                {
                    using (CertificateHandle nativeCert =
                        NativeMethods.CertCreateSelfSignCertificate(
                            keyContainer,
                            nameBlob,
                            NativeMethods.SelfSignFlags.None,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            ref beginTime,
                            ref expireTime,
                            IntPtr.Zero))
                    {
                        if (nativeCert.IsInvalid)
                        {
                            throw new CryptographicException(
                                String.Format(
                                    CultureInfo.InvariantCulture,
                                        ResourceRetriever.GetResourceString(
                                            "ApplicationCertificateUnableToCreateCert"),
                                        Util.GetLastErrorMessage()));
                        }
                        else
                        {
                            // okay to use DangerousGetHandle here as handle is valid and 
                            // used for creation of the certificate only. No reference is added
                            //
                            certificate = new X509Certificate2(nativeCert.DangerousGetHandle());
                        }

                        if (persist)
                        {
                            AddNativeCertToStore(nativeCert, storeLocation);
                        }
                    }
                }
            }

            return certificate;
        }

        /// <summary>
        /// Removes the key container for the specified certificate name.
        /// </summary>
        /// 
        /// <param name="certificateName">
        /// The certificate name which was used in creating
        /// the key container.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="certificateName"/> is <b>null</b>, empty, or all whitespace.
        /// </exception>
        [SecurityCritical]
        public void DeleteKeyContainer(string certificateName)
        {
            Validator.ThrowIfStringIsEmptyOrWhitespace(
                certificateName,
                "certificateName");

            KeyContainerHandle keyContainer = null;

            // Remove the key container...
            try
            {
                NativeMethods.CryptAcquireContext(
                    out keyContainer,
                        GetKeyContainerName(certificateName),
                        null,
                        NativeMethods.ProviderType.RsaFull,
                        NativeMethods.ContextFlags.DeleteKeySet);
            }
            finally
            {
                if (keyContainer != null)
                {
                    keyContainer.Dispose();
                }
            }
        }

        public byte[] GetEncodedName(string distinguishedName)
        {
            int encodingSize = 0;
            StringBuilder errorString = null;

            // first figure out how big of a buffer is needed
            NativeMethods.CertStrToName(
                NativeMethods.CertEncodingType.X509AsnEncoding | NativeMethods.CertEncodingType.PKCS7AsnEncoding,
                distinguishedName,
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
                    distinguishedName,
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

        #region private methods

        /// <summary>
        /// Adds the certificate to the store
        /// </summary>
        [SecurityCritical]
        private static void AddNativeCertToStore(
            CertificateHandle nativeCert,
            StoreLocation storeLocation)
        {
            IntPtr marshalStoreName = IntPtr.Zero;
            CertificateStoreHandle store = null;
            CertificateHandle addedCert = null;

            NativeMethods.CertSystemStoreFlags storeFlags =
                StoreLocationToCertSystemStoreFlags(storeLocation);

            try
            {
                marshalStoreName = Marshal.StringToHGlobalUni("My");

                store = NativeMethods.CertOpenStore(
                        new IntPtr(NativeMethods.CERT_STORE_PROV_SYSTEM),
                        0,
                        IntPtr.Zero,
                        (int)storeFlags,
                        marshalStoreName);

                // add the certificate to the store                           
                if (!NativeMethods.CertAddCertificateContextToStore(
                        store,
                        nativeCert,
                        NativeMethods.AddDisposition.ReplaceExisting,
                        out addedCert))
                {
                    throw new CryptographicException(String.Format(
                        CultureInfo.InvariantCulture,
                        ResourceRetriever.GetResourceString(
                            "ApplicationCertificateUnableToAddCertToStore"),
                            Util.GetLastErrorMessage()));
                }
            }
            finally
            {
                if (marshalStoreName != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(marshalStoreName);
                }
                if (store != null)
                {
                    store.Dispose();
                }
                if (addedCert != null)
                {
                    addedCert.Dispose();
                }
            }
        }

        private static NativeMethods.CertSystemStoreFlags StoreLocationToCertSystemStoreFlags(
            StoreLocation storeLocation)
        {
            NativeMethods.CertSystemStoreFlags result = NativeMethods.CertSystemStoreFlags.CurrentUser;

            switch (storeLocation)
            {
                case StoreLocation.CurrentUser:
                    result = NativeMethods.CertSystemStoreFlags.CurrentUser;
                    break;

                case StoreLocation.LocalMachine:
                    result = NativeMethods.CertSystemStoreFlags.LocalMachine;
                    break;
            }

            return result;
        }


        /// <summary>
        /// Generate a key pair to be used in the certificate
        /// </summary>
        [SecurityCritical]
        private static KeyContainerHandle GenerateKeys(string certificateName)
        {
            KeyContainerHandle keyContainer = null;
            KeyHandle key = null;

            try
            {
                // generate the key container to put the key in
                if (!NativeMethods.CryptAcquireContext(out keyContainer,
                                         GetKeyContainerName(certificateName),
                                         null,
                                         NativeMethods.ProviderType.RsaFull,
                                         NativeMethods.ContextFlags.NewKeySet | NativeMethods.ContextFlags.Silent))
                {
                    Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());

                    throw new CryptographicException(
                            ResourceRetriever.GetResourceString(
                                "ApplicationCertificateUnableToAcquireContext"),
                                win32Exception.Message);
                }

                // generate the key
                if (!NativeMethods.CryptGenKey(keyContainer,
                                NativeMethods.AlgorithmType.Signature,
                                NativeMethods.KeyFlags.Exportable,
                                out key))
                {
                    Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());

                    throw new CryptographicException(
                        ResourceRetriever.GetResourceString(
                            "ApplicationCertificateUnableToGenerateKey"),
                        win32Exception.Message);
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Dispose();
                    key = null;
                }
            }

            return keyContainer;
        }

        private static string GetKeyContainerName(string certificateName)
        {
            return "SelfSignedCert" + certificateName;
        }

        #endregion
    }
}
