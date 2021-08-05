// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.Saml.Sp;
using System;

namespace SimpleIdServer.OpenID.Startup.Converters
{
    public class SamlSpOptionsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(SamlSpOptions);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var opts = serializer.Deserialize<SamlSpOptionsLite>(reader);
            return opts.ToOptions();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var opts = SamlSpOptionsLite.Create(value as SamlSpOptions);
            serializer.Serialize(writer, opts);
        }
    }
}
