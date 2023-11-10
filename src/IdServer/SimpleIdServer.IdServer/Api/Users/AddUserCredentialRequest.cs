// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users;

public class AddUserCredentialRequest
{
    [JsonPropertyName(UserCredentialNames.Active)]
    public bool Active { get; set; }
    [JsonPropertyName(UserNames.Credential)]
    public UserCredential Credential { get; set; }
}
