using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.IntegrationTest
{
    public static class TestUtilities
    {
        public static async Task RemoveAllThingsAsync<T>(IThingClient thingClient, Guid recordId) where T : IThing
        {
            IReadOnlyCollection<T> things = await thingClient.GetThingsAsync<T>(recordId);

            if (things.Count > 0)
            {
                await thingClient.RemoveThingsAsync(recordId, things.ToList());
            }
        }
    }
}
