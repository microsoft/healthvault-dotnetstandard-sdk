// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace Microsoft.Health
{
    /// <summary>
    /// helper for making web calls
    /// </summary>
    /// 
    internal class EasyWebRequest : IDisposable
    {
        /// <summary> default constructor for GET </summary>
        internal EasyWebRequest() { }

        /// <summary> 
        /// constructor for string POST 
        /// </summary>
        /// 
        /// <param name="stringRequest">
        /// text to send 
        /// </param>
        /// 
        private EasyWebRequest(string stringRequest)
        {
            _stringRequest = stringRequest;
        }

        /// <summary> 
        /// constructor for Xml POST 
        /// </summary>
        /// 
        /// <param name="utf8EncodedXml">
        /// Utf8 encoded Xml to send 
        /// </param>
        /// 
        /// <param name="length">
        /// Significant byte count in the buffer.
        /// </param>
        /// 
        private EasyWebRequest(Byte[] utf8EncodedXml, int length)
        {
            _xmlRequest = utf8EncodedXml;
            _xmlRequestLength = length;
        }

        /// <summary>
        /// Sets the default proxy to use for all instances of
        /// EasyWebRequest. The initial value is system default.
        /// i.e. the value returned by System.Net.WebRequest.DefaultWebProxy
        /// To disable proxy usage, set this property to null.
        /// </summary>
        /// 
        internal static IWebProxy DefaultWebProxy
        {
            get { return _defaultWebProxy; }
            set { _defaultWebProxy = value; }
        }
        private static IWebProxy _defaultWebProxy = WebRequest.DefaultWebProxy;

        /// <summary>
        /// Gets or sets the number of times the request will be retried if
        /// the server returns an Internal Error 500.
        /// </summary>
        /// 
        /// <remarks>
        /// This value can be configured using the 
        /// "RequestRetryOnInternal500Count" key in the application 
        /// configuration file.
        /// </remarks>
        /// 
        internal static int RetryOnInternal500Count
        {
            get { return _retryCount; }
            set { _retryCount = value; }
        }
        private static int _retryCount =
            HealthApplicationConfiguration.Current.RetryOnInternal500Count;

        /// <summary>
        /// Gets or sets the number of seconds that will be slept before the
        /// request will be retried after encountering an Internal Error 
        /// 500.
        /// </summary>
        /// 
        /// <remarks>
        /// This value can be configured using the 
        /// "RequestRetryOnInternal500SleepSeconds" key in the application 
        /// configuration file.
        /// </remarks>
        /// 
        internal static int RetryOnInternal500SleepSeconds
        {
            get { return _retrySleepSeconds; }
            set { _retrySleepSeconds = value; }
        }
        private static int _retrySleepSeconds =
            HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds;

        /// <summary>
        /// Sets the proxy to use with this instance of
        /// EasyWebRequest. The default setting is to use
        /// <see cref="EasyWebRequest.DefaultWebProxy"/>.
        /// To disable proxy usage, set this property to null.
        /// </summary>
        /// 
        internal IWebProxy WebProxy
        {
            get { return _webProxy; }
            set { _webProxy = value; }
        }
        private IWebProxy _webProxy = DefaultWebProxy;

        /// <summary>
        /// Gets the dictionary of headers that will be added to the web request.
        /// </summary>
        /// 
        internal Dictionary<string, string> Headers
        {
            get { return _headers; }
        }
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        /// If the <see cref="RequestCancelTrigger"/> is set by calling 
        /// <see cref="EventWaitHandle.Set()"/> the pending request will be aborted and a 
        /// HealthServiceRequestCancelledException will be thrown.
        /// </summary>
        /// 
        internal ManualResetEvent RequestCancelTrigger
        {
            get { return _requestCancelTrigger; }
            set { _requestCancelTrigger = value; }
        }
        private ManualResetEvent _requestCancelTrigger = new ManualResetEvent(false);

        /// <summary>
        /// Cancels the request.
        /// </summary>
        /// 
        internal void CancelRequest()
        {
            if (_requestCancelTrigger != null)
            {
                _requestCancelTrigger.Set();
            }
        }

        /// <summary>
        /// Gets the request size in bytes.
        /// </summary>
        /// 
        internal Int64 RequestSize
        {
            get { return _requestSize; }
        }
        private Int64 _requestSize;

        /// <summary>
        /// Gets the response size in bytes.
        /// </summary>
        /// 
        internal Int64 ResponseSize
        {
            get { return _xmlResponse.HasValue ? _xmlResponse.Value.Count + _xmlResponse.Value.Offset : 0; }
        }

        internal virtual void Fetch(Uri url)
        {
            bool internal500ErrorEncountered;
            int retryCount = RetryOnInternal500Count;
            do
            {
                internal500ErrorEncountered = false;

                GetRequest(url);
                try
                {
                    if (_isGetRequest)
                    {
                        if (_forceAsyncRequest)
                        {
                            this.WaitForCompletion();
                        }
                        else
                        {
                            this.StartGetRequest();
                        }
                    }
                    else
                    {
                        this.StartPostRequest();
                        this.WaitForCompletion();
                    }
                }
                catch (WebException exception)
                {
                    if (exception.Response != null &&
                        ((HttpWebResponse)exception.Response).StatusCode ==
                        HttpStatusCode.InternalServerError &&
                        retryCount > 0)
                    {
                        internal500ErrorEncountered = true;

                        // The retry sleep is measured in seconds
                        System.Threading.Thread.Sleep(RetryOnInternal500SleepSeconds * 1000);
                    }
                    else
                    {
                        throw;
                    }
                }
                --retryCount;
            } while (
                internal500ErrorEncountered &&
                retryCount >= 0);
        }

        /// <summary> 
        /// Do the dance --- custom handler
        /// </summary>
        /// 
        /// <param name="url"> 
        /// url to request 
        /// </param>
        /// 
        /// <param name="customHandler">
        /// response handler (nullable)
        /// </param>
        /// 
        internal void Fetch(Uri url, IEasyWebResponseHandler customHandler)
        {
            _customHandler = customHandler;
            Fetch(url);
        }

        /// <summary> 
        /// approx request timeout value (default infinite) 
        /// </summary>
        /// 
        internal int TimeoutMilliseconds
        {
            get { return (_timeoutMilliseconds); }
            set { _timeoutMilliseconds = value; }
        }

        /// <summary>
        /// Transform the response stream depending on the content type.
        /// </summary>
        /// 
        /// <param name="outputStream">
        /// Output stream to be transformed
        /// </param>
        /// 
        /// <param name="contentEncoding">
        /// Http request/response content encoding
        /// </param>
        /// 
        /// <param name="leaveOpen">
        /// If true, leave the outputStream open
        /// when transform stream is closed
        /// </param>
        /// 
        /// <returns>
        /// Transformed stream. Closing this stream will
        /// close the input stream
        /// </returns>
        /// 
        internal static Stream CreateOutputCompressionStream(
            Stream outputStream,
            string contentEncoding,
            bool leaveOpen)
        {
            string[] encodings
                = SDKHelper.SplitAndTrim(contentEncoding.ToLower(), ',');

            for (int i = 0; i < encodings.Length; ++i)
            {
                switch (encodings[i])
                {
                    case "deflate":
                        // content needs to be compressed using RFC 1951
                        // (combination of LZ77 algorithm and Huffman coding)
                        // implemented by DeflateStream
                        return new DeflateStream(
                            outputStream,
                            CompressionMode.Compress,
                            leaveOpen);

                    case "gzip":
                        // content needs to be compressed using gzip RFC 1952
                        // implemented by GZipStream
                        return new GZipStream(
                            outputStream,
                            CompressionMode.Compress,
                            leaveOpen);

                    default:
                        // nothing to do
                        continue;
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Transforms the input stream depending on the content type
        /// </summary>
        /// 
        /// <param name="inputStream">
        /// Stream to be transformed
        /// </param>
        /// 
        /// <param name="contentEncoding">
        /// Http request/response content encoding
        /// </param>
        /// 
        /// <param name="leaveOpen">
        /// If true, leave the outputStream open
        /// when transform stream is closed
        /// </param>
        /// 
        /// <returns>
        /// Transformed stream. Closing this stream will
        /// close the input stream
        /// </returns>
        /// 
        internal static Stream CreateInputDecompressionStream(
            Stream inputStream, string contentEncoding, bool leaveOpen)
        {
            string[] encodings
                = SDKHelper.SplitAndTrim(contentEncoding.ToLower(), ',');

            for (int i = 0; i < encodings.Length; ++i)
            {
                switch (encodings[i])
                {
                    case "deflate":
                        // content was compressed using RFC 1951 (combination
                        // of LZ77 algorithm and Huffman coding) implemented
                        // by DeflateStream
                        return new DeflateStream(
                            inputStream,
                            CompressionMode.Decompress,
                            leaveOpen);

                    case "gzip":
                        // content was compressed using gzip RFC 1952
                        // implemented by GZipStream
                        return new GZipStream(
                            inputStream,
                            CompressionMode.Decompress,
                            leaveOpen);

                    default:
                        // nothing to do
                        continue;
                }
            }

            return inputStream;
        }

        #region Request Handling

        private HttpWebRequest GetRequest(Uri url)
        {
            _webRequest =
                (HttpWebRequest)WebRequest.Create(url);
            _webRequest.Proxy = this.WebProxy;
            _webRequest.KeepAlive = HealthApplicationConfiguration.Current.ConnectionUseHttpKeepAlive;
            _webRequest.ServicePoint.Expect100Continue = false;
            _webRequest.ServicePoint.UseNagleAlgorithm = false;
            _webRequest.ServicePoint.MaxIdleTime = HealthApplicationConfiguration.Current.ConnectionMaxIdleTime;
            _webRequest.ServicePoint.ConnectionLeaseTimeout = HealthApplicationConfiguration.Current.ConnectionLeaseTimeout;

            if (_headers.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in _headers)
                {
                    _webRequest.Headers.Add(header.Key, header.Value);
                }
            }

            // set up the method, etc. based on the input we were given
            if (_xmlRequest == null && _stringRequest == null)
            {
                _webRequest.Method = "GET";
                _isGetRequest = true;
            }
            else
            {
                _webRequest.Method = "POST";

                if (_xmlRequest != null)
                {
                    _webRequest.ContentType = "text/xml";

                    if (!String.IsNullOrEmpty(RequestCompressionMethod)
                        && _xmlRequestLength
                            > RequestCompressionThreshold * 1024)
                    {
                        _webRequest.Headers.Add("Content-Encoding",
                            RequestCompressionMethod);
                    }
                }
                else
                {
                    _webRequest.ContentType =
                        "application/x-www-form-urlencoded";
                }
            }

            // response compression accepted or not?
            if (!String.IsNullOrEmpty(ResponseCompressionMethods))
            {
                _webRequest.Headers.Add("Accept-Encoding",
                        ResponseCompressionMethods);
            }

            return _webRequest;
        }

        private void StartGetRequest()
        {
            HttpWebResponse webResponse = null;
            Stream responseStream = null;

            _webRequest.Timeout = _timeoutMilliseconds;

            try
            {
                webResponse = (HttpWebResponse)_webRequest.GetResponse();

                string contentEncoding = webResponse.Headers["Content-Encoding"];
                contentEncoding = contentEncoding ?? String.Empty;

                responseStream =
                    CreateInputDecompressionStream(
                        webResponse.GetResponseStream(),
                        contentEncoding,
                        false);

                SetResponse(responseStream);

                if (_customHandler != null)
                {
                    _customHandler.HandleResponse(CreateResponseWrapper(), webResponse.Headers);
                }
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                }

                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        private void SetResponse(Stream responseStream)
        {
            if (responseStream == null)
            {
                _xmlResponse = null;
                return;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                Byte[] buff = new Byte[1024 * 2];
                int count;
                while ((count = responseStream.Read(buff, 0, buff.Length)) > 0)
                {
                    ms.Write(buff, 0, count);
                }
                ms.Flush();

                // Skip the preamble.
                buff = ms.GetBuffer();
                int offset = 0;
                count = (int)ms.Length;
                byte[] pre = Encoding.UTF8.GetPreamble();
                if (count >= 3 && buff[0] == pre[0] && buff[1] == pre[1] && buff[2] == pre[2])
                {
                    offset = 3;
                    count -= 3;
                }

                _xmlResponse = new ArraySegment<Byte>(buff, offset, count);
            }
        }

        /// <summary>
        /// Creates a MemoryStream wrapper over the buffered response data.
        /// </summary>
        /// 
        internal MemoryStream CreateResponseWrapper()
        {
            if (!_xmlResponse.HasValue)
            {
                return null;
            }

            return
                new MemoryStream(
                    _xmlResponse.Value.Array,
                    _xmlResponse.Value.Offset,
                    _xmlResponse.Value.Count,
                    false,
                    true);
        }

        private void StartPostRequest()
        {
            // we do this with a callback so that we can stream UTF8 and
            // avoid having to set the content length

            _requestSize = -1L;
            _asyncException = null;
            _eventAsyncReady = new ManualResetEvent(false);
            _timeStarted = DateTime.Now;

            try
            {
                _webRequest.BeginGetRequestStream(
                             new AsyncCallback(RequestCallback), _webRequest);

                WaitHandle[] waitHandles = null;
                if (_requestCancelTrigger != null)
                {
                    _requestCancelTrigger.Reset();
                    waitHandles = new WaitHandle[] { _eventAsyncReady, _requestCancelTrigger };
                }
                else
                {
                    waitHandles = new WaitHandle[] { _eventAsyncReady };
                }

                int waitResult = WaitHandle.WaitAny(waitHandles, _timeoutMilliseconds, false);
                if (waitResult == WaitHandle.WaitTimeout)
                {
                    try
                    {
                        _webRequest.Abort();
                    }
                    catch (Exception)
                    { /* eat it */
                    }

                    if (_requestSize < 0)
                    {
                        throw Validator.WebException(
                            "TimeoutConstructingRequest",
                            WebExceptionStatus.Timeout);
                    }
                    else
                    {
                        throw new WebException(
                            ResourceRetriever.FormatResourceString(
                                "TimeoutSendingRequest", _requestSize),
                            WebExceptionStatus.Timeout);
                    }
                }
                else if (waitResult == 1)
                {
                    // The app is attempting to abort the request
                    try
                    {
                        _webRequest.Abort();
                    }
                    catch (Exception)
                    { /* eat it */
                    }

                    throw new HealthServiceRequestCancelledException();
                }

                // this gets thrown on another thread --- propagate it into ours
                if (_asyncException != null)
                    throw _asyncException;
            }
            finally
            {
                _eventAsyncReady.Close();
                _eventAsyncReady = null;
            }
        }

        private void RequestCallback(IAsyncResult asyncResult)
        {
            Stream requestStream = null;

            try
            {
                string contentEncoding = _webRequest.Headers["Content-Encoding"];
                contentEncoding = contentEncoding ?? String.Empty;

                requestStream = CreateOutputCompressionStream(
                    _webRequest.EndGetRequestStream(asyncResult),
                    contentEncoding,
                    false);

                if (_xmlRequest != null)
                {
                    this.WriteXmlToStream(requestStream);
                }
                else if (_stringRequest != null)
                {
                    this.WriteTextToStream(requestStream);
                }
                /* else GET request so do nothing on purpose */
            }
            catch (Exception e)
            {
                _asyncException = e;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
                _eventAsyncReady.Set();
            }
        }

        private void WriteXmlToStream(Stream stream)
        {
            _requestSize = _xmlRequestLength;
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(_xmlRequest, 0, _xmlRequestLength);
            }
        }

        private void WriteTextToStream(Stream stream)
        {
            _requestSize = _stringRequest.Length;
            using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding()))
            {
                writer.Write(_stringRequest);
            }
        }

        #endregion
        #region Response Handling

        /// <summary> 
        /// wait for request to complete
        /// </summary>
        /// 
        private void WaitForCompletion()
        {
            int timeoutMilliseconds;

            _xmlResponse = null;
            _asyncException = null;
            _eventAsyncReady = new ManualResetEvent(false);

            // we do this with a callback because we have to --- according
            // to MSDN you can't mix async and sync stream usage 

            _webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), _webRequest);

            if (_timeoutMilliseconds == Timeout.Infinite)
            {
                timeoutMilliseconds = _timeoutMilliseconds;
            }
            else
            {
                DateTime timeNow = DateTime.Now;
                TimeSpan spanSoFar = timeNow.Subtract(_timeStarted);
                timeoutMilliseconds = _timeoutMilliseconds - spanSoFar.Milliseconds;
            }

            try
            {
                WaitHandle[] waitHandles = null;

                if (_requestCancelTrigger != null)
                {
                    waitHandles = new WaitHandle[] { _eventAsyncReady, _requestCancelTrigger };
                }
                else
                {
                    waitHandles = new WaitHandle[] { _eventAsyncReady };
                }

                int waitResult = WaitHandle.WaitAny(waitHandles, timeoutMilliseconds, false);
                if (waitResult == WaitHandle.WaitTimeout)
                {
                    try
                    {
                        _webRequest.Abort();
                    }
                    catch (Exception)
                    { /* eat it */
                    }

                    if (!_xmlResponse.HasValue)
                    {
                        throw Validator.WebException(
                            "TimeoutReceivingResponse",
                            WebExceptionStatus.Timeout);
                    }
                    else
                    {
                        throw new WebException(
                            ResourceRetriever.FormatResourceString(
                                "TimeoutProcessingResponse", _xmlResponse.Value.Count),
                            WebExceptionStatus.Timeout);
                    }
                }
                else if (waitResult == 1)
                {
                    // The app is attempting to abort the request
                    try
                    {
                        _webRequest.Abort();
                    }
                    catch (Exception)
                    { /* eat it */
                    }

                    throw new HealthServiceRequestCancelledException();
                }

                // this gets thrown on another thread --- propagate it into ours
                if (_asyncException != null)
                {
                    throw _asyncException;
                }
            }
            finally
            {
                _eventAsyncReady.Close();
                _eventAsyncReady = null;

                if (_requestCancelTrigger != null)
                {
                    _requestCancelTrigger.Reset();
                }
            }
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebResponse webResponse = null;
            Stream responseStream = null;

            try
            {
                webResponse = (HttpWebResponse)_webRequest.EndGetResponse(asyncResult);

                string contentEncoding = webResponse.Headers["Content-Encoding"];
                contentEncoding = contentEncoding ?? String.Empty;

                responseStream =
                    CreateInputDecompressionStream(
                        webResponse.GetResponseStream(),
                        contentEncoding,
                        false);

                SetResponse(responseStream);

                if (HealthVaultPlatformTrace.LoggingEnabled)
                {
                    HealthVaultPlatformTrace.LogResponse(ResponseText);
                }

                if (_customHandler != null)
                {
                    _customHandler.HandleResponse(CreateResponseWrapper(), webResponse.Headers);
                }
            }
            catch (Exception e) // Possible third-party callout, catch-all OK
            {
                _asyncException = e;
            }
            finally
            {
                if (_eventAsyncReady != null)
                {
                    _eventAsyncReady.Set();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        internal string ResponseText
        {
            get
            {
                if (!_xmlResponse.HasValue)
                {
                    return String.Empty;
                }

                return
                    Encoding.UTF8.GetString(
                        _xmlResponse.Value.Array,
                        _xmlResponse.Value.Offset,
                        _xmlResponse.Value.Count);
            }
        }

        #endregion

        private int _timeoutMilliseconds = Timeout.Infinite;
        private DateTime _timeStarted = DateTime.MinValue;

        protected string _stringRequest;
        protected Byte[] _xmlRequest; // utf8Encoded
        protected int _xmlRequestLength;
        private ArraySegment<Byte>? _xmlResponse;

        protected IEasyWebResponseHandler _customHandler;

        /// <summary>
        /// Gets or sets the request compression threshold in kilobytes.
        /// </summary>
        /// 
        /// <remarks>
        /// Only requests larger than the threshold are compressed. Note that
        /// this setting is applicable only if the RequestCompressionMethod is 
        /// valid.
        /// </remarks>
        /// 
        internal int RequestCompressionThreshold
        {
            get { return _requestCompressionThreshold; }
            set { _requestCompressionThreshold = value; }
        }
        private int _requestCompressionThreshold =
            HealthApplicationConfiguration.Current.RequestCompressionThreshold;

        /// <summary>
        /// Gets or sets the request compression method.
        /// </summary>
        /// 
        internal string RequestCompressionMethod
        {
            get { return _requestCompressionMethod; }
            set
            {
                _requestCompressionMethod = value;

                if (String.IsNullOrEmpty(_requestCompressionMethod))
                {
                    _requestCompressionMethod = String.Empty;
                }
                else
                {
                    if (!String.Equals(
                            _requestCompressionMethod,
                            "gzip",
                            StringComparison.OrdinalIgnoreCase) &&
                        !String.Equals(
                            _requestCompressionMethod,
                            "deflate",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw Validator.HealthServiceException("InvalidRequestCompressionMethod");
                    }
                }
            }
        }
        private string _requestCompressionMethod =
            HealthApplicationConfiguration.Current.RequestCompressionMethod;

        /// <summary>
        /// Gets or sets the comma separated response compression methods.
        /// </summary>
        /// 
        internal string ResponseCompressionMethods
        {
            get { return _responseCompressionMethods; }
            set
            {
                _responseCompressionMethods = value;

                if (String.IsNullOrEmpty(_responseCompressionMethods))
                {
                    _responseCompressionMethods = String.Empty;
                }
                else
                {
                    string[] methods = SDKHelper.SplitAndTrim(
                            _responseCompressionMethods.ToLower(),
                            ',');

                    for (int i = 0; i < methods.Length; ++i)
                    {
                        if (!String.Equals(
                                methods[i],
                                "gzip",
                                StringComparison.Ordinal) &&
                            !String.Equals(
                                methods[i],
                                "deflate",
                                StringComparison.Ordinal))
                        {
                            throw Validator.HealthServiceException("InvalidResponseCompressionMethods");
                        }
                    }

                    _responseCompressionMethods = String.Join(",", methods);
                }
            }
        }
        private string _responseCompressionMethods =
            HealthApplicationConfiguration.Current.ResponseCompressionMethods;

        private HttpWebRequest _webRequest;
        private ManualResetEvent _eventAsyncReady;
        private Exception _asyncException;

        private bool _isGetRequest;

        /// <summary>
        /// If true will make Get requests asynchronous instead of the default synchronous.
        /// </summary>
        /// 
        internal bool ForceAsyncRequest
        {
            get { return _forceAsyncRequest; }
            set { _forceAsyncRequest = value; }
        }
        private bool _forceAsyncRequest;

        #region IDisposable
        ~EasyWebRequest()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the request.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up the cancel request trigger.
        /// </summary>
        /// 
        /// <param name="disposing"></param>
        /// 
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_requestCancelTrigger != null)
                {
                    _requestCancelTrigger.Close();
                    _requestCancelTrigger = null;
                }
            }
        }

        #endregion IDisposable

        // Factory for testing
        /// <summary> default constructor for GET </summary>
        static internal EasyWebRequest Create()
        {
            return Create(null);
        }

        /// <summary> 
        /// constructor for string POST 
        /// </summary>
        /// 
        /// <param name="stringRequest">
        /// text to send 
        /// </param>
        /// 
        static internal EasyWebRequest Create(string stringRequest)
        {
            EasyWebRequest instance;

            if (_requestOverride != null)
            {
                instance = _requestOverride;
            }
            else
            {
                instance = new EasyWebRequest();
            }

            instance._stringRequest = stringRequest;

            return instance;
        }

        /// <summary> 
        /// constructor for Xml POST 
        /// </summary>
        /// 
        /// <param name="utf8EncodedXml">
        /// Utf8 encoded Xml to send 
        /// </param>
        /// 
        /// <param name="length">
        /// Significant byte count in the buffer.
        /// </param>
        /// 
        static internal EasyWebRequest Create(Byte[] utf8EncodedXml, int length)
        {
            EasyWebRequest instance = Create();
            instance._xmlRequest = utf8EncodedXml;
            instance._xmlRequestLength = length;

            return instance;
        }

        static EasyWebRequest _requestOverride;

        static public EasyWebRequest RequestOverride
        {
            get { return _requestOverride; }
            set { _requestOverride = value; }
        }
    }
}

