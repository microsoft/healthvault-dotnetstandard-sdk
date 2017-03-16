// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Web.Certificate
{
    // TODO: The cert store is currently created in StoreName.AuthRoot but we need to verify if this is correct.

    /// <summary>
    /// Wrapper around X509 Store for HealthVault.
    /// </summary>
    internal sealed class CertificateStore : IDisposable
    {
        private X509Store store;

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
            this.store = new X509Store(StoreName.AuthRoot, storeType);
            this.store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
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
        internal X509Certificate2 this[string subjectName] => this.FindBySubject(subjectName);

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
        internal X509Certificate2 this[Guid appID] => this.FindBySubject(ApplicationCertificate.MakeCertSubject(appID));

        /// <summary>
        /// Adds the certificate to the store.
        /// </summary>
        ///
        /// <param name="cert">
        /// The certificate to be added to the store.
        /// </param>
        internal void Add(X509Certificate2 cert)
        {
            this.store.Add(cert);
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
                throw new ArgumentNullException(nameof(cert));
            }

            if (this.Contains(cert.Subject))
            {
                this.store.Remove(cert);
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
            return this[appID] != null;
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
            return this.FindBySubject(subjectName) != null;
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

            X509Certificate2Collection certs = this.store.Certificates;
            foreach (X509Certificate2 cert in certs)
            {
                if (cert.Subject.Equals(subjectName, StringComparison.OrdinalIgnoreCase))
                {
                    return cert;
                }
            }

            return null;
        }

        #region Dispose

        /// <summary>
        /// Clean up the contained resources that need disposing.
        /// </summary>
        /// <param name="disposing">true if called from Dispose, false if from the finalizer</param>
        ///
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.store?.Dispose();
                this.store = null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
