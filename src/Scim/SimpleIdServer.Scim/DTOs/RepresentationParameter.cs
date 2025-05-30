// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure.Converters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.DTOs;

[JsonConverter(typeof(RepresentationParameterConverter))]
public class RepresentationParameter : BaseParameter
{
    public RepresentationParameter() : base()
    {

    }

    /// <summary>
    ///  A String that is an identifier for the resource as defined by the provisioning client.
    /// </summary>
    [JsonPropertyName(StandardSCIMRepresentationAttributes.ExternalId)]
    public string ExternalId { get; set; }
    [JsonIgnore]
    public JsonObject Attributes { get; set; }
}