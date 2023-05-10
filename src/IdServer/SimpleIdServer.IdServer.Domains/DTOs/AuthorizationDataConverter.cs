// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains.DTOs
{
    public class AuthorizationDataConverter : JsonConverter<AuthorizationData>
    {
        public override AuthorizationData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException("Cannot read AuthorizationData");

        public override void Write(Utf8JsonWriter writer, AuthorizationData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(AuthorizationDataParameters.Type, value.Type);
            if(!string.IsNullOrWhiteSpace(value.Identifier))
                writer.WriteString(AuthorizationDataParameters.Identifier, value.Identifier);

            if(value.Locations != null && value.Locations.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.Locations);
                foreach (var location in value.Locations)
                    writer.WriteStringValue(location);
                writer.WriteEndArray();
            }

            if (value.Actions != null && value.Actions.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.Actions);
                foreach (var act in value.Actions)
                    writer.WriteStringValue(act);
                writer.WriteEndArray();
            }

            if (value.DataTypes != null && value.DataTypes.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.DataTypes);
                foreach (var dt in value.DataTypes)
                    writer.WriteStringValue(dt);
                writer.WriteEndArray();
            }

            if(value.Types != null && value.Types.Any())
            {
                writer.WriteStartArray(AuthorizationDataParameters.Types);
                foreach (var t in value.Types)
                    writer.WriteStringValue(t);
                writer.WriteEndArray();
            }

            if (value.AdditionalData != null)
            {
                foreach (var kvp in value.AdditionalData)
                {
                    writer.WritePropertyName(kvp.Key);
                    try
                    {
                        JsonNode.Parse(kvp.Value);
                        writer.WriteRawValue(kvp.Value);
                    }
                    catch
                    {
                        writer.WriteStringValue(kvp.Value);
                    }
                }
            }

            writer.WriteEndObject();
        }
    }
}
