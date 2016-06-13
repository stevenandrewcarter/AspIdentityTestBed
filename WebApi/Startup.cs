using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using WebApi.Infrastructure;
using WebApi.Providers;

namespace WebApi {
  public class Startup {
    public void Configuration(IAppBuilder app) {
      var httpConfig = new HttpConfiguration();
      ConfigureOAuthTokenGeneration(app);
      ConfigureOAuthTokenConsumption(app);
      ConfigureWebApi(httpConfig);
      app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
      app.UseWebApi(httpConfig);
    }

    private void ConfigureOAuthTokenGeneration(IAppBuilder app) {
      app.CreatePerOwinContext(ApplicationDbContext.Create);
      app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
      var oAuthServerOptions = new OAuthAuthorizationServerOptions {
        AllowInsecureHttp = true,
        TokenEndpointPath = new PathString("/oauth/token"),
        AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
        Provider = new CustomOAuthProvider(),
        AccessTokenFormat = new CustomJwtFormat("http://localhost:52123")
      };
      app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
      app.UseOAuthAuthorizationServer(oAuthServerOptions);
    }

    private void ConfigureWebApi(HttpConfiguration config) {
      config.MapHttpAttributeRoutes();
      var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
      jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }

    private void ConfigureOAuthTokenConsumption(IAppBuilder app) {
      var issuer = "http://localhost:52123";
      var audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
      var audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);
      app.UseJwtBearerAuthentication(
        new JwtBearAuthenticationOptions {
          AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
          AllowedAudiences = new[] { audienceId },
          IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[] {
            new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
          }
        });
    }
  }
}