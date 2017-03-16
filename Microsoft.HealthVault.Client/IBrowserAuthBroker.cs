using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Runs through authorization UI with a browser.
    /// </summary>
    public interface IBrowserAuthBroker
    {
        /// <summary>
        /// Runs through authorization UI with a browser for the given URL.
        /// </summary>
        /// <param name="startUrl">The URL of the page to load in the browser.</param>
        /// <param name="stopUrlPrefix">The stop URL minus any query parameters.</param>
        /// <returns>The URL of the success page.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the flow by pressing the back button or closing
        /// the browser window.</exception>
        /// <exception cref="BrowserAuthException">Thrown if the authorization fails.</exception>
        Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix);
    }
}
