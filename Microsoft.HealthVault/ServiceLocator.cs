namespace Microsoft.HealthVault
{
    internal class ServiceLocator : IServiceLocator
    {
        public virtual T GetInstance<T>()
        {
            return Ioc.Get<T>();
        }
    }
}
