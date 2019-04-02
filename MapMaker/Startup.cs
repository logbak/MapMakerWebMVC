using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MapMaker.Startup))]
namespace MapMaker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
