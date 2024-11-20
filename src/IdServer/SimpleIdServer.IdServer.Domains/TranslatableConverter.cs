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
        var node = JsonNode.Parse(ref reader).AsObject();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var result = Activator.CreateInstance<T>();
        var translateResult = result as ITranslatable;
        var props = properties.Select(p =>
        {
            var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
            return attr == null ? (p, null) : (p, attr.Name);
        }).Where(kvp => kvp.Name != null);
        foreach(var kvp in node)
        {
            var nodeVal = node[kvp.Key];
            var prop = props.FirstOrDefault(p => p.Name == kvp.Key);
            if (prop.p == null)
            {
                var splitted = kvp.Key.Split("#");
                if (splitted.Count() != 2 || nodeVal == null) continue;
                translateResult.Translations.Add(new Translation
                {
                    Key = splitted[0],
                    Language = splitted[1],
                    Value = nodeVal.GetValue<string>()
                });
                continue;
            }

            var setMethod = prop.p.GetSetMethod();
            if (setMethod == null) continue;
            var val = Extract(nodeVal, prop.p.PropertyType);
            prop.p.SetValue(result, val);
        }

        return result;
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
            else if (propertyType == typeof(DateTime?) && obj != null)
                writer.WriteString(prop.Item2, (DateTime)obj);
            else if (TryGetEnumType(propertyType, out Type resultType))
                writer.WriteString(prop.Item2, Enum.GetName(resultType, obj));
            else if (propertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                if (propertyType == typeof(JsonObject))
                {
                    writer.WritePropertyName(prop.Item2);
                    writer.WriteRawValue(((JsonObject)obj).ToJsonString());
                }
                else
                {
                    writer.WriteStartArray(prop.Item2);
                    var values = (IEnumerable<object>)obj;
                    foreach (var v in values)
                    {
                        switch (v)
                        {
                            case string s:
                                writer.WriteStringValue(s);
                                break;
                            default:
                                writer.WriteRawValue(JsonSerializer.Serialize(v));
                                break;
                        }
                    }

                    writer.WriteEndArray();
                }
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
