using System.Security.Cryptography.X509Certificates;

namespace Microsoft.HealthVault.Certificate
{
    internal interface ICertificateUtilities
    {
        /// <summary>
        /// Creates a certificate
        /// </summary>
        /// <param name="certificateName">The name of the certificate</param>
        /// <param name="numberOfYears">Number of years</param>
        /// <param name="persist">Indicates if the certificate  should be persisted</param>
        /// <param name="storeLocation">Store location for the certificate</param>
        /// <returns>Returns a new certificate with the required credentials</returns>
        X509Certificate2 CreateCert(string certificateName, short numberOfYears, bool persist, StoreLocation storeLocation);

        /// <summary>
        /// Converts the distinguished name into a cert blob.
        /// </summary>
        /// <param name="distinguishedName">The certificates distinguished name</param>
        /// <returns>encoded form of the name</returns>
        byte[] GetEncodedName(string distinguishedName);

        /// <summary>
        /// Removes the key container for the specified application identifier.
        /// </summary>
        /// <param name="certificateName">
        /// The certificate name which was used in creating
        /// the key container.
        /// </param>
        void DeleteKeyContainer(string certificateName);
    }
}
