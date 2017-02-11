// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Microsoft.Health.Web;
using Microsoft.Health.Web.Authentication;

namespace Microsoft.Health.Rest
{
    /// <summary>
    /// Represents an individual request to a HealthVault REST service.
    /// The class wraps up the header generation and web request/response.
    /// </summary>
    /// 
    /// <remarks>
    /// This class is not thread safe. A new instance should be created when multiple requests 
    /// must execute concurrently.
    /// </remarks>
    /// 
    public class HealthServiceRestRequest : IEasyWebResponseHandler
    {
        //// TODO: GCorvera Hook up response id
        private const string ResponseIdContextKey = "WC_ResponseId";

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceRestRequest"/> 
        /// class for the specified parameters.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        /// 
        /// <param name="httpVerb">
        /// The HTTP Verb to execute against the service.
        /// </param>
        /// 
        /// <param name="path">
        /// The path to the resource in the URL.
        /// </param>
        /// 
        /// <param name="queryStringParameters">
        /// A collection of query string parameters.
        /// </param>
        /// 
        /// <param name="requestBody">
        /// The body for this request.
        /// </param>
        /// 
        /// <param name="apiRoot">
        /// Domain name override, used to redirect to other endpoints.
        /// </param>
        /// 
        /// <param name="optionalHeaders">
        /// Optional headers to include in the API request.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="httpVerb"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="path"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public HealthServiceRestRequest(
            HealthServiceConnection connection,
            string httpVerb,
            string path,
            NameValueCollection queryStringParameters = null,
            string requestBody = null,
            Uri apiRoot = null,
            NameValueCollection optionalHeaders = null)
        {
            var fullUri = new UriBuilder(apiRoot ?? HealthApplicationConfiguration.Current.RestHealthVaultUrl ?? new Uri(RestConstants.DefaultMshhvRoot));
            fullUri.Path = path;

            var query = HttpUtility.ParseQueryString(fullUri.Query);
            if (queryStringParameters != null)
            {
                query.Add(queryStringParameters);
            }

            fullUri.Query = query.ToString();

            Initialize(connection, httpVerb, fullUri.Uri, requestBody, optionalHeaders);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceRestRequest"/> 
        /// class for the specified parameters.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        /// 
        /// <param name="httpVerb">
        /// The HTTP Verb to execute against the service.
        /// </param>
        /// 
        /// <param name="fullUri">
        /// The full URI of the API request.
        /// </param>
        /// 
        /// <param name="requestBody">
        /// The body for this request.
        /// </param>
        /// 
        /// <param name="optionalHeaders">
        /// Optional headers to include in the API request.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="httpVerb"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="fullUri"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public HealthServiceRestRequest(
            HealthServiceConnection connection,
            string httpVerb,
            Uri fullUri,
            string requestBody = null,
            NameValueCollection optionalHeaders = null)
        {
            Initialize(connection, httpVerb, fullUri, requestBody, optionalHeaders);
        }

