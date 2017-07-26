using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAAS_AIMS.Startup))]
namespace SAAS_AIMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
