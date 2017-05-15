// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;
using System.Web;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Cookie;
using Microsoft.HealthVault.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using Arg = NSubstitute.Arg;

namespace Microsoft.HealthVault.Web.UnitTest.Providers
{
    /// <summary>
    /// Verifies functionality in <see cref="WebConnectionInfoProvider"/>
    /// </summary>
    [TestClass]
    public class WebConnectionInfoProviderTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();
        }

        /// <summary>
        /// Verify that webconnectioninfo is created when UserAuthToken and InstanceId is specified
        ///
        /// Mock IWebHealthVaultConnection and set personclient which return a dummy personinfo
        /// </summary>
        [TestMethod]
        public async Task WhenTokenAndInstanceId()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            WebHealthVaultConfiguration webHealthVaultConfiguration = Substitute.For<WebHealthVaultConfiguration>();
            ICookieDataManager cookieDataManager = Substitute.For<ICookieDataManager>();

            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);
            serviceLocator.GetInstance<ICookieDataManager>().Returns(cookieDataManager);

            WebHealthVaultConnection webHealthVaultConnection = Substitute.For<WebHealthVaultConnection>(serviceLocator);

            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConnection).As<IWebHealthVaultConnection>());

            IPersonClient mockedPersonClient = Substitute.For<IPersonClient>();
            mockedPersonClient.GetPersonInfoAsync().Returns(Task.FromResult(new PersonInfo()));

            Ioc.Container.Configure(c => c.ExportInstance(mockedPersonClient).As<IPersonClient>());

            // Act
            IWebConnectionInfoProvider webConnectionInfoProvider = new WebConnectionInfoProvider(serviceLocator);
            WebConnectionInfo webConnectionInfo = await webConnectionInfoProvider.CreateWebConnectionInfoAsync("someToken", "1");

            // Assert
            Assert.AreEqual("someToken", webConnectionInfo.UserAuthToken);
        }

        /// <summary>
        /// Verify that <see cref="WebConnectionInfoProvider"/> WebConnectionInfo loads from cookie
        /// </summary>
        [TestMethod]
        public void WhenLoadedFromCookie()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            // mock configuration
            MockConfiguration(serviceLocator);

            // mock request
            HttpContextBase httpContext = Substitute.For<HttpContextBase>();
            var requestWrapper = MockHttpRequestWithCookie();
            httpContext.Request.Returns(requestWrapper);

            // mock cookie data manager to send dummy webconnection info
            var webConnectionInfo = MockCookieDataManager(serviceLocator);

            IWebConnectionInfoProvider webConnectionInfoProvider = new WebConnectionInfoProvider(serviceLocator);

            // Act
            WebConnectionInfo result = webConnectionInfoProvider.TryLoad(httpContext);

            // Assert
            Assert.AreEqual(webConnectionInfo.UserAuthToken, result.UserAuthToken);
        }

        /// <summary>
        /// Verify that <see cref="WebConnectionInfoProvider"/> when the cookie is not present
        /// then null is returned making the upstream logic to authenticate
        /// </summary>
        [TestMethod]
        public void WhenCookieNotPresent()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            // mock configuration
            MockConfiguration(serviceLocator);

            // mock request
            HttpContextBase httpContext = Substitute.For<HttpContextBase>();
            var requestWrapper = MockHttpRequestWithCookie("Some");
            httpContext.Request.Returns(requestWrapper);

            IWebConnectionInfoProvider webConnectionInfoProvider = new WebConnectionInfoProvider(serviceLocator);

            // Act
            WebConnectionInfo result = webConnectionInfoProvider.TryLoad(httpContext);

            // Assert
            Assert.IsNull(result);
        }

        #region Mock helpers

        private static void MockConfiguration(IServiceLocator serviceLocator)
        {
            WebHealthVaultConfiguration webHealthVaultConfiguration =
                new WebHealthVaultConfiguration
                {
                    UseAspSession = false,
                    CookieName = "Test"
                };
            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);
        }

        private static WebConnectionInfo MockCookieDataManager(IServiceLocator serviceLocator)
        {
            WebConnectionInfo webConnectionInfo = new WebConnectionInfo { UserAuthToken = "test" };
            string serializedWebConnectionInfo = JsonConvert.SerializeObject(
                webConnectionInfo,
                new JsonSerializerSettings { Converters = { new WebConnectionInfoConverter() } });

            ICookieDataManager cookieDataManager = Substitute.For<ICookieDataManager>();
            cookieDataManager.Decompress(Arg.Any<string>()).Returns(serializedWebConnectionInfo);

            serviceLocator.GetInstance<ICookieDataManager>().Returns(cookieDataManager);
            return webConnectionInfo;
        }

        private static HttpRequestWrapper MockHttpRequestWithCookie(string cookieName = "Test")
        {
            HttpRequest httpRequest = new HttpRequest("", "http://www.bing.com", "");

            HttpCookie cookie = new HttpCookie(cookieName)
            {
                ["w"] = "1:10-sdfsd"
            };

            httpRequest.Cookies.Add(cookie);

            HttpRequestWrapper requestWrapper = new HttpRequestWrapper(httpRequest);
            return requestWrapper;
        }

        #endregion
    }
}
