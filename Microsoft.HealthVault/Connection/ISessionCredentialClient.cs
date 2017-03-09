using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    internal interface ISessionCredentialClient
    {
        IConnectionInternal Connection { get; set; }

        Task<SessionCredential> GetSessionCredentialAsync(CancellationToken token);

        string ConstructCreateTokenInfoXml();
    }
}