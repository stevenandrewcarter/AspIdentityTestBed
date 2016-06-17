using Newtonsoft.Json.Serialization;
using Owin;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace DigestAuthentication {
  public class Startup {
    public void Configuration(IAppBuilder app) {
      var httpConfig = new HttpConfiguration();
      ConfigureWebApi(httpConfig);
      app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
      app.UseWebApi(httpConfig);
    }

    private void ConfigureWebApi(HttpConfiguration config) {
      config.MapHttpAttributeRoutes();
      var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
      jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
  }
}