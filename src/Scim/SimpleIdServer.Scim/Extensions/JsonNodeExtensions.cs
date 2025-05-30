// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Scim.Extensions;

public static class JsonNodeExtensions
{
    private static readonly Regex ArrayAccessRegex = new Regex(@"^(\w+)\[(\d+)\]$", RegexOptions.Compiled);

    public static bool IsEmpty(this JsonNode token)
        => token == null || string.IsNullOrWhiteSpace(token.ToString());


    public static JsonNode? SelectToken(this JsonNode token, string query)
    {
        if (string.IsNullOrWhiteSpace(query) || token == null)
            return null;

        query = query.Trim();

        if (query.StartsWith("$."))
        {
            query = query.Substring(2);
        }

        if (query.StartsWith("..") || query.StartsWith("$.."))
        {
            var targetProperty = query.StartsWith("..") ? query.Substring(2) : query.Substring(3);
            return FindFirstRecursively(token, targetProperty);
        }

        var parts = query.Split('.');
        JsonNode? current = token;
        foreach (var part in parts)
        {
            if (current == null)
                return null;

            var match = ArrayAccessRegex.Match(part);
            if (match.Success)
            {
                var arrayProp = match.Groups[1].Value;
                var index = int.Parse(match.Groups[2].Value);

                if (current is JsonObject obj &&
                    obj.TryGetPropertyValue(arrayProp, out var arrayNode) &&
                    arrayNode is JsonArray array &&
                    index >= 0 && index < array.Count)
                {
                    current = array[index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (current is JsonObject obj &&
                    obj.TryGetPropertyValue(part, out var next))
                {
                    current = next;
                }
                else
                {
                    return null;
                }
            }
        }

        return current;
    }

    private static JsonNode? FindFirstRecursively(JsonNode node, string propertyName)
    {
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                if (string.Equals(kvp.Key, propertyName, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;

                var found = FindFirstRecursively(kvp.Value!, propertyName);
                if (found != null)
                    return found;
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
            {
                var found = FindFirstRecursively(item!, propertyName);
                if (found != null)
                    return found;
            }
        }

        return null;
    }
}