        /// <summary>
        /// Optional parameter to specify what record to access.
        /// </summary>
        public Guid RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }

        private Guid _recordId;

        /// <summary>
        /// The response details from the server
        /// </summary>
        public HealthServiceRestResponseData Response
        {
            get { return _response; }
        }

        private HealthServiceRestResponseData _response;

        /// <summary>
        /// To allow applications to keep track of calls to platform, the application
        /// can optionally set a correlation id. This will be passed up in web requests to
        /// HealthVault and used when HealthVault writes to its logs. If issues occur, this
        /// id can be used by the HealthVault team to help debug the issue.
        /// 
        /// For asp.net applications, we want to avoid the use of thread local for setting
        /// the request id since a single web request is not guaranteed to fully execute on the
        /// same thread - using HttpContext.Items is the recommended way.
        /// 
        /// For non web applications, this method sets a [ThreadStatic] variable which stores the 
        /// id in thread local storage. All HealthVault requests made on this thread will re-use this
        /// variable
        /// </summary>
        public static void SetCorrelationId(Guid correlationId)
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                httpContext.Items[RestConstants.CorrelationIdContextKey] = correlationId;
            }

            // _correlationId is a ThreadStatic variable so it is stored in thread local and used
            // by all health service requests in this thread. This is the primary usage for
            // non web applications to set the request id
            _correlationId = correlationId;
        }

        /// <summary>
        /// Builds up the request and reads the response.        
        /// </summary>
        public void Execute()
        {
            int retryCount = HealthApplicationConfiguration.Current.RetryOnInternal500Count;
            int retrySleepSeconds = HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds;
            do
            {
                try
                {
                    Fetch();

                    // Completed successfully, break the do-while loop
                    break;
                }
                catch (WebException exception)
                {
                    var response = exception.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.InternalServerError && retryCount > 0)
                    {
                        // The retry sleep is measured in seconds
                        System.Threading.Thread.Sleep(retrySleepSeconds * 1000);
                    }
                    else
                    {
                        SetResponse(response);                        
                    }
                }

                --retryCount;
            }
            while (retryCount >= 0);
        }
        
        private void Initialize(
            HealthServiceConnection connection,
            string httpVerb,
            Uri fullUri,
            string requestBody = null,
            NameValueCollection optionalHeaders = null)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "CtorServiceNull");
            Validator.ThrowIfStringNullOrEmpty(httpVerb, "httpVerb");
            Validator.ThrowIfArgumentNull(fullUri, "fullUri", "CtorServiceUrlNull");

            _isContentRequest = IsContentVerb(httpVerb);
            _connection = connection;
            _verb = httpVerb;
            _uri = fullUri;
            _body = _isContentRequest ? requestBody ?? String.Empty : null;
            _optionalheaders = optionalHeaders;
        }

        private void Fetch()
        {
            try
            {
                FetchInternal(_uri);
            }
            catch (WebException we)
            {
                // An Unauthorized response might mean the token expired, we try to request for a 
                // new token from the user and retry the call again 
                var response = we.Response as HttpWebResponse;
                if (response != null &&
                    response.StatusCode == HttpStatusCode.Unauthorized &&
                    _connection.Credential.ExpireAuthenticationResult(_connection.ApplicationId))
                {
                    FetchInternal(_uri);
                }
                else
                {
                    throw;
                }
            }
        }

        private void FetchInternal(Uri uri)
        {
            var httpRequest = CreateRequest(uri);

            SetHeaders(httpRequest);

            if (_isContentRequest)
            {
                using (var requestStream = httpRequest.GetRequestStream())
                {
                    var data = Encoding.UTF8.GetBytes(_body);
                    requestStream.Write(data, 0, data.Length);
                }
            }

            var response = (HttpWebResponse)httpRequest.GetResponse();
            SetResponse(response);
        }

        private HttpWebRequest CreateRequest(Uri uri)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpRequest.KeepAlive = HealthApplicationConfiguration.Current.ConnectionUseHttpKeepAlive;
            httpRequest.ServicePoint.MaxIdleTime = HealthApplicationConfiguration.Current.ConnectionMaxIdleTime;
            httpRequest.ServicePoint.ConnectionLeaseTimeout = HealthApplicationConfiguration.Current.ConnectionLeaseTimeout;
            httpRequest.ServicePoint.Expect100Continue = false;
            httpRequest.ServicePoint.UseNagleAlgorithm = false;

            httpRequest.Method = _verb;

            return httpRequest;
        }

        private void SetHeaders(HttpWebRequest request)
        {
            request.Headers = new WebHeaderCollection();
            request.Headers.Add(RestConstants.AuthorizationHeaderName, GetAuthorizationHeader());

            if (_isContentRequest)
            {
                request.ContentType = RestConstants.JsonContentType;
                request.Headers.Add(RestConstants.Sha256HeaderName, GetContentSHA256Header());
                request.ContentLength = _body.Length;
            }

            request.Date = DateTime.UtcNow;
            request.UserAgent = GetUserAgent();

            if (_correlationId != Guid.Empty)
            {
                request.Headers.Add(RestConstants.CorrelationIdHeaderName, _correlationId.ToString());
            }

            request.Headers.Add(RestConstants.HmacHeaderName, String.Format(CultureInfo.InvariantCulture, RestConstants.V1HMACSHA256Format, GetHmacHeader(request)));

            if (_optionalheaders != null)
            {
                request.Headers.Add(_optionalheaders);
            }
        }
        
        private void SetResponse(HttpWebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[1024 * 1024 * 8];
                    int count;

                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }

                    ms.Flush();

                    _response = new HealthServiceRestResponseData
                    {
                        StatusCode = response.StatusCode,
                        ResponseBody = Encoding.UTF8.GetString(ms.GetBuffer()),
                        Headers = response.Headers
                    };

                    _response.Headers = new WebHeaderCollection();

                    if (response.Headers != null)
                    {
                        foreach (string key in response.Headers.Keys)
                        {
                            _response.Headers.Add(key, response.Headers[key].ToString());
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception when getting the version shouldn't stop execution")]
        [System.Security.SecuritySafeCritical]
        private static string GetUserAgent()
        {
            string fileVersion = "Unknown";
            string systemInfo = "Unknown";

            // safe attempt to obtain the assembly file version, and system information
            try
            {
                fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                systemInfo = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}; CLR {1}",
                    Environment.OSVersion.VersionString,
                    Environment.Version);
            }
            catch (Exception)
            {
                // failure in obtaining version or system info should not
                // prevent the initialization from continuing.
            }

            return String.Format(CultureInfo.InvariantCulture, RestConstants.MSHSDKVersion, fileVersion, systemInfo);
        }

        private string GetContentSHA256Header()
        {
            var result = String.Empty;
            if (_isContentRequest)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(_body));
                    result = Convert.ToBase64String(hash);
                }
            }

            return result;
        }

        private string GetAuthorizationHeader()
        {
            List<string> tokens = new List<string>();

            if (_connection.Credential != null)
            {
                _connection.Credential.AuthenticateIfRequired(_connection, _connection.ApplicationId);
            }

            if (_connection.Credential != null && !String.IsNullOrEmpty(_connection.AuthenticationToken))
            {
                _connection.Credential.AddRestAuthorizationHeaderToken(tokens, _connection.ApplicationId);

                OfflineWebApplicationConnection offlineConnection = _connection as OfflineWebApplicationConnection;
                if (offlineConnection != null)
                {
                    if (offlineConnection.OfflinePersonId != Guid.Empty)
                    {
                        tokens.Add(String.Format(CultureInfo.InvariantCulture, RestConstants.AuthorizationHeaderElement, RestConstants.OfflinePersonId, offlineConnection.OfflinePersonId.ToString()));
                    }
                }
            }

            if (RecordId != Guid.Empty)
            {
                tokens.Add(String.Format(CultureInfo.InvariantCulture, RestConstants.AuthorizationHeaderElement, RestConstants.RecordId, RecordId));
            }

            return String.Format(CultureInfo.InvariantCulture, RestConstants.MSHV1HeaderFormat, String.Join(",", tokens.ToArray<string>()));
        }

        private string GetHmacHeader(HttpWebRequest request)
        {
            string authHeader = request.Headers.Get("Authorization");
            string contentSha256Header = _isContentRequest ? request.Headers.Get(RestConstants.Sha256HeaderName) : String.Empty;
            string contentTypeHeader = _isContentRequest ? request.ContentType : String.Empty;
            string dateHeader = request.Date.ToString("u", CultureInfo.InvariantCulture);

            UriBuilder uriBuilder = new UriBuilder(_uri);
            var data = String.Format(CultureInfo.InvariantCulture, RestConstants.HmacFormat, _verb, uriBuilder.Path, authHeader, contentSha256Header, contentTypeHeader, dateHeader);

            //// TODO: GCORVERA Does this use the app's private key to generate the hmac?  See Credential.AuthenticateData() for reference.  Not a check-in blocker -- we can figure this out later.?
            using (var hmacsha256 = HMACSHA256.Create())
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }

        private static bool IsContentVerb(string httpVerb)
        {
            switch (httpVerb.ToUpperInvariant())
            {
                case "POST":
                case "PUT":
                case "PATCH":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary> 
        /// Represents the <see cref="IEasyWebResponseHandler"/> callback.
        /// </summary>
        /// 
        /// <param name="stream">
        /// The response stream.
        /// </param>       
        /// 
        /// <param name="responseHeaders">
        /// Response headers.
        /// </param>
        public void HandleResponse(System.IO.Stream stream, WebHeaderCollection responseHeaders)
        {
            // TODO: GCorvera Hook Up Handle Response, do we need it?
            throw new NotImplementedException();
        }

        [ThreadStatic]
        private static Guid _correlationId;

        private HealthServiceConnection _connection;
        private string _verb;
        private string _body;
        private Uri _uri;
        private NameValueCollection _optionalheaders;
        private bool _isContentRequest;
    }
}
