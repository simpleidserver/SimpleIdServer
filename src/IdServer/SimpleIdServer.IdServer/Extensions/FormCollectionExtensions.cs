// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.AspNetCore.Http
{
    public static class FormCollectionExtensions
    {
        public static JsonObject ToJsonObject(this IFormCollection formCollection)
        {
            var jObj = new JsonObject();
            foreach(var record in formCollection)
                jObj.Add(record.Key, GetValue(record.Value));
            return jObj;
        }

        private static JsonNode GetValue(StringValues strValues)
        {
            if (strValues.Count() == 1)
            {
                try
                {
                    return JsonNode.Parse(strValues.First());
                }
                catch
                {
                    return JsonValue.Create(strValues.First());
                }
            }

            return JsonSerializer.SerializeToNode(strValues.Select(s => s));
        }
    }
}