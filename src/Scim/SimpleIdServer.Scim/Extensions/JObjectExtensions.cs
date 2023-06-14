// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class JObjectExtensions
    {
        public static ICollection<string> GetKeys(this JObject jObj)
        {
            return jObj.Properties().Select(p => p.Name).ToList();
        }

        public static bool HasElement(this JObject jObj, string name, string schema)
        {
            string value;
            if (((value = jObj.GetStringIgnoreCase(name)) != null) || ((value = jObj.GetStringIgnoreCase($"{schema}:{name}")) != null))
            {
                return !string.IsNullOrWhiteSpace(value);
            }

            if (!jObj.ContainsKey(schema))
            {
                return false;
            }

            var attr = jObj[schema] as JObject;
            if (attr == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(attr.GetStringIgnoreCase(name));
        }

        public static string GetStringIgnoreCase(this JObject jObj, string name)
        {
            if (jObj.TryGetValue(name, StringComparison.InvariantCultureIgnoreCase, out JToken value)) return value.ToString();
            return null;
        }

        public static bool TryGetEnumIgnoreCase<T>(this JObject jObj, string name, out T result) where T : struct
        {
            result = default(T);
            string r = jObj.GetStringIgnoreCase(name);
            if (string.IsNullOrWhiteSpace(r)) return false;
            var enumName = Enum.GetNames(typeof(T)).FirstOrDefault(n => n.Equals(r, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(enumName))
            {
                return false;
            }

            result = (T)Enum.Parse(typeof(T), enumName);
            return true;
        }

        public static IEnumerable<string> GetArrayIgnoreCase(this JObject jObj, string name)
        {
            JToken value = null;
            if (!jObj.TryGetValue(name, StringComparison.InvariantCultureIgnoreCase, out value)) return new string[0];
            var jArr = value as JArray;
            if (jArr == null) return new string[0];
            return jArr.Values<string>().ToList();
        }

        public static void RemoveIgnoreCase(this JObject jObj, string name)
        {
            JToken value = null;
            var child = jObj.Children().FirstOrDefault(x => string.Equals(x.Path, name, StringComparison.InvariantCultureIgnoreCase));
            if (child == null) return;
            jObj.Remove(child.Path);
        }

        public static JToken ToCamelCase(this JToken token)
        {
            if(token.Type == JTokenType.Object) return JObject.FromObject(token.ToObject<ExpandoObject>(), JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            if(token.Type == JTokenType.Array)
            {
                var result = new JArray();
                foreach (JToken record in (token as JArray)) result.Add(record.ToCamelCase());
                return result;
            }

            return token;
        }
    }
}