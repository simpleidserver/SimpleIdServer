// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class QueryCollectionExtensions
    {
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

        public static JObject ToJObject(this IQueryCollection queryCollection)
        {
            var jObj = new JObject();
            foreach(var record in queryCollection)
            {
                jObj.Add(record.Key, GetValue(record.Value));
            }

            return jObj;
        }

        private static JToken GetValue(StringValues strValues)
        {
            if (strValues.Count() == 1)
            {
                return new JValue(strValues.First());
            }

            return new JArray(strValues);
        }
    }
}
