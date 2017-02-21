// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Rest
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
        private const string Optional = "Optional Headers";
        private CancellationTokenSource cancellationTokenSource;
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
            HttpMethod httpVerb,
            string path,
            NameValueCollection queryStringParameters = null,
            string requestBody = null,
            Uri apiRoot = null,
            IEnumerable<string> optionalHeaders = null)
        {
            var fullUri = new UriBuilder(apiRoot ?? HealthApplicationConfiguration.Current.RestHealthVaultUrl ?? new Uri(RestConstants.DefaultMshhvRoot));
            fullUri.Path = path;

            IDictionary<string, string> queryAsDictionary = fullUri.Uri.ParseQuery();

            NameValueCollection query = new NameValueCollection();
            foreach (var key in queryAsDictionary.Keys)
            {
                query.Add(key, queryAsDictionary[key]);
            }

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
            HttpMethod httpVerb,
            Uri fullUri,
            string requestBody = null,
            IEnumerable<string> optionalHeaders = null)
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

        private int _timeoutSeconds;

        /// <summary>
        /// Gets or sets the timeout for the request, in seconds.
        /// </summary>
        ///
        /// <returns>
        /// An integer representing the timeout, in seconds.
        /// </returns>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The timeout value is set to less than 0.
        /// </exception>
        ///
        public int TimeoutSeconds
        {
            get { return _timeoutSeconds; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "TimeoutSeconds",
                    "TimeoutMustBePositive");

                _timeoutSeconds = value;
            }
        }

        /// <summary>
        /// The response details from the server
        /// </summary>
        public HealthServiceRestResponseData Response
        {
            get { return _response; }
        }

        private HealthServiceRestResponseData _response;

        /// <summary>
        /// Builds up the request and reads the response.        
        /// </summary>
        public async void ExecuteAsync()
        {
            int retryCount = HealthApplicationConfiguration.Current.RetryOnInternal500Count;
            int retrySleepSeconds = HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds;
            do
            {
                try
                {
                    FetchAsync();

                    // Completed successfully, break the do-while loop
                    break;
                }
                catch (HealthHttpException exception)
                {
                    var response = exception.InnerException as HealthHttpException;
                    if (response != null && response.StatusCode == HttpStatusCode.InternalServerError && retryCount > 0)
                    {
                        // The retry sleep is measured in seconds
                        await Task.Delay(TimeSpan.FromSeconds(retrySleepSeconds * 1000));
                    }
                    else
                    {
                        throw;
                    }
                }

                --retryCount;
            }
            while (retryCount >= 0);
        }
        
        private void Initialize(
            HealthServiceConnection connection,
            HttpMethod httpVerb,
            Uri fullUri,
            string requestBody = null,
            IEnumerable<string> optionalHeaders = null)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "CtorServiceNull");
            Validator.ThrowIfStringNullOrEmpty(httpVerb.Method, "httpVerb");
            Validator.ThrowIfArgumentNull(fullUri, "fullUri", "CtorServiceUrlNull");

            _isContentRequest = IsContentVerb(httpVerb);
            _connection = connection;
            _verb = httpVerb;
            _uri = fullUri;
            _body = _isContentRequest ? requestBody ?? string.Empty : null;
            _optionalheaders = optionalHeaders;
            _timeoutSeconds = connection.RequestTimeoutSeconds;
        }

        private async void FetchAsync()
        {
            try
            {
               await FetchInternalAsync(_uri);
            }
            catch (HealthHttpException we)
            {
                // An Unauthorized response might mean the token expired, we try to request for a 
                // new token from the user and retry the call again 
                var response = we.InnerException as HealthHttpException;
                if (response != null &&
                    response.StatusCode == HttpStatusCode.Unauthorized &&
                    _connection.Credential.ExpireAuthenticationResult(_connection.ApplicationId))
                {
                    await FetchInternalAsync(_uri);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task FetchInternalAsync(Uri uri)
        {
            var httpRequest = CreateRequest(uri);
            HttpResponseMessage response = null;
            this.cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this._timeoutSeconds));

            SetHeaders(httpRequest);

            if (_isContentRequest)
            {
                using (Stream requestStream = await httpRequest.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var data = Encoding.UTF8.GetBytes(_body);
                    requestStream.Write(data, 0, data.Length);
                }
            }

            using (HttpClient client = CreateHttpClient())
            {
                response = await client.SendAsync(httpRequest, this.cancellationTokenSource.Token).ConfigureAwait(false);
            }

            await SetResponseAsync(response);
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            return new HttpClient(handler);
        }

        private HttpRequestMessage CreateRequest(Uri uri)
        {
            var httpRequest = new HttpRequestMessage(); 
            httpRequest.RequestUri = uri;

            httpRequest.Method = _verb;

            return httpRequest;
        }

        private void SetHeaders(HttpRequestMessage request)
        {
            //request.Headers = new HttpRequestHeaders();
            var authHeader = GetAuthorizationHeader();
            request.Headers.Add(RestConstants.AuthorizationHeaderName, authHeader.Result);

            if (_isContentRequest)
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(RestConstants.JsonContentType);
                request.Headers.Add(RestConstants.Sha256HeaderName, GetContentSHA256Header());
                request.Content.Headers.ContentLength = _body.Length;
            }

            request.Headers.Date = DateTime.UtcNow;
            request.Headers.UserAgent.ParseAdd(GetUserAgent());

            if (_correlationId != Guid.Empty)
            {
                request.Headers.Add(RestConstants.CorrelationIdHeaderName, _correlationId.ToString());
            }

            request.Headers.Add(RestConstants.HmacHeaderName, String.Format(CultureInfo.InvariantCulture, RestConstants.V1HMACSHA256Format, GetHmacHeader(request)));

            if (_optionalheaders != null)
            {
                request.Headers.Add(Optional, _optionalheaders);
            }
        }
        
        private async Task SetResponseAsync(HttpResponseMessage response)
        {
            using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[1024 * 1024 * 8];
                    int count;

                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }

                    _response = new HealthServiceRestResponseData
                    {
                        StatusCode = response.StatusCode,
                        ResponseBody = Encoding.UTF8.GetString(buffer),
                        Headers = response.Headers
                    };

                    ms.Flush();

                    if (response.Headers != null)
                    {
                        _response.Headers = response.Headers; 
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

            // TODO: this is not currently accessible in .Net Standard 1.4- we should revisit once 2.0 is released. 
            // safe attempt to obtain the assembly file version, and system information
            //try
            //{
            //    fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            //    systemInfo = String.Format(
            //        CultureInfo.InvariantCulture,
            //        "{0}; CLR {1}",
            //        Environment.OSVersion.VersionString,
            //        Environment.Version);
            //}
            //catch (Exception)
            //{
            //    // failure in obtaining version or system info should not
            //    // prevent the initialization from continuing.
            //}

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

        private async Task<string> GetAuthorizationHeader()
        {
            List<string> tokens = new List<string>();

            if (_connection.Credential != null)
            {
               await _connection.Credential.AuthenticateIfRequiredAsync(_connection, _connection.ApplicationId).ConfigureAwait(false); ;
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

            return string.Format(CultureInfo.InvariantCulture, RestConstants.MSHV1HeaderFormat, String.Join(",", tokens.ToArray<string>()));
        }

        private string GetHmacHeader(HttpRequestMessage request)
        {
            AuthenticationHeaderValue authHeader = request.Headers.Authorization;
            string contentSha256Header = _isContentRequest ? request.Headers.GetValues(RestConstants.Sha256HeaderName).FirstOrDefault() : string.Empty;
            string contentTypeHeader = _isContentRequest ? request.Headers.GetType().ToString() : string.Empty;
            string dateHeader = request.Headers.Date?.ToString("u", CultureInfo.InvariantCulture);

            UriBuilder uriBuilder = new UriBuilder(_uri);
            var data = String.Format(CultureInfo.InvariantCulture, RestConstants.HmacFormat, _verb, uriBuilder.Path, authHeader.ToString(), contentSha256Header, contentTypeHeader, dateHeader);

            //// TODO: GCORVERA Does this use the app's private key to generate the hmac?  See Credential.AuthenticateData() for reference.  Not a check-in blocker -- we can figure this out later.?
            using (var hmacsha256 = new HMACSHA256())
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }

        private static bool IsContentVerb(HttpMethod httpVerb)
        {
            switch (httpVerb.Method)
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
        public void HandleResponse(Stream stream, HttpResponseHeaders responseHeaders)
        {
            // TODO: GCorvera Hook Up Handle Response, do we need it?
            throw new NotImplementedException();
        }

        [ThreadStatic]
        private static Guid _correlationId;

        private HealthServiceConnection _connection;
        private HttpMethod _verb;
        private string _body;
        private Uri _uri;
        private IEnumerable<string> _optionalheaders;
        private bool _isContentRequest;
    }

}
