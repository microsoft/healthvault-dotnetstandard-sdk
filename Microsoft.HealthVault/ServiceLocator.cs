
namespace Microsoft.HealthVault
{
    internal class ServiceLocator : IServiceLocator
    {
        public T GetInstance<T>()
        {
            return Ioc.Get<T>();
        }
    }
}
