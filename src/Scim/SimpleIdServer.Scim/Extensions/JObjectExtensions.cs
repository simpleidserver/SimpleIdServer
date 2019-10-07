using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class JObjectExtensions
    {
        public static IEnumerable<string> GetSchemas(this JObject jObj)
        {
            return GetArray(jObj, SCIMConstants.StandardSCIMRepresentationAttributes.Schemas);
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

            var enumName = Enum.GetNames(typeof(T)).FirstOrDefault(n => n.ToLowerInvariant() == r.ToLowerInvariant());
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