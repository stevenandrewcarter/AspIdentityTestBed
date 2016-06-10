using Newtonsoft.Json.Serialization;
using Owin;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using WebApi.Infrastructure;

namespace WebApi {
  public class Startup {
    public void Configuration(IAppBuilder app) {
      var httpConfig = new HttpConfiguration();
      ConfigureOAuthTokenGeneration(app);
      ConfigureWebApi(httpConfig);
      app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
      app.UseWebApi(httpConfig);
    }

    private void ConfigureOAuthTokenGeneration(IAppBuilder app) {
      app.CreatePerOwinContext(ApplicationDbContext.Create);
      app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
    }

    private void ConfigureWebApi(HttpConfiguration config) {
      config.MapHttpAttributeRoutes();
      var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
      jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
  }
}