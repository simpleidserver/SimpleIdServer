// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    public class CredentialRequestConverter : JsonConverter<CredentialRequest>
    {
        public override CredentialRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new CredentialRequest();
            var properties = typeToConvert.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            var propertyName = string.Empty;
            var arr = new List<string>();
            var rootDepth = reader.CurrentDepth;
            bool continueToRead = true;
            while (continueToRead && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        break;
                    case JsonTokenType.String:
                        var prop = props.FirstOrDefault(p => p.Name == propertyName);
                        if (prop.p != null)
                        {
                            if (prop.p.PropertyType == typeof(double?))
                                prop.p.SetValue(result, double.Parse(reader.GetString()));
                            else if (prop.p.PropertyType == typeof(bool))
                                prop.p.SetValue(result, bool.Parse(reader.GetString()));
                            else
                            {
                                var val = reader.GetString();
                                if (propertyName == "proof") prop.p.SetValue(result, JsonSerializer.Deserialize<CredentialProofRequest>(val));
                                else prop.p.SetValue(result, val);
                            }
                        }
                        break;
                    case JsonTokenType.StartObject:
                        result.OtherParameters.Add(propertyName, ExtractObject(reader));
                        break;
                    case JsonTokenType.StartArray:
                        result.OtherParameters.Add(propertyName, ExtractArray(reader));
                        break;
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, CredentialRequest value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private static JsonObject ExtractObject(Utf8JsonReader reader)
        {
            var propertyName = string.Empty;
            var result = new JsonObject();
            bool isEndObject = false;
            while(!isEndObject && reader.Read())
            {
                switch(reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        break;
                    case JsonTokenType.String:
                        result.Add(propertyName, reader.GetString());
                        break;
                    case JsonTokenType.StartArray:
                        var lst = ExtractArray(reader);
                        result.Add(propertyName, lst);
                        break;
                    case JsonTokenType.EndObject:
                        isEndObject = false;
                        break;
                }
            }

            return result;
        }

        private static JsonArray ExtractArray(Utf8JsonReader reader)
        {
            var propertyName = string.Empty;
            var result = new JsonArray();
            bool isEndArray = false;
            while (!isEndArray && reader.Read())
            {
                isEndArray = reader.TokenType == JsonTokenType.EndArray;
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        result.Add(reader.GetString());
                        break;
                }
            }

            return result;
        }
    }
}
