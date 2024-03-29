﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialInstance;

public class SearchCredentialInstancesRequest
{
    [JsonPropertyName("credential_configuration_id")]
    public string CredentialConfigurationId { get; set; }
}
