using Microsoft.HealthVault.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.Clients
{
    [TestClass]
    public class VocabularyClientTests
    {
        [TestMethod]
        public void CreateClient()
        {
            VocabularyClient client = new VocabularyClient();
            Assert.IsTrue(client.Connection != null);
        }
    }
}
