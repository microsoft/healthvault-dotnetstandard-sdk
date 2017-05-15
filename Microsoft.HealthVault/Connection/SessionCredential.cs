namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// The authenticated session token
    /// </summary>
    ///
    /// <remarks>
    /// The token has a limited time-to-live.
    /// When the token expires, requests will fail with access denied.
    /// Connections will automatically refresh the token when expired
    /// </remarks>
    public class SessionCredential
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        public string SharedSecret { get; set; }
    }
}
