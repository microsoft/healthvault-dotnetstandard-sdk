// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Cookie;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Web.Providers
{
    /// <summary>
    /// <see cref="IWebConnectionInfoProvider"/>
    /// </summary>
    internal class WebConnectionInfoProvider : IWebConnectionInfoProvider
    {
        private const string HvTokenExpiration = "e";
        private const string HvTokenWebConnectionInfo = "w";
        private const string CookieVersion = "1:";

        private readonly IServiceLocator _serviceLocator;

        private readonly WebHealthVaultConfiguration _webHealthVaultConfiguration;
        private readonly ICookieDataManager _cookieDataManager;

        private readonly bool _useSession;
        private readonly string _cookieName;

        private readonly JsonSerializerSettings _serializerSettings;

        public WebConnectionInfoProvider(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;

            _webHealthVaultConfiguration = serviceLocator.GetInstance<WebHealthVaultConfiguration>();
            _cookieDataManager = serviceLocator.GetInstance<ICookieDataManager>();

            _useSession = _webHealthVaultConfiguration.UseAspSession;
            _cookieName = _webHealthVaultConfiguration.CookieName;

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Converters.Add(new WebConnectionInfoConverter());
        }

        public async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            IServiceInstanceProvider serviceInstanceProvider = _serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            IWebHealthVaultConnection connection = Ioc.Container.Locate<IWebHealthVaultConnection>(
                extraData:
                new
                {
                    serviceLocator = _serviceLocator
                });

            WebHealthVaultConnection webHealthVaultConnection = connection as WebHealthVaultConnection;
            webHealthVaultConnection.ServiceInstance = serviceInstance;
            webHealthVaultConnection.UserAuthToken = token;

            IPersonClient personClient = webHealthVaultConnection.CreatePersonClient();

            var personInfo = await personClient.GetPersonInfoAsync();

            WebConnectionInfo webConnectionInfo = new WebConnectionInfo
            {
                PersonInfo = personInfo,
                ServiceInstanceId = instanceId,
                SessionCredential = webHealthVaultConnection.SessionCredential,
                UserAuthToken = token
            };

            return webConnectionInfo;
        }

        public WebConnectionInfo TryLoad(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentException(nameof(httpContext));
            }

            HttpCookie cookie = GetCookie(httpContext);

            if (cookie == null)
            {
                return null;
            }

            string cookieVersionSerializedWebConnectionInfo = cookie[HvTokenWebConnectionInfo];
            int versionIndex = cookieVersionSerializedWebConnectionInfo.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);

            string serializedWebConnectionInfo = cookieVersionSerializedWebConnectionInfo.Substring(versionIndex + 1);

            string cookieData = UnmarshalCookieVersion1(serializedWebConnectionInfo);
            WebConnectionInfo webConnectionInfo = JsonConvert.DeserializeObject<WebConnectionInfo>(cookieData, _serializerSettings);

            return webConnectionInfo;
        }

        public void Save(HttpContextBase httpConext, WebConnectionInfo webConnectionInfo)
        {
            if (httpConext == null)
            {
                throw new ArgumentException(nameof(httpConext));
            }

            if (webConnectionInfo == null)
            {
                throw new ArgumentException(nameof(webConnectionInfo));
            }

            string serializedWebConnectionInfo = JsonConvert.SerializeObject(webConnectionInfo, _serializerSettings);
            string marshalledCookie = CookieVersion + MarshalCookieVersion1(serializedWebConnectionInfo);

            SetCookie(httpConext, marshalledCookie);
        }

        public void Clear(HttpContextBase httpContext)
        {
            if (_useSession)
            {
                httpContext.Session.Remove(_cookieName);
                return;
            }

            HttpCookie cookie = httpContext.Request.Cookies[_cookieName];

            if (cookie == null)
            {
                return;
            }

            cookie.Expires = DateTime.Now.AddDays(-1d);
            httpContext.Response.Cookies.Remove(_cookieName);
            httpContext.Response.Cookies.Add(cookie);
        }

        private HttpCookie CreateCookie(HttpContextBase httpContext, string webConnectionInfo)
        {
            HttpCookie cookie = new HttpCookie(_webHealthVaultConfiguration.CookieName)
            {
                HttpOnly = true,
                Secure = _webHealthVaultConfiguration.UseSslForSecurity,
                [HvTokenWebConnectionInfo] = webConnectionInfo
            };

            if (!String.IsNullOrEmpty(_webHealthVaultConfiguration.CookieDomain))
            {
                cookie.Domain = _webHealthVaultConfiguration.CookieDomain;
            }

            if (!String.IsNullOrEmpty(_webHealthVaultConfiguration.CookiePath))
            {
                cookie.Path = _webHealthVaultConfiguration.CookiePath;
            }

            // We only set the expiration if it is not a
            // session cookie.
            if (!_useSession)
            {
                if (string.IsNullOrEmpty(webConnectionInfo))
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    return cookie;
                }

                HttpCookie existingCookie = GetCookie(httpContext);

                DateTime cookieExpires;
                if (existingCookie == null)
                {
                    cookieExpires = DateTime.Now.AddMinutes(
                        _webHealthVaultConfiguration.CookieTimeoutDuration.TotalMinutes);
                }
                else
                {
                    string expirationString = existingCookie[HvTokenExpiration];
                    DateTime expiration;

                    if (!DateTime.TryParse(expirationString, out expiration))
                    {
                        cookieExpires = DateTime.Now.AddMinutes(_webHealthVaultConfiguration.CookieTimeoutDuration
                            .TotalMinutes);
                    }
                    else
                    {
                        cookieExpires = expiration.ToLocalTime();
                    }
                }

                cookie.Expires = cookieExpires;
                cookie[HvTokenExpiration] = cookie.Expires.ToUniversalTime()
                    .ToString(CultureInfo.InvariantCulture);
            }

            return cookie;
        }

        private HttpCookie GetCookie(HttpContextBase httpContext)
        {
            if (_useSession)
            {
                HttpCookie cookie = new HttpCookie("wrapper");
                cookie.Values["p"] = (string)httpContext.Session[_cookieName];
                return cookie;
            }

            return httpContext.Request.Cookies[_cookieName];
        }

        private void SetCookie(HttpContextBase httpContext, string cookieData)
        {
            HttpCookie newCookie = CreateCookie(httpContext, cookieData);

            if (newCookie != null)
            {
                if (_useSession)
                {
                    httpContext.Session[_cookieName] = newCookie.Values["p"];
                }
                else
                {
                    var httpCookieCollection = httpContext.Response.Cookies;

                    httpCookieCollection.Remove(_cookieName);
                    httpCookieCollection.Add(newCookie);
                }
            }
        }

        private string UnmarshalCookieVersion1(string serializedWebConnectionInfo)
        {
            string[] lengthAndData = serializedWebConnectionInfo.Split(new[] { '-' }, 2);
            string compressedData = lengthAndData[1];

            return _cookieDataManager.Decompress(compressedData);
        }

        private string MarshalCookieVersion1(string webConnectionInfoJson)
        {
            int bufferLength;
            string compressedData = _cookieDataManager.Compress(webConnectionInfoJson, out bufferLength);
            return bufferLength.ToString(CultureInfo.InvariantCulture) + "-" + compressedData;
        }
    }
}