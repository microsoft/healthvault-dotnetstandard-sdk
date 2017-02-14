// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.HealthVault.Certificate
{

    /// <summary>
    /// Wrapper around X509 Store for HealthVault.
    /// </summary>
    internal class CertificateStore : IDisposable
    {
        X509Store m_store;

        /// <summary>
        /// Creates an instance of CertificateStore.
        /// </summary>
        /// 
        /// <exception ref="CryptographicException">
        /// The store is unreadable.
        /// </exception>
        ///
        /// <exception ref="SecurityException">
        /// The caller does not have the required permissions.
        /// </exception>
        ///
        internal CertificateStore()
            : this(StoreLocation.CurrentUser)
        {
        }

        /// <summary>
        /// Creates an instance of CertificateStore.
        /// </summary>
        /// 
        /// <param name="storeType">Specifies store type.</param>
        /// 
        /// <exception ref="CryptographicException">
        /// The store is unreadable.
        /// </exception>
        ///
        /// <exception ref="SecurityException">
        /// The caller does not have the required permissions.
        /// </exception>
        ///
        internal CertificateStore(StoreLocation storeType)
        {
            this.m_store = new X509Store(storeType);
            this.m_store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Gets certificate by looking up subject name in the store.
        /// </summary>
        /// 
        /// <param name="subjectName">
        /// Subject name of the certificate to lookup in the certificate store.
        /// </param>
        /// 
        /// <returns>The certificate associated with the subject name.</returns>
        internal X509Certificate2 this[string subjectName]
        {
            get
            {
                return this.FindBySubject(subjectName);
            }
        }

        /// <summary>
        /// Gets the X509Certificate asociated with the appID.
        /// </summary>
        /// 
        /// <param name="appID">
        /// Id of the application to lookup the certificate.
        /// </param>
        /// 
        /// <returns>
        /// The certificate associated with the appID.
        /// </returns>
        /// 
        internal X509Certificate2 this[Guid appID]
        {
            get
            {
                return this.FindBySubject(ApplicationCertificate.MakeCertSubject(appID));
            }
        }

        /// <summary>
        /// Adds the certificate to the store.
        /// </summary>
        /// 
        /// <param name="cert">
        /// The certificate to be added to the store.
        /// </param>
        internal void Add(X509Certificate2 cert)
        {
            this.m_store.Add(cert);
        }

        /// <summary>
        /// Removes certificate from the store.
        /// </summary>
        /// 
        /// <param name="appID">
        /// Id for the application for which the certificate is to be removed.
        /// </param>
        internal void RemoveCert(Guid appID)
        {
            X509Certificate2 cert = this[appID];
            if (cert != null)
            {
                this.RemoveCert(cert);
            }
        }

        /// <summary>
        /// Removes certificate from store.
        /// </summary>
        /// 
        /// <param name="cert">
        /// Certificate that is to be removed.
        /// </param>
        internal void RemoveCert(X509Certificate2 cert)
        {
            if (cert == null)
            {
                throw new ArgumentNullException("cert");
            }

            if (this.Contains(cert.Subject))
            {
                this.m_store.Remove(cert);
            }
        }

        /// <summary>
        /// Checks if the certificate associated with appID exists in store.
        /// </summary>
        /// 
        /// <param name="appID">
        /// Id for the application.
        /// </param>
        /// 
        /// <returns>
        /// Boolean value indicating whether certificate for appID exists in store
        /// </returns>
        internal bool Contains(Guid appID)
        {
            return (this[appID] != null);
        }

        /// <summary>
        /// Checks if the certificate by subjectName exists in store.
        /// </summary>
        /// 
        /// <param name="subjectName">
        /// Subject name of the certificate.
        /// </param>
        /// 
        /// <returns>
        /// Boolean value indicating whether certificate exists in store
        /// </returns>
        /// 
        /// <exception cref="ArgumentException"> 
        /// subjectName cannot be empty or null 
        /// </exception>
        /// 
        internal bool Contains(string subjectName)
        {
            Validator.ThrowIfStringNullOrEmpty(subjectName, "subjectName");
            return (this.FindBySubject(subjectName) != null);
        }

        /// <summary>
        /// Finds certificate by SubjectName.
        /// </summary>
        /// 
        /// <param name="subjectName">
        /// Subject name of certificate to be looked up.
        /// </param>
        /// 
        /// <returns>
        /// Returns the certificate associated with the subject name.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException"> 
        /// subjectName cannot be empty or null 
        /// </exception>
        /// 
        internal X509Certificate2 FindBySubject(string subjectName)
        {
            Validator.ThrowIfStringNullOrEmpty(subjectName, "subjectName");

            X509Certificate2Collection certs = this.m_store.Certificates;
            for (int i = 0; i < certs.Count; ++i)
            {
                X509Certificate2 cert = certs[i];
                if (cert.Subject.Equals(subjectName, StringComparison.OrdinalIgnoreCase))
                {
                    return cert;
                }
            }

            return null;
        }

        #region Dispose
        /// <summary>
        /// 	Clean up the contained resources that need disposing. 
        /// </summary>
        /// <param name="disposing">true if called from Dispose, false if from the finalizer</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.m_store = null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
