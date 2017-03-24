// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;
using System;
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

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Represents an individual request to a HealthVault service.
    /// The class wraps up the XML generation and web request/response.
    /// </summary>
    ///
    /// <remarks>
    /// This class is not thread safe. A new instance should be created when multiple requests
    /// must execute concurrently.
    /// </remarks>
    ///
    /// TODO: DO NOT USE OUTSIDE OF ConnectionInternalBase
    internal class HealthServiceRequest
    {
        private const string CorrelationIdContextKey = "WC_CorrelationId";
        private const string ResponseIdContextKey = "WC_ResponseId";

        private readonly object cancelLock = new object();
        private CancellationTokenSource cancellationTokenSource;

        private readonly IConnectionInternal connectionInternal;
        private readonly IHealthWebRequestFactory requestFactory;

        private readonly HealthVaultConfiguration config;

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceRequest"/>
        /// class for the specified method.
        /// </summary>
        ///
        /// <param name="connectionInternal">
        /// The client-side representation of the HealthVault service.
        /// </param>
        ///
        /// <param name="method">
        /// The method to invoke on the service.
        /// </param>
        ///
        /// <param name="methodVersion">
        /// The version of the method to invoke on the service.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connectionInternal"/> parameter is <b>null</b>.
        /// </exception>
        /// <param name="recordId">RecordId</param>
        /// <param name="config">The configuration to be used when assigning variables to this request</param>
        /// <param name="requestFactory">The factory for creating web requests from this service</param>
        /// <param name="sdkTelemetryInformation">Telemetry Information</param>
        public HealthServiceRequest(
            IConnectionInternal connectionInternal,
            HealthVaultMethods method,
            int methodVersion,
            Guid? recordId = null,
            HealthVaultConfiguration config = null,
            IHealthWebRequestFactory requestFactory = null,
            SdkTelemetryInformation sdkTelemetryInformation = null)
        {
            Validator.ThrowIfArgumentNull(connectionInternal, nameof(connectionInternal), Resources.CtorServiceNull);

            this.connectionInternal = connectionInternal;

            this.Method = method;
            this.MethodVersion = methodVersion;
            this.config = config ?? Ioc.Get<HealthVaultConfiguration>();
            this.requestFactory = requestFactory ?? Ioc.Get<IHealthWebRequestFactory>();
            this.CultureCode = CultureInfo.CurrentUICulture.Name;
            this.recordId = recordId.GetValueOrDefault(Guid.Empty);
            sdkTelemetryInformation = sdkTelemetryInformation ?? Ioc.Get<SdkTelemetryInformation>();

            this.Version = $"{sdkTelemetryInformation.Category}/{sdkTelemetryInformation.FileVersion} {sdkTelemetryInformation.OsInformation}";
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
            try
            {
                IHealthWebRequest easyWeb = this.BuildWebRequest(null);
                HttpResponseMessage response;

                try
                {
                    this.cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this.config.DefaultRequestTimeout));
                    response = await easyWeb.FetchAsync(
                        this.connectionInternal.ServiceInstance.GetHealthVaultMethodUrl(),
                        this.cancellationTokenSource.Token).ConfigureAwait(false);
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
                if (response.Headers != null
                    && response.Headers.Contains(ResponseIdContextKey)
                    && Guid.TryParse(response.Headers.GetValues(ResponseIdContextKey)?.FirstOrDefault(), out responseId))
                {
                    this.ResponseId = responseId;

                    if (HealthVaultPlatformTrace.LoggingEnabled)
                    {
                        HealthVaultPlatformTrace.Log(TraceEventType.Information, "Response Id: {0}", responseId);
                    }
                }

                Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                return CreateHealthServiceResponseData(responseStream, response.Headers);
            }
            catch (XmlException xmlException)
            {
                throw new HealthServiceException(
                    Resources.InvalidResponseFromXMLRequest,
                    xmlException);
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
        /// An instance of <see cref="IHealthWebRequest"/>.
        /// </returns>
        private IHealthWebRequest BuildWebRequest(string transform)
        {
            // Core
            this.BuildRequestXml(transform);

            // Do we need this log
            HealthVaultPlatformTrace.LogRequest(this.XmlRequest, this.CorrelationId);

            IHealthWebRequest easyWeb = this.requestFactory.CreateWebRequest(this.XmlRequest, this.XmlRequestLength);
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
        protected void BuildRequestXml(string transform = null)
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
                    if (this.connectionInternal.SessionCredential != null)
                    {
                        CryptoData crytpoData = this.connectionInternal.GetAuthData(this.Method, headerXml);

                        string authInnerXml = this.GetCryptoDataInnerXml(crytpoData);

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

        internal void GetInfoSection(
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

                    infoHash = string.Empty;

                    if (this.connectionInternal.SessionCredential != null)
                    {
                        // if we are not using an auth connection,
                        // then there is no point in hashing the info section
                        // because we cannot protect the resulting hash

                        CryptoData data = this.connectionInternal.GetInfoHash(infoSection);
                        infoHash = this.GetCryptoDataInnerXml(data);
                    }
                }
            }
        }

        #endregion

        #region AuthData

        private string GetCryptoDataInnerXml(CryptoData crytpoData)
        {
            StringBuilder builder = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                this.WriteCryptoDataInfoXml(writer, crytpoData);
            }

            return builder.ToString();
        }

        internal virtual void WriteCryptoDataInfoXml(XmlWriter writer, CryptoData cryptoData)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (cryptoData.Algorithm == HealthVaultConstants.Cryptography.HmacAlgorithm)
            {
                writer.WriteStartElement("hmac-data");
            }
            else
            {
                writer.WriteStartElement("hash-data");
            }

            writer.WriteAttributeString("algName", cryptoData.Algorithm);
            writer.WriteString(cryptoData.Value);
            writer.WriteEndElement();
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
                    writer.WriteElementString("method", this.Method.ToString());

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

                    if (this.connectionInternal.SessionCredential != null &&
                        !string.IsNullOrEmpty(this.connectionInternal.SessionCredential.Token))
                    {
                        this.connectionInternal.PrepareAuthSessionHeader(writer, this.RecordId);
                    }
                    else
                    {
                        // TODO: We should group the methods that use the masterappid into an anonymous client.
                        Guid appId;
                        if (this.Method == HealthVaultMethods.NewApplicationCreationInfo || this.Method == HealthVaultMethods.GetServiceDefinition)
                        {
                            // These always use the Master app ID from configuration
                            appId = this.config.MasterApplicationId;
                        }
                        else
                        {
                            // Otherwise use app instance ID from connection.
                            appId = this.connectionInternal.ApplicationId;
                        }

                        writer.WriteElementString("app-id", appId.ToString());
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
                        "msg-ttl", this.config.DefaultRequestTimeToLive.ToString(CultureInfo.InvariantCulture));

                    writer.WriteElementString("version", this.Version);

                    // if we are not using an authenticated connection,
                    // then do not include the info-hash because we cannot protect it
                    //      with the auth section.
                    if (this.connectionInternal.SessionCredential != null)
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
                if (newStreamCreated)
                {
                    responseStream?.Dispose();
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
        public HealthVaultMethods Method { get; set; }

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
        internal string Version { get; }

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
                return this.parameters ?? (this.parameters = string.Empty);
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
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.TimeoutSeconds), Resources.TimeoutMustBePositive);
                }

                this.timeoutSeconds = value;
            }
        }

        internal Guid ResponseId
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// This is a test hook so that the derived testing class can
        /// verify the XML request.
        /// </summary>
        ///
        internal byte[] XmlRequest { get; private set; }

        internal int XmlRequestLength { get; private set; }
    }
}
