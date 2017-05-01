using System;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest
{
    [TestClass]
    public class HealthVaultConnectionFactoryInternalTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();
            Ioc.Container.Configure(c => c.ExportInstance(CreateConfig()).As<HealthVaultConfiguration>());
            RegisterStubIoc<IHealthWebRequestClient>();
            RegisterStubIoc<IHealthServiceResponseParser>();
            RegisterStubIoc<ICryptographer>();
            Ioc.Container.Configure(c => c.ExportFactory(() => new HealthVaultSodaConnection(new ServiceLocator(), null, null)).As<HealthVaultSodaConnection>());
        }

        [TestMethod]
        public void WhenGetCalledMultipleTimes_ThenSameInstanceIsReturned()
        {
            var config = CreateConfig();

            HealthVaultConnectionFactoryInternal healthVaultConnectionFactoryInternal = new HealthVaultConnectionFactoryInternal();
            IHealthVaultSodaConnection connection = healthVaultConnectionFactoryInternal.GetOrCreateSodaConnection(config);

            IHealthVaultSodaConnection connection2 = healthVaultConnectionFactoryInternal.GetOrCreateSodaConnection(config);

            Assert.AreEqual(connection, connection2);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void WhenCalledWithoutMasterAppId_ThenInvalidOperationExceptionThrown()
        {
            HealthVaultConnectionFactoryInternal healthVaultConnectionFactoryInternal = new HealthVaultConnectionFactoryInternal();
            healthVaultConnectionFactoryInternal.GetOrCreateSodaConnection(new HealthVaultConfiguration());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void WhenCalledWithDifferentMasterAppId_ThenInvalidOperationExceptionThrown()
        {
            var config1 = CreateConfig();

            var config2 = new HealthVaultConfiguration
            {
                MasterApplicationId = new Guid("e2bab95d-cbb4-497e-ac2a-122d53a04b7a")
            };

            HealthVaultConnectionFactoryInternal healthVaultConnectionFactoryInternal = new HealthVaultConnectionFactoryInternal();
            IHealthVaultSodaConnection connection = healthVaultConnectionFactoryInternal.GetOrCreateSodaConnection(config1);

            healthVaultConnectionFactoryInternal.GetOrCreateSodaConnection(config2);
        }

        private static HealthVaultConfiguration CreateConfig()
        {
            return new HealthVaultConfiguration
            {
                MasterApplicationId = new Guid("4f274efe-c7b2-488d-a0d1-b1686866dec7")
            };
        }

        private static void RegisterStubIoc<T>() where T : class
        {
            Ioc.Container.Configure(c => c.ExportInstance(Substitute.For<T>()).As<T>());
        }
    }
}
