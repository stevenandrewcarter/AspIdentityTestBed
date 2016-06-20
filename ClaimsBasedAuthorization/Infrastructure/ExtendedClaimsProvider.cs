using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ClaimsBasedAuthorization.Infrastructure {
  /// <summary>
  /// Custom provider for adding in claims to a JWT
  /// </summary>
  public static class ExtendedClaimsProvider {
    /// <summary>
    /// Any number of claims can be added
    /// </summary>
    public static IEnumerable<Claim> GetClaims(ApplicationUser user) {
      var claims = new List<Claim>();
      // Users that have been registered a long time are know as veteran users
      var daysRegistered = (DateTime.Now.Date - user.JoinDate).TotalDays;
      claims.Add(CreateClaim("VETERAN_USER", daysRegistered > 180 ? "1" : "0"));
      // Users can be verified by email
      claims.Add(CreateClaim("EMAIL_CONFIRMED", user.EmailConfirmed ? "1" : "0"));
      return claims;
    }

    public static Claim CreateClaim(string type, string value) {
      return new Claim(type, value, ClaimValueTypes.String);
    }
  }
}