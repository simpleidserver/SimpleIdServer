// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users;

public class UpdateUserRequest
{
    [JsonPropertyName(UserNames.Email)]
    public string Email { get; set; }
    [JsonPropertyName(UserNames.EmailVerified)]
    public bool EmailVerified { get; set; }
    [JsonPropertyName(UserNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(UserNames.Lastname)]
    public string Lastname { get; set; }
    [JsonPropertyName(UserNames.Middlename)]
    public string Middlename { get; set; }
    [JsonPropertyName(UserNames.NotificationMode)]
    public string NotificationMode { get; set; }
}
