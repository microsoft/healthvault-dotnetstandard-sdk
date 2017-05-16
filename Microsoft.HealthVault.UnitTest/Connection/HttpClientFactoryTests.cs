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
        private IMessageHandlerFactory _subMessageHandlerFactory;
        private IDateTimeService _subDateTimeService;

        [TestInitialize]
        public void TestInitialize()
        {
            _subMessageHandlerFactory = Substitute.For<IMessageHandlerFactory>();
            _subDateTimeService = Substitute.For<IDateTimeService>();
        }

        [TestMethod]
        public void FactoryCaches()
        {
            DateTimeOffset firstNow = new DateTimeOffset(2017, 3, 27, 13, 40, 23, TimeSpan.Zero);
            DateTimeOffset secondNow = new DateTimeOffset(2017, 3, 27, 13, 42, 23, TimeSpan.Zero);

            _subDateTimeService.UtcNow
                .Returns(firstNow, secondNow);
            _subMessageHandlerFactory.Create().Returns(c => new HttpClientHandler());

            HttpClientFactory factory = CreateFactory();

            HttpClient firstClient = factory.GetOrCreateClient();
            HttpClient secondClient = factory.GetOrCreateClient();

            Assert.AreEqual(firstClient, secondClient);
        }

        [TestMethod]
        public void FactoryCacheExpires()
        {
            DateTimeOffset firstNow = new DateTimeOffset(2017, 3, 27, 13, 40, 23, TimeSpan.Zero);
            DateTimeOffset secondNow = new DateTimeOffset(2017, 3, 27, 13, 46, 23, TimeSpan.Zero);

            _subDateTimeService.UtcNow
                .Returns(firstNow, secondNow);
            _subMessageHandlerFactory.Create().Returns(c => new HttpClientHandler());

            HttpClientFactory factory = CreateFactory();

            HttpClient firstClient = factory.GetOrCreateClient();
            HttpClient secondClient = factory.GetOrCreateClient();

            Assert.AreNotEqual(firstClient, secondClient);
        }

        private HttpClientFactory CreateFactory()
        {
            return new HttpClientFactory(
                _subMessageHandlerFactory,
                _subDateTimeService);
        }
    }
}
