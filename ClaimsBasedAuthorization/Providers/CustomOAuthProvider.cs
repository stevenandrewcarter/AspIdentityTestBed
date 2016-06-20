using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using ClaimsBasedAuthorization.Infrastructure;
using System.Threading.Tasks;

namespace ClaimsBasedAuthorization.Providers {
  public class CustomOAuthProvider : OAuthAuthorizationServerProvider {

    public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
      context.Validated();
      return Task.FromResult<object>(null);
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
      var allowedOrigin = "*";
      context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
      var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
      var user = await userManager.FindAsync(context.UserName, context.Password);
      if (user == null) {
        context.SetError("invalid_grant", "The user name or password is incorrect.");
        return;
      }
      var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
      // Add any extra claims to the JWT that the applications would need
      oAuthIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
      var ticket = new AuthenticationTicket(oAuthIdentity, null);
      context.Validated(ticket);
    }
  }
}