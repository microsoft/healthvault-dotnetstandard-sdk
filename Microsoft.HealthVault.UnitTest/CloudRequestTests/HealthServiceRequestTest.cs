// Copyright(c) Microsoft Corporation.

using System;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.CloudRequestTests
{
    [TestClass]
    public class HealthServiceRequestTest
    {
        private IConnectionInternal connection;
        private HealthVaultConfiguration config;
        private IHealthWebRequestFactory webRequestFactory;
        private HealthServiceInstance serviceInstance;

        [TestInitialize]
        public void InitializeTest()
        {
            this.connection = Substitute.For<IConnectionInternal>();
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

        #region Ctor tests


        [TestMethod]
        public void CreateServiceRequest()
        {
            var req = this.CreateDefault();

            Assert.AreEqual(req.Method, HealthVaultMethods.GetPersonInfo);
            Assert.AreEqual(req.MethodVersion, 5);
        }

        [TestMethod]
        public void CreateInvalidServiceRequest()
        {
            try
            {
                HealthServiceRequest req = new HealthServiceRequest(null, HealthVaultMethods.GetPersonInfo, 5);
                Assert.Fail("Expecting an ArgumentNullException.");
            }
            catch (ArgumentNullException)
            {
                // pass
            }
        }

        #endregion Ctor tests

        #region Properties

        [TestMethod]
        public void TimeoutSeconds_setNegative()
        {
            HealthServiceRequest req = CreateDefault();

            try
            {
                req.TimeoutSeconds = -1;
                Assert.Fail("Expecting an ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // pass
            }
        }

        [TestMethod]
        public void TimeoutSeconds_setZero()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 0;
            Assert.AreEqual(req.TimeoutSeconds, 0, "TimeoutSeconds");
        }

        [TestMethod]
        public void TimeoutSeconds_setPositive()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 1;
            Assert.AreEqual(req.TimeoutSeconds, 1, "TimeoutSeconds");
        }

        #endregion Properties

        [TestMethod]
        public async Task ExecuteMethod()
        {
            HealthServiceRequest req = CreateDefault();
            HealthServiceResponseData response = await req.ExecuteAsync();
            Assert.AreEqual(response.Code, HealthServiceStatusCode.Ok);
            // TODO: add some validation of the response to make sure that it was parsed correctly for what we expect

            // Note: investigate this, as it does not seem to be working as expected
            this.webRequestFactory.Received().CreateWebRequest(Arg.Any<byte[]>(), Arg.Any<int>());
        }
    }
}
