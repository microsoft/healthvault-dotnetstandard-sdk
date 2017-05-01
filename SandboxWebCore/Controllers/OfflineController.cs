using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.HealthVault.AspNetCore;
using Microsoft.HealthVault.AspNetCore.Connection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;

namespace SandboxWebCore.Controllers
{
    public class OfflineController : Controller
    {
        public static string OfflinePersonId = "0e8bcb88-b286-4ff9-a2aa-6d15224233cf";
        public static Guid RecordId = Guid.Parse("9fc0b131-b3a8-4773-8dc0-5f25f8dbc37c");

        public static string InstanceId = "1";

        // GET: Offline
        public async Task<ActionResult> Index()
        {
            var thingClient = await this.CreateThingClientAsync();

            IReadOnlyCollection<Weight> weights = await thingClient.GetThingsAsync<Weight>(RecordId);

            return this.View(weights);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWeight()
        {
            IThingClient thingClient = await this.CreateThingClientAsync();

            await thingClient.CreateNewThingsAsync(RecordId, new List<Weight> { new Weight(new HealthServiceDateTime(DateTime.Now), new WeightValue(10)) });

            return this.RedirectToAction("Index", new RouteValueDictionary());
        }

        private async Task<IThingClient> CreateThingClientAsync()
        {
            IOfflineHealthVaultConnection offlineHealthVaultConnection = await WebHealthVaultFactory.CreateOfflineConnectionAsync(OfflinePersonId, InstanceId);
            IThingClient thingClient = offlineHealthVaultConnection.CreateThingClient();
            return thingClient;
        }
    }
}