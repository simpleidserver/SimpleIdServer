// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class UserAddressClaimsExtractor : BaseAttributeClaimsExtractor, IMapperClaimsExtractor
    {
        public ScopeClaimMapperTypes Type => ScopeClaimMapperTypes.ADDRESS;

        public Task<object> Extract(ClaimsExtractionParameter parameter, ScopeClaimMapper mapper)
        {
            var values = new Dictionary<string, object>();
            TryAddClaim(mapper.UserAttributeStreetName);
            TryAddClaim(mapper.UserAttributeLocalityName);
            TryAddClaim(mapper.UserAttributeRegionName);
            TryAddClaim(mapper.UserAttributePostalCodeName);
            TryAddClaim(mapper.UserAttributeCountryName);
            TryAddClaim(mapper.UserAttributeFormattedName);
            return Task.FromResult((object)values);

            void TryAddClaim(string name)
            {
                var cl = parameter.Context.User.OAuthUserClaims.SingleOrDefault(c => c.Name == name);
                if (cl == null) return;
                values.Add(name, cl.Value);
            }
        }
    }
}
