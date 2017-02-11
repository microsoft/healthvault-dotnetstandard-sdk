// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.Authentication;
using Microsoft.Health.Rest;

namespace Microsoft.Health.Web.Authentication
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

        internal string DigestMethod
        {
            get { return _digestMethod; }
            set { _digestMethod = value; }
        }
        private string _digestMethod;

        internal string SignMethod
        {
            get { return _signMethod; }
            set { _signMethod = value; }
        }
        private string _signMethod;

        /// <summary> 
        /// Gets the thumbprint of the signing cert.
        /// </summary>
        /// 
        internal string Thumbprint
        {
            get { return _cert.Thumbprint; }
        }

        /// <summary>
        /// Gets or sets the application identifier of the credential.
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
        public Guid ApplicationId
        {
            get { return _applicationId; }
            internal set { _applicationId = value; }
        }
        private Guid _applicationId;

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
        internal RSACryptoServiceProvider RsaProvider
        {
            get { return (RSACryptoServiceProvider)_cert.PrivateKey; }
        }

        private Int64 TokenIssuedRefreshCounter
        {
            get { return _tokenIssuedRefreshCounter; }
            set { _tokenIssuedRefreshCounter = value; }
        }
        private Int64 _tokenIssuedRefreshCounter;

        private AuthenticatedSessionKeySet KeySet
        {
            get { return _keySet; }
            set { _keySet = value; }
        }
        private AuthenticatedSessionKeySet _keySet;

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
        public string SubCredential
        {
            get { return _subCredential; }
            set { _subCredential = value; }
        }
        private string _subCredential;

        private string _certSubject;

        private StoreLocation _storeLocation = StoreLocation.LocalMachine;
        private X509Certificate2 _cert;
        private bool _certOriginFromStore;

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
            _applicationId = applicationId;
            _cert = certificate;
            _digestMethod = HealthApplicationConfiguration.Current.SignatureHashAlgorithmName;
            _signMethod = HealthApplicationConfiguration.Current.SignatureAlgorithmName;

            LoadAuthTokenPair(applicationId);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebApplicationCredential"/> 
        /// class with the default values and in the anonymous context.
        /// </summary>
        /// <param name="applicationId"/>
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
                HealthApplicationConfiguration.Current.GetApplicationCertificate(applicationId))
        {
            _certOriginFromStore = true;
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
        /// <param name="applicationId"/>
        /// <param name="subCredential"/>
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
            SubCredential = subCredential;
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
        /// <param name="applicationId"/>
        /// <param name="subCredential"/>
        /// <param name="certificate"/>
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
            SubCredential = subCredential;
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
            Initialize(applicationId, storeLocation, certSubject);
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

            ApplicationId = applicationId;
            _storeLocation = storeLocation;
            _certSubject = certSubject;
            _certOriginFromStore = true;

            SetupSignatureCertRsaProvider(applicationId, storeLocation, certSubject);

            LoadAuthTokenPair(applicationId);
        }

        private void LoadAuthTokenPair(
            Guid applicationId)
        {
            CreateAuthenticationTokenResult newResult;

            bool haveResults =
                WebApplicationCredential.GetAuthTokenPair(
                    applicationId,
                    out this._tokenIssuedRefreshCounter,
                    out this._keySet,
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
            DigestMethod = CryptoConfiguration.SignatureHashAlgorithmName;
            SignMethod = CryptoConfiguration.SignatureAlgorithmName;

            _cert =
                HealthApplicationConfiguration.Current.GetApplicationCertificateFromStore(
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
        /// <returns>
        /// <b>true</b> if calling <cref name="Authenticate"/> is required;
        /// otherwise, <b>false</b>. 
        /// </returns>
        /// 
        internal override void AuthenticateIfRequired(
            HealthServiceConnection connection,
            Guid applicationId)
        {
            if (WebApplicationCredential.IsAuthenticationExpired(
                    applicationId,
                    this.TokenIssuedRefreshCounter))
            {
                Authenticate(
                    connection,
                    applicationId);
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
        private void Authenticate(
            HealthServiceConnection connection,
            Guid applicationId)
        {
            WebApplicationCredential.AuthenticateKeySetPair(
                connection,
                applicationId,
                this._cert);

            LoadAuthTokenPair(applicationId);
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

            WebApplicationCredential.ExpireKeySetPair(
                applicationId,
                TokenIssuedRefreshCounter);

            return !IsAuthenticationRetryDisabled;
        }

        /// <summary>
        /// Gets if the credential currently is enabled to support retries.
        /// </summary>
        /// 
        internal bool IsAuthenticationRetryDisabled
        {
            get { return _isAuthenticationRetryDisabled; }
            set { _isAuthenticationRetryDisabled = value; }
        }
        private bool _isAuthenticationRetryDisabled;

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

            if (!String.IsNullOrEmpty(SubCredential))
            {
                writer.WriteElementString(
                    "user-auth-token",
                    SubCredential);
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

            if (!String.IsNullOrEmpty(SubCredential))
            {
                result += HttpUtility.UrlEncode(
                    "<user-auth-token>"
                    + SubCredential
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

            if (!String.IsNullOrEmpty(SubCredential))
            {
                tokens.Add(
                    String.Format(
                        CultureInfo.InvariantCulture,
                        RestConstants.AuthorizationHeaderElement,
                        RestConstants.UserToken,
                        SubCredential));
            }
        }

        /// <summary>
        /// Inserts a dictionary of authentication results.
        /// </summary>
        /// 
        /// <remarks>
        /// This may be called from two primary code paths:
        /// 
        /// 1) WebApplicationCredential.AuthenticateKeySetPair
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
            if (result.ApplicationId == ApplicationId)
            {
                if (CreateAuthenticationTokenResult.IsAuthenticated(result))
                {
                    AuthenticationTokenKeySetPair pair = _liveKeySetPairs.GetPair(ApplicationId);

                    if (pair == null)
                    {
                        // this should only be true for a call to
                        // CreateAuthenticatedSessionToken on an 
                        // unitialized WebApplicationCredential state -
                        // both static and instance.
                        pair = _liveKeySetPairs.CreatePair(ApplicationId);
                    }

                    lock (pair)
                    {
                        // update the static keyset pair
                        pair.KeySet = KeySet.Clone();
                        pair.Update(result);
                    }

                    // update the instance with the authentication info
                    LoadAuthTokenPair(ApplicationId);
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
                KeySet.HMAC.KeyMaterial = Convert.FromBase64String(keyNav.Value);
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
            string result = String.Empty;

            CryptoHmac hmac = KeySet.HMAC;

            hmac.Reset();
            hmac.ComputeHash(data, index, count);
            result = hmac.Finalize().GetXml();

            return result;
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
            return AuthenticateWebApplicationData(data, index, count);
        }

        /// <summary>
        /// Compute and apply the signature to the request XML.
        /// </summary>
        /// 
        private string SignRequestXml(string requestXml)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            Byte[] paramBlob = encoding.GetBytes(requestXml);
            Byte[] sigBlob = RsaProvider.SignData(paramBlob, DigestMethod);

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

            XmlWriter writer = null;

            try
            {
                writer = XmlWriter.Create(requestXml, settings);

                writer.WriteStartElement("content");

                writer.WriteStartElement("app-id");
                writer.WriteString(ApplicationId.ToString());
                writer.WriteEndElement();

                writer.WriteElementString("hmac", CryptoConfiguration.HmacAlgorithmName);

                writer.WriteStartElement("signing-time");
                writer.WriteValue(DateTime.Now.ToUniversalTime());
                writer.WriteEndElement();

                writer.WriteEndElement(); // content
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
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

            string requestXml = GetContentSection();

            // SIG
            writer.WriteStartElement("sig");
            writer.WriteAttributeString("digestMethod", DigestMethod);
            writer.WriteAttributeString("sigMethod", SignMethod);
            writer.WriteAttributeString("thumbprint", Thumbprint);
            writer.WriteString(SignRequestXml(requestXml));
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
            writer.WriteElementString("app-id", ApplicationId.ToString());
            writer.WriteElementString("user-auth-token", SubCredential);

            if (_certOriginFromStore)
            {
                writer.WriteStartElement("cert-from-store");

                writer.WriteElementString("store", _storeLocation.ToString());

                if (_certSubject != null)
                {
                    writer.WriteElementString("subject", _certSubject);
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
            SubCredential = reader.ReadElementContentAsString();

            if (reader.LocalName.Equals("cert-from-store", StringComparison.OrdinalIgnoreCase))
            {
                reader.ReadStartElement();

                _storeLocation =
                    (StoreLocation)
                    Enum.Parse(typeof(StoreLocation), reader.ReadElementContentAsString());

                if (reader.LocalName.Equals("subject", StringComparison.OrdinalIgnoreCase))
                {
                    _certSubject = reader.ReadElementContentAsString();
                }

                reader.ReadEndElement();
                Initialize(applicationId, _storeLocation, _certSubject);
            }
            else
            {
                _applicationId = applicationId;
                _cert = HealthApplicationConfiguration.Current.ApplicationCertificate;
                _digestMethod = HealthApplicationConfiguration.Current.SignatureHashAlgorithmName;
                _signMethod = HealthApplicationConfiguration.Current.SignatureAlgorithmName;
                _certOriginFromStore = false;

                LoadAuthTokenPair(applicationId);
            }

            if (!reader.EOF)
            {
                reader.ReadEndElement(); // appserver
            }

        }

        #endregion

        #region static authentication results management

        private static AuthSessionKeySetPairs _liveKeySetPairs = new AuthSessionKeySetPairs();

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
        private static void ExpireKeySetPair(Guid applicationId, Int64 refreshCounter)
        {
            AuthenticationTokenKeySetPair pair =
                _liveKeySetPairs.GetPair(applicationId);

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
        /// True if <cref name="AuthenticateKeySetPair"/> needs to be called.
        /// </returns>
        /// 
        private static bool IsAuthenticationExpired(
            Guid applicationId,
            Int64 refreshCounter)
        {
            AuthenticationTokenKeySetPair pair =
                _liveKeySetPairs.GetPair(applicationId);

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
        /// <param name="applicationId"></param>
        /// <param name="refreshCounter"></param>
        /// <param name="keySet"></param>
        /// <param name="result"></param>
        /// 
        private static bool GetAuthTokenPair(
            Guid applicationId,
            out Int64 refreshCounter,
            out AuthenticatedSessionKeySet keySet,
            out CreateAuthenticationTokenResult result)
        {
            keySet = null;
            result = null;
            refreshCounter = 0;

            if (_liveKeySetPairs == null)
            {
                return false;
            }

            AuthenticationTokenKeySetPair pair = _liveKeySetPairs.GetPair(applicationId);

            if (pair == null)
            {
                // create a new unauthenticated pair so that we at least 
                // have the KeySet
                pair = _liveKeySetPairs.CreatePair(applicationId);

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
        private static void AuthenticateKeySetPair(
            HealthServiceConnection connection,
            Guid applicationId,
            X509Certificate2 certificate)
        {
            AuthenticateKeySetPair(
                _liveKeySetPairs,
                connection,
                applicationId,
                certificate);
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
        private static void AuthenticateKeySetPair(
            AuthSessionKeySetPairs keySetPairs,
            HealthServiceConnection connection,
            Guid applicationId,
            X509Certificate2 certificate)
        {
            AuthenticationTokenKeySetPair pair =
                keySetPairs.GetPair(applicationId);

            if (pair == null)
            {
                pair = keySetPairs.CreatePair(applicationId);
            }

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

                        WebApplicationCredential cred =
                            new WebApplicationCredential(applicationId, certificate);

                        cred.KeySet = pair.KeySet.Clone();

                        // create the new token
                        // this will implicitly result in a call to
                        // UpdateAuthenticationResults
                        cred.CreateAuthenticatedSessionToken(
                            connection,
                            applicationId);
                    }
                }
            }
        }

        #endregion
    }
}

