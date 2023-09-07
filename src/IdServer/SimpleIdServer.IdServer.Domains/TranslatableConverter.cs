// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace SimpleIdServer.IdServer.Domains;

public class TranslatableConverter<T> : JsonConverter<T> where T : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
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

        var translationsProperty = typeof(T).GetProperty("Translations", BindingFlags.Instance | BindingFlags.Public);
        var translations = translationsProperty.GetValue(value) as ICollection<Translation>;
        if (translations != null)
            foreach (var translation in translations)
            {
                var key = translation.Key;
                if (!string.IsNullOrWhiteSpace(translation.Language)) key = $"{key}#{translation.Language}";
                writer.WriteString(key, translation.Value);
            }

        writer.WriteEndObject();
    }
}
