// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.SelfIdServer.Api.Authorization
{
    public class AuthorizationRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;
        
    }
}
