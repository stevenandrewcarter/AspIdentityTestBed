using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ClaimsBasedAuthorization.Filters {
    /// <summary>
    /// Custom Attribute that provides claims verification and validation
    /// </summary>
    public class ClaimsAuthorizationAttribute : AuthorizationFilterAttribute {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        /// <summary>
        /// Check the given claims for the ClaimType and ClaimValue. If they exist return a success
        /// </summary>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken) {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (!principal.Identity.IsAuthenticated) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }
            // Check for the Claim, if the claims do not match return unauthorized response
            if (!(principal.HasClaim(x => x.Type == ClaimType && x.Value == ClaimValue))) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }
            //User is Authorized, complete execution
            return Task.FromResult<object>(null);
        }

    }
}