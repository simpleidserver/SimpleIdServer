// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
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

        public ClaimsExtractor(IEnumerable<IMapperClaimsExtractor> mapperClaimsExtractors)
        {
            _mapperClaimsExtractors = mapperClaimsExtractors;
        }

        public async Task<Dictionary<string, object>> ExtractClaims(ClaimsExtractionParameter parameter)
        {
            var result = new Dictionary<string, object>();
            foreach(var scope in parameter.Scopes)
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
