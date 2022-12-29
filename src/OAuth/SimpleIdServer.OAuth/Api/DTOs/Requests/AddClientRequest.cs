// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.OAuth.Api.DTOs.Requests
{
    public class AddClientRequest
    {
        [BindProperty(Name = OAuthClientParameters.ClientId)]
        [JsonPropertyName(OAuthClientParameters.ClientId)]
        public string ClientId { get; set; }
        /*
        [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
        public string? TokenEndPointAuthMethod { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.GrantTypes)]
        public IEnumerable<string> GrantTypes { get; set; } = new List<string>();
        [JsonPropertyName(OAuthClientParameters.ResponseTypes)]
        public IEnumerable<string> ResponseTypes { get; set; } = new List<string>();
        [JsonPropertyName(OAuthClientParameters.Contacts)]
        public IEnumerable<string> Contacts { get; set; } = new List<string>();
        [JsonPropertyName(OAuthClientParameters.JwksUri)]
        public string? JwksUri { get; set; } = null;
        */
    }
}
