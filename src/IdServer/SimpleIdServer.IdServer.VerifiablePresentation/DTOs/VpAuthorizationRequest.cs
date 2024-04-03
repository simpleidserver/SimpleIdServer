// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

public class VpAuthorizationRequest
{
    [JsonPropertyName("response_mode")]
    public string ResponseMode { get; set; } = "direct_post";
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = "vp_token";
    [JsonPropertyName("response_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResponseUri { get; set; } = null;
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = null!;
    [JsonPropertyName("state")]
    public string State { get; set; } = null!;
    [JsonPropertyName("presentation_definition_uri")]
    public string PresentationDefinitionUri { get; set; } = null!;
}
