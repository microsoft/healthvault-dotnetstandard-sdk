using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.HealthVault.Web.Mvc;

namespace SampleMVC.Controllers
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