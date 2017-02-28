// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Represents an individual request to a HealthVault service.
    /// The class wraps up the XML generation and web request/response.
    /// </summary>
    ///
    /// <remarks>
    /// An instance of this class can be retrieved by calling the
    /// <see cref="HealthServiceConnection.CreateRequest(string, int)"/>
    /// method.
    /// This class is not thread safe. A new instance should be created when multiple requests
    /// must execute concurrently.
    /// </remarks>
    ///
    public class HealthServiceRequest
    {
        private const string CorrelationIdContextKey = "WC_CorrelationId";
        private const string ResponseIdContextKey = "WC_ResponseId";

        private CancellationTokenSource cancellationTokenSource;
        private readonly object cancelLock = new object();

        /// <summary>
        /// Constructs the version identifier for this version of the HealthVault .NET APIs.
        /// </summary>
        [SecuritySafeCritical]
        private static string ConstructVersionString()
        {
            string fileVersion = "?";
            string systemInfo = "Unknown";

            // TODO: this is not currently accessible in .Net Standard 1.4- we should revisit once 2.0 is released.
            // safe attempt to obtain the assembly file version, and system information
            //            try
            //            {
            //                fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            //                systemInfo = String.Format(
            //                    CultureInfo.InvariantCulture,
            //                    "{0}; CLR {1}",
            //                    Environment.OSVersion.VersionString,
            //                    Environment.Version);
            //            }
            //            catch (Exception)
            //            {
            //                // failure in obtaining version or system info should not
            //                // prevent the initialzation from continuing.
            //            }

            return string.Format(CultureInfo.InvariantCulture, "HV-NET/{0} ({1})", fileVersion, systemInfo);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceRequest"/>
        /// class for the specified method.
        /// </summary>
        ///
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        ///
        /// <param name="methodName">
        /// The name of the method to invoke on the service.
        /// </param>
        ///
        /// <param name="methodVersion">
        /// The version of the method to invoke on the service.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="methodName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public HealthServiceRequest(
            HealthServiceConnection connection,
            string methodName,
            int methodVersion)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "CtorServiceNull");
            Validator.ThrowIfStringNullOrEmpty(methodName, "methodName");

            this.connection = connection;
            AuthenticatedConnection authenticatedConnection = connection as AuthenticatedConnection;
            if (authenticatedConnection != null)
            {
                this.ImpersonatedPersonId = authenticatedConnection.ImpersonatedPersonId;
            }

            List<HealthServiceRequest> pendingRequests = connection.PendingRequests;

            lock (pendingRequests)
            {
                connection.PendingRequests.Add(this);
            }

            this.MethodName = methodName;
            this.timeoutSeconds = connection.RequestTimeoutSeconds;
            this.TimeToLiveSeconds = connection.RequestTimeToLive;
            this.CultureCode = connection.Culture.Name;
            this.MethodVersion = methodVersion;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceRequest"/>
        /// class for the specified method.
        /// </summary>
        ///
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        ///
        /// <param name="methodName">
        /// The name of the method to invoke on the service.
        /// </param>
        ///
        /// <param name="methodVersion">
        /// The version of the method to invoke on the service.
        /// </param>
        ///
        /// <param name="record">
        /// The record to use.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="methodName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public HealthServiceRequest(
            HealthServiceConnection connection,
            string methodName,
            int methodVersion,
            HealthRecordAccessor record)
            : this(connection, methodName, methodVersion)
        {
            this.recordId = record.Id;
        }

        public HealthServiceRequest(
            HealthServiceConnection connection,
            string methodName,
            int methodVersion,
            HealthRecordAccessor record,
            Guid correlationId)
            : this(connection, methodName, methodVersion, record)
        {
            this.CorrelationId = correlationId;
        }

        /// <summary>
        /// To allow applications to keep track of calls to platform, the application
        /// can optionally set a correlation id. This will be passed up in web requests to
        /// HealthVault and used when HealthVault writes to its logs. If issues occur, this
        /// id can be used by the HealthVault team to help debug the issue.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets the response id that was returned by the HealthVault platform. The response id
        /// is found in the response headers and is set when a request finishes executing.
        /// 
        /// If an error occurs / exception thrown, the caller can call GetLastResponseId to get that response
        /// id which can be used to look up error information in the HealthVault logs.
        /// </summary>
        public Guid ResposeId { get; set; }

        public async Task<HealthServiceResponseData> ExecuteAsync()
        {
            if (this.connection.Credential != null)
            {
                await this.connection.Credential.AuthenticateIfRequiredAsync(this.connection, this.connection.ApplicationId).ConfigureAwait(false);
            }

            try
            {
                return await this.ExecuteInternalAsync().ConfigureAwait(false);
            }
            catch (HealthServiceAuthenticatedSessionTokenExpiredException)
            {
                if (this.connection.Credential != null)
                {
                    // Mark the credential's authentication result as expired,
                    // so that in the following
                    // Credential.AuthenticateIfRequired we fetch a new token.
                    if (this.connection.Credential.ExpireAuthenticationResult(this.connection.ApplicationId))
                    {
                        await this.connection.Credential.AuthenticateIfRequiredAsync(this.connection, this.connection.ApplicationId).ConfigureAwait(false);

                        return await this.ExecuteInternalAsync().ConfigureAwait(false);
                    }
                }

                throw;
            }
        }

        private async Task<HealthServiceResponseData> ExecuteInternalAsync()
        {
            try
            {
                EasyWebRequest easyWeb = this.BuildWebRequest(null);
                easyWeb.RequestCompressionMethod = this.RequestCompressionMethod;
                HttpResponseMessage response;

                try
                {
                    this.cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this.timeoutSeconds));
                    response = await easyWeb.FetchAsync(this.connection.RequestUrl, this.cancellationTokenSource.Token).ConfigureAwait(false);
                }
                finally
                {
                    lock (this.cancelLock)
                    {
                        if (this.cancellationTokenSource != null)
                        {
                            this.cancellationTokenSource.Dispose();
                            this.cancellationTokenSource = null;
                        }
                    }
                }

                // Platform returns a platform request id with the responses. This allows
                // developers to have additional information if necessary for debugging/logging purposes.
                Guid responseId;
                if (response.Headers != null && Guid.TryParse(response.Headers.GetValues(ResponseIdContextKey)?.FirstOrDefault(), out responseId))
                {
                    this.ResponseId = responseId;

                    if (HealthVaultPlatformTrace.LoggingEnabled)
                    {
                        HealthVaultPlatformTrace.Log(TraceEventType.Information, "Response Id: {0}", responseId);
                    }
                }

                HealthServiceResponseData result = null;

                Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                result = CreateHealthServiceResponseData(responseStream, response.Headers);

                return result;
            }
            catch (XmlException xmlException)
            {
                throw new HealthServiceException(
                    ResourceRetriever.GetResourceString("InvalidResponseFromXMLRequest"),
                    xmlException);
            }
            finally
            {
                List<HealthServiceRequest> pendingRequests = this.connection.PendingRequests;
                lock (pendingRequests)
                {
                    this.connection.PendingRequests.Remove(this);
                }
            }
        }

        /// <summary>
        /// Cancels any pending request to HealthVault that was initiated with the same connection
        /// as this request.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this method will cancel any requests that was started using the connection.
        /// It is up to the caller to start the request on another thread. Cancelling will cause
        /// an OperationCancelledException to be thrown on the thread the request was
        /// executed on.
        /// </remarks>
        ///
        public void CancelRequest()
        {
            lock (this.cancelLock)
            {
                if (this.cancellationTokenSource != null)
                {
                    this.cancellationTokenSource.Cancel();
                }
                else
                {
                    this.pendingCancel = true;
                }
            }
        }

        /// <summary>
        /// Same as Execute, but takes a transform url (or tag) that is
        /// used to transform the result (on the server). Since the
        /// response is no longer necessarily xml, it is returned as
        /// a string
        /// </summary>
        ///
        /// <param name="transform">
        /// The public URL of the transform to apply to the results. If <b>null</b>,
        /// no transform is applied and the results are returned
        /// as a string.
        /// </param>
        ///
        public virtual async Task<string> ExecuteForTransformAsync(string transform)
        {
            if (this.connection.Credential != null)
            {
                await this.connection.Credential.AuthenticateIfRequiredAsync(this.connection, this.connection.ApplicationId).ConfigureAwait(false);
            }

            try
            {
                return await this.ExecuteForTransformInternalAsync(transform).ConfigureAwait(false);
            }
            catch (HealthServiceAuthenticatedSessionTokenExpiredException)
            {
                if (this.connection.Credential != null)
                {
                    // Mark the credential's authentication result as expired,
                    // so that in the following
                    // Credential.AuthenticateIfRequired we fetch a new token.
                    if (this.connection.Credential.ExpireAuthenticationResult(this.connection.ApplicationId))
                    {
                        await this.connection.Credential.AuthenticateIfRequiredAsync(this.connection, this.connection.ApplicationId).ConfigureAwait(false);

                        return await this.ExecuteForTransformInternalAsync(transform).ConfigureAwait(false);
                    }
                }

                throw;
            }
        }

        internal virtual async Task<string> ExecuteForTransformInternalAsync(string transform)
        {
            HttpResponseMessage response = null;
            try
            {
                EasyWebRequest easyWeb = this.BuildWebRequest(transform);
                easyWeb.RequestCompressionMethod = this.RequestCompressionMethod;

                try
                {
                    this.cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this.timeoutSeconds));
                    response = await easyWeb.FetchAsync(this.connection.RequestUrl, this.cancellationTokenSource.Token).ConfigureAwait(false);
                }
                finally
                {
                    lock (this.cancelLock)
                    {
                        if (this.cancellationTokenSource != null)
                        {
                            this.cancellationTokenSource.Dispose();
                            this.cancellationTokenSource = null;
                        }
                    }
                }

                using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (MemoryStream responseMemoryStream = new MemoryStream())
                {
                    // Copy the response stream to a memory stream so we can go over it multiple times.
                    await responseStream.CopyToAsync(responseMemoryStream).ConfigureAwait(false);

                    // Look over the response to see if it's an error and extract response ID
                    responseMemoryStream.Position = 0;
                    this.HandleTransformResponse(responseMemoryStream, response.Headers);

                    // Reset stream position and read as string for result
                    responseMemoryStream.Position = 0;

                    using (StreamReader reader = new StreamReader(responseMemoryStream))
                    {
                        return await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                List<HealthServiceRequest> pendingRequests = this.connection.PendingRequests;
                lock (pendingRequests)
                {
                    this.connection.PendingRequests.Remove(this);
                }
            }
        }

        /// <summary>
        /// Handles the response stream and headers from transform request.
        /// </summary>
        ///
        /// <param name="stream">The response stream.</param>
        /// <param name="responseHeaders">The response header collection.</param>
        ///
        public void HandleTransformResponse(MemoryStream stream, HttpResponseHeaders responseHeaders)
        {
            // Platform returns a platform request id with the responses. This allows
            // developers to have additional information if necessary for debugging/logging purposes.
            Guid responseId;
            if (responseHeaders != null && Guid.TryParse(responseHeaders.GetValues("WC_ResponseId").FirstOrDefault(), out responseId))
            {
                this.ResponseId = responseId;
            }

            ProcessTransformResponseForErrors(stream);
        }

        private static void ProcessTransformResponseForErrors(MemoryStream responseStream)
        {
            // Now look at the errors in the response before returning. If we see HV XML returned
            // containing a failure status code, throw an exception
            XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
            settings.CloseInput = false;
            settings.IgnoreWhitespace = false;

            try
            {
                using (XmlReader reader = XmlReader.Create(responseStream, settings))
                {
                    reader.NameTable.Add("wc");

                    if (SDKHelper.ReadUntil(reader, "response") &&
                        SDKHelper.ReadUntil(reader, "status") &&
                        SDKHelper.ReadUntil(reader, "code"))
                    {
                        HealthServiceResponseData responseData = new HealthServiceResponseData
                        {
                            CodeId = reader.ReadElementContentAsInt()
                        };

                        if (responseData.Code != HealthServiceStatusCode.Ok)
                        {
                            responseData.Error = HandleErrorResponse(reader);

                            HealthServiceException e = HealthServiceExceptionHelper.GetHealthServiceException(responseData);

                            throw e;
                        }
                    }
                }
            }
            catch (FormatException)
            {
            }
            catch (MissingFieldException)
            {
            }
            catch (XmlException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        #region helpers

        /// <summary>
        /// Creates a Web request that can be sent to HealthVault.
        /// </summary>
        ///
        /// <param name="transform">
        /// The optional XSL to apply.
        /// </param>
        ///
        /// <returns>
        /// An instance of <see cref="EasyWebRequest"/>.
        /// </returns>
        private EasyWebRequest BuildWebRequest(string transform)
        {
            lock (this.cancelLock)
            {
                if (this.pendingCancel || this.connection.CancelAllRequests)
                {
                    this.pendingCancel = false;
                    throw new OperationCanceledException();
                } 
            }

            this.BuildRequestXml(transform);

            HealthVaultPlatformTrace.LogRequest(this.XmlRequest, this.CorrelationId);

            EasyWebRequest easyWeb = new EasyWebRequest(this.XmlRequest, this.XmlRequestLength);
            easyWeb.WebProxy = this.connection.WebProxy;

            if (this.CorrelationId != Guid.Empty)
            {
                easyWeb.Headers.Add(CorrelationIdContextKey, this.CorrelationId.ToString());
            }

            return easyWeb;
        }

        /// <summary>
        /// Connects the XML using default values.
        /// </summary>
        ///
        /// <exception cref="XmlException">
        /// There is a failure building up the XML.
        /// </exception>
        ///
        /// <private>
        /// This is protected so that the derived testing class can call it
        /// to create the request XML and then verify it is correct.
        /// </private>
        ///
        protected void BuildRequestXml()
        {
            this.BuildRequestXml(null);
        }

        /// <summary>
        /// Gets or sets the request compression method used by the connection.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the request compression method.
        /// </returns>
        ///
        public string RequestCompressionMethod
        {
            get { return this.connection.RequestCompressionMethod; }
            set { this.connection.RequestCompressionMethod = value; }
        }

        /// <summary>
        /// Connects the XML using the specified optional XSL.
        /// </summary>
        /// <param name="transform">The optional XSL to apply.</param>
        ///
        /// <exception cref="XmlException">
        /// There is a failure building up the XML.
        /// </exception>
        ///
        /// <private>
        /// This is protected so that the derived testing class can call it
        /// to create the request XML and then verify it is correct.
        /// </private>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes the code more readable")]
        protected void BuildRequestXml(string transform)
        {
            // first, construct the non-authenticated sections sequentially
            string infoHash;
            byte[] infoXml;
            int infoXmlLength;
            byte[] headerXml;
            int headerXmlLength;

            this.GetInfoSection(out infoHash, out infoXml, out infoXmlLength);
            this.GetHeaderSection(transform, infoHash, out headerXml, out headerXmlLength);

            using (MemoryStream requestXml = new MemoryStream(infoXml.Length + headerXml.Length + 512))
            {
                XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;

                using (XmlWriter writer = XmlWriter.Create(requestXml, settings))
                {
                    // now, construct the final xml sequentially
                    // <request>
                    writer.WriteStartElement("wc-request", "request", "urn:com.microsoft.wc.request");

                    // <auth>
                    // If we have an authenticated section, then construct the auth data otherwise do
                    // not include an auth section.
                    if (this.connection.Credential != null)
                    {
                        string authInnerXml = this.connection.Credential.AuthenticateData(headerXml, 0, headerXmlLength);

                        if (!string.IsNullOrEmpty(authInnerXml))
                        {
                            writer.WriteStartElement("auth");
                            writer.WriteRaw(authInnerXml);
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteRaw(Encoding.UTF8.GetString(headerXml, 0, headerXmlLength));
                    writer.WriteRaw(Encoding.UTF8.GetString(infoXml, 0, infoXmlLength));

                    // </request>
                    writer.WriteEndElement();

                    writer.Flush();

                    // MemoryStream.Flush() does nothing, don't call
                    this.XmlRequest = requestXml.ToArray();
                    this.XmlRequestLength = (int)requestXml.Length;
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes the code more readable")]
        private void GetInfoSection(
            out string infoHash,
            out byte[] infoSection,
            out int infoSectionLength)
        {
            XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;

            using (MemoryStream infoXml = new MemoryStream(this.Parameters.Length + 16))
            {
                using (XmlWriter writer = XmlWriter.Create(infoXml, settings))
                {
                    writer.WriteStartElement("info");
                    writer.WriteRaw(this.Parameters);
                    writer.WriteEndElement();
                    writer.Flush();

                    infoSection = infoXml.ToArray();
                    infoSectionLength = (int)infoXml.Length;

                    // if we are not using an auth connection,
                    // then there is no point in hashing the info section
                    // because we cannot protect the resulting hash
                    infoHash =
                        this.connection.Credential != null
                                ? CryptoHash.CreateInfoHash(infoSection, 0, infoSectionLength)
                                : string.Empty;
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes the code more readable")]
        private void GetHeaderSection(
            string transform,
            string infoHash,
            out byte[] headerXml,
            out int headerXmlLength)
        {
            XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;

            using (MemoryStream xmlHeader = new MemoryStream(2048))
            {
                using (XmlWriter writer = XmlWriter.Create(xmlHeader, settings))
                {
                    // <header>
                    writer.WriteStartElement("header");

                    // <method>
                    writer.WriteElementString("method", this.MethodName);

                    if (this.MethodVersion != null)
                    {
                        // <method-version>
                        writer.WriteElementString("method-version", this.MethodVersion.Value.ToString());
                    }

                    if (this.targetPersonId != Guid.Empty)
                    {
                        // <target-person-id>
                        writer.WriteElementString("target-person-id", this.targetPersonId.ToString());
                    }

                    if (this.recordId != Guid.Empty)
                    {
                        // <record-id>
                        writer.WriteElementString("record-id", this.recordId.ToString());
                    }

                    // header based on the kind of connection we have...

                    if (this.connection.Credential != null &&
                        !string.IsNullOrEmpty(this.connection.AuthenticationToken))
                    {
                        writer.WriteStartElement("auth-session");

                        this.connection.Credential.GetHeaderSection(this.connection.ApplicationId, writer);

                        OfflineWebApplicationConnection offlineConnection = this.connection as OfflineWebApplicationConnection;
                        if (offlineConnection != null)
                        {
                            if (offlineConnection.OfflinePersonId != Guid.Empty)
                            {
                                // person-specific request, but person is offline as far as
                                // HealthVault is concerned, so pass in offline info...
                                // <offline-person-info>
                                writer.WriteStartElement("offline-person-info");

                                // <offline-person-id>
                                writer.WriteElementString(
                                    "offline-person-id",
                                    offlineConnection.OfflinePersonId.ToString());

                                // </offline-person-info>
                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteElementString("app-id", this.connection.ApplicationId.ToString());
                    }

                    if (this.CultureCode != null)
                    {
                        writer.WriteElementString("culture-code", this.CultureCode);
                    }

                    if (transform != null)
                    {
                        writer.WriteElementString("final-xsl", transform);
                    }

                    writer.WriteElementString("msg-time", SDKHelper.XmlFromNow());
                    writer.WriteElementString(
                        "msg-ttl", this.TimeToLiveSeconds.ToString(CultureInfo.InvariantCulture));

                    writer.WriteElementString("version", Version);

                    // if we are not using an authenticated connection,
                    // then do not include the info-hash because we cannot protect it
                    //      with the auth section.
                    if (this.connection.Credential != null)
                    {
                        writer.WriteStartElement("info-hash");
                        writer.WriteRaw(infoHash);
                        writer.WriteEndElement();
                    }

                    // </header>
                    writer.WriteEndElement();

                    writer.Flush();

                    headerXml = xmlHeader.ToArray();
                    headerXmlLength = (int)xmlHeader.Length;
                }
            }
        }

        /// <summary>
        /// Handles the data retrieved by making the web request.
        /// </summary>
        ///
        /// <param name="stream">
        /// The response stream from the web request.
        /// </param>
        /// <param name="responseHeaders">The web response headers.</param>
        ///
        /// <exception cref ="HealthServiceException">
        /// HealthVault returns an exception in the form of an
        /// exception section in the response XML.
        /// </exception>
        ///
        public static HealthServiceResponseData CreateHealthServiceResponseData(Stream stream, HttpResponseHeaders responseHeaders)
        {
            HealthServiceResponseData result = new HealthServiceResponseData();
            result.ResponseHeaders = responseHeaders;

            bool newStreamCreated = false;
            MemoryStream responseStream = stream as MemoryStream;

            try
            {
                if (responseStream == null)
                {
                    newStreamCreated = true;
                    responseStream = CreateMemoryStream(stream);
                }

                result = ParseResponse(responseStream);
            }
            finally
            {
                if (newStreamCreated && responseStream != null)
                {
                    responseStream.Dispose();
                }
            }

            return result;
        }

        private static HealthServiceResponseData ParseResponse(MemoryStream responseStream)
        {
            HealthServiceResponseData result = new HealthServiceResponseData();
            XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
            settings.CloseInput = false;
            settings.IgnoreWhitespace = false;
            responseStream.Position = 0;
            XmlReader reader = XmlReader.Create(responseStream, settings);
            reader.NameTable.Add("wc");

            if (!SDKHelper.ReadUntil(reader, "code"))
            {
                throw new MissingFieldException("code");
            }

            result.CodeId = reader.ReadElementContentAsInt();

            if (result.Code == HealthServiceStatusCode.Ok)
            {
                if (reader.ReadToFollowing("wc:info"))
                {
                    result.InfoReader = reader;

                    byte[] buff = responseStream.ToArray();
                    int offset = 0;
                    int count = (int)responseStream.Length;

                    while (offset < count && buff[offset] != '<')
                    {
                        offset++;
                    }

                    result.ResponseText = new ArraySegment<byte>(buff, offset, count - offset);
                }

                return result;
            }

            result.Error = HandleErrorResponse(reader);

            HealthServiceException e =
                HealthServiceExceptionHelper.GetHealthServiceException(result);

            throw e;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'responseStream' variable is returned to the caller")]
        internal static MemoryStream CreateMemoryStream(Stream stream)
        {
            MemoryStream responseStream = new MemoryStream();

            try
            {
                int count;
                byte[] buff = new byte[1024 * 2];
                while ((count = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    responseStream.Write(buff, 0, count);
                }

                responseStream.Flush();
            }
            finally
            {
                stream = null;
            }

            return responseStream;
        }

        internal static HealthServiceResponseError HandleErrorResponse(XmlReader reader)
        {
            HealthServiceResponseError error = new HealthServiceResponseError();

            // <error>
            if (string.Equals(reader.Name, "error", StringComparison.Ordinal))
            {
                // <message>
                if (!SDKHelper.ReadUntil(reader, "message"))
                {
                    throw new MissingFieldException("message");
                }

                error.Message = reader.ReadElementContentAsString();

                // <context>
                SDKHelper.SkipToElement(reader);
                if (string.Equals(reader.Name, "context", StringComparison.Ordinal))
                {
                    HealthServiceErrorContext errorContext = new HealthServiceErrorContext();

                    // <server-name>
                    if (SDKHelper.ReadUntil(reader, "server-name"))
                    {
                        errorContext.ServerName = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        throw new MissingFieldException("server-name");
                    }

                    // <server-ip>
                    Collection<IPAddress> ipAddresses = new Collection<IPAddress>();

                    SDKHelper.SkipToElement(reader);
                    while (reader.Name.Equals("server-ip", StringComparison.Ordinal))
                    {
                        string ipAddressString = reader.ReadElementContentAsString();
                        IPAddress ipAddress = null;
                        if (IPAddress.TryParse(ipAddressString, out ipAddress))
                        {
                            ipAddresses.Add(ipAddress);
                        }

                        SDKHelper.SkipToElement(reader);
                    }

                    errorContext.SetServerIpAddresses(ipAddresses);

                    // <exception>
                    if (reader.Name.Equals("exception", StringComparison.Ordinal))
                    {
                        errorContext.InnerException = reader.ReadElementContentAsString();
                        SDKHelper.SkipToElement(reader);
                    }
                    else
                    {
                        throw new MissingFieldException("exception");
                    }

                    error.Context = errorContext;
                }

                // <error-info>
                if (SDKHelper.ReadUntil(reader, "error-info"))
                {
                    error.ErrorInfo = reader.ReadElementContentAsString();
                    SDKHelper.SkipToElement(reader);
                }
            }

            return error;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the method name to call.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the method name.
        /// </returns>
        ///
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the version of the method to call.
        /// </summary>
        ///
        /// <returns>
        /// An integer representing the version.
        /// </returns>
        ///
        /// <remarks>
        /// If <b>null</b>, the current version is called.
        /// </remarks>
        ///
        public int? MethodVersion { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the person being impersonated.
        /// </summary>
        ///
        /// <returns>
        /// A GUID representing the identifier.
        /// </returns>
        ///
        public Guid ImpersonatedPersonId
        {
            get { return this.targetPersonId; }
            set { this.targetPersonId = value; }
        }

        private Guid targetPersonId = Guid.Empty;

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        ///
        /// <returns>
        /// A GUID representing the identifier.
        /// </returns>
        ///
        public Guid RecordId
        {
            get { return this.recordId; }
            set { this.recordId = value; }
        }

        private Guid recordId;

        /// <summary>
        /// Gets or sets the culture-code for the request.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the culture-code.
        /// </returns>
        ///
        public string CultureCode { get; set; }

        /// <summary>
        /// Gets a string identifying this version of the HealthVault .NET APIs.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the version.
        /// </returns>
        ///
        internal static string Version { get; } = ConstructVersionString();

        private string parameters = string.Empty;

        /// <summary>
        /// Gets or sets the parameters for the method invocation.
        /// The parameters are specified via XML for the particular method.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the parameters.
        /// </returns>
        ///
        public string Parameters
        {
            get
            {
                // We can't return null - we use the return value for setting
                // xml element's innter text - we'd have to do the value check
                // in several places in the code...
                if (this.parameters == null)
                {
                    this.parameters = string.Empty;
                }

                return this.parameters;
            }

            set
            {
                this.parameters = value;
            }
        }

        private int timeoutSeconds;

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
            get { return this.timeoutSeconds; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "TimeoutSeconds",
                    "TimeoutMustBePositive");

                this.timeoutSeconds = value;
            }
        }

        internal Guid ResponseId
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// This is a test hook so that testing class can set different time
        /// to live to verify if HealthVault checks for it.
        /// </summary>
        ///
        internal int TimeToLiveSeconds { get; set; } = 300;

        /// <summary>
        /// This is a test hook so that the derived testing class can
        /// verify the XML request.
        /// </summary>
        ///
        internal byte[] XmlRequest { get; private set; }

        internal int XmlRequestLength { get; private set; }

        private bool pendingCancel;
        private HealthServiceConnection connection;
    }
}
