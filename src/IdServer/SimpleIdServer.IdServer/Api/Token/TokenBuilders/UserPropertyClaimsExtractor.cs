// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class UserPropertyClaimsExtractor : BaseAttributeClaimsExtractor, IMapperClaimsExtractor
    {
        public ScopeClaimMapperTypes Type => ScopeClaimMapperTypes.USERPROPERTY;

        public Task<object> Extract(ClaimsExtractionParameter parameter, ScopeClaimMapper mapper)
        {
            var user = parameter.Context.User;
            var property = mapper.UserPropertyName;
            var visibleAttributes = typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                {
                    var attr = p.GetCustomAttribute<UserPropertyAttribute>();
                    return attr == null ? false : attr.IsVisible;
                });

            var visibleAttribute = visibleAttributes.SingleOrDefault(a => a.Name == mapper.UserPropertyName);
            if (visibleAttribute == null) return Task.FromResult((object)null);
            var value = visibleAttribute.GetValue(user)?.ToString();
            if (value == null) return Task.FromResult((object)null);
            return Task.FromResult(Convert(value, mapper));
        }
    }
}
