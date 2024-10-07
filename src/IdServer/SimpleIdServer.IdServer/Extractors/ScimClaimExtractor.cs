// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Extractors;

public class ScimClaimExtractor : BaseClaimExtractor, IClaimExtractor
{
    public MappingRuleTypes MappingRuleType => MappingRuleTypes.SCIM;

    public object Extract(HandlerContext context, JsonObject scimObject, IClaimMappingRule mappingRule)
    {
        if (scimObject == null) return null;
        var expression = SCIMFilterParser.Parse(mappingRule.SourceScimPath);
        var tmp = new JsonObject();
        var parameter = scimObject.Filter(expression, tmp);
        if (parameter == null) return null;
        return Convert(parameter);
    }

    public object Convert(JsonNode json)
    {
        var valueKind = json.GetValueKind();
        if(valueKind == JsonValueKind.Object)
        {
            var dic = new Dictionary<string, object>();
            foreach(var kvp in json.AsObject())
            {
                dic.Add(kvp.Key, Convert(kvp.Value));
            }

            return dic;
        }

        if (valueKind == JsonValueKind.Array)
        {
            var result = new List<object>();
            foreach(var record in json.AsArray())
            {
                result.Add(Convert(record));
            }

            return result;
        }

        return json.ToString();
    }
}