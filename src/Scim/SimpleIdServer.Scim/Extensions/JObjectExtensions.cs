// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SimpleIdServer.Scim.Domains;
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
            if (jObj.ContainsKey(name) || jObj.ContainsKey($"{schema}:{name}"))
            {
                var value = string.Empty;
                if (jObj.ContainsKey(name))
                {
                    value = jObj[name].ToString();
                }
                else
                {
                    value = jObj[$"{schema}:{name}"].ToString();
                }

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

            return attr.ContainsKey(name) && !string.IsNullOrWhiteSpace(attr[name].ToString());
        }

        public static IEnumerable<string> GetSchemas(this JObject jObj)
        {
            return GetArrayIgnoreCase(jObj, StandardSCIMRepresentationAttributes.Schemas);
        }

        public static bool TryGetInt(this JObject jObj, string name, out int result)
        {
            result = 0;
            if (!jObj.ContainsKey(name))
            {
                return false;
            }

            var val = jObj[name].ToString();
            if (int.TryParse(val, out result))
            {
                return true;
            }

            return false;
        }

        public static bool TryGetString(this JObject jObj, string name, out string result)
        {
            result = null;
            if (!jObj.ContainsKey(name))
            {
                return false;
            }

            result = jObj[name].ToString();
            return true;
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