// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains.Clients;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public class DistributeHttpClaimsSource : IClaimsSource
    {
        private readonly DistributeHttpClaimsSourceOptions _distributeHttpClaimsSourceOptions;

        public DistributeHttpClaimsSource(DistributeHttpClaimsSourceOptions distributeHttpClaimsSourceOptions)
        {
            _distributeHttpClaimsSourceOptions = distributeHttpClaimsSourceOptions;
        }

        public Task Enrich(JwsPayload jwsPayload, OAuthClient oauthClient)
        {
            ClaimsSourceHelper.AddDistribute(jwsPayload, _distributeHttpClaimsSourceOptions.Name, _distributeHttpClaimsSourceOptions.Url,
                _distributeHttpClaimsSourceOptions.ApiToken, _distributeHttpClaimsSourceOptions.ClaimNames);
            return Task.FromResult(0);
        }
    }
}
