// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Client.DTOs;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Client.Serializers
{
    public static class RepresentationSerializer
    {
        public static SearchResult<RepresentationResult> DeserializeSearchRepresentations(JsonObject jsonObj)
        {
            var result = JsonSerializer.Deserialize<SearchResult<RepresentationResult>>(jsonObj.ToJsonString());
            var properties = typeof(RepresentationResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var visibleProperties = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            foreach (var jsonResource in jsonObj["Resources"].AsArray().Select(a => a.AsObject()))
            {
                var resource = result.Resources.First(r => jsonResource["id"].GetValue<string>() == r.Id);
                foreach (var record in jsonResource)
                {
                    if (visibleProperties.Any(p => p.Name == record.Key)) continue;
                    resource.AdditionalData.Add(record.Key, JsonNode.Parse(record.Value.ToJsonString()));
                }
            }

            return result;
        }

        public static RepresentationResult DeserializeRepresentation(JsonObject jsonObj)
        {
            var result = JsonSerializer.Deserialize<RepresentationResult>(jsonObj.ToJsonString());
            var properties = typeof(RepresentationResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var visibleProperties = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            foreach (var record in jsonObj)
            {
                if (visibleProperties.Any(p => p.Name == record.Key)) continue;
                result.AdditionalData.Add(record.Key, JsonNode.Parse(record.Value.ToJsonString()));
            }

            return result;
        }
    }
}
