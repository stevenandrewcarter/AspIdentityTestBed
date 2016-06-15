using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace BasicAuthentication.Controllers {

  /// <summary>
  /// Creates a Result with a Realm Response in the Header. This is part of the Basic Authentication Definition for when
  /// a unauthentication request is made.
  /// </summary>
  public class ResultWithChallenge : IHttpActionResult {
    private readonly IHttpActionResult next;
    private readonly string realm;

    public ResultWithChallenge(IHttpActionResult next, string realm) {
      this.next = next;
      this.realm = realm;
    }

    public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
      var res = await next.ExecuteAsync(cancellationToken);
      if (res.StatusCode == HttpStatusCode.Unauthorized) {
        res.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", realm));
      }
      return res;
    }
  }

  /// <summary>
  /// A Custom Filter to Provide Basic Authentication on any Route that is required
  /// </summary>
  public class BasicAuthenticator : Attribute, IAuthenticationFilter {
    private readonly string realm;
    public bool AllowMultiple { get { return false; } }

    /// <summary>
    /// Default Constructor, Provides the Realm string
    /// </summary>
    /// <param name="realm">Realm for Basic Authentication Challenge Response on a Unauthenticated request</param>
    public BasicAuthenticator(string realm) {
      this.realm = "realm=" + realm;
    }

    public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken) {
      var req = context.Request;
      // Check for the Basic Authentication Header
      if (req.Headers.Authorization != null && req.Headers.Authorization.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)) {
        // Decode the String from Base64
        Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        string credentials = encoding.GetString(Convert.FromBase64String(req.Headers.Authorization.Parameter));
        // Split the String on the ':' character
        string[] parts = credentials.Split(':');
        string userId = parts[0].Trim();
        string password = parts[1].Trim();
        // Check if the UserName and Password are Equal and not blank
        if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(password) && userId.Equals(password)) {
          // Build a Claim and place it on the Principle.
          var claims = new List<Claim>() { new Claim(ClaimTypes.Name, "badri") };
          var id = new ClaimsIdentity(claims, "Basic");
          var principal = new ClaimsPrincipal(new[] { id });
          context.Principal = principal;
        } else {
          context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
        }
      } else {
        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
      }
      return Task.FromResult(0);
    }

    public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) {
      context.Result = new ResultWithChallenge(context.Result, realm);
      return Task.FromResult(0);
    }
  }
}