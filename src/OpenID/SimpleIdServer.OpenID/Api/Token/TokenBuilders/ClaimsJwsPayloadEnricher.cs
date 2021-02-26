// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class ClaimsJwsPayloadEnricher : IClaimsJwsPayloadEnricher
    {
        public ClaimsJwsPayloadEnricher()
        {
            AllUserClaims = SimpleIdServer.Jwt.Constants.USER_CLAIMS.ToList();
        }

        protected List<string> AllUserClaims { get; private set; }

        public virtual void EnrichWithClaimsParameter(JwsPayload payload, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, OAuthUser user = null, DateTime? authDateTime = null, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            if (requestedClaims != null)
            {
                foreach (var claim in requestedClaims.Where(c => c.Type == claimType))
                {
                    if (AllUserClaims.Contains(claim.Name) && user != null)
                    {
                        payload.AddOrReplace(user.Claims.First(c => c.Type == claim.Name));
                    }
                    else
                    {
                        if (claim.Name == OAuthClaims.AuthenticationTime && authDateTime != null)
                        {
                            payload.Add(OAuthClaims.AuthenticationTime, authDateTime.Value.ConvertToUnixTimestamp());
                        }
                    }
                }
            }
        }

    }
}
