using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASI.MGC.FS.Startup))]
namespace ASI.MGC.FS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
