using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Web;
using Microsoft.HealthVault.Web.Attributes;
using Microsoft.HealthVault.Web.Connection;

namespace SandboxWeb.Controllers
{
    [RequireSignIn]
    public class OnlineController : Controller
    {
        // GET: HealthVault
        public async Task<ActionResult> Index()
        {
            IWebHealthVaultConnection webHealthVaultConnection = await WebHealthVaultFactory.CreateWebConnectionAsync();

            PersonInfo personInfo = await webHealthVaultConnection.GetPersonInfoAsync();

            IThingClient thingClient = webHealthVaultConnection.CreateThingClient();

            IReadOnlyCollection<Weight> weights = await thingClient.GetThingsAsync<Weight>(personInfo.GetSelfRecord().Id);

            return View(weights);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWeight()
        {
            IWebHealthVaultConnection webHealthVaultConnection = await WebHealthVaultFactory.CreateWebConnectionAsync();
            PersonInfo personInfo = await webHealthVaultConnection.GetPersonInfoAsync();

            IThingClient thingClient = webHealthVaultConnection.CreateThingClient();

            await thingClient.CreateNewThingsAsync(personInfo.GetSelfRecord().Id, new List<Weight> { new Weight(new HealthServiceDateTime(DateTime.Now), new WeightValue(10)) });

            return RedirectToAction("Index", new RouteValueDictionary());
        }
    }
}