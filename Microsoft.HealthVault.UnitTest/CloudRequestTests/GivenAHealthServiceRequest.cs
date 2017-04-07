// Copyright(c) Microsoft Corporation.

using System;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.CloudRequestTests
{
    [TestClass]
    public class GivenAHealthServiceRequest
    {
        private IConnectionInternal connection;
        private HealthVaultConfiguration config;
        private IHealthWebRequestFactory webRequestFactory;
        private HealthServiceInstance serviceInstance;

        [TestInitialize]
        public void InitializeTest()
        {
            this.connection = Substitute.For<IConnectionInternal>();
            this.connection.GetAuthData(Arg.Any<HealthVaultMethods>(), Arg.Any<byte[]>()).Returns(new CryptoData { Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm, Value = Guid.NewGuid().ToString() });
            this.config = Substitute.For<HealthVaultConfiguration>();
            this.webRequestFactory = Substitute.For<IHealthWebRequestFactory>();
            this.serviceInstance = new HealthServiceInstance("id", "name", "description", new Uri("http://microsoft.com"), new Uri("http://microsoft.com"));
            this.connection.ServiceInstance.Returns(this.serviceInstance);
            this.webRequestFactory.CreateWebRequest(Arg.Any<byte[]>(), Arg.Any<int>())
                .Returns(new TestHealthWebRequest());
        }

        private HealthServiceRequest CreateDefault()
        {
            return new HealthServiceRequest(
                connection,
                HealthVaultMethods.GetPersonInfo,
                5,
                config: this.config,
                requestFactory: this.webRequestFactory);
        }

        [TestMethod]
        public void WhenCreated_ShouldHaveTheCorrectValuesSet()
        {
            var req = this.CreateDefault();

            Assert.AreEqual(req.Method, HealthVaultMethods.GetPersonInfo);
            Assert.AreEqual(req.MethodVersion, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Expecting an ArgumentNullException.")]
        public void WhenCreatedWithoutAConnection_ShouldThrowAnException()
        {
            HealthServiceRequest req = new HealthServiceRequest(null, HealthVaultMethods.GetPersonInfo, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Expecting an ArgumentOutOfRangeException")]
        public void WhenSettingTimoutAsANegative_ShouldThrowAnException()
        {
            HealthServiceRequest req = CreateDefault();
            req.TimeoutSeconds = -1;
        }

        [TestMethod]
        public void WhenSettingTimeoutToZero_ShouldHaveTheCorrectValueSet()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 0;
            Assert.AreEqual(req.TimeoutSeconds, 0, "TimeoutSeconds");
        }

        [TestMethod]
        public void WhenSettingTimeoutTo1_ShouldHaveTheCorrectValueSet()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 1;
            Assert.AreEqual(req.TimeoutSeconds, 1, "TimeoutSeconds");
        }

        [TestMethod]
        public async Task WhenExecutingGetPersonInfo_ShouldHaveAValidResponse()
        {
            HealthServiceRequest req = CreateDefault();
            HealthServiceResponseData response = await req.ExecuteAsync();
            Assert.AreEqual(response.Code, HealthServiceStatusCode.Ok);
            // TODO: add some validation of the response to make sure that it was parsed correctly for what we expect

            // Note: investigate this, as it does not seem to be working as expected
            this.webRequestFactory.Received().CreateWebRequest(Arg.Any<byte[]>(), Arg.Any<int>());
        }

        [TestMethod]
        public async Task WhenCreatingASessionToken_ThenExistingAuthSessionIsNotPopulated()
        {
            HealthServiceRequest req = new HealthServiceRequest(
                connection,
                HealthVaultMethods.CreateAuthenticatedSessionToken,
                5,
                config: this.config,
                requestFactory: this.webRequestFactory);

            // Give an invalid (expired token)
            this.connection.GetInfoHash(Arg.Any<byte[]>()).Returns(new CryptoData { Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm, Value = Guid.NewGuid().ToString() });
            this.connection.SessionCredential.Returns(new SessionCredential { Token = Guid.NewGuid().ToString() });

            HealthServiceResponseData response = await req.ExecuteAsync();
            Assert.AreEqual(response.Code, HealthServiceStatusCode.Ok);

            //Anonymous methods should not add auth session
            this.connection.DidNotReceive().PrepareAuthSessionHeader(Arg.Any<XmlWriter>(), Arg.Any<Guid?>());
            this.webRequestFactory.Received().CreateWebRequest(Arg.Any<byte[]>(), Arg.Any<int>());
        }
    }
}
