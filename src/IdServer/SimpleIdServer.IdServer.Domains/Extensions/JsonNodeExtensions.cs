// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains.Extensions;

public static class JsonNodeExtensions
{
    public static object SerializeJson(this JsonNode jsonNode)
    {
        if (jsonNode is JsonValue)
            return jsonNode.GetValue<string>();

        if (jsonNode is JsonObject)
        {
            var jsonObj = jsonNode as JsonObject;
            var dic = new Dictionary<string, object>();
            foreach (var key in jsonObj)
            {
                dic.Add(key.Key, SerializeJson(key.Value));
            }

            return dic;
        }

        var jsonArr = jsonNode as JsonArray;
        var result = new List<object>();
        foreach (var record in jsonArr)
            result.Add(SerializeJson(record));
        return result;
    }
}
