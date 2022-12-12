// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Jwt
{

    public class JsonWebKeyConverter : JsonConverter<JsonWebKey>
    {
        private const string _emptyFieldName = "value__";
        private static Dictionary<Type, FieldInfo[]> _cachedMembers = new Dictionary<Type, FieldInfo[]>();
        private static IEnumerable<(string, PropertyInfo)> _cachedJsonWebKeyProperties = null;

        public override JsonWebKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var properties = GetProperties();
            var dic = new Dictionary<string, string>();
            var result = new JsonWebKey();
            bool isArray = false;
            dynamic lst = null;
            Type lstType = null;
            (string, PropertyInfo)? selectedProperty = null;
            string propertyName = string.Empty;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        selectedProperty = properties.SingleOrDefault(p => p.Item1.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                        break;
                    case JsonTokenType.String:
                        if (selectedProperty == null) continue;
                        var newValue = reader.GetString();
                        if (isArray)
                        {
                            var firstGenericArgument = selectedProperty.Value.Item2.PropertyType.GetGenericArguments()[0];
                            var val = Convert(firstGenericArgument, newValue);
                            var addMethodInfo = lstType.GetMethod("Add");
                            addMethodInfo.Invoke(lst, new object[] { val });
                        }
                        else
                        {
                            if (selectedProperty.Value.Item2 == null)
                                result.Content.Add(propertyName, newValue);
                            else
                            {
                                var val = Convert(selectedProperty.Value.Item2.PropertyType, newValue);
                                selectedProperty.Value.Item2.SetValue(result, val);
                            }
                        }
                        break;
                    case JsonTokenType.StartArray:
                        isArray = true;
                        if (selectedProperty == null) continue;
                        lstType = typeof(List<>).MakeGenericType(selectedProperty.Value.Item2.PropertyType.GetGenericArguments()[0]);
                        lst = Activator.CreateInstance(lstType);
                        selectedProperty.Value.Item2.SetValue(result, lst);
                        break;
                    case JsonTokenType.EndArray:
                        isArray = false;
                        break;
                }
            }

            return result;

            object Convert(Type propertyType, string str)
            {
                var jsonConverter = propertyType.GetCustomAttribute(typeof(JsonConverterAttribute)) as JsonConverterAttribute;
                if (jsonConverter == null) return str;
                if (!_cachedMembers.ContainsKey(propertyType))
                    _cachedMembers.Add(propertyType, propertyType.GetFields());

                var fields = _cachedMembers[propertyType].Where(f => f.Name != _emptyFieldName).Select(p =>
                {
                    var enumAttr = p.GetCustomAttribute(typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                    return enumAttr == null ? (p.Name, p) : (enumAttr.Value, p);
                });

                var field = fields.FirstOrDefault(p => p.Item1.Equals(str, StringComparison.InvariantCultureIgnoreCase));
                if (field.p == null) return null;
                var inst = Activator.CreateInstance(propertyType);
                return field.p.GetValue(inst);
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonWebKey value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var property in GetProperties())
            {
                var propertyValue = property.Item2.GetValue(value);
                if (property.Item2.PropertyType.IsEnum)
                {
                    var fieldName = Convert(property.Item2.PropertyType, propertyValue);
                    if (!string.IsNullOrWhiteSpace(fieldName))
                        writer.WriteString(property.Item1, fieldName);
                }
                else if (property.Item2.PropertyType.IsGenericType && property.Item2.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    writer.WriteStartArray(property.Item1);
                    var genericArg = property.Item2.PropertyType.GetGenericArguments()[0];
                    var enumeratorFn = typeof(IEnumerable<>).MakeGenericType(genericArg).GetMethod("GetEnumerator");
                    var enumerator = enumeratorFn.Invoke(propertyValue, new object[] { });
                    var moveNextFn = enumerator.GetType().GetMethod("MoveNext");
                    var currentProperty = enumerator.GetType().GetProperty("Current");
                    var lst = new List<string>();
                    var isNext = moveNextFn.Invoke(enumerator, new object[] { }) as bool?;
                    while (isNext.Value)
                    {
                        var val = currentProperty.GetValue(enumerator);
                        var convertedValue = Convert(genericArg, val);
                        writer.WriteStringValue(convertedValue);
                        isNext = moveNextFn.Invoke(enumerator, new object[] { }) as bool?;
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteString(property.Item1, propertyValue.ToString());
                }
            }

            foreach (var kvp in value.Content)
                writer.WriteString(kvp.Key, kvp.Value);

            writer.WriteEndObject();

            string? Convert(Type propertyType, object propertyValue)
            {
                if (!_cachedMembers.ContainsKey(propertyType))
                    _cachedMembers.Add(propertyType, propertyType.GetFields());
                var inst = Activator.CreateInstance(propertyType);
                var fieldName = _cachedMembers[propertyType].Where(f =>
                {
                    var val = (int)f.GetValue(inst);
                    return val == (int)propertyValue && f.Name != _emptyFieldName;
                }).Select(f =>
                {
                    var en = f.GetCustomAttributes();
                    var enumAttr = f.GetCustomAttribute(typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                    return enumAttr == null ? f.Name : enumAttr.Value;
                }).FirstOrDefault();
                return fieldName;
            }
        }

        private static IEnumerable<(string, PropertyInfo)> GetProperties()
        {
            if (_cachedJsonWebKeyProperties != null) return _cachedJsonWebKeyProperties;
            _cachedJsonWebKeyProperties = typeof(JsonWebKey).GetProperties().Where(s =>
            {
                return s.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null;
            }).Select(s =>
            {
                var propertyName = s.GetCustomAttribute(typeof(JsonPropertyNameAttribute)) as JsonPropertyNameAttribute;
                if (propertyName != null) return (propertyName.Name, s);
                return (s.Name, s);
            });
            return _cachedJsonWebKeyProperties;
        }
    }
}
