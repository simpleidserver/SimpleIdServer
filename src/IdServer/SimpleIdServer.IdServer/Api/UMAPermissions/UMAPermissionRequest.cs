// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.UMAPermissions
{
    public class UMAPermissionRequest
    {
        [JsonPropertyName(UMAPermissionNames.ResourceId)]
        [BindProperty(Name = UMAPermissionNames.ResourceId)]
        public string? ResourceId { get; set; }
        [JsonPropertyName(UMAPermissionNames.ResourceScopes)]
        [BindProperty(Name = UMAPermissionNames.ResourceScopes)]
        public IEnumerable<string> Scopes { get; set; }
    }
}
