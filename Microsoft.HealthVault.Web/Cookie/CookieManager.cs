// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Web;
using Microsoft.HealthVault.Web.Configuration;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Web.Cookie
{
    /// <summary>
    /// <see cref="ICookieManager"/>
    /// </summary>
    internal class CookieManager : ICookieManager
    {
        private const string HvTokenExpiration = "e";
        private const string HvTokenWebConnectionInfo = "w";
        private const string CookieVersion = "1:";

        private readonly IServiceLocator serviceLocator;

        private readonly WebHealthVaultConfiguration webHealthVaultConfiguration;
        private readonly ICookieDataManager cookieDataManager;

        private readonly bool useSession;
        private readonly string cookieName;

        private readonly JsonSerializerSettings serializerSettings;

        public CookieManager(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;

            this.webHealthVaultConfiguration = this.serviceLocator.GetInstance<WebHealthVaultConfiguration>();
            this.cookieDataManager = this.serviceLocator.GetInstance<ICookieDataManager>();

            useSession = webHealthVaultConfiguration.UseAspSession;
            cookieName = webHealthVaultConfiguration.CookieName;

            serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new WebConnectionInfoConverter());
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

            string serializedWebConnectionInfo = JsonConvert.SerializeObject(webConnectionInfo, serializerSettings);
            string marshalledCookie = CookieVersion + this.MarshalCookieVersion1(serializedWebConnectionInfo);

            this.SetCookie(httpConext, marshalledCookie);
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

            string cookieData = this.UnmarshalCookieVersion1(serializedWebConnectionInfo);
            WebConnectionInfo webConnectionInfo = JsonConvert.DeserializeObject<WebConnectionInfo>(cookieData, serializerSettings);

            return webConnectionInfo;
        }

        public void Clear(HttpContextBase httpContext)
        {
            if (useSession)
            {
                httpContext.Session.Remove(cookieName);
                return;
            }

            HttpCookie cookie = httpContext.Request.Cookies[cookieName];

            if (cookie == null)
            {
                return;
            }

            cookie.Expires = DateTime.Now.AddDays(-1d);
            httpContext.Response.Cookies.Remove(cookieName);
            httpContext.Response.Cookies.Add(cookie);
        }

        private HttpCookie CreateCookie(HttpContextBase httpContext, string webConnectionInfo)
        {
            HttpCookie cookie = new HttpCookie(this.webHealthVaultConfiguration.CookieName)
            {
                HttpOnly = true,
                Secure = this.webHealthVaultConfiguration.UseSslForSecurity,
                [HvTokenWebConnectionInfo] = webConnectionInfo
            };

            if (!String.IsNullOrEmpty(this.webHealthVaultConfiguration.CookieDomain))
            {
                cookie.Domain = this.webHealthVaultConfiguration.CookieDomain;
            }

            if (!String.IsNullOrEmpty(this.webHealthVaultConfiguration.CookiePath))
            {
                cookie.Path = this.webHealthVaultConfiguration.CookiePath;
            }

            // We only set the expiration if it is not a
            // session cookie.
            if (!useSession)
            {
                if (string.IsNullOrEmpty(webConnectionInfo))
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    return cookie;
                }

                HttpCookie existingCookie = this.GetCookie(httpContext);

                DateTime cookieExpires;
                if (existingCookie == null)
                {
                    cookieExpires = DateTime.Now.AddMinutes(
                        this.webHealthVaultConfiguration.CookieTimeoutDuration.TotalMinutes);
                }
                else
                {
                    string expirationString = existingCookie[HvTokenExpiration];
                    DateTime expiration;

                    if (!DateTime.TryParse(expirationString, out expiration))
                    {
                        cookieExpires = DateTime.Now.AddMinutes(this.webHealthVaultConfiguration.CookieTimeoutDuration
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
            if (this.useSession)
            {
                HttpCookie cookie = new HttpCookie("wrapper");
                cookie.Values["p"] = (string)httpContext.Session[cookieName];
                return cookie;
            }

            return httpContext.Request.Cookies[cookieName];
        }

        private void SetCookie(HttpContextBase httpContext, string cookieData)
        {
            HttpCookie newCookie = this.CreateCookie(httpContext, cookieData);

            if (newCookie != null)
            {
                if (this.useSession)
                {
                    httpContext.Session[cookieName] = newCookie.Values["p"];
                }
                else
                {
                    var httpCookieCollection = httpContext.Response.Cookies;

                    httpCookieCollection.Remove(cookieName);
                    httpCookieCollection.Add(newCookie);
                }
            }
        }

        private string UnmarshalCookieVersion1(string serializedWebConnectionInfo)
        {
            string[] lengthAndData = serializedWebConnectionInfo.Split(new[] { '-' }, 2);

            int expectedCompressedDataLength = Int32.Parse(lengthAndData[0], CultureInfo.InvariantCulture);

            string compressedData = lengthAndData[1];
            var compressedDataLength = compressedData.Length;

            if (compressedDataLength != expectedCompressedDataLength)
            {
                throw new Exception($"Excpected compressed data length: {expectedCompressedDataLength} should match actual compressed data length {compressedDataLength}");
            }

            return this.cookieDataManager.Decompress(compressedData);
        }

        private string MarshalCookieVersion1(string webConnectionInfoJson)
        {
            int bufferLength;
            string compressedData = this.cookieDataManager.Compress(webConnectionInfoJson, out bufferLength);
            return bufferLength.ToString(CultureInfo.InvariantCulture) + "-" + compressedData;
        }
    }
}