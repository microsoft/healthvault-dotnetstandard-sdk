using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
{
    internal interface IServiceLocator
    {
        T GetInstance<T>();
    }
}
