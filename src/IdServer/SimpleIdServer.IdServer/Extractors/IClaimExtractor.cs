// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Extractors
{
    public interface IClaimExtractor
    {
        MappingRuleTypes MappingRuleType { get; }
        object Extract(HandlerContext context, JsonObject scimObject, IClaimMappingRule mappingRule);
    }
}
