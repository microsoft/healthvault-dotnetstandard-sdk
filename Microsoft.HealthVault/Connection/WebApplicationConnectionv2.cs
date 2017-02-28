using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// WebApplicationConnection
    /// </summary>
    /// <seealso cref="Microsoft.HealthVault.Connection.ApplicationConnectionV2" />
    public class WebApplicationConnectionv2 : ApplicationConnectionV2
    {
        internal override string ConstructAuthSessionHeader(XPathNavigator xpath)
        {
            throw new System.NotImplementedException();
        }

        // This class may be base where the web package would implement the method.
        public override Task AuthenticateAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
