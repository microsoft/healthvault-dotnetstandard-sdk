using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SandboxMvc.Startup))]
namespace SandboxMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
