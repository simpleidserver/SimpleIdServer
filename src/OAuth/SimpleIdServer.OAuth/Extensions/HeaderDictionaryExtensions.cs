// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class HeaderDictionaryExtensions
    {
        public static JsonObject ToJObject(this IHeaderDictionary headerDictionary)
        {
            var jObj = new JsonObject();
            foreach(var kvp in headerDictionary)
                jObj.Add(kvp.Key, JsonSerializer.SerializeToNode(kvp.Value.ToArray()));
            return jObj;
        }
    }
}
