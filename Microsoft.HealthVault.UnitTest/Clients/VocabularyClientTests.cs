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

        [TestMethod]
        public void CreateClient()
        {
            var connection = Substitute.For<IConnectionInternal>();
            VocabularyClient client = new VocabularyClient { Connection = connection };
            Assert.IsTrue(client.Connection != null);
        }

        [TestMethod]
        public async Task GetVocabulary()
        {
            VocabularyClient client = new VocabularyClient();
            await client.GetVocabularyKeysAsync();

        }
    }
}
