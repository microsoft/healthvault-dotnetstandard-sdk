using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.HealthVault.AspNetCore;
using Microsoft.HealthVault.AspNetCore.Connection;

namespace SandboxWeb.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = HealthVaultAuthenticationDefaults.AuthenticationScheme)]
    public class OnlineController : Controller
    {
        // GET: HealthVault
        public async Task<ActionResult> Index()
        {
           IWebHealthVaultConnection webHealthVaultConnection =  await WebHealthVaultFactory.CreateWebConnectionAsync(HttpContext);

           PersonInfo personInfo =  await webHealthVaultConnection.GetPersonInfoAsync();

           IThingClient thingClient = webHealthVaultConnection.CreateThingClient();

           IReadOnlyCollection<Weight> weights = await thingClient.GetThingsAsync<Weight>(personInfo.GetSelfRecord().Id);
           
           return View(weights);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWeight()
        {
            IWebHealthVaultConnection webHealthVaultConnection = await HttpContext.CreateWebConnectionAsync();
            PersonInfo personInfo = await webHealthVaultConnection.GetPersonInfoAsync();

            IThingClient thingClient = webHealthVaultConnection.CreateThingClient();

            await thingClient.CreateNewThingsAsync(personInfo.GetSelfRecord().Id, new List<Weight> { new Weight(new HealthServiceDateTime(DateTime.Now), new WeightValue(10)) });

            return RedirectToAction("Index", new RouteValueDictionary());
        }
    }
}