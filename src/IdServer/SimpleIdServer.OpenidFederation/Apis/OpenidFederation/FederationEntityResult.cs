// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

/// <summary>
/// https://openid.net/specs/openid-federation-1_0.html#section-5.1.1.
/// All Entities in a federation MAY use this Entity Type. 
/// The Entities that provide federation API endpoints MUST use this Entity Type.
/// </summary>
public class FederationEntityResult
{
    [JsonPropertyName("organization_name")]
    public string OrganizationName { get; set; }
    /// <summary>
    /// The fetch endpoint.
    /// </summary>
    [JsonPropertyName("federation_fetch_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationFetchEndpoint { get; set; } = null;
    /// <summary>
    /// The list endpoint.
    /// </summary>
    [JsonPropertyName("federation_list_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationListEndpoint { get; set; } = null;
    /// <summary>
    /// The resolve endpoint.
    /// </summary>
    [JsonPropertyName("federation_resolve_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationResolveEndpoint { get; set; } = null;
    /// <summary>
    /// The Trust Mark status endpoint.
    /// </summary>
    [JsonPropertyName("federation_trust_mark_status_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationTrustMarkStatusEndpoint { get; set; }
    [JsonPropertyName("federation_trust_mark_list_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationTrustMarkEndpoint { get; set; }
    [JsonPropertyName("federation_historical_keys_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FederationHistoricalKeysEndpoint { get; set; }
}
