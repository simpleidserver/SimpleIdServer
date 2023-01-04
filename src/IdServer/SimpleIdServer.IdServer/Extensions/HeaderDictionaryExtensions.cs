// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.AspNetCore.Http
{
    public static class HeaderDictionaryExtensions
    {
        public static JsonObject ToJsonObject(this IHeaderDictionary headerDictionary)
        {
            var jObj = new JsonObject();
            foreach(var kvp in headerDictionary)
                jObj.Add(kvp.Key, JsonSerializer.SerializeToNode(kvp.Value.ToArray()));
            return jObj;
        }
    }
}
