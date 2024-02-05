using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

public class UpdateCredentialConfigurationDetailsRequest
{
    [JsonPropertyName("json_ld_context")]
    public string JsonLdContext { get; set; }
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}