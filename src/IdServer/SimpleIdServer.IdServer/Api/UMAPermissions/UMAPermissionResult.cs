// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.UMAPermissions
{
    public class UMAPermissionResult
    {
        [JsonPropertyName(TokenRequestParameters.Ticket)]
        [BindProperty(Name = TokenRequestParameters.Ticket)]
        public string Ticket { get; set; }
    }
}
