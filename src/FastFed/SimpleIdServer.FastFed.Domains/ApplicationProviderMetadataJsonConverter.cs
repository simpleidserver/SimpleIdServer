// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class ApplicationProviderMetadataJsonConverter : JsonConverter<ApplicationProviderMetadata>
{
    public override ApplicationProviderMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader).AsObject();
        var result = new ApplicationProviderMetadata();
        var properties = typeof(ApplicationProviderMetadata).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

    public override void Write(Utf8JsonWriter writer, ApplicationProviderMetadata value, JsonSerializerOptions options)
    {
        var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var props = properties.Select(p =>
        {
            var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
            return attr == null ? (p, null) : (p, attr.Name);
        }).Where(kvp => kvp.Name != null);
        writer.WriteStartObject();
        foreach (var prop in props)
        {
            var propertyType = prop.p.PropertyType;
            var obj = prop.p.GetValue(value);
            Type? ut = null;
            if (obj == null) continue;
            if (propertyType == typeof(string))
                writer.WriteString(prop.Item2, obj as string);
            else if (propertyType == typeof(bool))
                writer.WriteBoolean(prop.Item2, (bool)obj);
            else if (propertyType == typeof(double?) || propertyType == typeof(double))
                writer.WriteNumber(prop.Item2, (double)obj);
            else if (propertyType == typeof(DateTime))
                writer.WriteString(prop.Item2, (DateTime)obj);
            else if (TryGetEnumType(propertyType, out Type resultType))
                writer.WriteString(prop.Item2, Enum.GetName(resultType, obj));
            else
            {
                writer.WritePropertyName(prop.Item2);
                writer.WriteRawValue(JsonSerializer.Serialize(obj));
            }
        }

        if (value.OtherParameters != null)
        {
            foreach (var record in value.OtherParameters)
                writer.WriteString(record.Key, record.Value.ToString());
        }

        writer.WriteEndObject();
    }

    private static bool TryGetEnumType(Type incomingType, out Type resultType)
    {
        resultType = null;
        Type ut = null;
        if (incomingType.IsEnum || ((ut = Nullable.GetUnderlyingType(incomingType)) != null && ut.IsEnum))
        {
            resultType = ut ?? incomingType;
            return true;
        }

        return false;
    }
}
