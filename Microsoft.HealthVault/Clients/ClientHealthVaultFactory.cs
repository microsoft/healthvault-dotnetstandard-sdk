using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// Factory class for all HealthVault clients. 
    /// </summary>
    public static class ClientHealthVaultFactory
    {
        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        /// <returns>
        /// An instance implementing IThingClient
        /// </returns>
        public static IThingClient GetThingClient(IHealthVaultConnection connection)
        {
            return new ThingClient(connection);
        }

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        public static IVocabularyClient GetVocabularyClient(IHealthVaultConnection connection)
        {
            return new VocabularyClient(connection);
        }

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        public static IPersonClient GetPersonClient(IHealthVaultConnection connection)
        {
            return new PersonClient(connection);
        }

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        public static IPlatformClient GetPlatformClient(IHealthVaultConnection connection)
        {
            return new PlatformClient(connection);
        }

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <returns>
        /// An instance implementing IActionPlanClient
        /// </returns>
        public static IActionPlanClient GetActionPlanClient(IHealthVaultConnection connection)
        {
            return new ActionPlanClient(connection);
        }
    }
}
