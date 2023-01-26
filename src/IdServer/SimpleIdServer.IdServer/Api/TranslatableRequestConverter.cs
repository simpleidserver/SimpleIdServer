// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api
{
    public class TranslatableRequestConverter<T> : JsonConverter<T> where T : class
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = Activator.CreateInstance(typeToConvert);
            var properties = typeToConvert.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = properties.Select(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                return attr == null ? (p, null) : (p, attr.Name);
            }).Where(kvp => kvp.Name != null);
            var propertyName = string.Empty;
            var arr = new List<string>();
            bool isArray = false;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        break;
                    case JsonTokenType.String:
                        if (!isArray)
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
                                var splitted = propertyName.Split('#');
                                var name = splitted[0];
                                var language = splitted.Count() == 2 ? splitted[1] : null;
                                ((ITranslatableRequest)result).Translations.Add(new TranslationRequest
                                {
                                    Language = language,
                                    Name = name,
                                    Value = reader.GetString()
                                });
                            }
                        }
                        else
                        {
                            arr.Add(reader.GetString());
                        }
                        break;
                    case JsonTokenType.StartArray:
                        isArray = true;
                        arr = new List<string>();
                        break;
                    case JsonTokenType.EndArray:
                        {
                            isArray = false;
                            var prop = props.First(p => p.Name == propertyName);
                            prop.p.SetValue(result, arr);
                        }
                        break;
                }
            }

            return (T)result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {

        }
    }
}
