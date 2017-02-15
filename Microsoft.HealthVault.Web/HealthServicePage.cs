// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Web.Authentication;
using System;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// A base page for ASP.NET applications building against HealthVault.
    /// </summary>
    ///
    /// <remarks>
    /// By deriving from this base page, ASP.NET applications can inherit
    /// much of the data management capabilities that are needed to implement
    /// a HealthVault application. The base page handles redirecting to the
    /// HealthVault Shell to authenticate the user, getting information
    /// about the records the user is authorized to use, and serializing and
    /// making available to other pages the person's information and
    /// self/selected record.
    /// </remarks>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SecurityCritical]
    public class HealthServicePage : System.Web.UI.Page
    {
        /// <summary>
        /// Initializes the page to use SSL if necessary.
        /// </summary>
        ///
        /// <remarks>
        /// This is called on init of every page. We handle the SSL
        /// redirection at this point. Any page inheriting this class and
        /// wanting to be an insecure page must override this delegate
        /// method and set the isSecure member variable to false before
        /// calling the parent init.
        /// </remarks>
        ///
        /// <param name="e">
        /// Event arguments for the Init event thrown
        /// </param>
        [SecuritySafeCritical]
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Redirect to Secure page if necessary
            WebApplicationUtilities.PageOnInit(Context, IsPageSslSecure);
        }

        /// <summary>
        /// True if the page requires the user to be logged in to HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// The default implementation returns true. For pages that don't
        /// require logon, the page should override the property and return
        /// false.
        /// </remarks>
        ///
        protected virtual bool LogOnRequired => true;

        /// <summary>
        /// Gets a value indicating whether the page is for multi-record application.
        /// </summary>
        ///
        /// <remarks>
        /// By default the value is set to true. The default can be overridden on an application
        /// wide basis by setting the WCPage_IsMRA setting in the web.config file. The value can
        /// also be overridden on a per page basis by overriding this property in a derived class.
        /// </remarks>
        ///
        protected virtual bool IsMra => HealthWebApplicationConfiguration.Current.IsMultipleRecordApplication;

        /// <summary>
        /// Gets or sets the unique application identifier.
        /// </summary>
        ///
        /// <remarks>
        /// By default the value is set to <see cref="HealthApplicationConfiguration.ApplicationId"/>. If the
        /// application needs to change the application identifier it can set the value during
        /// <see cref="OnInit"/>.
        /// </remarks>
        ///
        protected virtual Guid ApplicationId { get; set; } = HealthWebApplicationConfiguration.Current.ApplicationId;

        /// <summary>
        /// Handles the PreLoad event for the page.
        /// </summary>
        ///
        /// <param name="e">
        /// Event arguments for the event.
        /// </param>
        ///
        /// <remarks>
        /// The base implementation calls <see cref="WebApplicationUtilities.PageOnPreLoad(HttpContext,bool)"/>
        /// and then calls the
        /// <see cref='System.Web.UI.Page.OnPreLoad(EventArgs)'/>.
        ///
        /// If a derived class overrides this method, it must call the base
        /// implementation so that the user data gets initialized.
        /// </remarks>
        [SecuritySafeCritical]
        protected override void OnPreLoad(EventArgs e)
        {
            _personInfo =
                WebApplicationUtilities.PageOnPreLoad(
                    Context,
                    LogOnRequired,
                    IsMra,
                    ApplicationId);
            base.OnPreLoad(e);
        }

        /// <summary>
        /// Initializes the user data for the page.
        /// </summary>
        ///
        /// <param name="logOnRequired">
        /// If true and the user hasn't already logged in to HealthVault,
        /// the page will automatically redirect to the HealthVault logon page
        /// and then return to this page with the auth-token.
        /// </param>
        ///
        /// <remarks>
        /// The base implementation of OnPreLoad will automatically call this
        /// method with the value specified in the <see cref="LogOnRequired"/>
        /// property. This method should only be called explicitly if there
        /// is a need to initialize the user data again after the page has
        /// been loaded.
        /// </remarks>
        ///
        protected void InitializeUserData(bool logOnRequired)
        {
            _personInfo =
                WebApplicationUtilities.PageOnPreLoad(
                    Context,
                    logOnRequired,
                    IsMra,
                    ApplicationId);
        }

        /// <summary>
        /// Gets a credential used to authenticate the web application to
        /// HealthVault.
        /// </summary>
        ///
        public WebApplicationCredential ApplicationAuthenticationCredential
        {
            get
            {
                if (_tier1AppAuthCredential == null)
                {
                    _tier1AppAuthCredential =
                        WebApplicationUtilities.GetApplicationAuthenticationCredential(ApplicationId);
                }
                return _tier1AppAuthCredential;
            }
        }
        private WebApplicationCredential _tier1AppAuthCredential;

        /// <summary>
        /// Gets a HealthVault connection authenticated at tier 1 for the configured
        /// default HealthVault web-service instance.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthVault.ApplicationConnection"/> connection.
        /// </returns>
        ///
        /// <remarks>
        /// If a connection has already been made on the page, that connection
        /// is returned. If no connection has been made, a new connection is
        /// created and returned.
        /// </remarks>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// If the application private key could not be found in the
        /// certificate store to sign requests.
        /// </exception>
        ///
        public ApplicationConnection ApplicationConnection
        {
            get
            {
                if (_tier1AuthConnection == null)
                {
                    _tier1AuthConnection =
                        WebApplicationUtilities.GetApplicationConnection(ApplicationId);
                }
                return _tier1AuthConnection;
            }
        }
        private ApplicationConnection _tier1AuthConnection;

        /// <summary>
        /// Gets a HealthVault connection authenticated at tier 3.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="WebApplicationConnection"/>
        /// connection.
        /// </returns>
        ///
        /// <remarks>
        /// If a connection has already been made on the page, that connection
        /// is returned. If no connection has been made, a new connection is
        /// created and returned.
        /// </remarks>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// If the application private key could not be found in the
        /// certificate store to sign requests.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If a person has not been logged in.
        /// </exception>
        ///
        public WebApplicationConnection AuthenticatedConnection
        {
            get
            {
                if (_tier3AuthConnection == null)
                {
                    Validator.ThrowInvalidIf(!IsLoggedIn, "PersonNotLoggedIn");

                    _tier3AuthConnection =
                        WebApplicationUtilities.GetAuthenticatedConnection(HttpContext.Current);
                }
                return _tier3AuthConnection;
            }
        }
        private WebApplicationConnection _tier3AuthConnection;

        /// <summary>
        /// Gets a HealthVault connection without an authentication token.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="ApplicationConnection"/> connection.
        /// </returns>
        ///
        /// <remarks>
        /// If a connection has already been made on the page, that connection
        /// is returned. If no connection has been made, a new connection is
        /// created a returned.
        /// </remarks>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// If the application private key could not be found in the
        /// certificate store to sign requests.
        /// </exception>
        ///
        public ApplicationConnection DictionaryConnection
        {
            get
            {
                if (_dictionaryConnection == null)
                {
                    _dictionaryConnection = this.ApplicationConnection;
                }
                return _dictionaryConnection;
            }
        }
        private ApplicationConnection _dictionaryConnection;

        /// <summary>
        /// Gets a HealthVault connection without an authentication token.
        /// </summary>
        ///
        /// <returns>
        /// A connection to HealthVault that does not contain user
        /// authentication information.
        /// </returns>
        ///
        /// <remarks>
        /// If a connection has already been made on the page, that connection
        /// is returned. If no connection has been made, a new connection is
        /// created a returned.
        /// </remarks>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// If the application private key could not be found in the
        /// certificate store to sign requests.
        /// </exception>
        ///
        public static AnonymousConnection AnonymousConnection
        {
            get
            {
                return new AnonymousConnection();
            }
        }

        /// <summary>
        /// Sets the selected health record for the application.
        /// </summary>
        ///
        /// <param name="activeRecord">
        /// The health record to set as the "active" record for the
        /// application.
        /// </param>
        ///
        /// <remarks>
        /// By setting the selected record, the HealthVault page framework will
        /// ensure that every page of the application will have the same
        /// record selected by serializing the record information into the
        /// session, and deserializing it for each page.
        /// </remarks>
        ///
        public void SetSelectedRecord(HealthRecordInfo activeRecord)
        {
            _personInfo.SelectedRecord = activeRecord;

            if (!_personInfo.AuthorizedRecords.ContainsKey(activeRecord.Id))
            {
                _personInfo.AuthorizedRecords.Add(activeRecord.Id, activeRecord);
            }
            WebApplicationUtilities.SavePersonInfoToCookie(Context, _personInfo);
        }

        /// <summary>
        /// Cleans the application's session of HealthVault information and
        /// then repopulates it.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// If a person isn't logged on when this is called.
        /// </exception>
        ///
        [SecurityCritical]
        public void RefreshAndPersist()
        {
            _personInfo =
                WebApplicationUtilities.RefreshAndSavePersonInfoToCookie(Context, _personInfo);
        }

        /// <summary>
        /// Cleans the application's session of HealthVault information and
        /// then repopulates it using the specified authentication token.
        /// </summary>
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
        public void RefreshAndPersist(string authToken)
        {
            _personInfo =
                WebApplicationUtilities.RefreshAndSavePersonInfoToCookie(Context, authToken, _personInfo.Connection.ServiceInstance);
        }

        /// <summary>
        /// Gets information about the logged-in person.
        /// </summary>
        ///
        /// <value>
        /// Information about the logged-in person or null if not logged-in.
        /// </value>
        ///
        /// <remarks>
        /// This information is a mirror of the
        /// <see cref="HealthVault.PersonInfo"/> class and is used to
        /// serialize information to and from the session.<br/>
        /// <br/>
        /// PersonInfo should never be set to null. If the application wants
        /// to log off the user, call <see cref="SignOut(string)"/>.
        /// </remarks>
        ///
        public PersonInfo PersonInfo
        {
            get { return _personInfo; }
            set
            {
                _personInfo = value;
                WebApplicationUtilities.SavePersonInfoToCookie(
                    HttpContext.Current,
                    _personInfo,
                    true);
            }
        }

        /// <summary>
        /// Gets whether a person is logged-in or not.
        /// </summary>
        ///
        /// <value>
        /// True if a person is logged-in, or false otherwise.
        /// </value>
        ///
        /// <remarks>
        /// The page handles logon automatically if the
        /// <see cref="LogOnRequired"/> is set to true.
        /// </remarks>
        ///
        public bool IsLoggedIn
        {
            get { return _personInfo != null; }
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the queryString params
        /// appended.
        /// </summary>
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
        public void RedirectToShellUrl(
            string targetLocation,
            string targetQuery)
        {
            RedirectToShellUrl(targetLocation, targetQuery, null);
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the queryString params
        /// appended.
        /// </summary>
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
        public void RedirectToShellUrl(
            string targetLocation,
            string targetQuery,
            string actionUrlQueryString)
        {
            var redirectParameters = new ShellRedirectParameters
            {
                TargetLocation = targetLocation,
                TargetQueryString = targetQuery,
                ActionQueryString = actionUrlQueryString,
            };

            // if logged in, attempt to redirect to the Shell instance
            // of the logged in user.
            if (IsLoggedIn
                && PersonInfo != null
                && PersonInfo.Connection != null
                && PersonInfo.Connection.ServiceInstance != null
                && PersonInfo.Connection.ServiceInstance.ShellUrl != null)
            {
                redirectParameters.ShellRedirectorUrl = PersonInfo.Connection.ServiceInstance.ShellUrl.OriginalString;
            }

            WebApplicationUtilities.RedirectToShellUrl(HttpContext.Current, redirectParameters);
        }

        /// <summary>
        /// Redirects to the HealthVault Shell URL with the query string params
        /// appended.
        /// </summary>
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
        public void RedirectToShellUrl(string targetLocation)
        {
            RedirectToShellUrl(targetLocation, null, null);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
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
                string targetLocation,
                string targetQuery)
        {
            return WebApplicationUtilities.ConstructShellTargetUrl(
                HttpContext.Current,
                targetLocation,
                targetQuery);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
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
            string targetLocation,
            string targetQuery,
            string actionUrlQueryString)
        {
            return WebApplicationUtilities.ConstructShellTargetUrl(
                HttpContext.Current,
                targetLocation,
                targetQuery,
                actionUrlQueryString);
        }

        /// <summary>
        /// Constructs a URL to be redirected to via the HealthVault URL
        /// redirector.
        /// </summary>
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
        public static Uri ConstructShellTargetUrl(string targetLocation)
        {
            return WebApplicationUtilities.ConstructShellTargetUrl(
                HttpContext.Current,
                targetLocation);
        }

        /// <summary>
        /// Gets a value indicating whether a page wants to use SSL for
        /// security.
        /// </summary>
        ///
        /// <remarks>
        /// Used to indicate if a page wants to use SSL for security.
        /// By default this property returns true as every page is assumed
        /// to use SSL when allowed. However individual pages can choose not
        /// to use SSL by overriding it and making it return false.
        /// </remarks>
        ///
        protected virtual bool IsPageSslSecure
        {
            get { return true; }
        }

        /// <summary>
        /// remove one variable from query string
        /// </summary>
        ///
        /// <param name="keys">
        /// variable(s) to remove
        /// </param>
        ///
        /// <returns>
        /// original url without key
        /// </returns>
        ///
        protected string StripFromQueryString(
            params string[] keys)
        {
            StringBuilder cleanUrl = new StringBuilder();
            cleanUrl.Append(Request.Url.GetLeftPart(UriPartial.Path));

            string sep = "?";
            string[] queryKeys = Request.QueryString.AllKeys;
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
                            Server.UrlEncode(Request.QueryString[ikey]));
                        sep = "&";
                    }
                }
            }

            return (cleanUrl.ToString());
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
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
        public void RedirectToLogOn(bool isMra)
        {
            string shellUrl = null;

            if (PersonInfo != null
                && PersonInfo.Connection != null
                && PersonInfo.Connection.ServiceInstance != null
                && PersonInfo.Connection.ServiceInstance.ShellUrl != null)
            {
                shellUrl = PersonInfo.Connection.ServiceInstance.ShellUrl.OriginalString;
            }

            var redirectParameters = new ShellRedirectParameters()
            {
                ShellRedirectorUrl = shellUrl,
                IsMultiRecordApplication = isMra,
                ActionQueryString = HttpContext.Current.Request.Url.PathAndQuery,
            };

            WebApplicationUtilities.RedirectToLogOn(HttpContext.Current, redirectParameters);
        }

        /// <summary>
        /// Redirects the caller's browser to the logon page for
        /// authentication.
        /// </summary>
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
        public void RedirectToLogOn()
        {
            RedirectToLogOn(IsMra);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL.
        /// </summary>
        ///
        public void SignOut()
        {
            SignOut(null);
        }

        /// <summary>
        /// Signs the person out, cleans up the HealthVault session
        /// information, and redirects the user's browser to the signout action
        /// URL with the specified querystring parameter if any.
        /// </summary>
        ///
        /// <param name="actionUrlQueryString">
        /// The query string parameters to pass to the signout action URL after
        /// cleaning up data.
        /// </param>
        ///
        public void SignOut(string actionUrlQueryString)
        {
            string userCredentialToken = GetUserCredentialToken();
            HealthServiceInstance serviceInstance = null;

            if (PersonInfo != null && PersonInfo.Connection != null)
            {
                serviceInstance = PersonInfo.Connection.ServiceInstance;
            }

            WebApplicationUtilities.SignOut(
                HttpContext.Current,
                actionUrlQueryString,
                ApplicationId,
                userCredentialToken,
                serviceInstance);
        }

        // private members
        private PersonInfo _personInfo;

        /// <summary>
        /// Gets the current page as a HealthServicePage.
        /// </summary>
        ///
        /// <remarks>
        /// This property can be used to retrieve an instance of the page from
        /// objects that don't have a reference to the page.
        /// The property accesses the <see cref="HttpContext"/> to retrieve
        /// the page and casts it to a HealthServicePage. If the page is not
        /// a HealthServicePage or the page hasn't been instantiated, null
        /// is returned.
        /// </remarks>
        ///
        public static HealthServicePage CurrentPage
        {
            get
            {
                return HttpContext.Current.Handler as HealthServicePage;
            }
        }

        private string GetUserCredentialToken()
        {
            string token = null;
            if (_personInfo != null &&
                _personInfo.Connection != null &&
                _personInfo.Connection.Credential is WebApplicationCredential)
            {
                token = ((WebApplicationCredential)_personInfo.Connection.Credential).SubCredential;
            }

            return token;
        }
    }
}
