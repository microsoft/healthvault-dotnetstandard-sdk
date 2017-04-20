// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport.MessageFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.AuthenticationFormatters;

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
    internal class HealthServiceMessage
    {
        private readonly AuthenticationFormatter authenticationFormatter;
        private readonly IAuthSessionOrAppId authSessionOrAppId;
        private readonly TimeSpan? requestTtl;

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceMessage" />
        /// class for the specified method.
        /// </summary>
        /// <param name="method">The method to invoke on the service.</param>
        /// <param name="methodVersion">The version of the method to invoke on the service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="recordId">RecordId</param>
        /// <param name="requestTtl">The request TTL.</param>
        /// <param name="sdkTelemetryInformation">Telemetry Information</param>
        /// <param name="authenticationFormatter">The authentication formatter.</param>
        /// <param name="authSessionOrAppId">The authentication session or application identifier.</param>
        public HealthServiceMessage(
            HealthVaultMethods method,
            int methodVersion,
            string parameters,
            Guid? recordId = null,
            TimeSpan? requestTtl = null,
            SdkTelemetryInformation sdkTelemetryInformation = null,
            AuthenticationFormatter authenticationFormatter = null,
            IAuthSessionOrAppId authSessionOrAppId = null)
        {
            this.Method = method;
            this.authenticationFormatter = authenticationFormatter;
            this.authSessionOrAppId = authSessionOrAppId;
            this.parameters = parameters;
            this.requestTtl = requestTtl ?? Ioc.Get<HealthVaultConfiguration>().RequestTimeToLiveDuration;
            this.MethodVersion = methodVersion;
            this.CultureCode = CultureInfo.CurrentUICulture.Name;
            this.RecordId = recordId.GetValueOrDefault(Guid.Empty);
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
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be disposed multiple times. Usings block makes the code more readable")]
        public void BuildRequestXml(string transform = null)
        {
            InfoSerializer info = new InfoSerializer(this.parameters);

            HeaderSerializer headerSection = new HeaderSerializer(
                this.Method,
                this.MethodVersion?.ToString(),
                this.CultureCode,
                this.Version,
                this.RecordId,
                this.TargetPersonId,
                transform,
                this.requestTtl, 
                this.authSessionOrAppId,
                info);

            using (MemoryStream requestXml = new MemoryStream())
            using (XmlWriter writer = XmlWriter.Create(requestXml, SDKHelper.XmlUtf8WriterSettings))
            {
                using (new TagWriter(writer, "request", "wc-request", "urn:com.microsoft.wc.request"))
                {
                    this.authenticationFormatter.Write(writer, headerSection);
                    headerSection.Write(writer);
                    info.Write(writer);
                }

                writer.Flush();
                this.XmlRequest = requestXml.ToArray();
                this.XmlRequestLength = (int)requestXml.Length;
            }
        }

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
        public Guid TargetPersonId { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        ///
        /// <returns>
        /// A GUID representing the identifier.
        /// </returns>
        ///
        public Guid RecordId { get; set; }

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

        private string parameters;

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
                // xml element's inner text - we'd have to do the value check
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

        /// <summary>
        /// Gets the method.
        /// </summary>
        public HealthVaultMethods Method { get; private set; }

        internal Guid ResponseId
        {
            get; set;
        }

        /// <summary>
        /// This is a test hook so that the derived testing class can
        /// verify the XML request.
        /// </summary>
        ///
        internal byte[] XmlRequest { get; private set; }

        internal int XmlRequestLength { get; private set; }
    }
}
