using System;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Session credential client for the client SDK.
    /// </summary>
    internal interface IClientSessionCredentialClient : ISessionCredentialClient
    {
        /// <summary>
        /// Gets or sets the app shared secret used to HMAC the request on this client.
        /// </summary>
        string AppSharedSecret { get; set; }
    }
}
