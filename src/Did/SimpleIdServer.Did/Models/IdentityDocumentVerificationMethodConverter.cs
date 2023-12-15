// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Models
{
    public class IdentityDocumentVerificationMethodConverter : JsonConverter<IdentityDocumentVerificationMethod>
    {
        public override IdentityDocumentVerificationMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new IdentityDocumentVerificationMethod();
            var properties = typeToConvert.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            var propertyName = string.Empty;
            var arr = new List<string>();
            var rootDepth = reader.CurrentDepth;
            bool isWritingPublicKeyJwk = false;
            bool continueToRead = true;
            while (continueToRead && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        // if(propertyName == "publicKeyJwk") result.PublicKeyJwk = new JsonObject();
                        break;
                    case JsonTokenType.StartObject:
                        isWritingPublicKeyJwk = propertyName == "publicKeyJwk";
                        break;
                    case JsonTokenType.String:
                        if(isWritingPublicKeyJwk)
                        {
                           // result.PublicKeyJwk.Add(propertyName, reader.GetString());
                        }
                        else
                        {
                            var prop = props.FirstOrDefault(p => p.Name == propertyName);
                            if (prop.p != null)
                            {
                                if (prop.p.PropertyType == typeof(double?))
                                    prop.p.SetValue(result, double.Parse(reader.GetString()));
                                else if (prop.p.PropertyType == typeof(bool))
                                    prop.p.SetValue(result, bool.Parse(reader.GetString()));
                                else
                                    prop.p.SetValue(result, reader.GetString());
                            }
                            else
                            {
                                result.AdditionalParameters.Add(propertyName, reader.GetString());
                            }
                        }

                        break;
                    case JsonTokenType.EndObject:
                        if (rootDepth == reader.CurrentDepth) continueToRead = false;
                        isWritingPublicKeyJwk = false;
                        break;
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, IdentityDocumentVerificationMethod value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            foreach (var prop in props)
            {
                var propertyType = prop.p.PropertyType;
                var obj = prop.p.GetValue(value);
                if (obj == null) continue;
                if (propertyType == typeof(string))
                    writer.WriteString(prop.Item2, obj as string);
                else if (propertyType == typeof(bool))
                    writer.WriteBoolean(prop.Item2, (bool)obj);
                else if (propertyType == typeof(double?) || propertyType == typeof(double))
                    writer.WriteNumber(prop.Item2, (double)obj);
                else if (propertyType == typeof(DateTime))
                    writer.WriteString(prop.Item2, (DateTime)obj);
                else if (propertyType == typeof(IEnumerable<string>) || propertyType == typeof(ICollection<string>))
                {
                    writer.WriteStartArray(prop.Item2);
                    var values = (IEnumerable<string>)obj;
                    foreach (var v in values)
                        writer.WriteStringValue(v);
                    writer.WriteEndArray();
                }
                else if (propertyType == typeof(JsonObject))
                {
                    writer.WritePropertyName(prop.Item2);
                    writer.WriteRawValue(((JsonObject)obj).ToJsonString());
                }
            }

            if (value.AdditionalParameters != null)
            {
                foreach (var kvp in value.AdditionalParameters)
                {
                    writer.WriteString(kvp.Key, kvp.Value);
                }
            }

            writer.WriteEndObject();
        }
    }
}
