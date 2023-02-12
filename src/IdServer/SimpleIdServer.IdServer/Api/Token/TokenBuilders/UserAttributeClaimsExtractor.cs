// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class UserAttributeClaimsExtractor : BaseAttributeClaimsExtractor, IMapperClaimsExtractor
    {
        public ScopeClaimMapperTypes Type => ScopeClaimMapperTypes.USERATTRIBUTE;

        public Task<object> Extract(ClaimsExtractionParameter parameter, ScopeClaimMapper mapper)
        {
            var user = parameter.Context.User;
            if(!mapper.IsMultiValued)
            {
                var attr = user.OAuthUserClaims.FirstOrDefault(c => c.Name == mapper.UserAttributeName);
                if (attr == null) return Task.FromResult((object)null);
                return Task.FromResult(Convert(attr.Value, mapper));
            }

            var claims = user.OAuthUserClaims.Where(c => c.Name == mapper.UserAttributeName).Select(c => Convert(c.Value, mapper)).ToList();
            return Task.FromResult((object)claims);
        }
    }
}
