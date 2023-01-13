// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.AspNetCore.Http
{
    public static class QueryCollectionExtensions
    {
        public static Dictionary<string, StringValues> GetQueries(this string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
                throw new ArgumentNullException(nameof(redirectUrl));

            redirectUrl = WebUtility.UrlDecode(redirectUrl);
            var uri = new Uri(redirectUrl);
            if (string.IsNullOrWhiteSpace(uri.Query)) return null;

            return QueryHelpers.ParseQuery(uri.Query);
        }

        public static JsonObject ToJsonObject(this Dictionary<string, StringValues> queries)
        {
            var jObj = new JsonObject();
            foreach (var record in queries)
            {
                jObj.Add(record.Key, GetValue(record.Value));
            }

            return jObj;
        }

        public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(this IQueryCollection query)
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach (var record in query)
            {
                result.Add(new KeyValuePair<string, string>(record.Key, record.Value));
            }

            return result;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out string[] values)
        {
            values = null;
            if (queryCollection.ContainsKey(name))
            {
                var result = queryCollection.Get(name).ToArray().Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
                if (!result.Any())
                {
                    return false;
                }

                values = result.ToArray();
                return true;
            }

            return false;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out int[] values)
        {
            values = null;
            string[] tmp;
            if (!queryCollection.TryGet(name, out tmp))
            {
                return false;
            }

            var result = new List<int>();
            foreach (var str in tmp)
            {
                var splitted = str.Split(',');
                foreach (var record in splitted)
                {
                    int number;
                    if (int.TryParse(record, out number))
                    {
                        result.Add(number);
                    }
                }
            }

            if (!result.Any())
            {
                return false;
            }

            values = result.ToArray();
            return true;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out DateTime value)
        {
            value = default(DateTime);
            if (queryCollection.ContainsKey(name))
            {
                DateTime result;
                if (DateTime.TryParse(queryCollection.Get(name).ToArray().First(), out result))
                {
                    value = result;
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out bool value)
        {
            value = false;
            if (queryCollection.ContainsKey(name))
            {
                bool result;
                if (bool.TryParse(queryCollection.Get(name).ToArray().First(), out result))
                {
                    value = result;
                    return true;
                }

                return false;
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

        public static bool TryGet(this IEnumerable<KeyValuePair<string, string>> queryCollection, string name, out IEnumerable<string> values)
        {
            values = null;
            if (queryCollection.ContainsKey(name))
            {
                values = queryCollection.Get(name);
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

        public static JsonObject ToJObject(this IQueryCollection queryCollection)
        {
            var jObj = new JsonObject();
            foreach(var record in queryCollection)
                jObj.Add(record.Key, GetValue(record.Value));

            return jObj;
        }

        private static JsonNode GetValue(StringValues strValues)
        {
            if (strValues.Count() == 1)
            {
                try
                {
                    return JsonObject.Parse(strValues[0]).AsObject();
                }
                catch
                {
                    return strValues[0];
                }
            }

            var result = new JsonArray();
            foreach (var str in strValues) result.Add(str);
            return result;
        }
    }
}
