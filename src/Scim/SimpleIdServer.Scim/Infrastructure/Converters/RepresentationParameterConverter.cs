// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Infrastructure.Converters;

public class RepresentationParameterConverter : JsonConverter<RepresentationParameter>
{
    public override RepresentationParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        var json = node.AsObject();
        var result = new RepresentationParameter
        {
            ExternalId = json.GetStringIgnoreCase(StandardSCIMRepresentationAttributes.ExternalId),
            Schemas = json.GetArrayIgnoreCase(StandardSCIMRepresentationAttributes.Schemas)
        };
        result.Attributes = json.ToCamelCase().AsObject();
        return result;
    }

    public override void Write(Utf8JsonWriter writer, RepresentationParameter value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
