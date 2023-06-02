// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users
{
    public class GenerateDecentralizedIdentityRequestConverter : JsonConverter<GenerateDecentralizedIdentityRequest>
    {
        public override GenerateDecentralizedIdentityRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new GenerateDecentralizedIdentityRequest();
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
                               prop.p.SetValue(result, reader.GetString());
                       }
                       else
                       {
                           result.Parameters.Add(propertyName, reader.GetString());
                       }

                        break;
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, GenerateDecentralizedIdentityRequest value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
