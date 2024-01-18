// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Clients;

public class UpdateClientRequest
{
    [JsonPropertyName(OAuthClientParameters.RedirectUris)]
    public ICollection<string> RedirectionUrls { get; set; }
    [JsonPropertyName(OAuthClientParameters.ClientName)]
    public string ClientName { get; set; }
    [JsonPropertyName(OAuthClientParameters.PostLogoutRedirectUris)]
    public ICollection<string> PostLogoutRedirectUris { get; set; }
    [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutSessionRequired)]
    public bool FrontChannelLogoutSessionRequired { get; set; }
    [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutUri)]
    public string FrontChannelLogoutUri { get; set; }
    [JsonPropertyName(OAuthClientParameters.BackChannelLogoutUri)]
    public string BackChannelLogoutUri { get; set; }
    [JsonPropertyName(OAuthClientParameters.BackChannelLogoutSessionRequired)]
    public bool BackChannelLogoutSessionRequired { get; set; }
    [JsonPropertyName(OAuthClientParameters.TokenExchangeType)]
    public TokenExchangeTypes? TokenExchangeType { get; set; }
    [JsonPropertyName(OAuthClientParameters.GrantTypes)]
    public ICollection<string> GrantTypes { get; set; }
    [JsonPropertyName(OAuthClientParameters.IsTokenExchangeEnabled)]
    public bool IsTokenExchangeEnabled { get; set; }
    [JsonPropertyName(OAuthClientParameters.IsConsentDisabled)]
    public bool IsConsentDisabled { get; set; }
    [JsonPropertyName(OAuthClientParameters.Parameters)]
    public Dictionary<string, string> Parameters { get; set; }
    [JsonPropertyName(OAuthClientParameters.JwksUri)]
    public string JwksUrl { get; set; }
    [JsonPropertyName(OAuthClientParameters.IsRedirectUrlCaseSensitive)]
    public bool IsRedirectUrlCaseSensitive { get; set; }
    [JsonPropertyName(OAuthClientParameters.AccessTokenType)]
    public AccessTokenTypes AccessTokenType { get; set; }
    [JsonPropertyName(OAuthClientParameters.RedirectToRevokeSessionUI)]
    public bool RedirectToRevokeSessionUI { get; set; }
}
