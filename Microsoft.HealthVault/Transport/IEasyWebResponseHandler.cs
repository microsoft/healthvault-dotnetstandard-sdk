// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.IO;
using System.Net.Http.Headers;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// handler to implement custom response handling
    /// </summary>
    ///
    internal interface IEasyWebResponseHandler
    {
        /// <summary>
        /// the callback
        /// </summary>
        ///
        /// <param name="stream">Response stream</param>
        /// <param name="responseHeaders">Response header collection</param>
        ///
        void HandleResponse(Stream stream, HttpResponseHeaders responseHeaders);
    }
}
