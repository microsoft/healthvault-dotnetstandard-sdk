// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.Rest;
using Microsoft.Health.Web.Authentication;

namespace Microsoft.Health.Authentication
{
    /// <summary>
    /// Provides base functionality for all Microsoft HealthVault service 
    /// authentication credential types. This class is abstract.
    /// </summary>
    /// 
    /// <remarks>
    /// Credentials serve as the basis for all authentication with the 
    /// Microsoft HealthVault service. Each credential class implements a specific
    /// credential type. As more credential types are required, a new 
    /// credential is required for each.
    /// You should inherit from this class only if support for a new 
    /// HealthVault authentication credential type is required.
    /// </remarks>
    /// 
    public abstract class Credential
    {
        #region properties

        /// <summary>
        /// Gets or sets the credential's shared secret.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="CryptoHash"/> representing the shared 
        /// secret.
        /// </returns>
        /// 
        /// <remarks>
        /// When a credential is used to establish an authenticated session
        /// with the Microsoft HealthVault service, it uses a shared secret
        /// to authenticate data transmissions. If the credential does not
        /// support an authenticated session, this property will be <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The value is set to <b>null</b>.
        /// </exception>
        /// 
        public CryptoHash SharedSecret
        {
            get { return _sharedSecret; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("value");
                }
                _sharedSecret = value;
            }
        }
        private CryptoHash _sharedSecret;

        private IDictionary<Guid, CreateAuthenticationTokenResult> AuthenticationResults
        {
            get { return _authResults; }
        }
        private Dictionary<Guid, CreateAuthenticationTokenResult> _authResults;

        /// <summary>
        /// Gets the credential's authentication results from prior successful 
        /// authentications with the Microsoft HealthVault service.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The application that was previously authenticated.
        /// </param>
        /// 
        /// <value>
        /// An instance of <see cref="CreateAuthenticationTokenResult"/>.
        /// </value>
        /// 
        /// <remarks>
        /// The key of the dictionary is the application id of the application
        /// for which the authentication request was made. If an application ID
        /// is not present in the dictionary, then an authentication request
        /// has not been made for this application, and this method therefore 
        /// returns <b>null</b>.
        /// </remarks>
        /// 
        public virtual CreateAuthenticationTokenResult GetAuthenticationResult(Guid applicationId)
        {
            if (AuthenticationResults == null
                || AuthenticationResults.Count == 0
                || !AuthenticationResults.ContainsKey(applicationId))
            {
                return null;
            }

            return AuthenticationResults[applicationId];
        }

        /// <summary>
        /// Retrieves a value indicating whether an authentication request
        /// expires.
        /// <cref name="AuthenticationRequired"/> returns <b>true</b>.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The application identifier of the web application.
        /// </param>
        /// 
        /// <returns>
        /// <b>true</b> if the request should attempt reauthentication; 
        /// otherwise, <b>false</b>.
        /// </returns>
        /// 
        internal virtual bool ExpireAuthenticationResult(Guid applicationId)
        {
            if (AuthenticationResults != null)
            {
                AuthenticationResults.Remove(applicationId);
            }

            return false;
        }

        /// <summary>
        /// Gets the authentication HealthVault service method name.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the name.
        /// </returns>
        /// 
        /// <remarks>
        /// The authorization method changes based on the credential type.
        /// Some credentials are publicly available and some must be used only 
        /// by the shell.
        /// </remarks>
        /// 
        internal string AuthenticationMethodName
        {
            get { return _authMethodName; }
            set { _authMethodName = value; }
        }
        private string _authMethodName = "CreateAuthenticatedSessionToken";

        /// <summary>
        /// Authenticates an <paramref name="applicationId"/> if none exists.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use for reauthentication, if necessary.
        /// </param>
        /// 
        /// <param name="applicationId">
        /// The application ID to verify if authentication is required.
        /// </param>
        /// 
        internal virtual void AuthenticateIfRequired(
            HealthServiceConnection connection,
            Guid applicationId)
        {
            if (GetAuthenticationResult(applicationId) == null)
            {
                Authenticate(connection, applicationId);
            }
        }

