// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class JObjectExtensions
    {
        public static ICollection<string> GetKeys(this JObject jObj)
        {
            return jObj.Properties().Select(p => p.Name).ToList();
        }

        public static bool HasNotEmptyElement(this JObject jObj, string name, string schema)
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

                return !string.IsNullOrEmpty(value);
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
            return GetArray(jObj, StandardSCIMRepresentationAttributes.Schemas);
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

        public static bool TryGetEnum<T>(this JObject jObj, string name, out T result) where T : struct
        {
            result = default(T);
            string r;
            if (!jObj.TryGetString(name, out r))
            {
                return false;
            }

            var enumName = Enum.GetNames(typeof(T)).FirstOrDefault(n => n.Equals(r, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(enumName))
            {
                return false;
            }

            result = (T)Enum.Parse(typeof(T), enumName);
            return true;
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

        public static string GetString(this JObject jObj, string name)
        {
            if (!jObj.ContainsKey(name))
            {
                return null;
            }

            return jObj[name].ToString();
        }

        public static IEnumerable<string> GetArray(this JObject jObj, string name)
        {
            if (!jObj.ContainsKey(name))
            {
                return new string[0];
            }

            var jArr = jObj[name] as JArray;
            if (jArr == null)
            {
                return new string[0];
            }

            return jArr.Values<string>().ToList();
        }
    }
}