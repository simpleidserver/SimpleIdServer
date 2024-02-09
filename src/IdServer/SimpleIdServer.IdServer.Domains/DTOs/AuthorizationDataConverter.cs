// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains.DTOs
{
    public class AuthorizationDataConverter : JsonConverter<AuthorizationData>
    {
        public override AuthorizationData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader).AsObject();
            var result = new AuthorizationData();
            var properties = typeof(AuthorizationData).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            foreach(var kvp in node)
            {
                var nodeVal = node[kvp.Key];
                var prop = props.FirstOrDefault(p => p.Name == kvp.Key);
                if(prop.p == null)
                {
                    result.AdditionalData.Add(kvp.Key, kvp.Value.ToString());
                    continue;
                }

                var setMethod = prop.p.GetSetMethod();
                if (setMethod == null) continue;
                var val = Extract(nodeVal, prop.p.PropertyType);
                prop.p.SetValue(result, val);
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, AuthorizationData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(AuthorizationDataParameters.Type, value.Type);
            if(!string.IsNullOrWhiteSpace(value.Identifier))
                writer.WriteString(AuthorizationDataParameters.Identifier, value.Identifier);

            if(value.Locations != null && value.Locations.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.Locations);
                foreach (var location in value.Locations)
                    writer.WriteStringValue(location);
                writer.WriteEndArray();
            }

            if (value.Actions != null && value.Actions.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.Actions);
                foreach (var act in value.Actions)
                    writer.WriteStringValue(act);
                writer.WriteEndArray();
            }

            if (value.DataTypes != null && value.DataTypes.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.DataTypes);
                foreach (var dt in value.DataTypes)
                    writer.WriteStringValue(dt);
                writer.WriteEndArray();
            }

            if (value.AdditionalData != null)
            {
                foreach (var kvp in value.AdditionalData)
                {
                    writer.WritePropertyName(kvp.Key);
                    try
                    {
                        JsonNode.Parse(kvp.Value);
                        writer.WriteRawValue(kvp.Value);
                    }
                    catch
                    {
                        writer.WriteStringValue(kvp.Value);
                    }
                }
            }

            writer.WriteEndObject();
        }

        private static object Extract(JsonNode node, Type type)
        {
            switch (node)
            {
                case JsonValue jsonVal:
                    Type resultType;
                    var isEnum = TryGetEnumType(type, out resultType);
                    var getValueMethod = typeof(JsonValue).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(isEnum ? typeof(string) : type);
                    var value = getValueMethod.Invoke(jsonVal, new object[] { });
                    return isEnum ? (value == null ? null : Enum.Parse(resultType, value?.ToString())) : value;
                case JsonArray jsonArray:
                    var genericType = type.GenericTypeArguments[0];
                    var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
                    var addMethod = result.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                    foreach (var record in jsonArray)
                    {
                        addMethod.Invoke(result, new object[] { Extract(record, genericType) });
                    }

                    return result;
                case JsonObject jsonObj:
                    return jsonObj.Deserialize(type);
            }

            return node;
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
}
