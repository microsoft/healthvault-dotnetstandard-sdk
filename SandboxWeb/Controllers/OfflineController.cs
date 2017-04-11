using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Web;
using Microsoft.HealthVault.Web.Connection;

namespace SandboxWeb.Controllers
{
    public class OfflineController : Controller
    {
        public static string OfflinePersonId = "0e8bcb88-b286-4ff9-a2aa-6d15224233cf";
        public static Guid RecordId = Guid.Parse("9fc0b131-b3a8-4773-8dc0-5f25f8dbc37c");

        public static string InstanceId = "1";

        // GET: Offline
        public async Task<ActionResult> Index()
        {
            var thingClient = await CreateThingClientAsync();

            IReadOnlyCollection<Weight> weights = await thingClient.GetThingsAsync<Weight>(RecordId);

            return View(weights);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWeight()
        {
            IThingClient thingClient = await CreateThingClientAsync();

            await thingClient.CreateNewThingsAsync(RecordId, new List<Weight> { new Weight(new HealthServiceDateTime(DateTime.Now), new WeightValue(10)) });

            return RedirectToAction("Index", new RouteValueDictionary());
        }

        private async Task<IThingClient> CreateThingClientAsync()
        {
            IOfflineHealthVaultConnection offlineHealthVaultConnection =
                await WebHealthVaultFactory.CreateOfflineConnectionAsync(OfflinePersonId, InstanceId);
            IThingClient thingClient = offlineHealthVaultConnection.CreateThingClient();
            return thingClient;
        }
    }
}