// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

public class OpenidFederationMetadataJsonConverter : JsonConverter<OpenidFederationMetadataResult>
{
    public override OpenidFederationMetadataResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader).AsObject();
        var result = new OpenidFederationMetadataResult();
        var properties = typeof(OpenidFederationMetadataResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var props = properties.Select(p =>
        {
            var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
            return attr == null ? (p, null) : (p, attr.Name);
        }).Where(kvp => kvp.Name != null);
        foreach (var kvp in node)
        {
            var nodeVal = node[kvp.Key];
            var prop = props.FirstOrDefault(p => p.Name == kvp.Key);
            if (prop.p == null)
            {
                result.OtherParameters.Add(kvp.Key, JsonNode.Parse(kvp.Value.ToJsonString()).AsObject());
                continue;
            }

            var setMethod = prop.p.GetSetMethod();
            if (setMethod == null) continue;
            var val = nodeVal.Deserialize(prop.p.PropertyType);
            prop.p.SetValue(result, val);
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, OpenidFederationMetadataResult value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.FederationEntity != null)
        {
            writer.WritePropertyName("federation_entity");
            writer.WriteRawValue(JsonSerializer.Serialize(value.FederationEntity));
        }

        foreach (var kvp in value.OtherParameters)
        {
            writer.WritePropertyName(kvp.Key);
            writer.WriteRawValue(JsonSerializer.Serialize(kvp.Value));
        }

        writer.WriteEndObject();
    }
}