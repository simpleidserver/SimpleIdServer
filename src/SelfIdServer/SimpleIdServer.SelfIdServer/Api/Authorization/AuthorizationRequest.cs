// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.SelfIdServer.Api.Authorization;

public class AuthorizationRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = null!;
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = null!;
    /// <summary>
    /// Optional : This parameter is used by the RP to provide information about itself to a Self-Issued OP that would normally be provided to an OP during Dynamic RP Registration, as specified in Section 7.3. 
    /// </summary>
    [JsonPropertyName("client_metadata")]
    public string ClientMetadata { get; set; } = null;
    /// <summary>
    /// Optional : This parameter is used by the RP to provide information about itself to a Self-Issued OP that would normally be provided to an OP during Dynamic RP Registration.
    /// </summary>
    [JsonPropertyName("client_metadata_uri")]
    public string? ClientMetadataUri { get; set; } = null;
    /// <summary>
    /// Optional : Space-separated string that specifies the types of ID Token the RP wants to obtain, with the values appearing in order of preference.
    /// Allowed individual values are "subject_signed_id_token" and "attester_signed_id_token".
    /// </summary>
    [JsonPropertyName("id_token_type")]
    public string? IdTokenType { get; set; } = null;
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = null!;
}