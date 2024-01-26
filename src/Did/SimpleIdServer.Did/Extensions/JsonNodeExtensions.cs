// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System;

namespace SimpleIdServer.Did.Extensions;

public static class JsonNodeExtensions
{
    public static object Serialize(this JsonNode jsonNode)
    {
        var jsonValue = jsonNode as JsonValue;
        var jsonArray = jsonNode as JsonArray;
        var jsonObj = jsonNode as JsonObject;
        if (jsonValue != null) return jsonValue.GetValue<string>();
        if (jsonArray != null)
        {
            var result = new List<object>();
            foreach (var child in jsonArray) result.Add(Serialize(child));
            return result;
        }

        if (jsonObj != null)
        {
            var result = new Dictionary<string, object>();
            foreach (var record in jsonObj) result.Add(record.Key, Serialize(record.Value));
            return result;
        }

        throw new NotImplementedException();
    }
}
