// ********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
// ********************************************************

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web;
using Microsoft.Health.Web.Authentication;

namespace Microsoft.Health.Web.Mvc
{
    /// <summary>
    /// Manages the authentication of users to HealtVault and the <see cref="PersonInfo"/> object.
    /// </summary>
    public class AuthenticationManager
    {
        private HttpContextBase _context;
        private string _cookieName;
        private bool _useSession;

        /// <summary>
        /// Constructs an instance of the <see cref="AuthenticationManager"/> class using the 
        /// default <see cref="HealthWebApplicationConfiguration"/> settings.
        /// </summary>
        /// <param name="context">The current context</param>
        public AuthenticationManager(HttpContextBase context)
            : this(context, HealthVault.Config.CookieName, HealthVault.Config.UseAspSession)
        {
        }

        /// <summary>
        /// Constructs an instance of the <see cref="AuthenticationManager"/> class using specific
        /// settings
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="cookieName">The name of the cookie used to store authenticationf info</param>
        /// <param name="useSession">Whether to use ASP.NET session state</param>
        public AuthenticationManager(HttpContextBase context, string cookieName, bool useSession)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (String.IsNullOrEmpty(cookieName))
            {
                throw new ArgumentException("cookieName cannot be empty or null", "cookieName");
            }

            _context = context;
            _cookieName = cookieName;
            _useSession = useSession;
        }

        /// <summary>
        /// The <see cref="PersonInfo"/> object representing currently authenticated HealthVault person
        /// </summary>
        public PersonInfo PersonInfo
        {
            get
            {
                if (_context.User != null)
                {
                    return _context.User.PersonInfo();
                }

                return null;
            }
        }

        internal string CredentialToken
        {
            get
            {
                string token = null;
                if (PersonInfo != null &&
                    PersonInfo.Connection != null &&
                    PersonInfo.Connection.Credential is WebApplicationCredential)
                {
                    token = ((WebApplicationCredential)PersonInfo.Connection.Credential).SubCredential;
                }

                return token;
            }
        }

        /// <summary>
        /// Attempt to authentication the user.
        /// </summary>
        /// <remarks>This may result in a redirect if it is the beginning of a session.</remarks>
        public void Authenticate()
        {
            PersonInfo personInfo = ProcessAuthToken();
            if (personInfo == null)
            {
                // Try the cookie
                personInfo = LoadPersonInfo();
            }

            if (personInfo == null)
            {
                ClearContextUser();
            }
        }

        private PersonInfo ProcessAuthToken()
        {
            string authToken = _context.Request.Params["wctoken"];
            if (String.IsNullOrEmpty(authToken))
            {
                return null;
            }

            // Use the authToken issued by HealthVault to retrieve a PersonInfo object
            PersonInfo personInfo = RequestPersonInfo(authToken);
            SavePersonInfo(personInfo);

            // Then redirect to the original Url. The user's identity etc will be written to a cookie.
            // All subsequent authentication will use the cookie
            Redirect();
            return personInfo;
        }

        /// <summary>
        /// Loads the <see cref="PersonInfo"/> object from cookies or session state.
        /// </summary>
        /// <returns>A <see cref="PersonInfo"/> object representing the current authenticated user</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Fail gracefully if unable to load the personInfo for *any* reason")]
        public PersonInfo LoadPersonInfo()
        {
            HttpCookie cookie = GetCookie();
            if (cookie != null)
            {
                try
                {
                    PersonInfo person = PersonInfoFromCookie(cookie);
                    SetCurrentPersonInfo(person);
                    return person;
                }
                catch
                {
                    ClearPersonInfo();
                }
            }

            return null;
        }

        private static PersonInfo RequestPersonInfo(string authToken)
        {
            return WebApplicationUtilities.GetPersonInfo(authToken, HealthVault.Config.ApplicationId);
        }

