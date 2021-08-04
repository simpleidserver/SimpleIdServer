// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.Common.Extensions
{
    public static class UserExtensions
    {
        private static Dictionary<string, string> CLAIM_MAPPINGS = new Dictionary<string, string>
        {
            { Jwt.Constants.UserClaims.Subject, ClaimTypes.NameIdentifier },
            { Jwt.Constants.UserClaims.Name, ClaimTypes.Name },
            { Jwt.Constants.UserClaims.Email, ClaimTypes.Email },
            { Jwt.Constants.UserClaims.BirthDate, ClaimTypes.DateOfBirth },
            { Jwt.Constants.UserClaims.Gender, ClaimTypes.Gender },
            { Jwt.Constants.UserClaims.GivenName, ClaimTypes.GivenName }
        };

        public static List<Claim> ToClaims(this User oauthUser)
        {
            var claims = new List<Claim>();
            foreach (var cl in oauthUser.Claims)
            {
                if (!CLAIM_MAPPINGS.ContainsKey(cl.Type))
                {
                    continue;
                }

                claims.Add(new Claim(CLAIM_MAPPINGS[cl.Type], cl.Value));
            }

            return claims;
        }

        public static User BuildUser(this ClaimsPrincipal claimsPrincipal)
        {
            var userClaims = claimsPrincipal.BuildClaims();
            var claimSub = userClaims.FirstOrDefault(c => c.Type == Jwt.Constants.UserClaims.Subject);
            var claimName = userClaims.FirstOrDefault(c => c.Type == Jwt.Constants.UserClaims.Name);
            string sub = claimSub == null ? null : claimSub.Value;
            string name = claimName == null ? null : claimName.Value;
            var user = User.Create(name, sub);
            user.UpdateClaims(userClaims);
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
