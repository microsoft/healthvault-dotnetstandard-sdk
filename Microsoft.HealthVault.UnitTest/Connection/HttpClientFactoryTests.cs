using System;
using System.Net.Http;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Connection
{
    [TestClass]
    public class HttpClientFactoryTests
    {
        private IMessageHandlerFactory subMessageHandlerFactory;
        private IDateTimeService subDateTimeService;

        [TestInitialize]
        public void TestInitialize()
        {
            this.subMessageHandlerFactory = Substitute.For<IMessageHandlerFactory>();
            this.subDateTimeService = Substitute.For<IDateTimeService>();
        }

        [TestMethod]
        public void FactoryCaches()
        {
            DateTimeOffset firstNow = new DateTimeOffset(2017, 3, 27, 13, 40, 23, TimeSpan.Zero);
            DateTimeOffset secondNow = new DateTimeOffset(2017, 3, 27, 13, 42, 23, TimeSpan.Zero);

            this.subDateTimeService.UtcNow
                .Returns(firstNow, secondNow);
            this.subMessageHandlerFactory.Create().Returns(c => new HttpClientHandler());

            HttpClientFactory factory = this.CreateFactory();

            HttpClient firstClient = factory.GetFreshClient();
            HttpClient secondClient = factory.GetFreshClient();

            Assert.AreEqual(firstClient, secondClient);
        }

        [TestMethod]
        public void FactoryCacheExpires()
        {
            DateTimeOffset firstNow = new DateTimeOffset(2017, 3, 27, 13, 40, 23, TimeSpan.Zero);
            DateTimeOffset secondNow = new DateTimeOffset(2017, 3, 27, 13, 46, 23, TimeSpan.Zero);

            this.subDateTimeService.UtcNow
                .Returns(firstNow, secondNow);
            this.subMessageHandlerFactory.Create().Returns(c => new HttpClientHandler());

            HttpClientFactory factory = this.CreateFactory();

            HttpClient firstClient = factory.GetFreshClient();
            HttpClient secondClient = factory.GetFreshClient();

            Assert.AreNotEqual(firstClient, secondClient);
        }

        private HttpClientFactory CreateFactory()
        {
            return new HttpClientFactory(
                this.subMessageHandlerFactory,
                this.subDateTimeService);
        }
    }
}
