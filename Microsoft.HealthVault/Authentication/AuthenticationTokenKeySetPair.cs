// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault.Web.Authentication
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
        internal AuthenticationTokenKeySetPair()
        {
            RefreshSharedSecret();
        }

        internal AuthenticatedSessionKeySet KeySet
        {
            get { return _keySet; }
            set { _keySet = value; }
        }
        private AuthenticatedSessionKeySet _keySet;

        internal void RefreshSharedSecret()
        {
            this.KeySet =
                new AuthenticatedSessionKeySet(
                    HealthApplicationConfiguration.Current.CryptoConfiguration.HmacAlgorithmName,
                    ServiceLocator.Current.CryptoService.GenerateHmacSharedSecret());
        }

        internal CreateAuthenticationTokenResult AuthenticationResult
        {
            get { return _authenticationResult; }
            set { _authenticationResult = value; }
        }
        private CreateAuthenticationTokenResult _authenticationResult;

        internal bool IsAuthenticated()
        {
            return
                !(
                _isAuthenticationResultExpired
                || _authenticationResult == null);
        }

        internal bool IsAuthenticationExpired(Int64 refreshCounter)
        {
            return !IsAuthenticated() || refreshCounter != this.RefreshCounter;
        }

        /// <summary>
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
        internal void Update(
            CreateAuthenticationTokenResult result)
        {
            LastRefreshed = DateTime.UtcNow;
            RefreshCounter++;

            AuthenticationResult = result;

            IsAuthenticationResultExpired = false;
        }

        internal bool IsAuthenticationResultExpired
        {
            get { return _isAuthenticationResultExpired; }
            set { _isAuthenticationResultExpired = value; }
        }
        private bool _isAuthenticationResultExpired;

        internal DateTime LastRefreshed
        {
            get { return _lastRefreshed; }
            set { _lastRefreshed = value; }
        }
        private DateTime _lastRefreshed;

        internal Int64 RefreshCounter
        {
            get { return _refreshCounter; }
            set { _refreshCounter = value; }
        }
        private Int64 _refreshCounter;
    }
}

