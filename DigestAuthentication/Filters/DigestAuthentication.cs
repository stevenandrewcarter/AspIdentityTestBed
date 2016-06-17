using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace DigestAuthentication.Filters {
  /// <summary>
  /// Creates a Result with a Digest Response in the Header. The Digest Response will contain the following headers
  /// WWW-Authenticate: Digest realm="givenRealm"
  ///                   qop="Quality of Protection, which will use MD5 for this example"
  ///                   nonce="Randomly generated Nonce"
  ///                   opaque="Provided server information encoded into base64 (This is generally used to pass around additional information)"
  /// </summary>
  public class ResultWithChallenge : IHttpActionResult {
    private readonly IHttpActionResult next;
    private readonly string realm;
    private readonly string opaque;

    public ResultWithChallenge(IHttpActionResult next, string realm, string opaque) {
      this.next = next;
      this.realm = realm;
      this.opaque = opaque;
    }

    public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
      var res = await next.ExecuteAsync(cancellationToken);
      if (res.StatusCode == HttpStatusCode.Unauthorized) {
        // Generate the Unauthorized Response
        var nonce = Guid.NewGuid().ToString("N");
        var headerResponse = new List<string>();
        headerResponse.Add(realm);
        headerResponse.Add("qop=\"MD5\"");
        headerResponse.Add("nonce=\"" + nonce + "\"");
        headerResponse.Add(opaque);
        res.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Digest", string.Join(",", headerResponse.ToArray())));
      }
      return res;
    }
  }

  public class Authentication : Attribute, IAuthenticationFilter {
    private readonly string realm;
    private readonly string opaque;
    public bool AllowMultiple { get { return false; } }

    /// <summary>
    /// Default Constructor for the Digest Authentication Filter. For the Quality of Protection we will use MD5
    /// </summary>
    /// <param name="realm">The Realm that is being protect. For example 'test@testdomain.com'</param>
    /// <param name="opaque">Optional parameter that provides additional information to each request that the client is expected to provide on additional requests</param>
    public Authentication(string realm, string opaque = "") {
      this.realm = "realm=\"" + realm + "\"";   
      var bytes = Encoding.UTF8.GetBytes(opaque);
      this.opaque = "opaque=\"" + Convert.ToBase64String(bytes) + "\"";
    }

    private string CalculateMD5Hash(string input) {
      // step 1, calculate MD5 hash from input
      var md5 = MD5.Create();
      var inputBytes = Encoding.ASCII.GetBytes(input);
      var hash = md5.ComputeHash(inputBytes);
      // step 2, convert byte array to hex string
      var sb = new StringBuilder();
      for (var i = 0; i < hash.Length; i++) {
        sb.Append(hash[i].ToString("X2"));
      }
      return sb.ToString();
    }

    private bool CheckAuthentication(string requestMethod, Dictionary<string, string> headerParameters) {
      // Cannot use a blank Username!
      if (!headerParameters.ContainsKey("username") || string.IsNullOrEmpty(headerParameters["username"])) {
        return false;
      }
      // Generate our MD5 hash to determine if the header values match our values. Username and Password are the same for the example
      var ha1 = CalculateMD5Hash(headerParameters["username"] + ":" + headerParameters["realm"] + ":" + headerParameters["username"]);
      var ha2 = CalculateMD5Hash(requestMethod + ":" + headerParameters["uri"]);
      var response = CalculateMD5Hash(ha1.ToLower() + ":" + headerParameters["nonce"] + ":" + ha2.ToLower());
      return response.Equals(headerParameters["response"], StringComparison.OrdinalIgnoreCase);     
    }

    public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken) {
      var req = context.Request;
      // Check for the Digest Authentication Header
      if (req.Headers.Authorization != null && req.Headers.Authorization.Scheme.Equals("digest", StringComparison.OrdinalIgnoreCase)) {
        // Get the header values        
        var parameters = req.Headers.Authorization.Parameter.Split(',');
        var headerParameters = new Dictionary<string, string>();
        foreach (var parameter in parameters) {
          var pair = parameter.Split('=');
          headerParameters.Add(pair[0].Trim(), pair[1].Replace('"', ' ').Trim());
        }   
        if (CheckAuthentication(req.Method.Method, headerParameters)) {
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
      context.Result = new ResultWithChallenge(context.Result, realm, opaque);
      return Task.FromResult(0);
    }
  }
}