﻿using System.Collections.Generic;
using System.Security.Claims;

namespace WebApi.Infrastructure {
  public class RolesFromClaims {
    public static IEnumerable<Claim> CreateRolesBasedOnClaims(ClaimsIdentity identity) {
      var claims = new List<Claim>();
      if (identity.HasClaim(c => c.Type == "FTE" && c.Value == "1") && identity.HasClaim(ClaimTypes.Role, "Admin")) {
        claims.Add(new Claim(ClaimTypes.Role, "IncidentResolvers"));
      }
      return claims;
    }
  }
}