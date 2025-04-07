// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Serializers
{
    public class IgnoreEmptyOthJsonConverter : JsonConverter<JsonWebKey>
    {
        public override JsonWebKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<JsonWebKey>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, JsonWebKey value, JsonSerializerOptions options)
        {
            if (value.Oth != null && value.Oth.Count == 0)
            {
                // The "options" parameter should not be passed. If it is, the IgnoreEmptyOthJsonConverter instance must be removed; otherwise, it will cause an infinite loop.
                var json = JsonSerializer.Serialize(value);
                var jsonObject = JsonNode.Parse(json).AsObject();
                jsonObject.Remove("oth");
                jsonObject.WriteTo(writer);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}
