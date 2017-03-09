namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Connection request header
    /// </summary>
    internal interface IConnectionRequestHeaderProvider
    {
        CryptoData GetAuthData(string methodName, string text);

        CryptoData GetBodyHash(string text);

        string PrepareAuthSessionHeader();
    }
}