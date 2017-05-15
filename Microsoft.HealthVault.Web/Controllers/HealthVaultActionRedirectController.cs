// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Web.Mvc;
using Microsoft.HealthVault.Web.Helpers;

namespace Microsoft.HealthVault.Web.Controllers
{
    /// <summary>
    /// A redirector controller to be used as the "action" page for Microsoft
    /// HealthVault applications.
    /// </summary>
    ///
    /// <remarks>
    /// All HealthVault applications are required to expose a set
    /// of URLs for information and functionality that they expose.
    /// For instance, all applications must expose a Service Agreement,
    /// privacy statement, a home page, help, etc.
    ///
    /// This controller acts as a simple redirector for these action pages such
    /// that the application can easily configure these action pages through
    /// their web.config file.
    ///
    /// Web app should register this controller as a mapped route in global.aspx.cs page.
    /// In the app web.config file, add entries for each of the
    /// action URLs that your application supports using HVPage_Action as the
    /// prefix for the key. For example, for the Service Agreement action URL
    /// create a setting in the web.config with key
    /// HVPage_ActionServiceAgreement and value containing the URL to your
    /// application Service Agreement.
    /// </remarks>
    public class HealthVaultActionRedirectController : Controller
    {
        /// <summary>
        /// Handles the basic request to /Redirect
        /// </summary>
        /// <param name="action">The target of the redirect</param>
        /// <param name="actionqs">A query string that was originally passed to HealthVault Shell.
        /// If it is a url, it will be redirected to.</param>
        /// <returns>A redirect to either the actionqs (if it is a url) or the application homepage</returns>
        public virtual ActionResult Index(string action, string actionqs)
        {
            ActionRedirectHelper actionRedirectHelper = Ioc.Container.Locate<ActionRedirectHelper>();

            string targetLocation = actionRedirectHelper.TryGetTargetLocation(action, actionqs);

            if (!string.IsNullOrEmpty(targetLocation))
            {
                return Redirect(targetLocation);
            }

            if (actionRedirectHelper.IsValidActionUrl(Request.Url.Host, actionqs))
            {
                return Redirect(actionqs);
            }

            return Redirect(Request.ApplicationPath);
        }
    }
}
