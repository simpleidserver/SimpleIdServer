// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Api.Users;

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

public class UpdateUserPasswordRequest
{
    [JsonPropertyName(UserCredentialNames.OldValue)]
    public string OldValue { get; set; }
    [JsonPropertyName(UserCredentialNames.Value)]
    public string Value { get; set; }
}
