// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users;

public class UnlinkExternalAuthProviderRequest
{
    [JsonPropertyName(UserExternalAuthProviderNames.Scheme)]
    public string Scheme { get; set; }
    [JsonPropertyName(UserExternalAuthProviderNames.Subject)]
    public string Subject { get; set; }
}
