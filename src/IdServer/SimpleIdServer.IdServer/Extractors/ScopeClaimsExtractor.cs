// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Extractors
{
    public interface IScopeClaimsExtractor
    {
        Task<Dictionary<string, object>> ExtractClaims(HandlerContext context, IEnumerable<Scope> scopes, ScopeProtocols protocol);
    }
    
    public class ScopeClaimsExtractor : IScopeClaimsExtractor
    {
        private readonly IClaimsExtractor _claimsExtractor;

        public ScopeClaimsExtractor(IClaimsExtractor claimsExtractor)
        {
            _claimsExtractor = claimsExtractor;
        }

        public Task<Dictionary<string, object>> ExtractClaims(HandlerContext context, IEnumerable<Scope> scopes, ScopeProtocols protocol)
        {
            var claimMappers = scopes.Where(s => s.Protocol == protocol).SelectMany(s => s.ClaimMappers);
            foreach (var mapper in claimMappers)
                mapper.TargetClaimPath = protocol == ScopeProtocols.OPENID ? mapper.TargetClaimPath : mapper.SAMLAttributeName;
            return _claimsExtractor.ResolveGroupsAndExtract(context, claimMappers);
        }
    }
}
