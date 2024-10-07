// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Extractors
{
    public class PropertyClaimExtractor : BaseClaimExtractor, IClaimExtractor
    {
        public MappingRuleTypes MappingRuleType => MappingRuleTypes.USERPROPERTY;

        public object Extract(HandlerContext context, JsonObject scimObject, IClaimMappingRule mappingRule)
        {
            var property = mappingRule.SourceUserProperty;
            var visibleAttributes = typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                {
                    var attr = p.GetCustomAttribute<UserPropertyAttribute>();
                    return attr == null ? false : attr.IsVisible;
                });

            var visibleAttribute = visibleAttributes.SingleOrDefault(a => a.Name == mappingRule.SourceUserProperty);
            if (visibleAttribute == null) return null;
            var value = visibleAttribute.GetValue(context.User)?.ToString();
            if (value == null) return null;
            return Convert(value, mappingRule);
        }
    }
}
