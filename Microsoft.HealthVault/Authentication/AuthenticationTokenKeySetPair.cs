// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Configurations;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// A keyset token pair is an explicit binding between an authentication
    /// token and the shared secret that was used to create the token.
    /// Since the token is opaque, the client doesn't really know when the
    /// token will expire by examination.  However, since there is a 1:1
    /// binding between the token and the shared secret, we can use the
    /// shared secret expiration as the token expiration.
    /// </summary>
    ///
    /// <remarks>
    /// All access to an instnace of the pair must be governed through
    /// the reader/writer lock, as the token may expire at any time.
    ///
    /// That said, any user of the WebApplicationCredential should first
    /// if verify the token is expired.  If it is not, then the caller should
    /// be able to use it to make at least one call.
    /// </remarks>
    ///
    internal class AuthenticationTokenKeySetPair
    {
        private IConfiguration configuration = Ioc.Get<IConfiguration>();

        internal AuthenticationTokenKeySetPair()
        {
            this.RefreshSharedSecret();
        }

        internal AuthenticatedSessionKeySet KeySet { get; set; }

        internal void RefreshSharedSecret()
        {
            this.KeySet =
                new AuthenticatedSessionKeySet(
                    this.configuration.CryptoConfiguration.HmacAlgorithmName,
                    ServiceLocator.Current.CryptoService.GenerateHmacSharedSecret());
        }

        internal CreateAuthenticationTokenResult AuthenticationResult { get; set; }

        internal bool IsAuthenticated()
        {
            return
                !(
                this.IsAuthenticationResultExpired
                || this.AuthenticationResult == null);
        }

        internal bool IsAuthenticationExpired(long refreshCounter)
        {
            return !this.IsAuthenticated() || refreshCounter != this.RefreshCounter;
        }

        /// <summary>
        /// Update the authentication key set pair
        /// </summary>
        ///
        /// <remarks>
        /// In order to avoid unnecessary authentication actions, it is
        /// expected that the caller will first call
        /// <cref name="IsAuthenticationExpired"/> before calling this.
        /// </remarks>
        ///
        /// <param name="result">
        /// The authenticated session token.
        /// </param>
        ///
        internal void Update(CreateAuthenticationTokenResult result)
        {
            this.LastRefreshed = DateTime.UtcNow;
            this.RefreshCounter++;

            this.AuthenticationResult = result;

            this.IsAuthenticationResultExpired = false;
        }

        internal bool IsAuthenticationResultExpired { get; set; }

        internal DateTime LastRefreshed { get; set; }

        internal long RefreshCounter { get; set; }
    }
}
