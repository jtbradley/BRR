using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blue_Ribbon.Startup))]
namespace Blue_Ribbon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
