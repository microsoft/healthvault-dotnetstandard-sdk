// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.HealthVault.Helpers;

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

        // TODO: We need to decide how we want to pass these utilities around. There are so many static methods right now... We may need a static ambient context to access
        // user provided types such as these.
        private static ICertificateUtilities certificateUtilities;

        #region Constructor

        /// <summary>
        /// Creates an instance of ApplicationCertificate with the specified certificate
        /// </summary>
        ///
        private ApplicationCertificate(X509Certificate2 certificate)
        {
            this.Certificate = new X509Certificate2(certificate.Export(X509ContentType.Pfx));
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
            StoreLocation storeLocation = StoreLocation.CurrentUser)
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
                    // Use an existing cert, if any
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
            return DefaultCertSubjectPrefix + appId.ToString("D");
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
            return certificateUtilities.CreateCert(certificateName, numberOfYears, persist, storeLocation);
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
        public static void DeleteCertificate(Guid applicationId, StoreLocation storeLocation = StoreLocation.CurrentUser)
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
        ///
        /// <param name="storeLocation">
        /// The certificate store from which to remove the certificate.
        /// </param>
        [SecurityCritical]
        public static void DeleteCertificate(string certificateName, StoreLocation storeLocation = StoreLocation.CurrentUser)
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
            certificateUtilities.DeleteKeyContainer(certificateName);
        }

        #endregion
    }
}
