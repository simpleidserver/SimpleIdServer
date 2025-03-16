// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public interface IClaimsJwsPayloadEnricher
    {
        void EnrichWithClaimsParameter(Dictionary<string, object> claims, IEnumerable<AuthorizedClaim> requestedClaims, User user = null, DateTime? authDateTime = null, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken);
    }

    public class ClaimsJwsPayloadEnricher : IClaimsJwsPayloadEnricher
    {
        public ClaimsJwsPayloadEnricher()
        {
            AllUserClaims = Config.DefaultUserClaims.All.ToList();
        }

        protected List<string> AllUserClaims { get; private set; }

        public virtual void EnrichWithClaimsParameter(Dictionary<string, object> claims, IEnumerable<AuthorizedClaim> requestedClaims, User user = null, DateTime? authDateTime = null, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            if (requestedClaims != null)
            {
                foreach (var claim in requestedClaims.Where(c => c.Type == claimType))
                {
                    if (AllUserClaims.Contains(claim.Name) && user != null)
                    {
                        var cl = user.Claims.FirstOrDefault(c => c.Type == claim.Name);
                        if (cl != null) claims.AddOrReplace(cl.Type, cl.Value);
                    }
                    else
                    {
                        if (claim.Name == JwtRegisteredClaimNames.AuthTime && authDateTime != null)
                            claims.Add(JwtRegisteredClaimNames.AuthTime, authDateTime.Value.ConvertToUnixTimestamp());
                    }
                }
            }
        }
    }
}
