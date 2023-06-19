// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    public class BaseCredentialTemplateJsonConverter : JsonConverter<BaseCredentialTemplate>
    {
        public override BaseCredentialTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, BaseCredentialTemplate value, JsonSerializerOptions options)
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
                else if (propertyType == typeof(ICollection<CredentialTemplateDisplay>))
                {
                    writer.WriteStartArray(prop.Item2);
                    var displays = (IEnumerable<CredentialTemplateDisplay>)obj;
                    foreach (var display in displays)
                        writer.WriteRawValue(JsonSerializer.Serialize(display));

                    writer.WriteEndArray();
                }
            }

            if (value.Parameters.Any() && value.Parameters != null)
            {
                foreach (var grp in value.Parameters.GroupBy(kvp => kvp.Name))
                    Write(grp.Select(kvp => kvp));
            }

            writer.WriteEndObject();

            void Write(IEnumerable<CredentialTemplateParameter> parameters)
            {
                var firstParameter = parameters.First();
                writer.WritePropertyName(firstParameter.Name);
                if(firstParameter.IsArray) writer.WriteStartArray();
                foreach(var parameter in parameters)
                {
                    switch(firstParameter.ParameterType)
                    {
                        case CredentialTemplateParameterTypes.STRING:
                            writer.WriteStringValue(parameter.Value); 
                            break;
                        case CredentialTemplateParameterTypes.JSON:
                            writer.WriteRawValue(parameter.Value);
                            break;
                    }
                }
                if (firstParameter.IsArray) writer.WriteEndArray();
            }
        }
    }
}
