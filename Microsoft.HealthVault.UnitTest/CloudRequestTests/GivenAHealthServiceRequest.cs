// Copyright(c) Microsoft Corporation.

using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Transport.MessageFormatters.AuthenticationFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.HeaderFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.SessionFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.CloudRequestTests
{
    [TestClass]
    public class GivenAHealthServiceRequest
    {
        private HealthServiceMessage CreateDefault()
        {
            return new HealthServiceMessage(
                HealthVaultMethods.GetPersonInfo,
                5,
                null,
                null,
                null);
        }

        [TestMethod]
        public void WhenCreated_ShouldHaveTheCorrectValuesSet()
        {
            var req = this.CreateDefault();

            Assert.AreEqual(req.Method, HealthVaultMethods.GetPersonInfo);
            Assert.AreEqual(req.MethodVersion, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Expecting an ArgumentOutOfRangeException")]
        public void WhenSettingTimoutAsANegative_ShouldThrowAnException()
        {
            HealthServiceMessage req = CreateDefault();
            req.TimeoutSeconds = -1;
        }

        [TestMethod]
        public void WhenSettingTimeoutToZero_ShouldHaveTheCorrectValueSet()
        {
            HealthServiceMessage req = CreateDefault();

            req.TimeoutSeconds = 0;
            Assert.AreEqual(req.TimeoutSeconds, 0, "TimeoutSeconds");
        }

        [TestMethod]
        public void WhenSettingTimeoutTo1_ShouldHaveTheCorrectValueSet()
        {
            HealthServiceMessage req = CreateDefault();

            req.TimeoutSeconds = 1;
            Assert.AreEqual(req.TimeoutSeconds, 1, "TimeoutSeconds");
        }

        [TestMethod]
        public async Task WhenCreatingASessionToken_ThenExistingAuthSessionIsNotPopulated()
        {
            HealthVaultMethods method = HealthVaultMethods.CreateAuthenticatedSessionToken;
            HealthServiceMessage req = new HealthServiceMessage(
                method,
                5,
                null,
                authenticationFormatter: new NoAuthenticationFormatter(),
                authSessionOrAppId: new AppIdFormatter(method, Guid.Empty, Guid.Empty));

            req.BuildRequestXml();

            var request = Encoding.UTF8.GetString(req.XmlRequest);

            Assert.IsFalse(request.Contains("auth-token"));
            Assert.IsTrue(request.Contains("app-id"));
        }
    }
}
