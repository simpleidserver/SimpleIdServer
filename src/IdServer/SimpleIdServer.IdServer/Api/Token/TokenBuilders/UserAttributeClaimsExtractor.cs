// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class UserAttributeClaimsExtractor : BaseAttributeClaimsExtractor, IMapperClaimsExtractor
    {
        public ScopeClaimMapperTypes Type => ScopeClaimMapperTypes.USERATTRIBUTE;

        public Task<KeyValuePair<string, object>?> Extract(ClaimsExtractionParameter parameter, ScopeClaimMapper mapper)
        {
            var user = parameter.Context.User;
            if(!mapper.IsMultiValued)
            {
                var attr = user.OAuthUserClaims.FirstOrDefault(c => c.Name == mapper.UserAttributeName);
                if (attr == null) return Task.FromResult((KeyValuePair<string, object>?)null);
                KeyValuePair<string, object>? r = new KeyValuePair<string, object>(mapper.UserAttributeName, Convert(attr.Value, mapper));
                return Task.FromResult(r);
            }

            var claims = user.OAuthUserClaims.Where(c => c.Name == mapper.UserAttributeName).Select(c => Convert(c.Value, mapper)).ToList();
            KeyValuePair<string, object>? result = new KeyValuePair<string, object>(mapper.TokenClaimName, claims);
            return Task.FromResult(result);
        }
    }
}
