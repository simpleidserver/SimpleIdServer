// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.Common.Extensions
{
    public static class UserExtensions
    {
        private static Dictionary<string, string> CLAIM_MAPPINGS = new Dictionary<string, string>
        {
            { Jwt.Constants.UserClaims.Subject, ClaimTypes.NameIdentifier },
            { Jwt.Constants.UserClaims.Name, ClaimTypes.Name },
            { Jwt.Constants.UserClaims.Email, ClaimTypes.Email }
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
    }
}
