// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public interface IClaimsJwsPayloadEnricher
    {
        void EnrichWithClaimsParameter(Dictionary<string, object> claims, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, User user = null, DateTime? authDateTime = null, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken);
    }

    public class ClaimsJwsPayloadEnricher : IClaimsJwsPayloadEnricher
    {
        public ClaimsJwsPayloadEnricher()
        {
            AllUserClaims = SIDOpenIdConstants.AllUserClaims.ToList();
        }

        protected List<string> AllUserClaims { get; private set; }

        public virtual void EnrichWithClaimsParameter(Dictionary<string, object> claims, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, User user = null, DateTime? authDateTime = null, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            if (requestedClaims != null)
            {
                foreach (var claim in requestedClaims.Where(c => c.Type == claimType))
                {
                    if (AllUserClaims.Contains(claim.Name) && user != null)
                    {
                        var cl = user.Claims.First(c => c.Type == claim.Name);
                        claims.AddOrReplace(cl.Type, cl.Value);
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
