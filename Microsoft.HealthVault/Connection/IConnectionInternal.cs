using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Connection
{
    // TODO: Do we need an internal connection
    public interface IConnectionInternal : IConnection
    {
        CryptoData GetAuthData(string methodName, byte[] data);

        CryptoData GetInfoHash(byte[] data);

        void PrepareAuthSessionHeader(XmlWriter writer);

        // TODO: Temp. fix to quick run
        void StoreSessionCredentialInCookieXml(XmlWriter writer);

        void SetSessionCredentialFromCookieXml(XPathNavigator navigator);
    }
}