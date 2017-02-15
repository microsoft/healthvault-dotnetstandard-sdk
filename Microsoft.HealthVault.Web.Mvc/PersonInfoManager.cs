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
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web;
using Microsoft.HealthVault.Web.Authentication;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// Manages the authentication of users to HealtVault and the <see cref="PersonInfo"/> object.
    /// </summary>
    public class PersonInfoManager
    {
        private readonly HttpContextBase _context;
        private readonly string _cookieName;
        private readonly bool _useSession;

        /// <summary>
        /// Constructs an instance of the <see cref="PersonInfoManager"/> class using the 
        /// default <see cref="HealthWebApplicationConfiguration"/> settings.
        /// </summary>
        /// <param name="context">The current context</param>
        public PersonInfoManager(HttpContextBase context)
            : this(context, HealthVault.Config.CookieName, HealthVault.Config.UseAspSession)
        {
        }

        /// <summary>
        /// Constructs an instance of the <see cref="PersonInfoManager"/> class using specific
        /// settings
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="cookieName">The name of the cookie used to store authenticationf info</param>
        /// <param name="useSession">Whether to use ASP.NET session state</param>
        public PersonInfoManager(HttpContextBase context, string cookieName, bool useSession)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(cookieName))
            {
                throw new ArgumentException("cookieName cannot be empty or null", nameof(cookieName));
            }

            _context = context;
            _cookieName = cookieName;
            _useSession = useSession;
        }

        /// <summary>
        /// The <see cref="PersonInfo"/> object representing currently authenticated HealthVault person
        /// </summary>
        public PersonInfo PersonInfo => _context.User?.PersonInfo();

        internal string CredentialToken
        {
            get
            {
                string token = null;
                if (PersonInfo?.Connection?.Credential is WebApplicationCredential)
                {
                    token = ((WebApplicationCredential)PersonInfo.Connection.Credential).SubCredential;
                }

                return token;
            }
        }

        /// <summary>
        /// Attempts to reload the <see cref="PersonInfo"/> object from the HealthVault service.
        /// </summary>
        public void Refresh()
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

            SetSelectedRecord(freshPersonInfo);
            Save(freshPersonInfo);
        }

        /// <summary>
        /// Persists the <see cref="PersonInfo"/> object to cookies or session state.
        /// </summary>
        /// <param name="personInfo">The <see cref="PersonInfo"/> object to persist</param>
        public void Save(PersonInfo personInfo)
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

            InitializePersonInfo(personInfo);
        }

        /// <summary>
        /// Clears the <see cref="PersonInfo"/> object from cookies, session state, and context.
        /// </summary>
        /// <remarks>This will effectively sign the user out of your website.</remarks>
        public void Clear()
        {
            ClearCookie();
            ClearUser();
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Fail gracefully if unable to load the personInfo for *any* reason")]
        internal PersonInfo Load()
        {
            HttpCookie cookie = GetCookie();
            if (cookie != null)
            {
                try
                {
                    PersonInfo person = PersonInfoFromCookie(cookie);
                    InitializePersonInfo(person);
                    return person;
                }
                catch
                {
                    Clear();
                }
            }

            return null;
        }

        private void InitializePersonInfo(PersonInfo personInfo)
        {
            personInfo.ApplicationSettingsChanged +=
                (o, e) => Save((PersonInfo)o);
            personInfo.SelectedRecordChanged +=
                (o, e) => Save((PersonInfo)o);

            SetUser(personInfo);
        }

        private void SetSelectedRecord(PersonInfo newPersonInfo)
        {
            if (PersonInfo.SelectedRecord != null &&
                newPersonInfo.AuthorizedRecords.ContainsKey(PersonInfo.SelectedRecord.Id))
            {
                newPersonInfo.SelectedRecord =
                    newPersonInfo.AuthorizedRecords[PersonInfo.SelectedRecord.Id];
            }
        }

        // Manage the current request's user context/principal
        private void ClearUser()
        {
            _context.User = new GenericPrincipal(new HealthVaultIdentity(null), null);
        }

        private void SetUser(PersonInfo personInfo)
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

            return _context.Request.Cookies[_cookieName];
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