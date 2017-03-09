using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleMVC.Startup))]
namespace SampleMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
