// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class FormCollectionExtensions
    {
        public static JObject ToJObject(this IFormCollection formCollection)
        {
            var jObj = new JObject();
            foreach(var record in formCollection)
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