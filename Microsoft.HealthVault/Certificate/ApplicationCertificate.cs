// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.HealthVault.Certificate
{
    /// <summary>
    /// Generates a new HealthVault application certificate.
    /// </summary>
    /// 
    /// <remarks>
    /// This certificate is typically used by HealthVaultClientApplication.
    /// </remarks>
    /// 
    public class ApplicationCertificate
    {
        #region constants
        /// <summary>
        /// Client application certificates will be prefixed by HVClient
        /// </summary>
        private const string DefaultCertSubjectPrefix = "HVClientApp-";

        /// <summary>
        /// Default number of years for certificate validity
        /// </summary>
        private const short NumberOfYears = 31;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of ApplicationCertificate with the specified certificate
        /// </summary>
        /// 
        private ApplicationCertificate(X509Certificate2 certificate)
        {
            Certificate = new X509Certificate2(certificate.Export(X509ContentType.Pfx));
        }

        /// <summary>
        /// Generate an X509 certificate that works with the HealthVault SDK using the
        /// specified name.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="certificateName"/> is <b>null</b>, empty, or all whitespace.
        /// </exception>
        [SecurityCritical]
        public static ApplicationCertificate CreateCertificate(
            string certificateName)
        {
            Validator.ThrowIfStringNullOrEmpty(
                certificateName,
                "certificateName");

            Validator.ThrowIfStringIsWhitespace(
                certificateName,
                "certificateName");

            return CreateCertificate(certificateName, true, false, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Generate an X509 certificate that works with the HealthVault SDK using the
        /// specified name.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="certificateName"/> is <b>null</b>, empty, or all whitespace.
        /// </exception>
        [SecurityCritical]
        public static ApplicationCertificate CreateCertificate(
            string certificateName,
            StoreLocation storeLocation)
        {
            Validator.ThrowIfStringNullOrEmpty(
                certificateName,
                "certificateName");

            Validator.ThrowIfStringIsWhitespace(
                certificateName,
                "certificateName");

            return CreateCertificate(certificateName, true, false, storeLocation);
        }

        /// <summary>
        /// Generate or fetch a persisted certificate in the specified certificate store.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier of the application to create the certificate for.
        /// </param>
        /// 
        /// <returns>
        /// An ApplicationCertificate instance containing the certificate for the 
        /// specified application.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> is Guid.Empty.
        /// </exception>
        [SecurityCritical]
        public static ApplicationCertificate CreatePersistedCertificate(Guid applicationId)
        {
            return CreatePersistedCertificate(
                applicationId,
                StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Generate or fetch a persisted certificate in the specified certificate store.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier of the application to create the certificate for.
        /// </param>
        /// 
        /// <param name="storeLocation">
        /// The store location to fetch or create the certificate in.
        /// </param>
        /// 
        /// <returns>
        /// An ApplicationCertificate instance containing the certificate for the 
        /// specified application.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> is Guid.Empty.
        /// </exception>
        [SecurityCritical]
        public static ApplicationCertificate CreatePersistedCertificate(
            Guid applicationId,
            StoreLocation storeLocation)
        {
            Validator.ThrowArgumentExceptionIf(
                applicationId == Guid.Empty,
                "applicationId",
                "InvalidApplicationIdConfiguration");

            return CreateCertificate(
                MakeCertName(applicationId),
                false,
                true,
                storeLocation);
        }


        /// <summary>
        /// Generate or fetch a persisted certificate in the specified certificate store.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier of the application to create the certificate for.
        /// </param>
        /// 
        /// <param name="alwaysCreate">
        /// If true, a new certificate will be created even if it already exists in
        /// the specified certificate store.
        /// </param>
        /// 
        /// <param name="persist">
        /// If true, the certificate is persisted in the specified certificate store, otherwise
        /// the key container is deleted.
        /// </param>
        /// 
        /// <param name="storeLocation">
        /// The store location to fetch or create the certificate in.
        /// </param>
        /// 
        /// <returns>
        /// An ApplicationCertificate instance containing the certificate for the 
        /// specified application.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> is Guid.Empty.
        /// </exception>
        [SecurityCritical]
        public static ApplicationCertificate CreateCertificate(
            Guid applicationId,
            bool alwaysCreate,
            bool persist,
            StoreLocation storeLocation)
        {
            Validator.ThrowArgumentExceptionIf(
                applicationId == Guid.Empty,
                "applicationId",
                "InvalidApplicationIdConfiguration");

            return CreateCertificate(
                MakeCertName(applicationId),
                alwaysCreate,
                persist,
                storeLocation);
        }
        
        /// <summary>
        /// Generate or fetch a persisted certificate in the specified certificate store.
        /// </summary>
        /// 
        /// <param name="certificateName">
        /// The name to use when creating the certificate.
        /// </param>
        /// 
        /// <param name="alwaysCreate">
        /// If true a new certificate will be created even if it already exists in
        /// the specified certificate store.
        /// </param>
        /// 
        /// <param name="persist">
        /// If true, the certificate is persisted in the specified certificate store, otherwise
        /// the key container is deleted.
        /// </param>
        /// 
        /// <param name="storeLocation">
        /// The store location to fetch or create the certificate in.
        /// </param>
        /// 
        /// <returns>
        /// An ApplicationCertificate instance containing the certificate for the 
        /// specified application.
        /// </returns>
        /// 
        [SecurityCritical]
        public static ApplicationCertificate CreateCertificate(
            string certificateName,
            bool alwaysCreate,
            bool persist,
            StoreLocation storeLocation)
        {
            ApplicationCertificate result = null;
            string certificateSubject = MakeCertSubject(certificateName);

            using (CertificateStore store = new CertificateStore(storeLocation))
            {
                X509Certificate2 certificate = null;
                if (!alwaysCreate)
                {
                    //
                    // Use an existing cert, if any
                    //
                    certificate = store[certificateSubject];
                }

                if (certificate == null)
                {
                    certificate = CreateCert(certificateName, NumberOfYears, persist, storeLocation);
                }

                if (certificate != null)
                {
                    result = new ApplicationCertificate(certificate);

                    if (!persist)
                    {
                        DeleteKeyContainer(certificateName);
                    }
                }
            }

            return result;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// 
        public X509Certificate2 Certificate { get; private set; }

        #endregion

        #region static methods

        /// <summary>
        /// Make subject for certificate using the specified application identifier.
        /// </summary>
        /// 
        /// <param name="appId">
        /// The unique application identifier.
        /// </param>
        /// 
        /// <returns>
        /// A certificate subject name using the application identifier to ensure uniqueness.
        /// </returns>
        /// 
        internal static string MakeCertSubject(Guid appId)
        {
            return MakeCertSubject(MakeCertName(appId));
        }

        /// <summary>
        /// Make subject for certificate using the specified certificate name.
        /// </summary>
        /// 
        /// <param name="certificateName">
        /// The name of the certificate to make the subject for.
        /// </param>
        /// 
        /// <returns>
        /// A certificate subject name using the specified certificate name.
        /// </returns>
        /// 
        internal static string MakeCertSubject(string certificateName)
        {
            return "CN=" + certificateName;
        }

        /// <summary>
        /// Make a certificate name using the application identifier to ensure uniqueness.
        /// </summary>
        /// 
        /// <param name="appId">
        /// The unique application identifier.
        /// </param>
        /// 
        /// <returns>
        /// A certificate name using the application identifier to ensure uniqueness.
        /// </returns>
        ///
        internal static string MakeCertName(Guid appId)
        {
            return ApplicationCertificate.DefaultCertSubjectPrefix + appId.ToString("D");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a certificate 
        /// </summary>
        /// 
        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            Justification = "The call to DangerousGetHandle is acceptable here as handle is valid and used for creation of the certificate only. No reference is added.")]
        [SecurityCritical]
        private static X509Certificate2 CreateCert(
            string certificateName,
            short numberOfYears,
            bool persist,
            StoreLocation storeLocation)
        {
            X509Certificate2 certificate = null;

            // convert the times to SystemTime structures
            NativeMethods.SystemTime beginTime = new NativeMethods.SystemTime(DateTime.Now);
            NativeMethods.SystemTime expireTime = new NativeMethods.SystemTime(DateTime.Now.AddYears(numberOfYears));

            // convert the name into a X500 name
            CertificateName certName = new CertificateName(MakeCertSubject(certificateName));

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

        private static string GetKeyContainerName(string certificateName)
        {
            return "SelfSignedCert" + certificateName;
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

        /// <summary>
        /// Removes the certificate for the specified application identifier
        /// from the certificate store and deletes the key container.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier of the application for which to remove the certificate from the current
        /// user store.
        /// </param>
        [SecurityCritical]
        public static void DeleteCertificate(Guid applicationId)
        {
            DeleteCertificate(applicationId, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Removes the certificate for the specified application identifier
        /// from the certificate store and deletes the key container.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier of the application for which to remove the certificate from the 
        /// specified store.
        /// </param>
        /// 
        /// <param name="storeLocation">
        /// The certificate store from which to remove the certificate.
        /// </param>
        [SecurityCritical]
        public static void DeleteCertificate(Guid applicationId, StoreLocation storeLocation)
        {
            using (CertificateStore store = new CertificateStore(storeLocation))
            {
                store.RemoveCert(applicationId);
            }

            DeleteKeyContainer(applicationId);
        }

        /// <summary>
        /// Removes the certificate with the specified certificate name
        /// from the certificate store and deletes the key container.
        /// </summary>
        /// 
        /// <param name="certificateName">
        /// The name of the certificate to delete.
        /// </param>
        [SecurityCritical]
        public static void DeleteCertificate(string certificateName)
        {
            DeleteCertificate(certificateName, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Removes the certificate with the specified certificate name
        /// from the certificate store and deletes the key container.
        /// </summary>
        /// 
        /// <param name="certificateName">
        /// The name of the certificate to delete.
        /// </param>
        /// 
        /// <param name="storeLocation">
        /// The certificate store from which to remove the certificate.
        /// </param>
        [SecurityCritical]
        public static void DeleteCertificate(string certificateName, StoreLocation storeLocation)
        {
            using (CertificateStore store = new CertificateStore(storeLocation))
            {
                X509Certificate2 cert = store[MakeCertSubject(certificateName)];
                if (cert != null)
                {
                    store.RemoveCert(cert);
                }
            }

            DeleteKeyContainer(certificateName);
        }

        /// <summary>
        /// Removes the key container for the specified application identifier.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The unique identifier for the HealthVault application which was used in creating
        /// the key container.
        /// </param>
        [SecurityCritical]
        public static void DeleteKeyContainer(Guid applicationId)
        {
            DeleteKeyContainer(MakeCertName(applicationId));
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
        public static void DeleteKeyContainer(string certificateName)
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

        #endregion
    }
}
