// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.HealthVault.Rest
{
    /// <summary>
    /// Contains the response information from the HealthVault service after
    /// processing a request on the rest endpoint
    /// </summary>
    public class HealthServiceRestResponseData
    {
        internal HealthServiceRestResponseData()
        {
        }

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HttpStatusCode"/> value.
        /// </value>        
        public HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }

            internal set
            {
                _statusCode = value;
            }
        }
        
        private HttpStatusCode _statusCode;

        /// <summary>
        /// The response body of the rest request.
        /// </summary>
        public string ResponseBody
        {
            get
            {
                return _responseBody;
            }

            internal set
            {
                _responseBody = value;
            }
        }

        private string _responseBody;

        /// <summary>
        /// The response headers
        /// </summary>
        public HttpResponseHeaders Headers
        {
            get
            {
                return _headers;
            }

            internal set
            {
                _headers = value;
            }
        }

        private HttpResponseHeaders _headers;
    }
}
