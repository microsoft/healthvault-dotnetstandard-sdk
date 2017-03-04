// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Rest;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Enables web applications to authenticate themselves,
    /// with or without the authentication context of a user.
    /// </summary>
    ///
    /// <remarks>
    /// Web applications can authenticate in two modes:
    ///
    /// First, an application can authenticate itself in the anonymous mode,
    /// that is, not in the context of a user. This mode enables application
    /// servers to communicate with the HealthVault Service as a trusted operation,
    /// but not explicitly on behalf of a user.
    ///
    /// Secondly, an application can authenticate itself in the context of a user.
    /// This mode enables applications to communicate with the HealthVault
    /// Service both as a trusted operation and in the user context. The
    /// application can therefore explicitly perform operations as that user
    /// or as trusted operations as in the anonymous case.
    ///
    /// The credential proves the application's identity by signing the
    /// authentication request with the application's private key.  The user's
    /// context is provided by the <cref name="SubCredential"/> property.
    /// </remarks>
    ///
    public class WebApplicationCredential : Credential
    {
        #region properties

        internal string DigestMethod { get; set; }

        internal string SignMethod { get; set; }

        /// <summary>
        /// Gets the thumbprint of the signing cert.
        /// </summary>
        ///
        internal string Thumbprint => this.cert.Thumbprint;

        /// <summary>
        /// Gets the application identifier of the credential.
        /// </summary>
        ///
        /// <returns>
        /// A GUID representing the application identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This property is set only internally.
        /// </remarks>
        ///
        public Guid ApplicationId { get; internal set; }

        /// <summary>
        /// Gets or sets the cached provider for signing binary large objects
        /// (blobs).
        /// </summary>
        ///
        /// <returns>
        /// An instance of <see cref="RSACryptoServiceProvider"/> representing
        /// the cached provider.
        /// </returns>
        ///
        internal RSACng RsaProvider
        {
            get { return (RSACng)this.cert.GetRSAPrivateKey(); }
        }

        private long TokenIssuedRefreshCounter
        {
            get { return this.tokenIssuedRefreshCounter; }
            set { this.tokenIssuedRefreshCounter = value; }
        }

        private long tokenIssuedRefreshCounter;

        private AuthenticatedSessionKeySet KeySet
        {
            get { return this.keySet; }
            set { this.keySet = value; }
        }

        private AuthenticatedSessionKeySet keySet;

        /// <summary>
        /// Gets or sets the sub-credential.
        /// </summary>
        ///
        /// <remarks>
        /// This is the credential token retrieved from the HealthVault
        /// platform.  By specifying a sub-credential, the web application
        /// credential can operate in the context of an authenticated user.
        /// </remarks>
        ///
        /// <returns>
        /// A string representing the sub-credential token.
        /// </returns>
        ///
        public string SubCredential { get; set; }

        private string certSubject;

        private StoreLocation storeLocation = StoreLocation.LocalMachine;
        private X509Certificate2 cert;
        private bool certOriginFromStore;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is used only for Activator-based
        /// deserialization of the cookie XML. This constructor does not call
        /// Initialize() here because it is expected to be initialized from
        /// a call to <cref name="ReadCookieXml"/>.
        /// </remarks>
        ///
        public WebApplicationCredential()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class with the specified application certificate.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The unique application identifier.
        /// </param>
        ///
        /// <param name="certificate">
        /// The application certificate containing the application's private key.
        /// </param>
        ///
        public WebApplicationCredential(
            Guid applicationId,
            X509Certificate2 certificate)
        {
            this.ApplicationId = applicationId;
            this.cert = certificate;
            this.DigestMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureHashAlgorithmName;
            this.SignMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureAlgorithmName;

            this.LoadAuthTokenPair(applicationId);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class with the default values and in the anonymous context.
        /// </summary>
        /// <param name="applicationId"> The guid associated with this instance of HealthVault</param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="applicationId"/> parameter is empty.
        /// </exception>
        ///
        /// <exception cref="SecurityException">
        /// The required application-specific certificate is not found.
        /// </exception>
        ///
        public WebApplicationCredential(
            Guid applicationId)
            : this(
                applicationId,
                ApplicationCertificateStore.Current.GetApplicationCertificate(applicationId))
        {
            this.certOriginFromStore = true;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class using the specified application identifier and sub-credential.
        /// </summary>
        ///
        /// <remarks>
        /// In order to enable a web application to authenticate with the
        /// HealthVault Service, the application must first create a
        /// <see cref="WebApplicationCredential"/>. The credential consists of
        /// two explicit parameters, and implicitly utilizes the web
        /// application's private key to sign the credential XML. The
        /// sub-credential is the token received from the browser, and it
        /// represents the approval for a user to run this application for the
        /// lifetime of the token.
        /// </remarks>
        /// <param name="applicationId"> The guid associated with this instance of HealthVault</param>
        /// <param name="subCredential"> The sub-credential of the current instance </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="applicationId"/> parameter is empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subCredential"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="SecurityException">
        /// The required application-specific certificate is not found.
        /// </exception>
        ///
        public WebApplicationCredential(
            Guid applicationId,
            string subCredential)
            : this(applicationId)
        {
            Validator.ThrowIfStringNullOrEmpty(subCredential, "subCredential");
            this.SubCredential = subCredential;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class using the specified application identifier and sub-credential.
        /// </summary>
        ///
        /// <remarks>
        /// In order to enable a web application to authenticate with the
        /// HealthVault Service, the application must first create a
        /// <see cref="WebApplicationCredential"/>. The credential consists of
        /// two explicit parameters, and implicitly utilizes the web
        /// application's private key to sign the credential XML. The
        /// sub-credential is the token received from the browser, and it
        /// represents the approval for a user to run this application for the
        /// lifetime of the token.
        /// </remarks>
        /// <param name="applicationId"> The application ID </param>
        /// <param name="subCredential"> The sub-credential associated with the application ID</param>
        /// <param name="certificate"> The security certificate associated with this application ID</param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="applicationId"/> parameter is empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subCredential"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="SecurityException">
        /// The required application-specific certificate is not found.
        /// </exception>
        ///
        public WebApplicationCredential(
            Guid applicationId,
            string subCredential,
            X509Certificate2 certificate)
            : this(applicationId, certificate)
        {
            Validator.ThrowIfStringNullOrEmpty(subCredential, "subCredential");
            this.SubCredential = subCredential;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/>
        /// class using the specified parameters
        /// </summary>
        ///
        /// <remarks>
        /// In order to enable a web application to authenticate with the
        /// HealthVault Service, the application must first create a
        /// <see cref="WebApplicationCredential"/>. The credential consists of
        /// three explicit parameters. The store location is where the certificate
        /// is stored. The subject for the certificate is used to lookup
        /// the certificate from the store
        /// </remarks>
        ///
        /// <param name="applicationId">
        /// Application Id.
        /// </param>
        ///
        /// <param name="storeLocation">
        /// Location of store where the certificate is stored.
        /// </param>
        ///
        /// <param name="certSubject">
        /// Subject of the certificate for the application.
        /// </param>
        ///
        public WebApplicationCredential(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            this.Initialize(applicationId, storeLocation, certSubject);
        }

        /// <summary>
        /// Initializes the credential.
        /// </summary>
        ///
        /// <remarks>
        /// This method is called during construction,is for internal use only,
        /// and is subject to change.
        /// </remarks>
        ///
        /// <param name="applicationId">
        /// The application identifier of the web application.
        /// </param>
        ///
        /// <param name="storeLocation">
        /// Location of store where the certificate is stored.
        /// </param>
        ///
        /// <param name="certSubject">
        /// Subject of the certificate for the application.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="applicationId"/> parameter is empty.
        /// </exception>
        ///
        /// <exception cref="SecurityException">
        /// The application certificate could not be found in the
        /// specified certificate store.
        /// </exception>
        ///
        private void Initialize(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            Validator.ThrowArgumentExceptionIf(
                applicationId == Guid.Empty,
                "applicationId",
                "WebApplicationCredentialAppIdEmpty");

            this.ApplicationId = applicationId;
            this.storeLocation = storeLocation;
            this.certSubject = certSubject;
            this.certOriginFromStore = true;

            this.SetupSignatureCertRsaProvider(applicationId, storeLocation, certSubject);

            this.LoadAuthTokenPair(applicationId);
        }

        private void LoadAuthTokenPair(
            Guid applicationId)
        {
            CreateAuthenticationTokenResult newResult;

            bool haveResults =
                GetAuthTokenPair(
                    applicationId,
                    out this.tokenIssuedRefreshCounter,
                    out this.keySet,
                    out newResult);

            if (haveResults)
            {
                // update the instance authentication results
                base.UpdateAuthenticationResults(newResult);
            }
        }

        /// <summary>
        /// Gets the application's private key using the specified
        /// certificate store.
        /// </summary>
        ///
        private void SetupSignatureCertRsaProvider(
            Guid applicationId,
            StoreLocation storeLocation,
            string certSubject)
        {
            this.DigestMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureHashAlgorithmName;
            this.SignMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureAlgorithmName;

            this.cert =
                ApplicationCertificateStore.Current.GetApplicationCertificateFromStore(
                    applicationId,
                    storeLocation,
                    certSubject);
        }

        #endregion

        #region credential overrides

        /// <summary>
        /// Retrieves a value indicating whether the specified
        /// <paramref name="applicationId"/> requires authentication.
        /// </summary>
        ///
        /// <remarks>
        /// This method verifies that the authentication token is valid
        /// for a limited timespan into the future, such that the token
        /// could be used to make a HealthVault Service method call
        /// within the validated timespan.
        /// </remarks>
        ///
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The application identifier to verify, if authentication is required.
        /// </param>
        ///
        internal override async Task AuthenticateIfRequiredAsync(
            IConnection connection,
            Guid applicationId)
        {
            if (IsAuthenticationExpired(applicationId, this.TokenIssuedRefreshCounter))
            {
                await this.AuthenticateAsync(connection, applicationId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Authenticates or re-authenticates the credential.
        /// </summary>
        ///
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The HealthVault application identifier.
        /// request is made for.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is null.
        /// </exception>
        ///
        /// <seealso cref="HealthServiceConnection"/>
        ///
        /// <returns>
        /// The credential to use for the request.
        /// </returns>
        ///
        private async Task AuthenticateAsync(
            IConnection connection,
            Guid applicationId)
        {
            await AuthenticateKeySetPairAsync(connection, applicationId, this.cert).ConfigureAwait(false);

            this.LoadAuthTokenPair(applicationId);
        }

        /// <summary>
        /// Expire an authentication keyset pair so that
        /// <cref name="AuthenticationRequired"/> will return true.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application identifier of the web application.
        /// </param>
        ///
        internal override bool ExpireAuthenticationResult(Guid applicationId)
        {
            // expire the instance authentication result as well
            base.ExpireAuthenticationResult(applicationId);

            ExpireKeySetPair(
                applicationId,
                this.TokenIssuedRefreshCounter);

            return !this.IsAuthenticationRetryDisabled;
        }

        /// <summary>
        /// Gets if the credential currently is enabled to support retries.
        /// </summary>
        ///
        internal bool IsAuthenticationRetryDisabled { get; set; }

        /// <summary>
        /// Derived classes can insert header section XML as appropriate.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application id context to write the header section for.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer that is constructing the header section.
        /// </param>
        ///
        internal override void GetHeaderSection(
            Guid applicationId,
            XmlWriter writer)
        {
            base.GetHeaderSection(
                applicationId,
                writer);

            if (!string.IsNullOrEmpty(this.SubCredential))
            {
                writer.WriteElementString(
                    "user-auth-token",
                    this.SubCredential);
            }
        }

        /// <summary>
        /// Derived classes can return header section query string parameter
        /// as appropriate.
        /// </summary>
        ///
        /// <return>
        /// The header section query string parameter.
        /// </return>
        ///
        internal override string GetHeaderSection(
            Guid applicationId)
        {
            string result = base.GetHeaderSection(applicationId);

            if (!string.IsNullOrEmpty(this.SubCredential))
            {
                result += WebUtility.UrlEncode(
                    "<user-auth-token>"
                    + this.SubCredential
                    + "</user-auth-token>");
            }

            return result;
        }

        /// <summary>
        /// Adds the REST Authorization header tokens to the provided tokens collection for
        /// the given application ID.
        /// </summary>
        ///
        /// <param name="tokens">
        /// The collection of Authorization tokens to add to.
        /// </param>
        ///
        /// <param name="appId">
        /// The application ID context to write the token for.
        /// </param>
        internal override void AddRestAuthorizationHeaderToken(IList<string> tokens, Guid appId)
        {
            base.AddRestAuthorizationHeaderToken(tokens, appId);

            if (!string.IsNullOrEmpty(this.SubCredential))
            {
                tokens.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        RestConstants.AuthorizationHeaderElement,
                        RestConstants.UserToken,
                        this.SubCredential));
            }
        }

        /// <summary>
        /// Inserts a dictionary of authentication results.
        /// </summary>
        ///
        /// <remarks>
        /// This may be called from two primary code paths:
        ///
        /// 1) WebApplicationCredential.AuthenticateKeySetPairAsync
        ///     pair is already locked by calling thread.
        /// 2) and CreateAuthenticatedSessionToken
        ///     a) the credential could be totally unintialized, including the
        ///         static credentials, so there will not be a lock on the pair.
        ///         But it doesn't matter since there are no contending threads
        ///         to grab the pair during this call.  The lock call is a slight
        ///         overhead in this very rare case.
        ///     b) the credential already has an existing token, including
        ///         the static credentials.  This call will update the credentials
        ///         so it's possible the pair is being used by another thread.
        ///
        /// This method only update results for the application id for the web
        /// app.
        /// </remarks>
        ///
        /// <param name="result">
        /// The result of the authentication request.
        /// </param>
        [SecuritySafeCritical]
        internal override void UpdateAuthenticationResults(
            CreateAuthenticationTokenResult result)
        {
            if (result.ApplicationId == this.ApplicationId)
            {
                if (CreateAuthenticationTokenResult.IsAuthenticated(result))
                {
                    AuthenticationTokenKeySetPair pair = liveKeySetPairs.GetPair(this.ApplicationId) ??
                                                         liveKeySetPairs.CreatePair(this.ApplicationId);

                    lock (pair)
                    {
                        // update the static keyset pair
                        pair.KeySet = this.KeySet.Clone();
                        pair.Update(result);
                    }

                    // update the instance with the authentication info
                    this.LoadAuthTokenPair(this.ApplicationId);
                }
            }
        }

        /// <summary>
        /// Parses any method specific elements in the response.
        /// </summary>
        ///
        /// <param name="nav">
        /// The response XML path navigator.
        /// </param>
        [SecuritySafeCritical]
        internal override void ParseExtendedElements(XPathNavigator nav)
        {
            XPathNavigator keyNav = nav.SelectSingleNode("shared-secret");
            if (keyNav != null)
            {
                this.KeySet.HMAC.KeyMaterial = Convert.FromBase64String(keyNav.Value);
            }
        }

        #endregion

        #region data authentication

        /// <summary>
        /// Provides a mechanism for derived classes to override how this class
        /// authenticates data.
        /// </summary>
        ///
        /// <remarks>
        /// This method is for internal use only and is subject to change.
        /// </remarks>
        ///
        /// <param name="data">
        /// The data to be authenticated by the credential.
        /// </param>
        ///
        /// <param name="index">
        /// The starting index into the data.
        /// </param>
        ///
        /// <param name="count">
        /// The number of bytes from the start index to authenticate.
        /// </param>
        ///
        /// <returns>
        /// A string representing the data that was authenticated by the credential.
        /// </returns>
        ///
        protected virtual string AuthenticateWebApplicationData(
            byte[] data,
            int index,
            int count)
        {
            CryptoHmac hmac = this.KeySet.HMAC;

            hmac.Reset();
            hmac.ComputeHash(data, index, count);
            return hmac.Finalize().GetXml();
        }

        /// <summary>
        /// Applies the shared secret to the specified data.
        /// </summary>
        ///
        /// <remarks>
        /// After the initial authentication is made with the HealthVault Service,
        /// all subsequent calls to the HealthVault service must have
        /// authenticated header sections. This method produces the
        /// Hash Message Authentication Code (HMAC) data for the auth section.
        ///
        /// This method implements its own shared secret,
        /// so the SharedSecret property is <b>null</b>.
        /// </remarks>
        ///
        /// <param name="data">
        /// The data to authenticate.
        /// </param>
        ///
        /// <param name="index">
        /// The starting index into the data.
        /// </param>
        ///
        /// <param name="count">
        /// The number of bytes from the start index to authenticate.
        /// </param>
        ///
        /// <returns>
        /// The authenticated data is returned.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="data"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        internal override string AuthenticateData(
            byte[] data,
            int index,
            int count)
        {
            return this.AuthenticateWebApplicationData(data, index, count);
        }

        /// <summary>
        /// Compute and apply the signature to the request XML.
        /// </summary>
        ///
        private string SignRequestXml(string requestXml)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            byte[] paramBlob = encoding.GetBytes(requestXml);
            byte[] sigBlob = this.RsaProvider.SignData(paramBlob, new HashAlgorithmName(this.DigestMethod), RSASignaturePadding.Pkcs1); // SignData(paramBlob, DigestMethod);

            return Convert.ToBase64String(sigBlob);
        }

        #endregion

        #region info xml

        /// <summary>
        /// Generate the to-be signed content for the credential.
        /// </summary>
        ///
        /// <returns>
        /// Raw XML representing the ContentSection of the info secttion.
        /// </returns>
        ///
        internal string GetContentSection()
        {
            StringBuilder requestXml = new StringBuilder(2048);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(requestXml, settings))
            {
                writer.WriteStartElement("content");

                writer.WriteStartElement("app-id");
                writer.WriteString(this.ApplicationId.ToString());
                writer.WriteEndElement();

                writer.WriteElementString("hmac", ConfigurationBase.Current.CryptoConfiguration.HmacAlgorithmName);

                writer.WriteStartElement("signing-time");
                writer.WriteValue(DateTime.Now.ToUniversalTime());
                writer.WriteEndElement();

                writer.WriteEndElement(); // content
            }

            return requestXml.ToString();
        }

        /// <summary>
        /// Writes the XML that is used when authenticating with the
        /// HealthVault Service.
        /// </summary>
        ///
        /// <remarks>
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        /// <param name="writer">
        /// The XML writer that is written to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        [SecuritySafeCritical]
        public override void WriteInfoXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement("appserver2");

            string requestXml = this.GetContentSection();

            // SIG
            writer.WriteStartElement("sig");
            writer.WriteAttributeString("digestMethod", this.DigestMethod);
            writer.WriteAttributeString("sigMethod", this.SignMethod);
            writer.WriteAttributeString("thumbprint", this.Thumbprint);
            writer.WriteString(this.SignRequestXml(requestXml));
            writer.WriteEndElement(); // sig

            // CONTENT
            writer.WriteRaw(requestXml);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the browser cookie xml for this credential.
        /// </summary>
        ///
        /// <remarks>
        /// Since this generates cookie xml, it does not reveal any secrets.
        /// Rather, we use secret id's so that we may look up the secret later.
        /// </remarks>
        ///
        /// <param name="writer">
        /// The writer building the cookie XML.
        /// </param>
        [SecuritySafeCritical]
        internal override void WriteCookieXml(XmlWriter writer)
        {
            // write the type info
            base.WriteCookieXml(writer);

            writer.WriteStartElement("appserver");
            writer.WriteElementString("app-id", this.ApplicationId.ToString());
            writer.WriteElementString("user-auth-token", this.SubCredential);

            if (this.certOriginFromStore)
            {
                writer.WriteStartElement("cert-from-store");

                writer.WriteElementString("store", this.storeLocation.ToString());

                if (this.certSubject != null)
                {
                    writer.WriteElementString("subject", this.certSubject);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // appserver
        }

        /// <summary>
        /// Reconstruct the credential using cookie xml.
        /// </summary>
        ///
        /// <remarks>
        /// After enough configuration information is read from the cookie XML,
        /// the class instance is initialized via <cref name="Initialize"/>.
        /// </remarks>
        ///
        /// <param name="reader">
        /// XML reader of the cookie XML.
        /// </param>
        ///
        internal override void ReadCookieXml(XmlReader reader)
        {
            reader.ReadStartElement("appserver");

            Guid applicationId = new Guid(reader.ReadElementContentAsString());
            this.SubCredential = reader.ReadElementContentAsString();

            if (reader.LocalName.Equals("cert-from-store", StringComparison.OrdinalIgnoreCase))
            {
                reader.ReadStartElement();

                this.storeLocation =
                    (StoreLocation)Enum.Parse(typeof(StoreLocation), reader.ReadElementContentAsString());

                if (reader.LocalName.Equals("subject", StringComparison.OrdinalIgnoreCase))
                {
                    this.certSubject = reader.ReadElementContentAsString();
                }

                reader.ReadEndElement();
                this.Initialize(applicationId, this.storeLocation, this.certSubject);
            }
            else
            {
                this.ApplicationId = applicationId;
                this.cert = ApplicationCertificateStore.Current.ApplicationCertificate;
                this.DigestMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureHashAlgorithmName;
                this.SignMethod = ConfigurationBase.Current.CryptoConfiguration.SignatureAlgorithmName;
                this.certOriginFromStore = false;

                this.LoadAuthTokenPair(applicationId);
            }

            if (!reader.EOF)
            {
                reader.ReadEndElement(); // appserver
            }
        }

        #endregion

        #region static authentication results management

        private static AuthSessionKeySetPairs liveKeySetPairs = new AuthSessionKeySetPairs();

        /// <summary>
        /// Release lock of the pair so that the keyset pair after the request
        /// is complete.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application the request is for.
        /// </param>
        ///
        /// <param name="refreshCounter">
        /// The time stamp of the calling credential.  If this does not
        /// match the keyset pair's timestamp, then we do not update the
        /// expired status.
        /// </param>
        ///
        private static void ExpireKeySetPair(Guid applicationId, long refreshCounter)
        {
            AuthenticationTokenKeySetPair pair =
                liveKeySetPairs.GetPair(applicationId);

            if (pair == null)
            {
                // internal error, we should always have the application id
                // at this point.
                throw Validator.HealthServiceException("ConnectionNotAuthenticated");
            }

            lock (pair)
            {
                if (refreshCounter == pair.RefreshCounter)
                {
                    pair.IsAuthenticationResultExpired = true;
                }
            }
        }

        /// <summary>
        /// Gets if a keyset is active or not.
        /// </summary>
        ///
        /// <remarks>
        /// This method verifies that the authentication token is valid
        /// for a limited timespan into the future, such that the token
        /// could be used to make a Microsoft Health Service method call
        /// within the validated timespan.
        /// </remarks>
        ///
        /// <param name="applicationId">
        /// The application id to verify if authentication is required.
        /// </param>
        ///
        /// <param name="refreshCounter">
        /// The calling credential's refresh counter.
        /// </param>
        ///
        /// <returns>
        /// True if <cref name="AuthenticateKeySetPairAsync"/> needs to be called.
        /// </returns>
        ///
        private static bool IsAuthenticationExpired(
            Guid applicationId,
            long refreshCounter)
        {
            AuthenticationTokenKeySetPair pair =
                liveKeySetPairs.GetPair(applicationId);

            if (pair == null)
            {
                return true;
            }

            lock (pair)
            {
                return pair.IsAuthenticationExpired(refreshCounter);
            }
        }

        /// <summary>
        /// Gets the cached authentication keyset pair for the application id.
        /// </summary>
        ///
        /// <param name="applicationId">The uuid associated with this application</param>
        /// <param name="refreshCounter">the time until a credential refresh is needed</param>
        /// <param name="keySet">out parameter: the authorized keys associated with this account</param>
        /// <param name="result">out parameter: the result of the authentication attempt</param>
        ///
        private static bool GetAuthTokenPair(
            Guid applicationId,
            out long refreshCounter,
            out AuthenticatedSessionKeySet keySet,
            out CreateAuthenticationTokenResult result)
        {
            keySet = null;
            result = null;
            refreshCounter = 0;

            if (liveKeySetPairs == null)
            {
                return false;
            }

            AuthenticationTokenKeySetPair pair = liveKeySetPairs.GetPair(applicationId);

            if (pair == null)
            {
                // create a new unauthenticated pair so that we at least
                // have the KeySet
                pair = liveKeySetPairs.CreatePair(applicationId);

                keySet = pair.KeySet.Clone();

                return false;
            }

            lock (pair)
            {
                if (!pair.IsAuthenticated())
                {
                    keySet = pair.KeySet.Clone();

                    return false;
                }

                refreshCounter = pair.RefreshCounter;
                keySet = pair.KeySet.Clone();
                result = pair.AuthenticationResult;

                return true;
            }
        }

        /// <summary>
        /// Authenticate the keyset pair for the specified
        /// <paramref name="applicationId"/> by calling to the Microsoft Health
        /// Service to create a new authentication token.
        /// </summary>
        ///
        /// <remarks>
        /// In order to avoid unnecessary authentication actions, it is
        /// expected that the caller will first call
        /// <cref name="IsAuthenticationExpired"/> before calling this.
        /// </remarks>
        ///
        /// <param name="connection">
        /// The connection used to perform the authentication.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The application id of the keyset pair that will be authenticated.
        /// </param>
        ///
        /// <param name="certificate">
        /// The application's certificate containing the application's private key.
        /// </param>
        ///
        private static async Task AuthenticateKeySetPairAsync(
            IConnection connection,
            Guid applicationId,
            X509Certificate2 certificate)
        {
            await AuthenticateKeySetPairAsync(liveKeySetPairs, connection, applicationId, certificate).ConfigureAwait(false);
        }

        /// <summary>
        /// Authenticate the keyset pair for the specified
        /// <paramref name="applicationId"/> by calling to the Microsoft Health
        /// Service to create a new authentication token.
        /// </summary>
        ///
        /// <remarks>
        /// In order to avoid unnecessary authentication actions, it is
        /// expected that the caller will first call
        /// <cref name="IsAuthenticationExpired"/> before calling this.
        /// </remarks>
        ///
        /// <param name="keySetPairs">
        /// The keyset pairs collection that will contain the newly
        /// authenticated keyset pair.
        /// </param>
        ///
        /// <param name="connection">
        /// The connection used to perform the authentication.
        /// </param>
        ///
        /// <param name="applicationId">
        /// The application id of the keyset pair that will be authenticated.
        /// </param>
        ///
        /// <param name="certificate">
        /// The application's certificate containing the application's private key.
        /// </param>
        ///
        private static async Task AuthenticateKeySetPairAsync(
            AuthSessionKeySetPairs keySetPairs,
            IConnection connection,
            Guid applicationId,
            X509Certificate2 certificate)
        {
            AuthenticationTokenKeySetPair pair = keySetPairs.GetPair(applicationId) ?? keySetPairs.CreatePair(applicationId);

            if (!pair.IsAuthenticated())
            {
                lock (pair)
                {
                    if (!pair.IsAuthenticated())
                    {
                        if (pair.IsAuthenticationResultExpired)
                        {
                            // the existing live pair is already authenticated
                            // so create a new one with a fresh keyset
                            pair.RefreshSharedSecret();
                        }
                    }
                }

                WebApplicationCredential cred =
                            new WebApplicationCredential(applicationId, certificate) { KeySet = pair.KeySet.Clone() };

                // create the new token
                // this will implicitly result in a call to
                // UpdateAuthenticationResults
                await cred.CreateAuthenticatedSessionTokenAsync(connection, applicationId).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
