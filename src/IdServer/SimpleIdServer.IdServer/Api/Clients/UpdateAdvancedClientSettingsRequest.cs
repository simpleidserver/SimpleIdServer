// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class UpdateAdvancedClientSettingsRequest
{
    [JsonPropertyName(OAuthClientParameters.TokenSignedResponseAlg)]
    public string TokenSignedResponseAlg { get; set; }
    [JsonPropertyName(OAuthClientParameters.IdTokenSignedResponseAlg)]
    public string IdTokenSignedResponseAlg { get; set; }
    [JsonPropertyName(OAuthClientParameters.AuthorizationSignedResponseAlg)]
    public string AuthorizationSignedResponseAlg { get; set; }
    [JsonPropertyName(OAuthClientParameters.AuthorizationDataTypes)]
    public ICollection<string> AuthorizationDataTypes { get; set; }
    [JsonPropertyName(OAuthClientParameters.ResponseTypes)]
    public ICollection<string> ResponseTypes { get; set; }
    [JsonPropertyName(OAuthClientParameters.DPOPBoundAccessTokens)]
    public bool DPOPBoundAccessTokens { get; set; }
    [JsonPropertyName(OAuthClientParameters.DPOPNonceLifetimeInSeconds)]
    public double DPOPNonceLifetimeInSeconds { get; set; }
    [JsonPropertyName(OAuthClientParameters.IsDPOPNonceRequired)]
    public bool IsDPOPNonceRequired {  get; set; }
    [JsonPropertyName(OAuthClientParameters.TokenExpirationTimeInSeconds)]
    public double TokenExpirationTimeInSeconds { get; set; }
    [JsonPropertyName(OAuthClientParameters.UserCookieExpirationTimeInSeconds)]
    public double UserCookieExpirationTimeInSeconds { get; set; }
    [JsonPropertyName(OAuthClientParameters.AuthorizationCodeExpirationInSeconds)]
    public int AuthorizationCodeExpirationInSeconds { get; set; }
    [JsonPropertyName(OAuthClientParameters.DeviceCodeExpirationInSeconds)]
    public int DeviceCodeExpirationInSeconds { get; set; }
    [JsonPropertyName(OAuthClientParameters.DeviceCodePollingInterval)]
    public int DeviceCodePollingInterval { get; set; }
}
