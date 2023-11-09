// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class UpdateClientCredentialsRequest
{
    [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
    public string TokenEndpointAuthMethod { get; set; }
    [JsonPropertyName(OAuthClientParameters.ClientSecret)]
    public string ClientSecret { get; set; }
    [JsonPropertyName(OAuthClientParameters.TlsClientAuthSubjectDN)]
    public string TlsClientAuthSubjectDN { get; set; }
    [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanDNS)]
    public string TlsClientAuthSanDNS { get; set; }
    [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanEmail)]
    public string TlsClientAuthSanEmail { get; set; }
    [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanIp)]
    public string TlsClientAuthSanIp { get; set; }
}