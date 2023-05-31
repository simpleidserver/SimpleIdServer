// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users
{
    public class GenerateDecentralizedIdentityRequestConverter : JsonConverter<GenerateDecentralizedIdentityRequest>
    {
        public override GenerateDecentralizedIdentityRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, GenerateDecentralizedIdentityRequest value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
