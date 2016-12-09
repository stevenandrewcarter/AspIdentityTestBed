using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BasicIdentity.Startup))]
namespace BasicIdentity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
