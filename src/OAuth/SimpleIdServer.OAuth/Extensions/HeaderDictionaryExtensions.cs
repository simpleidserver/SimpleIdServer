// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class HeaderDictionaryExtensions
    {
        public static JObject ToJObject(this IHeaderDictionary headerDictionary)
        {
            var jObj = new JObject();
            foreach(var kvp in headerDictionary)
            {
                jObj.Add(kvp.Key, new JArray(kvp.Value.ToArray()));
            }

            return jObj;
        }
    }
}
