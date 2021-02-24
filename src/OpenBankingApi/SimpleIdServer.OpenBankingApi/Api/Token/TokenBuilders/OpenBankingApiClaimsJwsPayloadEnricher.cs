// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders
{
    public class OpenBankingApiClaimsJwsPayloadEnricher : ClaimsJwsPayloadEnricher
    {
        private readonly OpenBankingApiOptions _options;

        public OpenBankingApiClaimsJwsPayloadEnricher(IOptions<OpenBankingApiOptions> options) : base()
        {
            _options = options.Value;
        }

        public override void EnrichWithClaimsParameter(JwsPayload payload, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, OAuthUser user, DateTime? authDateTime, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            base.EnrichWithClaimsParameter(payload, requestedClaims, user, authDateTime, claimType);
            if (requestedClaims != null)
            {
                var requestedClaim = requestedClaims.FirstOrDefault(c => c.Name == _options.OpenBankingApiConsentClaimName);
                if (requestedClaim != null)
                {
                    payload.Add(_options.OpenBankingApiConsentClaimName, requestedClaim.Values.First());
                }
            }
        }
    }
}
