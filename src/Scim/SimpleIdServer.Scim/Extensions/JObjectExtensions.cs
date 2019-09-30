using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
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

        private static IEnumerable<string> GetArray(JObject jObj, string name)
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