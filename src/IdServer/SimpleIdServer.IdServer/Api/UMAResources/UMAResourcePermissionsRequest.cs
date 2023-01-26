// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.UMAResources
{
    public class UMAResourcePermissionsRequest
    {
        [JsonPropertyName(UMAResourcePermissionNames.Permissions)]
        [BindProperty(Name = UMAResourcePermissionNames.Permissions)]
        public ICollection<UMAResourcePermissionRequest> Permissions { get; set; }
    }

    public class UMAResourcePermissionRequest
    {
        [JsonPropertyName(UMAResourcePermissionNames.Scopes)]
        [BindProperty(Name = UMAResourcePermissionNames.Scopes)]
        public IEnumerable<string> Scopes { get; set; }
        [JsonPropertyName(UMAResourcePermissionNames.Claims)]
        [BindProperty(Name = UMAResourcePermissionNames.Claims)]
        public IEnumerable<UMAResourcePermissionClaimRequest> Claims { get; set; }
    }

    public class UMAResourcePermissionClaimRequest
    {
        [JsonPropertyName(UMAResourcePermissionNames.ClaimName)]
        [BindProperty(Name = UMAResourcePermissionNames.ClaimName)]
        public string ClaimName { get; set; }
        [JsonPropertyName(UMAResourcePermissionNames.ClaimValue)]
        [BindProperty(Name = UMAResourcePermissionNames.ClaimValue)]
        public string ClaimValue { get; set; }
        [JsonPropertyName(UMAResourcePermissionNames.ClaimType)]
        [BindProperty(Name = UMAResourcePermissionNames.ClaimType)]
        public string ClaimType { get; set; }
        [JsonPropertyName(UMAResourcePermissionNames.ClaimFriendlyName)]
        [BindProperty(Name = UMAResourcePermissionNames.ClaimFriendlyName)]
        public string ClaimFriendlyName { get; set; }
    }
}
