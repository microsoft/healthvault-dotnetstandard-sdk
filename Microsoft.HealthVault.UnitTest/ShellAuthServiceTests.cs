// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest
{
    [TestClass]
    public class ShellAuthServiceTests
    {
        private const string ApplicationInstanceId = "b5c5593b-afb4-466d-88f2-31707fb8634b";

        private const string ApplicationCreationToken =
            "AiAAACH/OO7nZ9ZOtOr7JE7RzhC7rPSay87cX4LyKDyVka/dDC62cR9c2Jzk3HpJjkRAZuAAAADGGYShB8PnpleM+P7OWr9I0WLuqLfcRPBUzqtJrjiMlpzsnyIwOJ2iqp+99Lj6SrlcAf3s5Ea8eMqi5xQDOljUcqbkhTVTavsWUSwT1GdfGvu2VwCRW6gnrGWriGV4dn+SMQru2xDH+u1ZubLIFkLy4Omw7n0LXWaVFjNmtH7AQjheGornC5QidP3p5WCSuT+V4ye+sH51ie9rfx5ZZRSclFdIWTVxGhquQciSnBaX31X9PHY+VikmNFJwY5CAA2IRxrNWzETquZZzxLoIbpiTTcC4azwSp1I44uH0bsqHmSAAAAB4hhVJRM5ftA7iYjnKPtw0hK2lSHtZ88p9H/DpB9CDZCAAAAB4hhVJRM5ftA7iYjnKPtw0hK2lSHtZ88p9H/DpB9CDZA==";

        private IBrowserAuthBroker _subBrowserAuthBroker;
        private HealthVaultConfiguration _healthVaultConfiguration;

        private static readonly Uri s_shellUrl = new Uri("https://contoso.com/shell");

        private static readonly Guid s_masterApplicationId = new Guid("30945bac-d221-4f89-8197-6983a390066d");
        private static readonly Guid s_appInstanceId = new Guid(ApplicationInstanceId);

        [TestInitialize]
        public void TestInitialize()
        {
            _subBrowserAuthBroker = Substitute.For<IBrowserAuthBroker>();
            _healthVaultConfiguration = new HealthVaultConfiguration();
        }

        [TestMethod]
        public async Task WhenCallingProvisionApplicationAsync_ThenBrowserUrlIsConstructedCorrectly()
        {
            await TestProvisionApplicationAsync();
        }

        [TestMethod]
        public async Task WhenCallingProvisionApplicationAsyncWithAlternateSettings_ThenBrowserUrlIsConstructedCorrectly()
        {
            _healthVaultConfiguration.IsMultiRecordApp = true;
            _healthVaultConfiguration.MultiInstanceAware = false;

            await TestProvisionApplicationAsync();
        }

        [TestMethod]
        public async Task WhenCallingAuthorizeAdditionalRecordsAsync_ThenBrowserUrlIsConstructedCorrectly()
        {
            Uri successUri = new Uri("https://contoso.com/success?instanceid=3");

            _subBrowserAuthBroker
                .AuthenticateAsync(Arg.Is<Uri>(url => CheckStartUrlAdditionalRecords(url)), Arg.Any<Uri>())
                .Returns(successUri);

            ShellAuthService service = CreateService();
            await service.AuthorizeAdditionalRecordsAsync(s_shellUrl, s_appInstanceId);

            await _subBrowserAuthBroker
                .Received()
                .AuthenticateAsync(Arg.Any<Uri>(), Arg.Any<Uri>());
        }

        private async Task TestProvisionApplicationAsync()
        {
            Uri successUri = new Uri("https://contoso.com/success?instanceid=3");

            _subBrowserAuthBroker
                .AuthenticateAsync(Arg.Is<Uri>(url => CheckStartUrlProvisionApplication(url)), Arg.Any<Uri>())
                .Returns(successUri);

            ShellAuthService service = CreateService();
            string instanceId = await service.ProvisionApplicationAsync(s_shellUrl, s_masterApplicationId, ApplicationCreationToken, ApplicationInstanceId);

            Assert.AreEqual("3", instanceId);
        }

        private bool CheckStartUrlProvisionApplication(Uri url)
        {
            string urlString = url.AbsoluteUri;

            if (_healthVaultConfiguration.IsMultiRecordApp)
            {
                Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("ismra=true")));
            }
            else
            {
                Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("ismra=false")));
            }

            if (_healthVaultConfiguration.MultiInstanceAware)
            {
                Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("aib=true")));
            }
            else
            {
                Assert.IsFalse(urlString.Contains(Uri.EscapeDataString("aib=true")));
            }

            Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("mobile=true")));
            Assert.IsTrue(urlString.Contains(Uri.EscapeDataString(Uri.EscapeDataString(ApplicationInstanceId))));
            Assert.IsTrue(urlString.Contains(Uri.EscapeDataString(Uri.EscapeDataString(ApplicationCreationToken))));

            return true;
        }

        private bool CheckStartUrlAdditionalRecords(Uri url)
        {
            string urlString = url.AbsoluteUri;

            if (_healthVaultConfiguration.IsMultiRecordApp)
            {
                Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("ismra=true")));
            }
            else
            {
                Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("ismra=false")));
            }

            Assert.IsFalse(urlString.Contains(Uri.EscapeDataString("aib=")));
            Assert.IsTrue(urlString.Contains(Uri.EscapeDataString("appid=" + s_appInstanceId)));

            return true;
        }

        private ShellAuthService CreateService()
        {
            return new ShellAuthService(
                _subBrowserAuthBroker,
                _healthVaultConfiguration);
        }
    }
}
