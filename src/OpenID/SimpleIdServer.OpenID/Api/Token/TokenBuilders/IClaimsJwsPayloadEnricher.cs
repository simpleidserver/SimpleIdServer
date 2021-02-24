// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public interface IClaimsJwsPayloadEnricher
    {
        void EnrichWithClaimsParameter(JwsPayload payload, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, OAuthUser user, DateTime? authDateTime, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken);
    }
}
