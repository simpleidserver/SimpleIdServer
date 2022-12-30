// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.OAuth.Api.Register
{
    public class RegisterClientRequest
    {
        [BindProperty(Name = OAuthClientParameters.ClientId)]
        [JsonPropertyName(OAuthClientParameters.ClientId)]
        public string ClientId { get; set; }
        [BindProperty(Name = OAuthClientParameters.SoftwareStatement)]
        [JsonPropertyName(OAuthClientParameters.SoftwareStatement)]
        public string? SoftwareStatement { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.ClientName)]
        [JsonPropertyName(OAuthClientParameters.ClientName)]
        public string? ClientName { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.GrantTypes)]
        [JsonPropertyName(OAuthClientParameters.GrantTypes)]
        public IEnumerable<string> GrantTypes { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.Scope)]
        [JsonPropertyName(OAuthClientParameters.Scope)]
        public string? Scope { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEndpointAuthMethod)]
        [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
        public string? TokenAuthMethod { get; set; } = null;
    }
}
