using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Credential //TODO: complete details
    /// </summary>
    public interface ISessionCredential
    {
        /// <summary>
        /// Gets or sets the toekn from CAST (Create Authenticated Session Token) call.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        string Token { get; set; }

        /// <summary>
        /// Gets or sets the toekn from Shared Secret from CAST (Create Authenticated Session Token) call.
        /// </summary>
        /// <value>
        /// The shared secret.
        /// </value>
        string SharedSecret { get; set; }

        /// <summary>
        /// This method verifies that the authentication token is valid
        /// for a limited timespan into the future, such that the token
        /// could be used to make a HealthVault Service method call
        /// within the validated timespan.
        /// </summary>
        Task AuthenticateIfRequiredAsync();

        /// <summary>
        /// Applies the shared secret to the specified data.
        /// </summary>
        ///
        /// <remarks>
        /// After the initial authentication is made with the HealthVault Service,
        /// all subsequent calls to the HealthVault service must have
        /// authenticated header sections. This method produces the
        /// Hash Message Authentication Code (HMAC) data for the auth section.
        ///
        /// This method implements its own shared secret,
        /// so the SharedSecret property is <b>null</b>.
        /// </remarks>
        ///
        /// <param name="data">
        /// The data to authenticate.
        /// </param>
        ///
        /// <param name="index">
        /// The starting index into the data.
        /// </param>
        ///
        /// <param name="count">
        /// The number of bytes from the start index to authenticate.
        /// </param>
        ///
        /// <returns>
        /// The authenticated data is returned.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="data"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        string AuthenticateData(
            byte[] data,
            int index,
            int count);
    }
}