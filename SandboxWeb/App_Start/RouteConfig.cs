using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SandboxWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Redirect",
                "Redirect",
                new { controller = "HealthVaultActionRedirect", action = "Index" },
                new[] { "Microsoft.HealthVault.Web.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
