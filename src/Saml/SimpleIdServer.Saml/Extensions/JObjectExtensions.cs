// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Saml.Extensions
{
    public static class JObjectExtensions
    {
        public static string GetStr(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            return result == null ? null : result.ToString();
        }

        public static int? GetInt(this JObject jObj, string name)
        {
            var str = jObj.GetStr(name);
            int result;
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out result))
            {
                return null;
            }

            return result;
        }

        public static JToken GetToken(this JObject jObj, string name)
        {
            JToken jToken;
            if (!jObj.TryGetValue(name, out jToken))
            {
                return null;
            }

            return jToken;
        }

        public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(this JObject jObj)
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach (JProperty record in jObj.Properties())
            {
                result.Add(new KeyValuePair<string, string>(record.Name, record.Value.ToString()));
            }

            return result;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out SortOrders order)
        {
            string orderStr;
            order = SortOrders.ASC;
            if (!queryCollection.TryGet(name, out orderStr))
            {
                return false;
            }

            SortOrders result;
            if (Enum.TryParse(orderStr.ToUpperInvariant(), out result))
            {
                order = result;
                return true;
            }

            return false;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out string value)
        {
            value = null;
            if (queryCollection.ContainsKey(name))
            {
                value = queryCollection.Get(name).ToArray().First();
                return true;
            }

            return false;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out int startIndex)
        {
            startIndex = 0;
            if (queryCollection.ContainsKey(name))
            {
                return int.TryParse(queryCollection.Get(name).First(), out startIndex);
            }

            return false;
        }

        public static bool ContainsKey(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name)
        {
            if (queryCollection.Any(q => q.Key == name))
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<string> Get(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name)
        {
            if (!queryCollection.ContainsKey(name))
            {
                return new string[0];
            }

            return queryCollection.Where(q => q.Key == name).Select(q => q.Value);
        }
    }
}
