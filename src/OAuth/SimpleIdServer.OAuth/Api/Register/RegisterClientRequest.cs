// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Domains.DTOs;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace SimpleIdServer.OAuth.Api.Register
{
    public class RegisterClientRequest
    {
        [BindProperty(Name = OAuthClientParameters.ClientId)]
        [JsonPropertyName(OAuthClientParameters.ClientId)]
        public string ClientId { get; set; }
        [BindProperty(Name = OAuthClientParameters.SoftwareStatement)]
        [JsonPropertyName(OAuthClientParameters.SoftwareStatement)]
        public string SoftwareStatement { get; set; }
    }
}
