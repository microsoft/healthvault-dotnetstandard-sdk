
namespace Microsoft.HealthVault
{
    internal interface IServiceLocator
    {
        T GetInstance<T>();
    }
}