        /// <summary>
        /// Authenticates or re-authenticates the credential.
        /// </summary>
        /// 
        private void Authenticate(
            HealthServiceConnection connection,
            Guid appId)
        {
            if (AuthenticationResults != null)
            {
                AuthenticationResults.Remove(appId);
            }

            CreateAuthenticatedSessionToken(connection, appId);
        }

        #endregion

        /// <summary>
        /// Retrieves a header section given the specified application ID and 
        /// XMLWriter.
        /// </summary>
        /// 
        /// <param name="appId">
        /// The application ID context to write the header section for.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer that is constructing the header section.
        /// </param>
        /// 
        /// <remarks>
        /// Derived classes can insert header section XML as appropriate.
        /// </remarks>
        /// 
        /// <exception cref="HealthServiceException">
        /// The <paramref name="appId"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        internal virtual void GetHeaderSection(
            Guid appId,
            XmlWriter writer)
        {
            CreateAuthenticationTokenResult result = GetAuthenticationResult(appId);

            if (result == null)
            {
                throw Validator.HealthServiceException("ConnectionNotAuthenticated");
            }

            // <auth-token>
            writer.WriteElementString(
                "auth-token",
                result.AuthenticationToken);
        }

        /// <summary>
        /// Retrieves a header section given the specified application ID.
        /// </summary>
        /// 
        /// <param name="appId">
        /// The application ID context to write the header section for.
        /// </param> 
        /// 
        /// <returns>
        /// A string representing the header section query parameter.
        /// </returns>
        /// 
        /// <remarks>
        /// Derived classes can return header section query string parameter 
        /// as appropriate.
        /// </remarks> 
        /// 
        /// <exception cref="HealthServiceException">
        /// The <paramref name="appId"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        internal virtual string GetHeaderSection(Guid appId)
        {
            CreateAuthenticationTokenResult result = GetAuthenticationResult(appId);

            if (result == null)
            {
                throw Validator.HealthServiceException("ConnectionNotAuthenticated");
            }

            return HttpUtility.UrlEncode(
                "<auth-token>"
                + result.AuthenticationToken
                + "</auth-token>");
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
        internal virtual void AddRestAuthorizationHeaderToken(IList<string> tokens, Guid appId)
        {
            CreateAuthenticationTokenResult result = GetAuthenticationResult(appId);

            if (result == null)
            {
                throw Validator.HealthServiceException("ConnectionNotAuthenticated");
            }

            tokens.Add(
                String.Format(
                    CultureInfo.InvariantCulture,
                    RestConstants.AuthorizationHeaderElement,
                    RestConstants.AppToken,
                    result.AuthenticationToken));
        }

        /// <summary>
        /// Inserts a new authentication result into the authentication results 
        /// dictionary.
        /// </summary>
        /// 
        /// <param name="result">
        /// The authentication result to add to the results for the specified
        /// application.
        /// </param>
        /// 
        internal virtual void AddAuthenticationResult(
            CreateAuthenticationTokenResult result)
        {
            if (_authResults == null)
            {
                _authResults = new Dictionary<Guid, CreateAuthenticationTokenResult>();
            }
            if (_authResults.ContainsKey(result.ApplicationId))
            {
                _authResults.Remove(result.ApplicationId);
            }
            _authResults.Add(result.ApplicationId, result);
        }

        /// <summary>
        /// Inserts a dictionary of authentication results.
        /// </summary>
        /// 
        /// <param name="result">
        /// The authentication result.
        /// </param>
        /// 
        internal virtual void UpdateAuthenticationResults(
            CreateAuthenticationTokenResult result)
        {
            AddAuthenticationResult(result);
        }

        /// <summary>
        /// Gets an authentication token in the context of the credential.
        /// </summary>
        /// 
        /// <remarks>
        /// The resulting authentication result is inserted into 
        /// <see cref="GetAuthenticationResult"/>.
        /// This method accesses the HealthVault service across the network.
        /// If the application ID specified is already in the dictionary,
        /// then the authentication attempt is not made and the cached 
        /// authentication result is used.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The client-side representation of the HealthVault service.
        /// </param>
        /// 
        /// <param name="appId">
        /// The HealthVault application identifier for which the request is 
        /// made.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="appId"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <seealso cref="Microsoft.Health.HealthServiceConnection"/>
        /// 
        public void CreateAuthenticatedSessionToken(
            HealthServiceConnection connection,
            Guid appId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "AuthenticatedConnectionNull");

            Validator.ThrowArgumentExceptionIf(
                appId == null || appId == Guid.Empty,
                "appId",
                "AuthenticationAppIDNullOrEmpty");

            AnonymousConnection anonConn =
                new AnonymousConnection(connection.ApplicationId, connection.RequestUrl);
            if (connection.WebProxy != null)
            {
                anonConn.WebProxy = connection.WebProxy;
            }

            MakeCreateTokenCall(
                "CreateAuthenticatedSessionToken",
                2,
                anonConn,
                appId,
                false);
        }

        #region create token web service helpers

        /// <summary>
        /// Creates authentication tokens and absence reasons for user credentials.
        /// </summary>
        /// 
        /// <remarks>
        /// This method takes only one application ID, which is the typical case.
        /// </remarks>
        /// 
        /// <param name="methodName">
        /// The HealthVault service method name to call.
        /// </param>
        /// 
        /// <param name="version">
        /// The version of the service method to call.
        /// </param>
        /// 
        /// <param name="connection">
        /// The <see cref="HealthServiceConnection"/> instance.
        /// </param>
        /// 
        /// <param name="appId">
        /// The application ID to create a token for.
        /// </param>
        /// 
        /// <param name="isMra">
        /// The application is a multi-record app.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="methodName"/>
        /// parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        internal void MakeCreateTokenCall(
            string methodName,
            int version,
            HealthServiceConnection connection,
            Guid appId,
            bool isMra)
        {
            MakeCreateTokenCall(
                methodName,
                version,
                connection,
                new ApplicationTokenCreationInfo(appId, isMra),
                null);
        }

        /// <summary>
        /// Creates authentication tokens and absence reasons for user credentials.
        /// </summary>
        /// 
        /// <remarks>
        /// This method takes only one application ID, which is the typical case.
        /// </remarks>
        /// 
        /// <param name="methodName">
        /// The HealthVault service method name to call.
        /// </param>
        /// 
        /// <param name="version">
        /// The version of the service method to call.
        /// </param>
        /// 
        /// <param name="connection">
        /// The <see cref="HealthServiceConnection"/> instance.
        /// </param>
        /// 
        /// <param name="appId">
        /// The application ID to create a token for.
        /// </param>
        /// 
        /// <param name="isMra">
        /// The application is a multi-record app.
        /// </param>
        /// 
        /// <param name="stsOriginalUrl">
        /// The original url from the STS request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="methodName"/>
        /// parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        internal void MakeCreateTokenCall(
            string methodName,
            int version,
            HealthServiceConnection connection,
            Guid appId,
            bool isMra,
            string stsOriginalUrl)
        {
            MakeCreateTokenCall(
                methodName,
                version,
                connection,
                new ApplicationTokenCreationInfo(appId, isMra),
                stsOriginalUrl);
        }

        /// <summary>
        /// Create authentication tokens and absence reasons for user 
        /// credentials for the calling app, given the specified method and connection.
        /// </summary>
        /// 
        /// <param name="methodName">
        /// The HealthVault service method name to call.
        /// </param>
        /// 
        /// <param name="version">
        /// The version of the service method to call.
        /// </param>
        /// 
        /// <param name="connection">
        /// The <see cref="HealthServiceConnection"/> instance.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="methodName"/>
        /// parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        internal void MakeCreateTokenCall(
            string methodName,
            int version,
            HealthServiceConnection connection)
        {
            MakeCreateTokenCallImpl(
                methodName,
                version,
                connection);
        }

        /// <summary>
        /// Create authentication tokens and absence reasons for user 
        /// credentials, given the specified method, connection, and multiple 
        /// application ID's.
        /// </summary>
        /// 
        /// <remarks>
        /// The Shell primarily uses multiple application ID's when 
        /// constructing simple credential tokens: One each for the Shell and 
        /// for the target application.
        /// </remarks>
        /// 
        /// <param name="methodName">
        /// The HealthVault service method name to call.
        /// </param>
        /// 
        /// <param name="version">
        /// The version of the service method to call.
        /// </param>
        /// 
        /// <param name="connection">
        /// The <see cref="HealthServiceConnection"/> instance.
        /// </param>
        /// 
        /// <param name="applicationTokenCreationInfo">
        /// The application token creation information.
        /// </param>
        /// 
        /// <param name="stsOriginalUrl">
        /// The original url from the STS request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="methodName"/>
        /// parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        internal void MakeCreateTokenCall(
            string methodName,
            int version,
            HealthServiceConnection connection,
            ApplicationTokenCreationInfo applicationTokenCreationInfo,
            string stsOriginalUrl)
        {
            Validator.ThrowIfStringNullOrEmpty(methodName, "CreateTokenMethodNameIsNullOrEmpty");

            Validator.ThrowArgumentExceptionIf(
                applicationTokenCreationInfo == null,
                "appTokenCreationInfo",
                "AuthenticationAppIDCollectionNullOrEmpty");

            if (IsTokenCached(applicationTokenCreationInfo))
            {
                return;
            }

            MakeCreateTokenCallImpl(methodName, version, connection, applicationTokenCreationInfo, stsOriginalUrl);
        }

        private void MakeCreateTokenCallImpl(
            string methodName,
            int version,
            HealthServiceConnection connection,
            ApplicationTokenCreationInfo applicationTokenCreationInfo = null,
            string stsOriginalUrl = null)
        {
            Validator.ThrowIfStringNullOrEmpty(methodName, "CreateTokenMethodNameIsNullOrEmpty");

            AuthenticationMethodName = methodName;

            HealthServiceRequest request =
                new HealthServiceRequest(connection, AuthenticationMethodName, version);

            request.Parameters =
                ConstructCreateTokenInfoXml(applicationTokenCreationInfo, stsOriginalUrl);

            request.Execute();

            CreateAuthenticationTokenResult createAuthTokenResult =
                GetAuthTokenAndAbsenceReasons(
                    request.Response.InfoNavigator);

            ParseExtendedElements(request.Response.InfoNavigator);

            UpdateAuthenticationResults(createAuthTokenResult);
        }

        /// <summary>
        /// Parses any method specific elements in the response.
        /// </summary>
        /// 
        /// <param name="nav">
        /// The response XML path navigator.
        /// </param>
        internal virtual void ParseExtendedElements(XPathNavigator nav)
        {
        }

        /// <summary>
        /// Determines whether the results are in the cache.
        /// </summary>
        /// 
        /// <param name="applicationTokenCreationInfo">
        /// The application token creation information.
        /// </param>
        /// 
        /// <returns>
        /// True if there is already a token in the cache. 
        /// </returns>
        /// 
        internal virtual bool IsTokenCached(
            ApplicationTokenCreationInfo applicationTokenCreationInfo)
        {
            if (AuthenticationResults == null ||
                AuthenticationResults.Count == 0)
            {
                return false;
            }

            if (AuthenticationResults.ContainsKey(applicationTokenCreationInfo.ApplicationId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves token information.
        /// </summary>
        /// 
        /// <param name="applicationTokenCreationInfo">
        /// The application token creation information.
        /// </param>
        /// 
        /// <param name="stsOriginalRequestUrl">
        /// The original URL of the STS request.
        /// </param>
        /// 
        /// <returns>
        /// An XML string representing the token information.
        /// </returns>
        /// 
        internal string ConstructCreateTokenInfoXml(
            ApplicationTokenCreationInfo applicationTokenCreationInfo,
            string stsOriginalRequestUrl)
        {
            StringBuilder infoXml = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            XmlWriter writer = null;

            try
            {
                writer = XmlWriter.Create(infoXml, settings);

                // Add the PersonInfo elements
                writer.WriteStartElement("auth-info");

                //app-id
                if (applicationTokenCreationInfo != null)
                {
                    ConstructCreateTokenInfoXmlAppIdPart(
                        writer,
                        applicationTokenCreationInfo);
                }

                // <credentials>
                writer.WriteStartElement("credential");
                WriteInfoXml(writer);
                writer.WriteEndElement();

                // <sts-info>
                if (stsOriginalRequestUrl != null)
                {
                    writer.WriteStartElement("sts-info");
                    writer.WriteElementString("original-request-url", stsOriginalRequestUrl);
                    writer.WriteEndElement();
                }

                // <second factor credential>

                // Close the auth-info tag
                writer.WriteEndElement();
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return (infoXml.ToString());
        }

        /// <summary>
        /// Creates the specified individual values of application identifiers 
        /// in the specified collection.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter receiving the values.
        /// </param>
        /// 
        /// <param name="applicationTokenCreationInfo">
        /// The application token creation information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        private static void ConstructCreateTokenInfoXmlAppIdPart(
            XmlWriter writer,
            ApplicationTokenCreationInfo applicationTokenCreationInfo)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            writer.WriteStartElement("app-id");

            if (applicationTokenCreationInfo.IsMRA)
            {
                writer.WriteAttributeString("is-multi-record-app", "true");
            }
            writer.WriteValue(applicationTokenCreationInfo.ApplicationId.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets authentication token absence reasons from response XML.
        /// </summary>
        /// 
        /// <param name="nav">
        /// The response XML path navigator.
        /// </param>
        /// 
        /// <returns>
        /// The dictionary of updated authentication results.
        /// </returns>
        /// 
        internal CreateAuthenticationTokenResult GetAuthTokenAndAbsenceReasons(
            XPathNavigator nav)
        {
            CreateAuthenticationTokenResult createAuthTokenResult = new CreateAuthenticationTokenResult();

            GetAuthenticationToken(createAuthTokenResult, nav);

            if (createAuthTokenResult.AuthenticationToken == null)
            {
                GetStsPayload(createAuthTokenResult, nav);
            }

            if (createAuthTokenResult.AuthenticationToken == null &&
                createAuthTokenResult.StsTokenPayload == null)
            {
                GetAbsenceReasons(createAuthTokenResult, nav);
            }

            return createAuthTokenResult;
        }

        /// <summary>
        /// Extracts the authentication token from the response XML.
        /// </summary>
        /// 
        /// <param name="createAuthTokenResult">
        /// The token results.
        /// </param>
        /// 
        /// <param name="nav">
        /// The path to the token.
        /// </param>
        /// 
        private void GetAuthenticationToken(
            CreateAuthenticationTokenResult createAuthTokenResult,
            XPathNavigator nav)
        {
            XPathExpression authTokenPath = GetAuthTokenXPath(nav);
            XPathNodeIterator navTokenIterator = nav.Select(authTokenPath);

            GetTokenByParseResponse(navTokenIterator, createAuthTokenResult);
        }

        /// <summary>
        /// Extracts the absence reasons from the response XML.
        /// </summary>
        /// 
        private void GetAbsenceReasons(
            CreateAuthenticationTokenResult createAuthTokenResult,
            XPathNavigator nav)
        {
            XPathExpression absenceReasonPath = GetAbsenceReasonXPath(nav);
            XPathNodeIterator navTokenIterator = nav.Select(absenceReasonPath);

            GetAbsenceReasonByParseResponse(
                navTokenIterator,
                createAuthTokenResult);
        }

        /// <summary>
        /// Extracts the STS payload.
        /// </summary>
        /// 
        private void GetStsPayload(
            CreateAuthenticationTokenResult createAuthTokenResult,
            XPathNavigator nav)
        {
            XPathExpression stsPayloadPath = GetStsPayloadPath(nav);
            XPathNavigator stsNav = nav.SelectSingleNode(stsPayloadPath);

            if (stsNav != null)
            {
                Guid applicationId = new Guid(stsNav.GetAttribute("app-id", String.Empty));

                ApplicationRecordAuthorizationAction action =
                    ApplicationRecordAuthorizationAction.Unknown;
                try
                {
                    action =
                        (ApplicationRecordAuthorizationAction)Enum.Parse(
                            typeof(ApplicationRecordAuthorizationAction),
                            stsNav.GetAttribute("app-record-auth-action",
                                String.Empty));
                }
                catch (ArgumentException)
                {
                }

                createAuthTokenResult.StsTokenPayload = stsNav.Value;
                createAuthTokenResult.ApplicationId = applicationId;
                createAuthTokenResult.ApplicationRecordAuthorizationAction = action;
                createAuthTokenResult.Status = AuthenticationTokenCreationStatus.Success;
            }
        }

        /// <summary>
        /// Gets one or more authorization tokens from the response XML.
        /// </summary>
        /// 
        /// <param name="navTokenIterator">
        /// The path to the token or tokens.
        /// </param>
        /// 
        /// <param name="createAuthTokenResult">
        /// The token results.
        /// </param>
        /// 
        private static void GetTokenByParseResponse(
            XPathNodeIterator navTokenIterator,
            CreateAuthenticationTokenResult createAuthTokenResult)
        {
            foreach (XPathNavigator tokenNav in navTokenIterator)
            {
                Guid applicationId = new Guid(tokenNav.GetAttribute("app-id",
                            String.Empty));

                ApplicationRecordAuthorizationAction action =
                    ApplicationRecordAuthorizationAction.Unknown;
                try
                {
                    action =
                        (ApplicationRecordAuthorizationAction)Enum.Parse(
                            typeof(ApplicationRecordAuthorizationAction),
                            tokenNav.GetAttribute("app-record-auth-action",
                                String.Empty));
                }
                catch (ArgumentException)
                {
                }

                createAuthTokenResult.ApplicationId = applicationId;
                createAuthTokenResult.Status = AuthenticationTokenCreationStatus.Success;
                createAuthTokenResult.AuthenticationToken = tokenNav.Value;
                createAuthTokenResult.ApplicationRecordAuthorizationAction = action;
            }
        }

        /// <summary>
        /// Gets absence reasons regarding why there are no auth tokens 
        /// from the response XML.
        /// </summary>
        /// 
        /// <param name="navTokenIterator">
        /// The path to the token or tokens.
        /// </param>
        /// 
        /// <param name="createAuthTokenResult">
        /// The token results.
        /// </param>
        /// 
        private static void GetAbsenceReasonByParseResponse(
            XPathNodeIterator navTokenIterator,
            CreateAuthenticationTokenResult createAuthTokenResult)
        {
            foreach (XPathNavigator tokenNav in navTokenIterator)
            {
                Guid applicationId = new Guid(tokenNav.GetAttribute("app-id",
                            String.Empty));

                try
                {
                    createAuthTokenResult.Status =
                        (AuthenticationTokenCreationStatus)Enum.Parse(
                            typeof(AuthenticationTokenCreationStatus),
                            tokenNav.Value);
                }
                catch (ArgumentException)
                {
                    createAuthTokenResult.Status = AuthenticationTokenCreationStatus.Unknown;
                }

                createAuthTokenResult.ApplicationId = applicationId;
                createAuthTokenResult.AuthenticationToken = null;
            }
        }

        private XPathExpression GetAuthTokenXPath(XPathNavigator infoNav)
        {
            return GetTokenXPathExpression(infoNav, "/wc:info/token");
        }

        private XPathExpression GetAbsenceReasonXPath(XPathNavigator infoNav)
        {
            return GetTokenXPathExpression(infoNav, "/wc:info/token-absence-reason");
        }

        private XPathExpression GetStsPayloadPath(XPathNavigator infoNav)
        {
            return GetTokenXPathExpression(infoNav, "//wc:info/sts-token-payload/payload");
        }

        private XPathExpression GetTokenXPathExpression(
            XPathNavigator infoNav,
            string xPath)
        {
            XPathExpression infoXPathExp = XPathExpression.Compile(xPath);

            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            String nsName = AuthenticationMethodName;
            if (nsName == "CreateAuthenticatedSessionToken")
            {
                nsName = "CreateAuthenticatedSessionToken2";
            }

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + nsName);

            infoXPathExp.SetContext(infoXmlNamespaceManager);

            return infoXPathExp;
        }

        #endregion

        /// <summary>
        /// Instantiates a credential from type information and subsequently 
        /// calls <see cref="ReadCookieXml"/> to initialize the object.
        /// </summary>
        /// 
        /// <param name="credNav"></param>
        /// 
        /// <returns>
        /// An instance of <see cref="Credential"/>.
        /// </returns>
        /// 
        internal static Credential CreateFromCookieXml(XPathNavigator credNav)
        {
            Credential cred = null;

            // expected schema:
            // <type>object Type</type>
            // <[credname]>...</[credname]>
            XPathNavigator typeNav = credNav.SelectSingleNode("type");

            if (typeNav != null)
            {
                switch (typeNav.Value)
                {
                    case "Microsoft.Health.Authentication.PassportCredential":
                        cred = new PassportCredential();
                        break;

                    case "Microsoft.Health.Web.Authentication.WebApplicationCredential":
                        cred = new WebApplicationCredential();
                        break;

                    default:
                        return null;
                }

                typeNav.MoveToFollowing(XPathNodeType.Element);
                XmlReader reader = typeNav.ReadSubtree();
                reader.Read();

                cred.ReadCookieXml(reader);
            }

            return cred;
        }

        /// <summary>
        /// Enables a credential to create a secure or safe representation 
        /// that can be stored as an HTTP cookie.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter receving the credential representation.
        /// </param>        
        /// 
        /// <remarks>
        /// The credential object is instantiated using Activator by using 
        /// stored type information in the cookie. The default behavior is to 
        /// write the type.
        /// </remarks>
        /// 
        /// <seealso cref="CreateFromCookieXml"/>
        /// 
        internal virtual void WriteCookieXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "type",
                this.GetType().ToString());
        }

        /// <summary>
        /// Reconstructs the credential state from the previously
        /// written cookie XML.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The XmlReader containing the credential state to be restored.
        /// </param>    
        /// 
        /// <remarks>
        /// This method runs after the type is read and an instance of this 
        /// object is created.
        /// </remarks>
        /// 
        /// <seealso cref="CreateFromCookieXml"/>
        /// 
        internal virtual void ReadCookieXml(XmlReader reader)
        {
        }

        /// <summary>
        /// Generates the credential-specific XML that is sent 
        /// to the HealthVault service.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter receiving the credentials.
        /// </param>
        /// 
        /// <remarks>
        /// When an inherited class implements WriteInfoXml, the method should
        /// produce XML in accordance with the HealthVault XML schema for the 
        /// specific credential type.
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>. 
        /// </exception>
        /// 
        public abstract void WriteInfoXml(XmlWriter writer);

        /// <summary>
        /// Applies the shared secret to the specified data.
        /// </summary>
        /// 
        /// <remarks>
        /// After the initial authentication is made with the HealthVault service,
        /// all subsequent calls to the HealthVault service must have
        /// authenticated header sections. This is the method that is used
        /// to produce the hmac-data for the "auth" section.
        /// 
        /// If this method is called, then the holding connection object 
        /// is trying to authenticate data, so the credential must have a shared secret.
        /// 
        /// If the credential implements its own shared secret,
        /// then this might be <b>null</b>.
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
        /// The authenticated data is returned. If <see cref="SharedSecret" />
        /// is <b>null</b>, String.Empty is returned.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="data"/> parameter is <b>null</b> or empty. 
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The shared secret parameter is <b>null</b>.
        /// </exception>
        /// 
        internal virtual string AuthenticateData(byte[] data, int index, int count)
        {
            Validator.ThrowArgumentExceptionIf(
                data == null || data.Length == 0,
                "data",
                "DataArrayMustContainData");

            Validator.ThrowArgumentExceptionIf(
                index < 0 || index >= data.Length,
                "index",
                "IndexValueInvalid");

            Validator.ThrowArgumentExceptionIf(
                count <= 0 || count > data.Length,
                "count",
                "CountInvalid");

            Validator.ThrowInvalidIfNull(SharedSecret, "SharedSecretMissing");

            SharedSecret.Reset();
            SharedSecret.ComputeHash(data, index, count);

            string hashXml = SharedSecret.Finalize().GetXml();

            return hashXml;
        }
    }
}

