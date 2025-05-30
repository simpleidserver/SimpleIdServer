// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim.Extensions;

public static class JsonObjectExtensions
{
    public static ICollection<string> GetKeys(this JsonObject jObj)
    {
        return jObj.Select(p => p.Key).ToList();
    }

    public static bool HasElement(this JsonObject jObj, string name, string schema)
    {
        string value;
        if (((value = jObj.GetStringIgnoreCase(name)) != null) || ((value = jObj.GetStringIgnoreCase($"{schema}:{name}")) != null))
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        if (!jObj.ContainsKey(schema))
        {
            return false;
        }

        var attr = jObj[schema] as JsonObject;
        if (attr == null)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(attr.GetStringIgnoreCase(name));
    }

    public static string GetStringIgnoreCase(this JsonObject jObj, string name)
    {
        var node = GetNodeIgnoreCase(jObj, name);
        return node?.ToString();
    }

    public static JsonNode GetNodeIgnoreCase(this JsonObject jObj, string name)
    {
        var match = jObj.FirstOrDefault(kv => string.Equals(kv.Key, name, StringComparison.InvariantCultureIgnoreCase));
        return match.Value;
    }

    public static bool TryGetEnumIgnoreCase<T>(this JsonObject jObj, string name, out T result) where T : struct
    {
        result = default(T);
        string r = jObj.GetStringIgnoreCase(name);
        if (string.IsNullOrWhiteSpace(r)) return false;
        var enumName = Enum.GetNames(typeof(T)).FirstOrDefault(n => n.Equals(r, StringComparison.InvariantCultureIgnoreCase));
        if (string.IsNullOrWhiteSpace(enumName))
        {
            return false;
        }

        result = (T)Enum.Parse(typeof(T), enumName);
        return true;
    }

    public static IEnumerable<string> GetArrayIgnoreCase(this JsonObject jObj, string name)
    {
        var match = jObj.FirstOrDefault(kv => string.Equals(kv.Key, name, StringComparison.InvariantCultureIgnoreCase));
        if (match.Value is not JsonArray jArr)
        {
            return Enumerable.Empty<string>();
        }

        return jArr
            .OfType<JsonValue>()
            .Select(val =>
            {
                try
                {
                    return val.GetValue<string>();
                }
                catch
                {
                    return null;
                }
            })
            .Where(s => s != null);
    }

    public static void RemoveIgnoreCase(this JsonObject jObj, string name)
    {
        var match = jObj.FirstOrDefault(kv => string.Equals(kv.Key, name, StringComparison.InvariantCultureIgnoreCase));
        if (match.Key != null)
        {
            jObj.Remove(match.Key);
        }
    }

    public static JsonNode ToCamelCase(this JsonNode token)
    {
        var type = token.GetValueKind();
        if (type == JsonValueKind.Object)
        {
            ExpandoObject expando = token.Deserialize<ExpandoObject>(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var json = JsonSerializer.Serialize(expando, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return JsonObject.Parse(json).AsObject();
        }

        if(type == JsonValueKind.Array)
        {
            var result = new JsonArray();
            foreach (JsonNode record in (token as JsonArray))
            {
                result.Add(record.ToCamelCase());
            }

            return result;
        }

        return token;
    }
}