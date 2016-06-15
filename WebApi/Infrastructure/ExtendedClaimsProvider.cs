using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApi.Infrastructure {
  public static class ExtendedClaimsProvider {
    public static IEnumerable<Claim> GetClaims(ApplicationUser user) {
      var claims = new List<Claim>();
      var daysInWork = (DateTime.Now.Date - user.JoinDate).TotalDays;
      claims.Add(CreateClaim("FTE", daysInWork > 90 ? "1" : "0"));
      return claims;
    }

    public static Claim CreateClaim(string type, string value) {
      return new Claim(type, value, ClaimValueTypes.String);
    }
  }
}