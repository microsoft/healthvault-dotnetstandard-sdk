using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Protects data for local storage on the device.
    /// </summary>
    internal interface ILocalDataProtection
    {
        /// <summary>
        /// Protects (i.e. encrypts) the specified data.
        /// </summary>
        /// <param name="data">A byte array to protect.</param>
        /// <returns>A byte array that represents a protected form of the specified data.</returns>
        Task<byte[]> ProtectAsync(byte[] data);

        /// <summary>
        /// Unprotects (i.e. decrypts) the specified data.
        /// </summary>
        /// <param name="data">A protected byte array.</param>
        /// <returns>The original byte array that was protected.</returns>
        Task<byte[]> UnprotectAsync(byte[] data);
    }
}
