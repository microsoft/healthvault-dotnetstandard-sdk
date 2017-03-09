// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// A collection of utility functions to help HealthVault web application developers
    /// authenticate and perform other actions with HealthVault.
    /// </summary>
    ///
    /// <remarks>
    /// If possible, it is recommended that HealthVault applications derive from
    /// <see cref="HealthServicePage"/>. If the requirements for the application don't allow for
    /// derivation due to deriving from another base class or needing access from control classes,
    /// the static utility methods in this class give the developer access to the same functionality
    /// that is available in the <see cref="HealthServicePage"/>.
    /// <br/><br/>
    /// Methods like <see cref="PageOnInit"/> and
    /// <see cref="PageOnPreLoadAsync(System.Web.HttpContext,bool,bool)"/> should be called from
    /// the application's page <see cref="System.Web.UI.Page.OnInit"/> and
    /// <see cref="System.Web.UI.Page.OnPreLoad"/> methods respectively.
    /// <br/><br/>
    /// Other methods help the application with management of the HealthVault cookie. For instance,
    /// <see cref="LoadPersonInfoFromCookie(System.Web.HttpContext)"/> reads the HealthVault cookie from the request and
    /// instantiates the <see cref="PersonInfo"/> instance for the logged in person. Note, some
    /// methods like <see cref="LoadPersonInfoFromCookie(System.Web.HttpContext)"/> require another method be called first
    /// to handle the user's authentication token. Methods like
    /// <see cref="SavePersonInfoToCookie(System.Web.HttpContext, PersonInfo)"/> and
    /// <see cref="RefreshAndSavePersonInfoToCookieAsync(System.Web.HttpContext,PersonInfo)"/> deal with storing the HealthVault user information in a
    /// cookie or session.
    /// </remarks>
    ///
    public static class WebApplicationUtilities
    {
        internal const string QueryStringToken = "WCToken";
        internal const string QueryStringInstanceId = "instanceid";
        internal const string PersistentTokenTtl = "suggestedtokenttl";

        internal const string WcTokenExpiration = "e";
        internal const string WcTokenPersonInfo = "p";

        private static int CookieMaxSize = 2048;

        private static WebConfiguration configuration = Ioc.Get<WebConfiguration>();

        #region OnInit

        /// <summary>
        /// Replicates the <see cref="HealthServicePage.OnInit"/> behavior by redirecting to a
        /// secure version of the page if the URL requested is insecure and the application requires
        /// a secure connection.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="isPageSslSecure">
        /// If true, the application requires all connections to this page be over a secure SSL
        /// channel.
        /// </param>
        ///
        /// <remarks>
        /// Applications can require certain pages (or all pages) to be accessed only over a secure
        /// SSL channel. To do this the application must set the "WCPage_SSLForSecure" config value
        /// in the web.config file and pass "true" to <paramref name="isPageSslSecure"/>.
        /// <br/><br/>
        /// If the conditions above are true the user's browser will automatically be redirected
        /// to a secure version of the page.
        /// </remarks>
        ///
        public static void PageOnInit(System.Web.HttpContext context, bool isPageSslSecure)
        {
            if (isPageSslSecure)
            {
                string redirectUrl = GetSSLRedirectURL(context.Request);
                if (redirectUrl != null)
                {
                    context.Response.Redirect(redirectUrl);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ///
        /// <remarks>
        /// This is usually called during page initialization (<see cref="System.Web.UI.Page.OnInit"/>).
        /// If a <see cref="System.Uri"/> is returned then the user should be redirected to the
        /// specified URL.
        /// </remarks>
        ///
        private static string GetSSLRedirectURL(HttpRequest request)
        {
            string result = null;
            if (configuration.UseSslForSecurity)
            {
                if (!request.IsSecureConnection)
                {
                    //RedirectToSecure
                    StringBuilder secureUrl = new StringBuilder();
                    secureUrl.Append(
                        configuration.SecureHttpScheme);
                    secureUrl.Append(request.Url.Host);
                    secureUrl.Append(request.Url.PathAndQuery);
                    result = secureUrl.ToString();
                }
            }
            else
            {
                if (request.IsSecureConnection)
                {
                    //RedirectToInsecure
                    StringBuilder inSecureUrl = new StringBuilder();
                    inSecureUrl.Append(
                        configuration.InsecureHttpScheme);
                    inSecureUrl.Append(request.Url.Host);
                    inSecureUrl.Append(request.Url.PathAndQuery);
                    result = inSecureUrl.ToString();
                }
            }
            return result;
        }

        #endregion OnInit

        #region OnPreLoad

        /// <summary>
        /// Ensures that the person is logged on if <paramref name="logOnRequired"/> is true.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="logOnRequired">
        /// True if the requested page requires the user to be logged on to HealthVault, or false
        /// otherwise. If true and the user isn't logged on, the user's browser will be automatically
        /// redirected to the HealthVault authentication page.
        /// </param>
        ///
        /// <remarks>
        /// It is recommended that HealthVault applications that cannot derive from
        /// <see cref="HealthServicePage"/> call this method during their pages OnPreLoad. This
        /// method will ensure that the HealthVault token is extracted from the URL query string,
        /// the authenticated person's <see cref="PersonInfo"/> is retrieved and stored in a cookie.
        /// This will make the person's information available through the cookie on all future
        /// requests until they log off.
        /// </remarks>
        ///
        public static async Task<PersonInfo> PageOnPreLoadAsync(System.Web.HttpContext context, bool logOnRequired)
        {
            return await PageOnPreLoad(
                    context,
                    logOnRequired,
                    false,
                    configuration.ApplicationId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures that the person is logged on if <paramref name="logOnRequired"/> is true.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="logOnRequired">
        /// True if the requested page requires the user to be logged on to HealthVault, or false
        /// otherwise. If true and the user isn't logged on, the user's browser will be automatically
        /// redirected to the HealthVault authentication page.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique application identifier.
        /// </param>
        ///
        /// <remarks>
        /// It is recommended that HealthVault applications that cannot derive from
        /// <see cref="HealthServicePage"/> call this method during their pages OnPreLoad. This
        /// method will ensure that the HealthVault token is extracted from the URL query string,
        /// the authenticated person's <see cref="PersonInfo"/> is retrieved and stored in a cookie.
        /// This will make the person's information available through the cookie on all future
        /// requests until they log off.
        /// </remarks>
        ///
        public static async Task<PersonInfo> PageOnPreLoad(System.Web.HttpContext context, bool logOnRequired, Guid appId)
        {
            return await PageOnPreLoad(context, logOnRequired, false /* isMra */, appId).ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures that the person is logged on if <paramref name="logOnRequired"/> is true.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="logOnRequired">
        /// True if the requested page requires the user to be logged on to HealthVault, or false
        /// otherwise. If true and the user isn't logged on, the user's browser will be automatically
        /// redirected to the HealthVault authentication page.
        /// </param>
        ///
        /// <param name="isMra">
        /// Whether this application simultaneously deals with multiple records
        /// for the same person.
        /// </param>
        ///
        /// <remarks>
        /// It is recommended that HealthVault applications that cannot derive from
        /// <see cref="HealthServicePage"/> call this method during their pages OnPreLoad. This
        /// method will ensure that the HealthVault token is extracted from the URL query string,
        /// the authenticated person's <see cref="PersonInfo"/> is retrieved and stored in a cookie.
        /// This will make the person's information available through the cookie on all future
        /// requests until they log off.
        /// </remarks>
        ///
        public static async Task<PersonInfo> PageOnPreLoadAsync(System.Web.HttpContext context, bool logOnRequired, bool isMra)
        {
            return await PageOnPreLoad(
                    context,
                    logOnRequired,
                    isMra,
                    configuration.ApplicationId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures that the person is logged on if <paramref name="logOnRequired"/> is true.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="logOnRequired">
        /// True if the requested page requires the user to be logged on to HealthVault, or false
        /// otherwise. If true and the user isn't logged on, the user's browser will be automatically
        /// redirected to the HealthVault authentication page.
        /// </param>
        ///
        /// <param name="isMra">
        /// Whether this application simultaneously deals with multiple records
        /// for the same person.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier for the application.
        /// </param>
        ///
        /// <remarks>
        /// It is recommended that HealthVault applications that cannot derive from
        /// <see cref="HealthServicePage"/> call this method during their pages OnPreLoad. This
        /// method will ensure that the HealthVault token is extracted from the URL query string,
        /// the authenticated person's <see cref="PersonInfo"/> is retrieved and stored in a cookie.
        /// This will make the person's information available through the cookie on all future
        /// requests until they log off.
        /// </remarks>
        ///
        public static async Task<PersonInfo> PageOnPreLoad(
            HttpContext context,
            bool logOnRequired,
            bool isMra,
            Guid appId)
        {
            // this will redirect if needed
            // NOTE: I reverted this code because it was spreading query
            // string info around as it was previously.
            await HandleTokenOnUrl(context, logOnRequired, appId).ConfigureAwait(false);

            // Get whatever's in the cookie...
            PersonInfo personInfo = await LoadPersonInfoFromCookie(context);

            // If we didn't just authenticate and the cookie was blank and
            // we ought to be logged in...
            if (logOnRequired && personInfo == null)
            {
                // If we need a signup code for account creation, get it now
                // and pass it to HealthVault.
                string signupCode = null;
                if (configuration.IsSignupCodeRequired)
                {
                    // TODO: IConnection-ify this.
                    // signupCode = HealthVaultPlatform.NewSignupCodeAsync(ApplicationConnection).Result;
                }

                RedirectToLogOn(context, isMra, context.Request.Url.PathAndQuery, signupCode);
            }

            SavePersonInfoToCookie(context, personInfo, false);
            return personInfo;
        }

        #endregion OnPreLoad

        /// <summary>
        /// Gets a credential used to authenticate the web application to
        /// HealthVault.
        /// </summary>
        public static WebApplicationCredential ApplicationAuthenticationCredential
        {
            get
            {
                return GetApplicationAuthenticationCredential(
                    configuration.ApplicationId);
            }
        }

        /// <summary>
        /// Gets a credential used to authenticate the web application to
        /// Microsoft HealthVault.
        /// </summary>
        ///
        /// <param name="appId">
        /// The unique application identifier to get the credential for.
        /// </param>
        ///
        /// <returns>
        /// The application credential for the specified application.
        /// </returns>
        ///
        public static WebApplicationCredential GetApplicationAuthenticationCredential(Guid appId)
        {
            return
                new WebApplicationCredential(
                    appId,
                    ApplicationCertificateStore.Current.ApplicationCertificate);
        }

        /// <summary>
        /// Cleans the application's session of HealthVault information and
        /// then repopulates it.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="personInfo">
        /// The information about the authenticated person that needs refreshing.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If a person isn't logged on when this is called.
        /// </exception>
        ///
        /// <remarks>
        /// This method should be called anytime an action occurs that will affect the
        /// <see cref="PersonInfo"/> object for the authenticated person. This includes changing
        /// the person's authorization for the application or changing the selected record.
        /// </remarks>
        ///
        public static async Task<PersonInfo> RefreshAndSavePersonInfoToCookieAsync(
            HttpContext context,
            PersonInfo personInfo)
        {
            Validator.ThrowInvalidIfNull(personInfo, "PersonNotLoggedIn");

            personInfo = await HealthVaultPlatform.GetPersonInfoAsync(personInfo.Connection).ConfigureAwait(false);

            SavePersonInfoToCookie(context, personInfo);
            return personInfo;
        }

        internal static void OnPersonInfoChanged(Object sender, EventArgs e)
        {
            PersonInfo personInfo = sender as PersonInfo;

            if (personInfo != null)
            {
                SavePersonInfoToCookie(System.Web.HttpContext.Current, personInfo);
            }
        }

        /// <summary>
        /// Stores the specified person's information in the cookie.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="personInfo">
        /// The authenticated person's information.
        /// </param>
        ///
        /// <remarks>
        /// If <paramref name="personInfo"/> is null, this call will not clear the cookie.
        /// </remarks>
        ///
        public static void SavePersonInfoToCookie(System.Web.HttpContext context, PersonInfo personInfo)
        {
            SavePersonInfoToCookie(context, personInfo, false);
        }

        /// <summary>
        /// Cleans the application's session of HealthVault information and
        /// then repopulates it using the specified authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="authToken">
        /// The authentication to use to populate the session with HealthVault
        /// information.
        /// </param>
        ///
        /// <exception cref="HealthServiceException">
        /// If HealthVault returns an error when getting information
        /// about the person in the <paramref name="authToken"/>.
        /// </exception>
        ///
        /// <remarks>
        /// This method should be called anytime the application takes a redirect from the
        /// HealthVault shell and there is a WCToken on the query string. Note, if you are calling
        /// <see cref="PageOnPreLoadAsync(System.Web.HttpContext, bool, bool)"/> or
        /// <see cref="PageOnPreLoadAsync(System.Web.HttpContext, bool)"/> this is handled for you.
        /// </remarks>
        public static async Task<PersonInfo> RefreshAndSavePersonInfoToCookieAsync(
            System.Web.HttpContext context,
            string authToken)
        {
            PersonInfo personInfo = await GetPersonInfoAsync(authToken).ConfigureAwait(false);
            SavePersonInfoToCookie(context, personInfo);
            return personInfo;
        }

        /// <summary>
        /// Cleans the application's session of HealthVault information and
        /// then repopulates it using the specified authentication token and
        /// HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="authToken">
        /// The authentication to use to populate the session with HealthVault
        /// information.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance that the session auth token
        /// was originally received from.
        /// </param>
        ///
        /// <exception cref="HealthServiceException">
        /// If HealthVault returns an error when getting information
        /// about the person in the <paramref name="authToken"/>.
        /// </exception>
        ///
        /// <remarks>
        /// This method should be called anytime the application takes a redirect from the
        /// HealthVault shell and there is a WCToken on the query string. Note, if you are calling
        /// <see cref="PageOnPreLoadAsync(System.Web.HttpContext, bool, bool)"/> or
        /// <see cref="PageOnPreLoadAsync(System.Web.HttpContext, bool)"/> this is handled for you.
        /// </remarks>
        public static async Task<PersonInfo> RefreshAndSavePersonInfoToCookieAsync(
            System.Web.HttpContext context,
            string authToken,
            HealthServiceInstance serviceInstance)
        {
            PersonInfo personInfo = await GetPersonInfoAsync(authToken, serviceInstance).ConfigureAwait(false);
            SavePersonInfoToCookie(context, personInfo);
            return personInfo;
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the queryString params
        /// appended.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// The <paramref name="targetQuery"/> will be URL encoded and passed as the targetqs
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        public static void RedirectToShellUrl(
            System.Web.HttpContext context,
            string targetLocation,
            string targetQuery)
        {
            context.Response.Redirect(
                ConstructShellTargetUrl(
                    context,
                    targetLocation,
                    targetQuery).OriginalString);
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the queryString params
        /// appended.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters passed to the calling application action URL after the
        /// target action is completed in the Shell.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// The <paramref name="targetQuery"/> will be URL encoded and passed as the targetqs
        /// parameter value to the redirector URL.
        /// The <paramref name="actionUrlQueryString"/> will be URL encoded and passed as the actionqs
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        public static void RedirectToShellUrl(
            System.Web.HttpContext context,
            string targetLocation,
            string targetQuery,
            string actionUrlQueryString)
        {
            context.Response.Redirect(
                ConstructShellTargetUrl(
                    context,
                    targetLocation,
                    targetQuery,
                    actionUrlQueryString).OriginalString);
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the query string params
        /// appended.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// </remarks>
        ///
        public static void RedirectToShellUrl(System.Web.HttpContext context, string targetLocation)
        {
            context.Response.Redirect(
                ConstructShellTargetUrl(
                    context,
                    targetLocation).OriginalString);
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the query string params
        /// appended.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="redirectParameters">
        /// HealthVault Shell redirect parameters.
        /// </param>
        ///
        public static void RedirectToShellUrl(System.Web.HttpContext context, ShellRedirectParameters redirectParameters)
        {
            context.Response.Redirect(
                ConstructShellTargetUrl(
                    context,
                    redirectParameters).OriginalString);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// The <paramref name="targetQuery"/> will be URL encoded and passed as the targetqs
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// If the specific target location causes an improper URL to be
        /// constructed.
        /// </exception>
        ///
        public static Uri ConstructShellTargetUrl(
            System.Web.HttpContext context,
            string targetLocation,
            string targetQuery)
        {
            return ConstructShellTargetUrl(context, targetLocation, targetQuery, null);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <param name="targetQuery">
        /// The query string value to pass to the URL to which redirection is
        /// taking place.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters passed to the calling application action URL after the
        /// target action is completed in the Shell.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// The <paramref name="targetQuery"/> will be URL encoded and passed as the targetqs
        /// parameter value to the redirector URL.
        /// The <paramref name="actionUrlQueryString"/> will be URL encoded and passed as the actionqs
        /// parameter value to the redirector URL.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// If the specific target location causes an improper URL to be
        /// constructed.
        /// </exception>
        ///
        public static Uri ConstructShellTargetUrl(
            System.Web.HttpContext context,
            string targetLocation,
            string targetQuery,
            string actionUrlQueryString)
        {
            var redirectParameters = new ShellRedirectParameters()
            {
                TargetLocation = targetLocation,
                TargetQueryString = targetQuery,
                ActionQueryString = actionUrlQueryString,
            };
            return ConstructShellTargetUrl(context, redirectParameters);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="redirectParameters">
        /// HealthVault Shell redirect parameters.
        /// </param>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// If the specific target location causes an improper URL to be
        /// constructed.
        /// </exception>
        public static Uri ConstructShellTargetUrl(
            System.Web.HttpContext context,
            ShellRedirectParameters redirectParameters)
        {
            redirectParameters = redirectParameters.Clone();
            redirectParameters.TokenRedirectionMethod = "post";
            ApplyWebConfigurationOnRedirectParameters(context, redirectParameters);

            return HealthServiceLocation.GetHealthServiceShellUrl(redirectParameters);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="targetLocation">
        /// A known constant indicating the internal HealthVault
        /// service Shell location to redirect to.
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a>.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="targetLocation"/> will be passed as the target parameter value to
        /// the redirector URL.
        /// </remarks>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="UriFormatException">
        /// If the specific target location causes an improper URL to be
        /// constructed.
        /// </exception>
        ///
        public static Uri ConstructShellTargetUrl(
            System.Web.HttpContext context,
            string targetLocation)
        {
            return ConstructShellTargetUrl(context, targetLocation, null, null);
        }

        /// <summary>
        /// Gets the authenticated person's information using the specified authentication token from
        /// the configured default HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="authToken">
        /// The authentication token for a user. This can be retrieved by extracting the WCToken
        /// query string parameter from the request after the user has been redirected to the
        /// HealthVault AUTH page. See <see cref="RedirectToShellUrl(System.Web.HttpContext, string)"/> for more information.
        /// </param>
        ///
        /// <returns>
        /// The information about the logged in person.
        /// </returns>
        ///
        /// <remarks>
        /// Applications that work with more than one HealthVault instance should not call this method.
        /// Instead, call the overload which takes an <see cref="HealthServiceInstance"/>, specifying the
        /// HealthVault instance where the session auth token was created.
        /// </remarks>
        ///
        public static async Task<PersonInfo> GetPersonInfoAsync(string authToken)
        {
            return await GetPersonInfoAsync(
                authToken,
                configuration.ApplicationId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the authenticated person's information using the specified authentication token from
        /// the specified HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="authToken">
        /// The authentication token for a user. This can be retrieved by extracting the WCToken
        /// query string parameter from the request after the user has been redirected to the
        /// HealthVault AUTH page. See <see cref="RedirectToShellUrl(System.Web.HttpContext, string)"/> for more information.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance where the session auth token was received from.
        /// </param>
        ///
        /// <returns>
        /// The information about the logged in person.
        /// </returns>
        public static async Task<PersonInfo> GetPersonInfoAsync(string authToken, HealthServiceInstance serviceInstance)
        {
            return await GetPersonInfoAsync(
                authToken,
                configuration.ApplicationId,
                serviceInstance)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the authenticated person's information using the specified authentication token,
        /// from the specified HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="authToken">
        /// The authentication token for a user. This can be retrieved by extracting the WCToken
        /// query string parameter from the request after the user has been redirected to the
        /// HealthVault AUTH page. See <see cref="RedirectToShellUrl(System.Web.HttpContext, string)"/> for more information.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier for the application.
        /// </param>
        ///
        /// <returns>
        /// The information about the logged in person.
        /// </returns>
        ///
        /// <remarks>
        /// Applications that work with more than one HealthVault instance should not call this method.
        /// Instead, call the overload which takes an <see cref="HealthServiceInstance"/>, specifying the
        /// HealthVault instance where the session auth token was created.
        /// </remarks>
        ///
        public static async Task<PersonInfo> GetPersonInfoAsync(string authToken, Guid appId)
        {
            return await GetPersonInfoAsync(authToken, appId, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the authenticated person's information using the specified authentication token,
        /// from the specified HealthVault web-service instance.
        /// </summary>
        ///
        /// <param name="authToken">
        /// The authentication token for a user. This can be retrieved by extracting the WCToken
        /// query string parameter from the request after the user has been redirected to the
        /// HealthVault AUTH page. See <see cref="RedirectToShellUrl(System.Web.HttpContext, string)"/> for more information.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier for the application.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault web-service instance.
        /// </param>
        ///
        /// <returns>
        /// The information about the logged in person.
        /// </returns>
        ///
        /// <remarks>
        /// Applications that work with more than one HealthVault instance should not call this method.
        /// Instead, call the overload which takes an <see cref="HealthServiceInstance"/>, specifying the
        /// HealthVault instance where the session auth token was created.
        /// </remarks>
        ///
        public static async Task<PersonInfo> GetPersonInfoAsync(string authToken, Guid appId, HealthServiceInstance serviceInstance)
        {
            //WebApplicationCredential cred =
            //    new WebApplicationCredential(
            //        appId,
            //        authToken,
            //        ApplicationCertificateStore.Current.ApplicationCertificate);

            // set up our cookie
            // TODO: IConnection-ify this.
            /*
            WebApplicationConnection connection =
                serviceInstance != null
                    ? new WebApplicationConnection(appId, serviceInstance, cred)
                    : new WebApplicationConnection(appId, cred);

            PersonInfo personInfo = await HealthVaultPlatform.GetPersonInfoAsync(connection).ConfigureAwait(false);
            personInfo.ApplicationSettingsChanged += new EventHandler(OnPersonInfoChanged);
            personInfo.SelectedRecordChanged += new EventHandler(OnPersonInfoChanged);

            return personInfo;
            */
            // return null;

            IHealthVaultConnection connection = Ioc.Get<IHealthVaultConnection>();
            IPersonClient personClient = connection.PersonClient;

            PersonInfo personInfo = await personClient.GetPersonInfoAsync();


            return personInfo;
        }

        private static async Task HandleTokenOnUrl(System.Web.HttpContext context, bool isLoginRequired, Guid appId)
        {
            string authToken = context.Request.Params[QueryStringToken];
            string instanceId = context.Request.Params[QueryStringInstanceId];

            if (!String.IsNullOrEmpty(authToken))
            {
                // map the instance id to an instance object containing the service URL.
                if (instanceId == null || !ServiceInfo.Current.ServiceInstances.ContainsKey(instanceId))
                {
                    throw Validator.InvalidOperationException("InstanceIdNotFound");
                }
                HealthServiceInstance serviceInstance = ServiceInfo.Current.ServiceInstances[instanceId];

                PersonInfo personInfo = await GetPersonInfoAsync(authToken, appId, serviceInstance).ConfigureAwait(false);

                int tokenTtl = -1;
                string tokenTtlString =
                    context.Request.QueryString[
                        PersistentTokenTtl];

                if (!String.IsNullOrEmpty(tokenTtlString))
                {
                    // Note, the tokenTtl parameter is ignored if it's not
                    // an int.
                    Int32.TryParse(tokenTtlString, out tokenTtl);
                }

                SavePersonInfoToCookie(context, personInfo, false, tokenTtl);

                // redirect to fixed-up url
                string newUrl =
                    StripFromQueryString(
                        context,
                        QueryStringToken,
                        QueryStringInstanceId,
                        PersistentTokenTtl);

                context.Response.Redirect(newUrl);
            }
        }

        /// <summary>
        /// Removes the specified variables from the query string.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="keys">
        /// variable(s) to remove
        /// </param>
        ///
        /// <returns>
        /// original url without key
        /// </returns>
        ///
        private static string StripFromQueryString(
            System.Web.HttpContext context,
            params string[] keys)
        {
            StringBuilder cleanUrl = new StringBuilder();
            cleanUrl.Append(context.Request.AppRelativeCurrentExecutionFilePath);

            string sep = "?";
            string[] queryKeys = context.Request.QueryString.AllKeys;
            for (int ikey = 0; ikey < queryKeys.Length; ++ikey)
            {
                if (queryKeys[ikey] != null)
                {
                    bool queryKeyMatch = false;

                    for (int index = 0; index < keys.Length; ++index)
                    {
                        if (String.Equals(
                                queryKeys[ikey],
                                keys[index],
                                StringComparison.OrdinalIgnoreCase))
                        {
                            queryKeyMatch = true;
                            break;
                        }
                    }

                    if (!queryKeyMatch)
                    {
                        cleanUrl.AppendFormat(
                            "{0}{1}={2}",
                            sep,
                            queryKeys[ikey],
                            context.Server.UrlEncode(context.Request.QueryString[ikey]));
                        sep = "&";
                    }
                }
            }

            return (cleanUrl.ToString());
        }

        private static string UnmarshalCookie(string serializedPersonInfo)
        {
            string personInfoXml;
            int serializationVersion = ParseSerializationVersion(ref serializedPersonInfo);
            switch (serializationVersion)
            {
                case 1:
                    personInfoXml = UnmarshalCookieVersion1(serializedPersonInfo);
                    break;
                case 2:
                    personInfoXml = UnmarshalCookieVersion2(serializedPersonInfo);
                    break;
                default:
                    throw new ArgumentException(
                        ResourceRetriever.FormatResourceString(
                            "UnknownCookieVersion",
                            serializationVersion));
            }

            return personInfoXml;
        }

        /// <summary>
        /// Gets the person's information from the cookie.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <returns>
        /// The person's information that was stored in the cookie or null if the cookie doesn't
        /// exist. Note, the <see cref="PersonInfo"/> returned may contain an expired authentication
        /// token.
        /// </returns>
        ///
        public static async Task<PersonInfo> LoadPersonInfoFromCookie(System.Web.HttpContext context)
        {
            string serializedPersonInfo = null;

            if (configuration.UseAspSession)
            {
                serializedPersonInfo =
                    (String)context.Session[configuration.CookieName];
            }
            else
            {
                HttpCookie cookie =
                    context.Request.Cookies[configuration.CookieName];

                if (cookie != null)
                {
                    serializedPersonInfo =
                        cookie[WcTokenPersonInfo];
                }
            }

            PersonInfo personInfo = null;
            try
            {
                personInfo = await DeserializePersonInfo(serializedPersonInfo);
                if (personInfo != null)
                {
                    personInfo.ApplicationSettingsChanged += new EventHandler(OnPersonInfoChanged);
                    personInfo.SelectedRecordChanged += new EventHandler(OnPersonInfoChanged);
                }
            }
            catch (Exception)
            {
                personInfo = null;  // safety first
                // loading the cookie failed, so remove it on the client
                SavePersonInfoToCookie(context, personInfo, true);
                throw;
            }

            return personInfo;
        }

        /// <summary>
        /// Gets the person's information from the cookie.
        /// </summary>
        ///
        /// <param name="cookie">
        /// The cookie to load the person's information from
        /// </param>
        ///
        /// <returns>
        /// The person's information that was stored in the cookie or null if the cookie doesn't
        /// contain the information. Note, the <see cref="PersonInfo"/> returned may contain an
        /// expired authentication token.
        /// </returns>
        ///
        public static async Task<PersonInfo> LoadPersonInfoFromCookie(HttpCookie cookie)
        {
            string serializedPersonInfo = null;
            if (cookie != null)
            {
                serializedPersonInfo =
                    cookie[WcTokenPersonInfo];
            }

            return await DeserializePersonInfo(serializedPersonInfo);
        }

        private static async Task<PersonInfo> DeserializePersonInfo(string serializedPersonInfo)
        {
            if (String.IsNullOrEmpty(serializedPersonInfo)) return null;

            Validator.ThrowInvalidIf(
                serializedPersonInfo.Length > CookieMaxSize,
                "CookieTooBig");

            PersonInfo personInfo = null;

            try
            {
                string personInfoXml = UnmarshalCookie(serializedPersonInfo);

                XPathDocument personDoc
                    = new XPathDocument(
                        XmlReader.Create(
                            new StringReader(personInfoXml),
                            SDKHelper.XmlReaderSettings));

                personInfo =
                    await PersonInfo.CreateFromXmlExcludeUrl(
                        null,
                        personDoc.CreateNavigator().SelectSingleNode(
                            "person-info"));
            }
            catch (Exception e)
            {
                MarshallTrace.TraceInformation(
                        "Unmarshalling of cookie failed with exception: "
                        + ExceptionToFullString(e));

                personInfo = null;  // safety first
                throw;
            }
            return personInfo;
        }

        private static int ParseSerializationVersion(ref string serializedPersonInfo)
        {
            int idx = serializedPersonInfo.IndexOf(':');
            if (idx == -1)
            {
                return 1;
            }

            string versionString = serializedPersonInfo.Substring(0, idx);
            serializedPersonInfo = serializedPersonInfo.Substring(idx + 1);

            int version;

            Validator.ThrowArgumentExceptionIf(
                !Int32.TryParse(versionString, out version),
                "version",
                "UnknownCookieVersion");

            return version;
        }

        private static string UnmarshalCookieVersion1(string serializedPersonInfo)
        {
            string[] lengthAndData = serializedPersonInfo.Split(new char[] { '-' }, 2);

            int undeflatedSize = Int32.Parse(lengthAndData[0], CultureInfo.InvariantCulture);
            string deflatedData = lengthAndData[1];

            return Decompress(deflatedData, undeflatedSize);
        }

        private static string UnmarshalCookieVersion2(string serializedPersonInfo)
        {
            string[] lengthAndData = serializedPersonInfo.Split(new char[] { '-' }, 2);
            Int16 unencryptedSize = Int16.Parse(lengthAndData[0], CultureInfo.InvariantCulture);

            byte[] data = Convert.FromBase64String(lengthAndData[1]);

            byte[] iv = new byte[16];
            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);

            SymmetricAlgorithm encryptionAlgorithm = GetEncryptionAlgorithm();
            encryptionAlgorithm.IV = iv;

            byte[] unencryptedData = new byte[unencryptedSize];

            MemoryStream dataStream = new MemoryStream(data, iv.Length, data.Length - iv.Length);
            using (CryptoStream cryptoStream = new CryptoStream(dataStream,
                    encryptionAlgorithm.CreateDecryptor(),
                    CryptoStreamMode.Read))
            {
                cryptoStream.Read(unencryptedData, 0, unencryptedData.Length);
            }

            ArraySegment<byte> decompressedData = DecompressInternal(unencryptedData);

            return UTF8Encoding.UTF8.GetString(
                decompressedData.Array,
                decompressedData.Offset,
                decompressedData.Count);
        }

        private static string MarshalCookieVersion1(string personInfoXml)
        {
            int bufferLength;
            string compressedData = Compress(personInfoXml, out bufferLength);
            return bufferLength.ToString(CultureInfo.InvariantCulture) + "-" + compressedData;
        }

        private static string MarshalCookieVersion2(string personInfoXml)
        {
            SymmetricAlgorithm encryptionAlgorithm = GetEncryptionAlgorithm();
            encryptionAlgorithm.GenerateIV();
            byte[] iv = encryptionAlgorithm.IV;

            int personInfoLength;
            ArraySegment<byte> compressedPersonInfo = CompressInternal(personInfoXml, out personInfoLength);

            MemoryStream output = new MemoryStream();
            using (CryptoStream encryptionStream = new CryptoStream(
                output,
                encryptionAlgorithm.CreateEncryptor(),
                CryptoStreamMode.Write))
            {
                encryptionStream.Write(iv, 0, iv.Length);
                encryptionStream.Write(compressedPersonInfo.Array, compressedPersonInfo.Offset, (int)compressedPersonInfo.Count);
                encryptionStream.FlushFinalBlock();

                return personInfoLength + "-" + Convert.ToBase64String(output.GetBuffer(), 0, (int)output.Length);
            }
        }

        private static SymmetricAlgorithm GetEncryptionAlgorithm()
        {
            Rijndael encryptionAlgorithm = Rijndael.Create();
            encryptionAlgorithm.BlockSize = 128;
            encryptionAlgorithm.Key = configuration.CookieEncryptionKey;

            return encryptionAlgorithm;
        }

        private static string ExceptionToFullString(Exception e)
        {
            string eString = e.ToString();
            StringBuilder buffer = new StringBuilder(
                    eString,
                    eString.Length + (e.InnerException != null ? 1024 : 0));

            e = e.InnerException;
            while (e != null)
            {
                buffer.AppendFormat("\r\n" + e.ToString());
                e = e.InnerException;
            }

            return buffer.ToString();
        }

        private static TraceSource MarshallTrace
            = new TraceSource("MarshallSource");

        /// <summary>
        /// Compress incoming string.
        /// </summary>
        ///
        /// <param name="data">
        /// String to be compressed.
        /// </param>
        /// <param name="bufferLength">
        /// The length of the incoming string in bytes.
        /// </param>
        ///
        /// <returns>
        /// Base 64 string of compressed data.
        /// </returns>
        ///
        public static string Compress(string data, out int bufferLength)
        {
            ArraySegment<byte> compressedBytes = CompressInternal(data, out bufferLength);
            return Convert.ToBase64String(compressedBytes.Array, compressedBytes.Offset, compressedBytes.Count);
        }

        private static ArraySegment<byte> CompressInternal(string data, out int length)
        {
            MemoryStream ms = new MemoryStream();
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, true))
            {
                byte[] b = new UTF8Encoding(false, true).GetBytes(data);
                length = b.Length;

                ds.Write(b, 0, b.Length);
            }
            return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// Compress incoming string.
        /// </summary>
        ///
        /// <param name="data">
        /// String to be compressed.
        /// </param>
        ///
        /// <returns>
        /// Base 64 string of compressed data.
        /// </returns>
        ///
        public static string Compress(string data)
        {
            int bufferLength;
            return Compress(data, out bufferLength);
        }

        /// <summary>
        /// Decompress a compressed string.
        /// </summary>
        ///
        /// <param name="compressedData">
        /// Base 64 String of compressed data.
        /// </param>
        ///
        /// <returns>
        /// Uncompressed string.
        /// </returns>
        ///
        public static string Decompress(string compressedData)
        {
            return Decompress(compressedData, -1);
        }

        /// <summary>
        /// Decompress a compressed string.
        /// </summary>
        ///
        /// <param name="compressedData">
        /// Base 64 string of compressed data.
        /// </param>
        /// <param name="decompressedDataLength">
        /// Length of uncompressed data.
        /// </param>
        ///
        /// <returns>
        /// Uncompressed string.
        /// </returns>
        ///
        private static string Decompress(string compressedData, int decompressedDataLength)
        {
            if (String.IsNullOrEmpty(compressedData))
            {
                return String.Empty;
            }

            byte[] stringBytes = Convert.FromBase64String(compressedData);

            ArraySegment<byte> decompressedBytes = DecompressInternal(stringBytes, decompressedDataLength);

            return UTF8Encoding.UTF8.GetString(
                decompressedBytes.Array,
                decompressedBytes.Offset,
                decompressedBytes.Count);
        }

        private static ArraySegment<byte> DecompressInternal(byte[] compressedData)
        {
            return DecompressInternal(compressedData, -1);
        }

        private static ArraySegment<byte> DecompressInternal(byte[] compressedData, int decompressedLength)
        {
            MemoryStream ms = new MemoryStream(compressedData, 0, compressedData.Length);

            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress, false))
            {
                if (decompressedLength != -1)
                {
                    byte[] buffer = new byte[decompressedLength];
                    ds.Read(buffer, 0, decompressedLength);
                    return new ArraySegment<byte>(buffer, 0, decompressedLength);
                }
                else
                {
                    return ReadToStreamEnd(ds);
                }
            }
        }

        private static ArraySegment<byte> ReadToStreamEnd(Stream stream)
        {
            int bytesRead;
            int totalBytesRead = 0;
            byte[] readBuffer = new byte[4096];

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    if (nextByte != -1)
                    {
                        Validator.ThrowInvalidIf(
                            readBuffer.Length > 32768,
                            "DecompressionSizeExceeded");

                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }
            return new ArraySegment<byte>(readBuffer, 0, totalBytesRead);
        }

        private static string PersonInfoAsCookie(PersonInfo personInfo)
        {
            return PersonInfoAsCookie(personInfo, true);
        }

        private static string PersonInfoAsCookie(PersonInfo personInfo, bool keepSizeUnderLimit)
        {
            string cookie = PersonInfoAsCookie(personInfo, PersonInfo.CookieOptions.Default);

            if (cookie.Length <= CookieMaxSize || !keepSizeUnderLimit)
            {
                return cookie;
            }

            // The cookie is too big to fit. Try it without app settings...
            cookie = PersonInfoAsCookie(personInfo, PersonInfo.CookieOptions.MinimizeApplicationSettings);
            if (cookie.Length <= CookieMaxSize)
            {
                return cookie;
            }

            // That didn't help. Try it with minimal records...
            cookie = PersonInfoAsCookie(personInfo, PersonInfo.CookieOptions.MinimizeRecords);
            if (cookie.Length <= CookieMaxSize)
            {
                return cookie;
            }

            // Reduce both...
            cookie = PersonInfoAsCookie(
                            personInfo,
                            PersonInfo.CookieOptions.MinimizeApplicationSettings |
                            PersonInfo.CookieOptions.MinimizeRecords);
            return cookie;
        }

        private static string PersonInfoAsCookie(PersonInfo personInfo, PersonInfo.CookieOptions cookieOptions)
        {
            string personInfoXml = personInfo.GetXmlForCookie(cookieOptions);

            int version = GetMarshalCookieVersion();
            switch (version)
            {
                case 1:
                    return "1:" + MarshalCookieVersion1(personInfoXml);
                case 2:
                    return "2:" + MarshalCookieVersion2(personInfoXml);
                default:
                    throw new ArgumentException(
                        ResourceRetriever.FormatResourceString(
                            "UnknownCookieVersion",
                            version));
            }
        }

        private static int GetMarshalCookieVersion()
        {
            if (configuration.CookieEncryptionKey != null)
            {
                return 2;
            }

            return 1;
        }

        /// <summary>
        /// Stores the specified person's information in the cookie.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="personInfo">
        /// The person's information to store. If null and <paramref name="clearIfNull"/> is true,
        /// the cookie will be cleared and the person will be logged off from HealthVault.
        /// </param>
        ///
        /// <param name="clearIfNull">
        /// If true and <paramref name="personInfo"/> is null, the cookie will be cleared and the
        /// person will be logged off from HealthVault.
        /// </param>
        ///
        public static void SavePersonInfoToCookie(
            System.Web.HttpContext context,
            PersonInfo personInfo,
            bool clearIfNull)
        {
            SavePersonInfoToCookie(context, personInfo, clearIfNull, -1);
        }

        private static void SavePersonInfoToCookie(
            System.Web.HttpContext context,
            PersonInfo personInfo,
            bool clearIfNull,
            int cookieTimeout)
        {
            if (personInfo != null || clearIfNull)
            {
                if (configuration.UseAspSession)
                {
                    if (personInfo == null)
                    {
                        context.Session.Remove(configuration.CookieName);
                    }
                    else
                    {
                        context.Session[configuration.CookieName]
                            = PersonInfoAsCookie(personInfo);
                    }
                }
                else
                {
                    HttpCookie existingCookie =
                        context.Request.Cookies[configuration.CookieName];
                    HttpCookie cookie =
                        SavePersonInfoToCookie(personInfo, existingCookie, cookieTimeout);

                    context.Response.Cookies.Remove(configuration.CookieName);
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        /// <summary>
        /// Stores the specified person's information in the cookie.
        /// </summary>
        ///
        /// <param name="personInfo">
        /// The authenticated person's information.
        /// </param>
        ///
        /// <returns>
        /// A cookie containing the person's information.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="personInfo"/> is null, the returned cookie will have an
        /// expiration date in the past, and adding it to a response would result in the cookie
        /// being cleared.
        /// </remarks>
        ///
        public static HttpCookie SavePersonInfoToCookie(PersonInfo personInfo)
        {
            return SavePersonInfoToCookie(personInfo, null, -1);
        }

        /// <summary>
        /// Stores the specified person's information in the cookie.
        /// </summary>
        ///
        /// <param name="personInfo">
        /// The authenticated person's information.
        /// </param>
        ///
        /// <param name="existingCookie">
        /// The existing cooke containing the person's information. The expiration date of this
        /// cookie will be used as the expiration date of the returned cookie.
        /// </param>
        ///
        /// <returns>
        /// A cookie containing the person's information.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="personInfo"/> is null, the returned cookie will have an
        /// expiration date in the past, and adding it to a response would result in the cookie
        /// being cleared.
        /// </remarks>
        ///
        public static HttpCookie SavePersonInfoToCookie(
            PersonInfo personInfo,
            HttpCookie existingCookie)
        {
            return SavePersonInfoToCookie(personInfo, existingCookie, -1);
        }

        private static HttpCookie SavePersonInfoToCookie(
            PersonInfo personInfo,
            HttpCookie existingCookie,
            int cookieTimeout)
        {
            HttpCookie cookie =
                new HttpCookie(configuration.CookieName);
            cookie.HttpOnly = true;
            cookie.Secure = configuration.UseSslForSecurity;

            if (personInfo == null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
            else
            {
                if (cookieTimeout > 0)
                {
                    // If a greater than zero cookie timeout is in the
                    // query, then it means the user wishes the
                    // persist their auth token. Use this value.
                    cookieTimeout = Math.Min(
                        cookieTimeout,
                        configuration.MaxCookieTimeoutMinutes);

                    // Save the absolute expiration in the cookie. This
                    // is when the auth token itself expires.
                    // Therefore, we do not want this to be a sliding
                    // expiration. We want the cookie expiration to
                    // match the auth token expiration. Whenever the
                    // cookie is re-written, we need to preserve the
                    // expiration date of that cookie.
                    cookie.Expires =
                        DateTime.Now.AddMinutes(cookieTimeout);
                    cookie[WcTokenExpiration] =
                        cookie.Expires.ToUniversalTime().ToString();
                }
                else if (existingCookie != null)
                {
                    // If we do not have an explicit cookie timeout to
                    // set but have an existing cookie, then we want
                    // to preserve the expiration on the new cookie.
                    string expirationString =
                        existingCookie[WcTokenExpiration];

                    // If the expiration was not set in the existing
                    // cookie, then it is a session cookie. Do not
                    // overwrite the expiration on it.
                    if (!String.IsNullOrEmpty(expirationString))
                    {
                        DateTime expiration;

                        if (!DateTime.TryParse(expirationString, out expiration))
                        {
                            // Somehow the expiration cookie value
                            // failed to parse. Set it to the
                            // web.config timeout value.
                            cookieTimeout = configuration.CookieTimeoutMinutes;

                            if (cookieTimeout > 0)
                            {
                                cookie.Expires =
                                    DateTime.Now.AddMinutes(cookieTimeout);
                                cookie[WcTokenExpiration] =
                                    cookie.Expires.ToUniversalTime().ToString();
                            }
                        }
                        else
                        {
                            cookie.Expires = expiration.ToLocalTime();
                            cookie[WcTokenExpiration] =
                                cookie.Expires.ToUniversalTime().ToString();
                        }
                    }
                }
                else
                {
                    // We do not have an explicit cookie timeout to
                    // set and no exiting cookie. Set the cookie
                    // timeout to the one in web.config.
                    cookieTimeout =
                        configuration.CookieTimeoutMinutes;

                    if (cookieTimeout > 0)
                    {
                        // We only set the expiration if it is not a
                        // session cookie.
                        // NOTE: We do not write the expiration date
                        // out to the cookie to preserve existing
                        // behavior.
                        cookie.Expires = DateTime.Now.AddMinutes(cookieTimeout);
                    }
                }

                cookie[WcTokenPersonInfo]
                    = PersonInfoAsCookie(personInfo);
            }

            if (!String.IsNullOrEmpty(configuration.CookieDomain))
            {
                cookie.Domain = configuration.CookieDomain;
            }

            if (!String.IsNullOrEmpty(configuration.CookiePath))
            {
                cookie.Path = configuration.CookiePath;
            }

            return cookie;
        }

        private static string GetActionUrlRedirectOverride(System.Web.HttpContext context)
        {
            Uri actionUrlRedirectOverride =
                configuration.ActionUrlRedirectOverride;

            if (actionUrlRedirectOverride == null) return null;

            string actionUrlRedirectOverrideString = actionUrlRedirectOverride.OriginalString;
            if (actionUrlRedirectOverride.IsAbsoluteUri)
            {
                return actionUrlRedirectOverrideString;
            }

            Uri currentRequest = context.Request.Url;
            StringBuilder buffer = new StringBuilder(
                                        currentRequest.Scheme
                                        + Uri.SchemeDelimiter
                                        + currentRequest.Authority,
                                        128);
            buffer.Append(context.Request.ApplicationPath);
            if (buffer[buffer.Length - 1] != '/'
                && !actionUrlRedirectOverrideString.StartsWith("/"))
            {
                buffer.Append("/");
            }
            buffer.Append(actionUrlRedirectOverrideString);

            return buffer.ToString();
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="isMra">
        /// Whether this application simultaneously deals with multiple records
        /// for the same person.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signin action URL after
        /// signin.
        /// </param>
        ///
        /// <remarks>
        /// After the user successfully authenticates, they get redirected
        /// to the action url for which the target is set to either
        /// AppAuthSuccess or AppAuthRejected depending on whether the user
        /// authorized one or more records for use with the application,
        /// with the authentication token in the query
        /// string. This is stripped out and used to populate HealthVault
        /// data for the page.
        /// </remarks>
        ///
        public static void RedirectToLogOn(
            System.Web.HttpContext context,
            bool isMra,
            string actionUrlQueryString)
        {
            RedirectToLogOn(context, isMra, actionUrlQueryString, null /* signupCode */);
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="isMra">
        /// Whether this application simultaneously deals with multiple records
        /// for the same person.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signin action URL after
        /// signin.
        /// </param>
        ///
        /// <param name="signupCode">
        /// The signup code for creating a HealthVault account.  This is required
        /// for applications in locations with limited access to HealthVault.
        /// Signup codes may be obtained from
        /// <see cref="Connection.ApplicationConnection.NewSignupCode" />,
        /// </param>
        ///
        /// <remarks>
        /// After the user successfully authenticates, they get redirected
        /// to the action url for which the target is set to either
        /// AppAuthSuccess or AppAuthRejected depending on whether the user
        /// authorized one or more records for use with the application,
        /// with the authentication token in the query
        /// string. This is stripped out and used to populate HealthVault
        /// data for the page.
        /// </remarks>
        ///
        public static void RedirectToLogOn(
            System.Web.HttpContext context,
            bool isMra,
            string actionUrlQueryString,
            string signupCode)
        {
            var redirectParameters = new ShellRedirectParameters()
            {
                IsMultiRecordApplication = isMra,
                ActionQueryString = actionUrlQueryString,
                SignupCode = signupCode
            };

            RedirectToLogOn(context, redirectParameters);
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="redirectParameters">
        /// HealthVault Shell redirect parameters.
        /// </param>
        ///
        /// <remarks>
        /// After the user successfully authenticates, they get redirected
        /// to the action url for which the target is set to either
        /// AppAuthSuccess or AppAuthRejected depending on whether the user
        /// authorized one or more records for use with the application,
        /// with the authentication token in the query
        /// string. This is stripped out and used to populate HealthVault
        /// data for the page.
        /// </remarks>
        public static void RedirectToLogOn(
            System.Web.HttpContext context,
            ShellRedirectParameters redirectParameters)
        {
            redirectParameters = redirectParameters.Clone();
            redirectParameters.TargetLocation = "AUTH";
            redirectParameters.TokenRedirectionMethod = "POST";
            redirectParameters.ApplicationId = configuration.ApplicationId;

            RedirectToShellUrl(context, redirectParameters);
        }

        private static void ApplyWebConfigurationOnRedirectParameters(System.Web.HttpContext httpCtx, ShellRedirectParameters redirectParameters)
        {
            string actionUrlRedirectOverride = GetActionUrlRedirectOverride(httpCtx);
            if (actionUrlRedirectOverride != null)
            {
                redirectParameters.ReturnUrl = actionUrlRedirectOverride;
            }
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="isMra">
        /// Whether this application simultaneously deals with multiple records
        /// for the same person.
        /// </param>
        ///
        /// <remarks>
        /// After the user successfully authenticates, they get redirected
        /// to the action url for which the target is set to either
        /// AppAuthSuccess or AppAuthRejected depending on whether the user
        /// authorized one or more records for use with the application, with
        /// the authentication token in the query
        /// string. This is stripped out and used to populate HealthVault
        /// data for the page.
        /// </remarks>
        ///
        public static void RedirectToLogOn(System.Web.HttpContext context, bool isMra)
        {
            RedirectToLogOn(context, isMra, context.Request.Url.PathAndQuery);
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <remarks>
        /// After the user successfully authenticates, they get redirected
        /// back to the action url for which the target is set to either
        /// AppAuthSuccess or AppAuthRejected depending on whether the user
        /// authorized one or more records for use with the application,
        /// with the authentication token in the query
        /// string. This is stripped out and used to populate HealthVault
        /// data for the page.<br/>
        /// <br/>
        /// This overload assumes that the applications does not simultaneously
        /// deal with multiple records for the same person i.e. isMra is false.
        /// </remarks>
        ///
        public static void RedirectToLogOn(System.Web.HttpContext context)
        {
            RedirectToLogOn(context, false);
        }

        /// <summary>
        /// Signs the person out and cleans up the HealthVault session
        /// information.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <remarks>
        /// This should only be used by the HealthVault Shell.
        /// </remarks>
        ///
        private static void LogOff(System.Web.HttpContext context)
        {
            SavePersonInfoToCookie(context, null, true);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        public static void SignOut(System.Web.HttpContext context)
        {
            SignOut(context, null);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier for the application.
        /// </param>
        ///
        public static void SignOut(System.Web.HttpContext context, Guid appId)
        {
            SignOut(context, null, appId);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter if any.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        public static void SignOut(System.Web.HttpContext context, string actionUrlQueryString)
        {
            SignOut(
                context,
                actionUrlQueryString,
                configuration.ApplicationId);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter if any.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier of the application.
        /// </param>
        ///
        public static void SignOut(System.Web.HttpContext context, string actionUrlQueryString, Guid appId)
        {
            SignOut(context, actionUrlQueryString, appId, null);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter (including user's credential token) if any.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier of the application.
        /// </param>
        ///
        /// <param name="credentialToken">
        /// The user's credential token to sign out.
        /// </param>
        ///
        public static void SignOut(System.Web.HttpContext context, string actionUrlQueryString, Guid appId, string credentialToken)
        {
            SignOut(
                context,
                actionUrlQueryString,
                appId,
                credentialToken,
                configuration.DefaultHealthVaultShellUrl.OriginalString);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter (including user's credential token) if any.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier of the application.
        /// </param>
        ///
        /// <param name="credentialToken">
        /// The user's credential token to sign out.
        /// </param>
        ///
        /// <param name="serviceInstance">
        /// The HealthVault instance where the user account resides.
        /// If <b>null</b>, the configured default HealthVault Shell URL will be used.
        /// </param>
        public static void SignOut(
            System.Web.HttpContext context,
            string actionUrlQueryString,
            Guid appId,
            string credentialToken,
            HealthServiceInstance serviceInstance)
        {
            string shellUrl = null;
            if (serviceInstance != null)
            {
                shellUrl = serviceInstance.ShellUrl.OriginalString;
            }
            SignOut(context, actionUrlQueryString, appId, credentialToken, shellUrl);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter (including user's credential token) if any.
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        /// <param name="appId">
        /// The unique identifier of the application.
        /// </param>
        ///
        /// <param name="credentialToken">
        /// The user's credential token to sign out.
        /// </param>
        ///
        /// <param name="shellRedirectorUrl">
        /// The base HealthVault Shell redirector URL.  If <b>null</b>,
        /// the configured default HealthVault Shell URL will be used.
        /// </param>
        public static void SignOut(
            System.Web.HttpContext context,
            string actionUrlQueryString,
            Guid appId,
            string credentialToken,
            string shellRedirectorUrl)
        {
            LogOff(context);

            var redirectParameters = new ShellRedirectParameters()
            {
                TargetLocation = "APPSIGNOUT",
                ShellRedirectorUrl = shellRedirectorUrl,
                ActionQueryString = actionUrlQueryString,
                ApplicationId = appId
            };
            redirectParameters.TargetParameters.Add("credtoken", credentialToken);

            RedirectToShellUrl(context, redirectParameters);
        }

        /// <summary>
        /// Redirects application to Shell help page
        /// </summary>
        ///
        /// <param name="context">
        /// The current request context.
        /// </param>
        ///
        /// <param name="topic">
        /// Optional topic string. For example, "faq" would redirect the user's browser to the
        /// HealthVault frequently asked questions.
        /// </param>
        ///
        public static void GotoHelp(System.Web.HttpContext context, string topic)
        {
            string targetQuery = String.IsNullOrEmpty(topic)
                                    ? String.Empty : "?topicid=" + topic + "&";

            Uri helpUrl = ConstructShellTargetUrl(context, "HELP", targetQuery);

            context.Response.Redirect(helpUrl.ToString());
        }
    }
}
