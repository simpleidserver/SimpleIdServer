// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfiguration
{
    public string Id { get; set; }
    public string JsonLdContext { get; set; }
    public string BaseUrl { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public string Scope { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public virtual List<CredentialConfigurationClaim> Claims { get; set; } = new List<CredentialConfigurationClaim>();
    public virtual List<CredentialConfigurationTranslation> Displays { get; set; } = new List<CredentialConfigurationTranslation>();
    public virtual List<Credential> Credentials { get; set; }
}