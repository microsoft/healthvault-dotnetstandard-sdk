using System.Web.Mvc;
using Microsoft.HealthVault.Web.Providers;

namespace Microsoft.HealthVault.Web.Attributes
{
    /// <summary>
    /// Provides functionality to sign out from HealthVault
    /// </summary>
    /// <seealso cref="System.Web.Mvc.ActionFilterAttribute" />
    public class SignOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = AuthProvider.SignOut(filterContext);
            }
        }
    }
}
