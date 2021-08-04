// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using Newtonsoft.Json;
using System;

namespace SimpleIdServer.OpenID.Startup.Converters
{
    public class FacebookOptionsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FacebookOptions);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var opts = serializer.Deserialize<FacebookOptionsLite>(reader);
            return opts.ToOptions();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var opts = FacebookOptionsLite.Create(value as FacebookOptions);
            serializer.Serialize(writer, opts);
        }
    }
}
