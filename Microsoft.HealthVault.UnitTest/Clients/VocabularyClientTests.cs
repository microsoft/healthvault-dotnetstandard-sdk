using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class VocabularyClientTests
    {
        private IConnectionInternal connection;
        private VocabularyClient client;

        [TestInitialize]
        public void InitializeTest()
        {
            this.connection = Substitute.For<IConnectionInternal>();
            this.client = new VocabularyClient { Connection = connection };
        }

        [TestMethod]
        public void CreateClient()
        {
            Assert.IsTrue(client.Connection == connection);
        }

        [TestMethod]
        public async Task GetVocabulary()
        {
            await client.GetVocabularyKeysAsync();
        }
    }
}