        /// <summary>
        /// Attempts to reload the <see cref="PersonInfo"/> object from the HealthVault service.
        /// </summary>
        public void RefreshPersonInfo()
        {
            if (PersonInfo == null)
            {
                return;
            }

            PersonInfo freshPersonInfo = 
                HealthVaultPlatform.GetPersonInfo(PersonInfo.ApplicationConnection);
            if (freshPersonInfo == null)
            {
                return;
            }

            if (PersonInfo.SelectedRecord != null && freshPersonInfo.AuthorizedRecords.ContainsKey(PersonInfo.SelectedRecord.Id))
            {
                freshPersonInfo.SelectedRecord = freshPersonInfo.AuthorizedRecords[PersonInfo.SelectedRecord.Id];
            }

            SavePersonInfo(freshPersonInfo);
        }

        /// <summary>
        /// Persists the <see cref="PersonInfo"/> object to the HealthVault service.
        /// </summary>
        /// <param name="personInfo">The <see cref="PersonInfo"/> object to persist</param>
        public void SavePersonInfo(PersonInfo personInfo)
        {
            if (personInfo == null)
            {
                return;
            }

            HttpCookie newCookie = PersonInfoToCookie(personInfo);
            if (newCookie != null)
            {
                SetCookie(newCookie);
            }

            SetCurrentPersonInfo(personInfo);
        }

        /// <summary>
        /// Clears the <see cref="PersonInfo"/> object from cookies, session state, and context.
        /// </summary>
        /// <remarks>This will effectively sign the user out of your website.</remarks>
        public void ClearPersonInfo()
        {
            ClearCookie();
            ClearContextUser();
        }

        // Redirect to the original Url, minus tokens from the query string
        // The user's identity etc will be written to a cookie.
        // All subsequent authentication will use the cookie
        private void Redirect()
        {
            NameValueCollection query = HttpUtility.ParseQueryString(_context.Request.Url.Query);
            query.Remove("wctoken");
            query.Remove("suggestedtokenttl");

            UriBuilder newUrl = new UriBuilder(_context.Request.Url);
            newUrl.Query = query.ToString();

            _context.Response.Redirect(newUrl.Uri.OriginalString);
        }

        private void SetCurrentPersonInfo(PersonInfo personInfo)
        {
            personInfo.ApplicationSettingsChanged +=
                new EventHandler((o, e) => SavePersonInfo((PersonInfo)o));
            personInfo.SelectedRecordChanged +=
                new EventHandler((o, e) => SavePersonInfo((PersonInfo)o));

            SetPersonInfoAsContextUser(personInfo);
        }

        // Manage the current request's user context/principal
        private void ClearContextUser()
        {
            _context.User = new GenericPrincipal(new HealthVaultIdentity(null), null);
        }

        private void SetPersonInfoAsContextUser(PersonInfo personInfo)
        {
            _context.User = new GenericPrincipal(new HealthVaultIdentity(personInfo), null);
        }

        // PersonInfo Serialization - to & from cookies
        private static PersonInfo PersonInfoFromCookie(HttpCookie cookie)
        {
            return WebApplicationUtilities.LoadPersonInfoFromCookie(cookie);
        }

        private HttpCookie PersonInfoToCookie(PersonInfo personInfo)
        {
            HttpCookie existingCookie = GetCookie();
            return WebApplicationUtilities.SavePersonInfoToCookie(personInfo, existingCookie);
        }

        // HealthVault Cookie CRUD
        private HttpCookie GetCookie()
        {
            if (_useSession)
            {
                HttpCookie cookie = new HttpCookie("wrapper");
                cookie.Values["p"] = (string)_context.Session[_cookieName];
                return cookie;
            }
            else
            {
                return _context.Request.Cookies[_cookieName];
            }
        }

        private void SetCookie(HttpCookie cookie)
        {
            if (_useSession)
            {
                _context.Session[_cookieName] = cookie.Values["p"];
            }
            else
            {
                _context.Response.Cookies.Remove(_cookieName);
                _context.Response.Cookies.Add(cookie);
            }
        }

        private void ClearCookie()
        {
            if (_useSession)
            {
                _context.Session.Remove(_cookieName);
            }
            else
            {
                HttpCookie cookie = _context.Request.Cookies[_cookieName];
                if (cookie == null)
                {
                    return;
                }

                cookie.Expires = DateTime.Now.AddDays(-1d);
                _context.Response.Cookies.Remove(_cookieName);
                _context.Response.Cookies.Add(cookie);
            }
        }
    }
}