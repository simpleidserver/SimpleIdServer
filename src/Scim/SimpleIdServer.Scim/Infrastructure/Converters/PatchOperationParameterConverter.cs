// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.Infrastructure.Converters;

public class PatchOperationParameterConverter : JsonConverter<PatchOperationParameter>
{
    public override PatchOperationParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        var json = node.AsObject();
        var result = new PatchOperationParameter
        {
            Path = json.GetStringIgnoreCase(SCIMConstants.PathOperationAttributes.Path)
        };
        if (json.TryGetEnumIgnoreCase(SCIMConstants.PathOperationAttributes.Operation, out SCIMPatchOperations op))
        {
            result.Operation = op;
        }
        var value = json.GetNodeIgnoreCase(SCIMConstants.PathOperationAttributes.Value);
        if (value != null)
        {
            result.Value = value.ToCamelCase();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, PatchOperationParameter value, JsonSerializerOptions options)
    {

    }
}
