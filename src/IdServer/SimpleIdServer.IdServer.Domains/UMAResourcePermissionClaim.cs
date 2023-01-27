// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UMAResourcePermissionClaim
    {
        public UMAResourcePermissionClaim() { }

        [JsonPropertyName(UMAResourcePermissionNames.ClaimType)]
        public string? ClaimType { get; set; } = null!;
        [JsonPropertyName(UMAResourcePermissionNames.ClaimFriendlyName)]
        public string? FriendlyName { get; set; } = null;
        [JsonPropertyName(UMAResourcePermissionNames.ClaimName)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(UMAResourcePermissionNames.ClaimValue)]
        public string Value { get; set; } = null!;
    }
}
