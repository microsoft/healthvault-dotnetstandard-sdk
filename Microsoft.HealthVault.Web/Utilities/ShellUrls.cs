// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.HealthVault.Web.Utilities
{
    internal static class ShellUrls
    {
        /// <summary>
        /// Creates a url to the Shell homepage
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <returns>A string containing the Shell homepage url.</returns>
        public static string Url(ControllerContext context)
        {
            return Url(context, null, new RouteValueDictionary());
        }

        /// <summary>
        /// Creates a url to a specific Shell target
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <param name="target">The name of the target</param>
        /// <returns>A string containing the Shell url for the given target</returns>
        public static string Url(ControllerContext context, string target)
        {
            return Url(context, target, new RouteValueDictionary());
        }

        /// <summary>
        /// Creates a url to a specific Shell target
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <param name="target">The name of the target</param>
        /// <param name="parameters">Parameters to pass to the Shell page</param>
        /// <returns>A string containing the Shell url for the given target</returns>
        public static string Url(ControllerContext context, string target, object parameters)
        {
            return Url(context, target, new RouteValueDictionary(parameters));
        }

        /// <summary>
        /// Creates a url to a specific Shell target
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <param name="target">The name of the target</param>
        /// <param name="parameters">Parameters to pass to the Shell page</param>
        /// <returns>A string containing the Shell url for the given target</returns>
        public static string Url(
            ControllerContext context,
            string target,
            IDictionary<string, object> parameters)
        {
            HttpContextBase httpContext = context.HttpContext;

            Uri shellUri = httpContext.Request.Url;
            string applicationPath = httpContext.Request.ApplicationPath;

            var urlBuilder = new ShellUrlBuilder(shellUri, target, applicationPath, parameters);
            return urlBuilder.ToString();
        }
    }
}
