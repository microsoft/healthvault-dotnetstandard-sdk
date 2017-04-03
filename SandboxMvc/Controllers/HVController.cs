using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.HealthVault;
using Microsoft.HealthVault.Web;
using Microsoft.HealthVault.Web.Attributes;

namespace SandboxMvc.Controllers
{
    [RequireSignIn]
    public class HVController : Controller
    {
        // GET: HV
        public ActionResult Index()
        {
            return View();
        }
    }
}