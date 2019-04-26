using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BCDHX.Startup))]
namespace BCDHX
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
