// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static Dictionary<string, StringValues> GetQueries(this string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                throw new ArgumentNullException(nameof(redirectUrl));
            }

            redirectUrl = WebUtility.UrlDecode(redirectUrl);
            var splitted = redirectUrl.Split('?');
            if (splitted.Count() != 2)
            {
                return null;
            }

            return QueryHelpers.ParseQuery(splitted.Last());
        }

        public static JObject ToJObj(this Dictionary<string, StringValues> queries)
        {
            var jObj = new JObject();
            foreach (var record in queries)
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
