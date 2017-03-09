using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Soda.Exceptions;

namespace Microsoft.HealthVault.Soda
{
    /// <summary>
    /// Runs through authorization UI with a browser.
    /// </summary>
    public interface IBrowserAuthBroker
    {
        /// <summary>
        /// Runs through authorization UI with a browser for the given URL.
        /// </summary>
        /// <param name="url">The URL of the page to load in the browser.</param>
        /// <param name="successFunc">A function to evaluate if a given page is considered a "success" page</param>
        /// <returns>The URL of the success page.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the flow by pressing the back button or closing
        /// the browser window.</exception>
        /// <exception cref="BrowserAuthException">Thrown if the authorization fails.</exception>
        Task<Uri> AuthenticateAsync(Uri url, Func<Uri, bool> successFunc);
    }
}
