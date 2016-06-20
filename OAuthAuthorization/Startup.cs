using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OAuthAuthorization.Startup))]
namespace OAuthAuthorization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
