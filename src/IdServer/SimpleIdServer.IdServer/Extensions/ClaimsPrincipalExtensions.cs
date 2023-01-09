// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace System.Security.Claims
{
    public static class ClaimsPrincipalExtensions
    {
        private static Dictionary<string, string> CLAIM_MAPPINGS = new Dictionary<string, string>
        {
            { JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier },
            { JwtRegisteredClaimNames.Name, ClaimTypes.Name },
            { JwtRegisteredClaimNames.Email, ClaimTypes.Email },
            { JwtRegisteredClaimNames.Birthdate, ClaimTypes.DateOfBirth },
            { JwtRegisteredClaimNames.Gender, ClaimTypes.Gender },
            { JwtRegisteredClaimNames.GivenName, ClaimTypes.GivenName }
        };

        public static User BuildUser(this ClaimsPrincipal claimsPrincipal, string scheme)
        {
            var userClaims = claimsPrincipal.BuildClaims();
            var claimSub = userClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.Sub);
            var claimName = userClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.Name);
            string sub = claimSub == null ? null : claimSub.Value;
            string name = claimName == null ? null : claimName.Value;
            var user = User.Create(name, null);
            if (claimSub != null)
            {
                userClaims.Remove(claimSub);
                userClaims.Add(new UserClaim(JwtRegisteredClaimNames.Sub, user.Id));
            }

            user.UpdateClaims(userClaims);
            user.AddExternalAuthProvider(scheme, sub);
            return user;
        }

        public static ICollection<UserClaim> BuildClaims(this ClaimsPrincipal claimsPrincipal)
        {
            var claims = claimsPrincipal.Claims;
            var userClaims = new List<UserClaim>();
            foreach (var claim in claims)
            {
                if (CLAIM_MAPPINGS.ContainsKey(claim.Type))
                {
                    userClaims.Add(new UserClaim
                    {
                        Name = claim.Type,
                        Value = claim.Value
                    });
                    continue;
                }

                if (CLAIM_MAPPINGS.Values.Contains(claim.Type))
                {
                    var kvp = CLAIM_MAPPINGS.First(k => k.Value == claim.Type);
                    userClaims.Add(new UserClaim
                    {
                        Name = kvp.Key,
                        Value = claim.Value
                    });
                }
            }

            return userClaims;
        }
    }
}
