using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MixedAuth.Startup))]
namespace MixedAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
