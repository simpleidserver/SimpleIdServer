﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialOfferRecord
{
    public string Id { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public List<string> GrantTypes { get; set; } = new List<string>();
    public List<string> CredentialConfigurationIds { get; set; } = new List<string>();
    [JsonIgnore]
    public string? PreAuthorizedCode { get; set; } = null;
    [JsonIgnore]
    public string? IssuerState { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
}