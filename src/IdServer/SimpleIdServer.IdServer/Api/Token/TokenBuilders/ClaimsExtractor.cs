// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public interface IClaimsExtractor
    {
        Task<Dictionary<string, object>> ExtractClaims(ClaimsExtractionParameter parameter);
    }

    public class ClaimsExtractionParameter
    {
        public HandlerContext Context { get; set; }
        public ScopeProtocols Protocol { get; set; }
        public IEnumerable<Scope> Scopes { get; set; }
    }

    public class ClaimsExtractor : IClaimsExtractor
    {
        private readonly IEnumerable<IMapperClaimsExtractor> _mapperClaimsExtractors;
        private readonly IGroupRepository _groupRepository;

        public ClaimsExtractor(IEnumerable<IMapperClaimsExtractor> mapperClaimsExtractors, IGroupRepository groupRepository)
        {
            _mapperClaimsExtractors = mapperClaimsExtractors;
            _groupRepository = groupRepository;
        }

        public async Task<Dictionary<string, object>> ExtractClaims(ClaimsExtractionParameter parameter)
        {
            var result = new Dictionary<string, object>();
            var grpPathLst = parameter.Context.User.Groups.SelectMany(g => g.ResolveAllPath()).Distinct();
            var allGroups = await _groupRepository.Query().Include(g => g.Roles).AsNoTracking().Where(g => grpPathLst.Contains(g.FullPath)).ToListAsync();
            var roles = allGroups.SelectMany(g => g.Roles).Select(r => r.Name).Distinct();
            foreach (var role in roles)
                parameter.Context.User.AddClaim(Constants.UserClaims.Role, role);

            foreach (var scope in parameter.Scopes)
            {
                if (scope.Protocol != parameter.Protocol) continue;
                foreach(var mapper in scope.ClaimMappers)
                {
                    var extractor = _mapperClaimsExtractors.Single(m => m.Type == mapper.MapperType);
                    var extractionResult = await extractor.Extract(parameter, mapper);
                    if (extractionResult != null)
                    {
                        switch(scope.Protocol)
                        {
                            case ScopeProtocols.OPENID:
                                result.Add(mapper.TokenClaimName, extractionResult);
                                break;
                            case ScopeProtocols.SAML:
                                result.Add(mapper.SAMLAttributeName, extractionResult);
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
