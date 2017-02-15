using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
{
    internal interface IServiceLocator
    {
        ICryptoService CryptoService { get; }
    }
}
