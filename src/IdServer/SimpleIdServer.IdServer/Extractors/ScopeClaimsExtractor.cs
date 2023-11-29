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
            var claimMappers = scopes
                .Where(s => (s.Protocol == ScopeProtocols.OPENID && protocol == ScopeProtocols.OAUTH) || s.Protocol == protocol)
                .SelectMany(s => s.ClaimMappers)
                .Where(s => protocol != ScopeProtocols.OAUTH || (protocol == ScopeProtocols.OAUTH && s.IncludeInAccessToken));
            foreach (var mapper in claimMappers)
                mapper.TargetClaimPath = protocol == ScopeProtocols.SAML ? mapper.SAMLAttributeName: mapper.TargetClaimPath;

            return _claimsExtractor.ResolveGroupsAndExtract(context, claimMappers);
        }
    }
}
